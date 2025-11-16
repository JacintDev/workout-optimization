import json
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense, Dropout, BatchNormalization
from tensorflow.keras.callbacks import EarlyStopping
from sklearn.preprocessing import MinMaxScaler
from sklearn.model_selection import train_test_split
import tf2onnx
import joblib
import pyodbc

# ========================
# Debug mód
# ========================
DEBUG_MODE = False  # True: részletes log, False: minimális output

# ========================
# 1) Adat betöltése
# ========================
with open('normalized_save.json', 'r') as f:
    data = json.load(f)  # Lista ismétlésekből, mind 14 elem (13 adat + 1 IsCorrect)

X = []
y = []

for ism in data:
    # Első 13 elem az adat:
    seq_np = np.array([[p['GyrosX'], p['GyrosY'], p['GyrosZ'],
                        p['AccelX'], p['AccelY'], p['AccelZ']] for p in ism[:13]])
    X.append(seq_np)

    # A 14. elem a címke:
    label = ism[13].get('IsCorrect', False)
    y.append(1 if label else 0)

X = np.array(X)  # (mintaszám, 13, 6)
y = np.array(y)  # (mintaszám,)

num_samples = len(X)
seq_len = X.shape[1]
num_features = X.shape[2]

# Osztályeloszlás (teljes adathalmaz)
unique, counts = np.unique(y, return_counts=True)
class_distribution = dict(zip(unique, counts))

print(f"Összes adat: {num_samples} minta (sorozathossz: {seq_len}, jellemzők: {num_features})")
print("Osztályeloszlás (címke: db):", class_distribution)

# ========================
# 2) Train / Val / Test split
#   - 60% train
#   - 20% val
#   - 20% test
# ========================

# Először: train+val vs test
X_train_val, X_test, y_train_val, y_test = train_test_split(
    X, y,
    test_size=0.2,
    random_state=42,
    stratify=y
)

# Aztán train vs val (train_val 80%-ából 25% lesz val → 0.8*0.25=0.2)
X_train, X_val, y_train, y_val = train_test_split(
    X_train_val, y_train_val,
    test_size=0.25,
    random_state=42,
    stratify=y_train_val
)

num_train = len(X_train)
num_val = len(X_val)
num_test = len(X_test)

print(f"Train minták: {num_train}, Val minták: {num_val}, Test minták: {num_test}")
print("###############################")

# ========================
# 3) Normalizálás csak TRAIN alapján
# ========================
scaler = MinMaxScaler()

X_train_reshaped = X_train.reshape(-1, 6)
X_val_reshaped = X_val.reshape(-1, 6)
X_test_reshaped = X_test.reshape(-1, 6)

X_train_scaled = scaler.fit_transform(X_train_reshaped).reshape(-1, 13, 6)
X_val_scaled = scaler.transform(X_val_reshaped).reshape(-1, 13, 6)
X_test_scaled = scaler.transform(X_test_reshaped).reshape(-1, 13, 6)

# Scaler mentése (ha később is kell)
joblib.dump(scaler, 'scaler.save')
scaler = joblib.load('scaler.save')  # visszatöltés, hogy biztosan ugyanazt használjuk

if DEBUG_MODE:
    print("Normalizálás megtörtént (csak TRAIN alapján).")
    print("data_min_ =", scaler.data_min_)
    print("data_max_ =", scaler.data_max_)
    print("data_range_ =", scaler.data_range_)
    print("#################")

# ========================
# 4) Modellépítő függvény
# ========================
def build_model():
    model = Sequential([
        LSTM(32, input_shape=(13, 6), return_sequences=True),
        BatchNormalization(),
        Dropout(0.3),
        LSTM(16, return_sequences=False),
        BatchNormalization(),
        Dropout(0.2),
        Dense(1, activation='sigmoid'),
    ])

    model.compile(
        optimizer='adam',
        loss='binary_crossentropy',
        metrics=['accuracy']
    )
    return model

