import json
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense
from tensorflow.keras.callbacks import EarlyStopping, ModelCheckpoint
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense, Dropout, BatchNormalization
from sklearn.preprocessing import MinMaxScaler
import tf2onnx
import joblib

# 1) Adat betöltése
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
from sklearn.utils import shuffle
X, y = shuffle(X, y, random_state=42)

print(f'Adat méret: {X.shape}')
print(f'Címke méret: {y.shape}')
print(y)

# 2) Normalizálás 0-1 közé minden jellemzőre
scaler = MinMaxScaler()
X_reshaped = X.reshape(-1, 6)         # (mintaszám × 13, 6)
X_scaled = scaler.fit_transform(X_reshaped)
X_scaled = X_scaled.reshape(-1, 13, 6)

# (Menthető fájlba is, ha később használni szeretnéd)
joblib.dump(scaler, 'scaler.save')

scaler = joblib.load('scaler.save')
print("Normalizálás megtörtént.")
print("data_min_ =", scaler.data_min_)
print("data_max_ =", scaler.data_max_)
print("data_range_ =", scaler.data_range_)
print("#################")

# 3) Modell felépítése
model = Sequential([
    LSTM(32, input_shape=(13, 6), return_sequences=True),
    BatchNormalization(),
    Dropout(0.3),
    LSTM(16, return_sequences=False),
    BatchNormalization(),
    Dropout(0.2),
    Dense(1, activation='sigmoid'),
])

model.compile(optimizer='adam',
              loss='binary_crossentropy',
              metrics=['accuracy'])

model.summary()


early_stop = EarlyStopping(
    monitor='val_loss',
    patience=5,
    restore_best_weights=True
)

checkpoint = ModelCheckpoint(
    filepath='best_model.h5',
    monitor='val_accuracy',
    save_best_only=True,
    mode='max',
    verbose=1
)

# 4) Betanítás
history = model.fit(X_scaled, y, epochs=30, batch_size=5, validation_split=0.3, callbacks=[early_stop, checkpoint])

# 5) Egy új ismétlésre való predikció
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
# Normalizálás predikció előtt
sample = np.array(sample).reshape(-1, 6)
sample2 = np.array(sample2).reshape(-1, 6)
sample_scaled = scaler.transform(sample).reshape(1, 13, 6)
sample_scaled2 = scaler.transform(sample2).reshape(1, 13, 6)

# Előrejelzés
prediction = model.predict(sample_scaled)
prediction2 = model.predict(sample_scaled2)
print("Valószínűség, hogy helyes:", prediction[0][0])
print("Helyes gyakorlat?", prediction[0][0] > 0.5)
# 6) Validációs statisztika
val_acc = history.history['val_accuracy'][-1]
num_val_samples = int(len(X_scaled) * 0.3)
correct_predictions = int(val_acc * num_val_samples)

print(f"A validációs mintákból {correct_predictions}/{num_val_samples}-t talált el.")

model = tf.keras.models.load_model("best_model.h5")
tspecs = [tf.TensorSpec(i.shape, name=i.name) for i in model.inputs]

onnx_model, _ = tf2onnx.convert.from_keras(
    model,
    input_signature=[tf.TensorSpec([None, 13, 6], tf.float32, name="input")],
    opset=11,
    output_path="best_model.onnx"
)