# ========================
# 5) Többszöri tanítás (5x), legjobb modell kiválasztása
# ========================
best_val_acc = -1.0
best_model = None
best_history = None
run_val_scores = []  # ide mentjük az összes futás legjobb val_accuracy-ját

for run in range(1, 6):
    if DEBUG_MODE:
        print(f'\n========== Futás {run}/5 ==========')

    model = build_model()

    early_stop = EarlyStopping(
        monitor='val_loss',
        patience=5,
        restore_best_weights=True
    )

    history = model.fit(
        X_train_scaled, y_train,
        epochs=30,
        batch_size=5,
        validation_data=(X_val_scaled, y_val),
        callbacks=[early_stop],
        verbose=1 if DEBUG_MODE else 0
    )

    run_best_val_acc = max(history.history['val_accuracy'])
    run_val_scores.append(run_best_val_acc)

    if DEBUG_MODE:
        print(f'Futás {run} legjobb val_accuracy: {run_best_val_acc:.4f}')

    if run_best_val_acc > best_val_acc:
        best_val_acc = run_best_val_acc
        best_model = model
        best_history = history
        if DEBUG_MODE:
            print(f'>>> ÚJ LEGJOBB MODELL! (új best_val_acc: {best_val_acc:.4f})')

# Összefoglaló a futások után (mindig kiírjuk)
print("\n=== EREDMÉNYEK ÖSSZEFOGLALÓJA (VALIDÁCIÓ) ===")
for idx, score in enumerate(run_val_scores, 1):
    print(f"Futás {idx}: legjobb val_accuracy = {score:.4f}")
print(f"\nÖsszes futás közül a legjobb val_accuracy (VAL): {best_val_acc:.4f}")

# Biztonság kedvéért mentsük is el a legjobb modellt H5-be
best_model.save("best_model.h5")
print("Legjobb modell elmentve: best_model.h5")

# ========================
# 6) Külön TESZT halmaz értékelése
# ========================
test_loss, test_acc = best_model.evaluate(X_test_scaled, y_test, verbose=0)
correct_test = int(test_acc * num_test)

print("\n=== TESZT EREDMÉNY ===")
print(f"Teszt accuracy (soha nem látott adaton): {test_acc:.4f} ({correct_test}/{num_test} minta)")

# ========================
# 7) Egy új ismétlésre való predikció (a legjobb modellel)
# ========================
sample = [
    [-15.1626, 21.75574, -34.74697, 0.475283, 0.904518, 0.0],
    [-11.97223, 7.577221, -19.76566, 0.458235, 0.91932, 0.0],
    [-5.610339, -24.16377, 29.39814, 0.483227, 0.918184, 0.0],
    [-4.72645, -27.82508, 71.77637, 0.602199, 0.813601, 0.0],
    [-2.811513, -4.320055, 18.2261, 0.652763, 0.722175, 0.0],
    [4.678234, 17.62824, -24.00697, 0.687654, 0.740585, 0.0],
    [11.79296, 38.18948, -41.21156, 0.633589, 0.795191, 0.0],
    [6.716588, 28.46723, -47.00135, 0.538645, 0.85884, 0.0],
    [17.54955, 31.7818, 11.92742, 0.464947, 0.912424, 0.0],
    [24.08159, 17.65937, 46.49049, 0.524748, 0.895147, 0.0],
    [17.84534, -7.601378, 75.90823, 0.62771, 0.80402, 0.0],
    [0.0, 4.495685, -1.952398, 0.697059, 0.685176, 0.0],
    [-11.62881, 7.719298, -54.64145, 0.578818, 0.812271, 0.0]
]

sample2 = [
    [-15.1626, 21.75574, -34.74697, 0.475283, 0.904518, 0.0],
    [-11.97223, 7.577221, -19.76566, 0.458235, 0.91932, 0.0],
    [-5.610339, -24.16377, 29.39814, 0.483227, 0.918184, 0.0],
    [-4.72645, -27.82508, 71.77637, 0.602199, 0.813601, 0.0],
    [-2.811513, -4.320055, 18.2261, 0.652763, 0.722175, 0.0],
    [4.678234, 17.62824, -24.00697, 0.687654, 0.740585, 0.0],
    [11.79296, 38.18948, -41.21156, 0.633589, 0.795191, 0.0],
    [6.716588, 28.46723, -47.00135, 0.538645, 0.85884, 0.0],
    [17.54955, 31.7818, 11.92742, 0.464947, 0.912424, 0.0],
    [24.08159, 17.65937, 46.49049, 0.524748, 0.895147, 0.0],
    [17.84534, -7.601378, 75.90823, 0.62771, 0.80402, 0.0],
    [0.0, 4.495685, -1.952398, 0.697059, 0.685176, 0.0],
    [-11.62881, 7.719298, -54.64145, 0.578818, 0.812271, 0.0]
]

sample = np.array(sample).reshape(-1, 6)
sample2 = np.array(sample2).reshape(-1, 6)

sample_scaled = scaler.transform(sample).reshape(1, 13, 6)
sample_scaled2 = scaler.transform(sample2).reshape(1, 13, 6)

prediction = best_model.predict(sample_scaled, verbose=0)
prediction2 = best_model.predict(sample_scaled2, verbose=0)

print("\nPredikciók a minta sorozatra:")
print("Valószínűség, hogy helyes (sample):", float(prediction[0][0]))
print("Helyes gyakorlat? (sample):", bool(prediction[0][0] > 0.5))

print("Valószínűség, hogy helyes (sample2):", float(prediction2[0][0]))
print("Helyes gyakorlat? (sample2):", bool(prediction2[0][0] > 0.5))

# ========================
# 8) Validációs statisztika a legjobb futásból
# ========================
if best_history is not None:
    val_acc_best = max(best_history.history['val_accuracy'])
    correct_val = int(val_acc_best * num_val)
    print(f"\nA legjobb futás validációs mintáiból kb. {correct_val}/{num_val}-t talált el.")

# ========================
# 9) Legjobb modell ONNX-re konvertálása és mentése SQL-be
# ========================
onnx_model, _ = tf2onnx.convert.from_keras(
    best_model,
    input_signature=[tf.TensorSpec([None, 13, 6], tf.float32, name="input")],
    opset=11
)

onnx_bytes = onnx_model.SerializeToString()

# Scaler metaadatok stringgé (szóközökkel elválasztott számok)
data_min_str = " ".join(map(str, scaler.data_min_))
data_range_str = " ".join(map(str, scaler.data_range_))

# ===== SQL Server kapcsolat (Dockerben futó MSSQL, C#-os conn alapján) =====
conn_str = (
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "SERVER=localhost;"
    "DATABASE=workoutoptimization;"
    "UID=sa;"
    "PWD=Horthy2000?;"
    "Encrypt=no;"
    "TrustServerCertificate=yes;"
)

conn = pyodbc.connect(conn_str)
cursor = conn.cursor()

# Modell meta
model_name = "BenchPress_LSTM"
model_version = 1  # ha módosítod a modellt, ezt tudod növelni

insert_sql = """
INSERT INTO MlModels (Name, Version, ModelData, DataMin, DataRange, Accurate)
VALUES (?, ?, ?, ?, ?, ?)
"""

cursor.execute(
    insert_sql,
    model_name,
    model_version,
    onnx_bytes,      # VARBINARY(MAX)
    data_min_str,    # VARCHAR(MAX)
    data_range_str,
    val_acc_best 
)

conn.commit()
cursor.close()
conn.close()

print("\nLegjobb ONNX modell + scaler metaadatok sikeresen elmentve az SQL adatbázisba.")
