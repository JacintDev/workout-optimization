import json
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense
from tensorflow.keras.callbacks import EarlyStopping
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense, Dropout, BatchNormalization
from sklearn.preprocessing import MinMaxScaler
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
num_ones = np.sum(y == 1)
num_zeros = np.sum(y == 0)

print(f"Number of 1s: {num_ones}")
print(f"Number of 0s: {num_zeros}")
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

# 4) Betanítás
history = model.fit(X_scaled, y, epochs=30, batch_size=15, validation_split=0.3, callbacks=[early_stop])
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
    model.compile(optimizer='adam',
                  loss='binary_crossentropy',
                  metrics=['accuracy'])
    return model

from sklearn.model_selection import train_test_split
val_accuracies = []
num_runs = 5

for i in range(num_runs):
    print(f"\n--- {i+1}. futtatás ---")

    # Train/val split újrasorsolása minden futtatáshoz
    X_train, X_val, y_train, y_val = train_test_split(X_scaled, y, test_size=0.3, random_state=42 + i)

    model = build_model()

    history = model.fit(X_train, y_train,
                        validation_data=(X_val, y_val),
                        epochs=30,
                        batch_size=2,
                        verbose=0,
                        callbacks=[early_stop])

    val_acc = history.history['val_accuracy'][-1]
    val_accuracies.append(val_acc)
    print(f"Validációs pontosság: {val_acc:.4f}")

# Átlagos és szórásos eredmény kiírása
avg_acc = np.mean(val_accuracies)
std_acc = np.std(val_accuracies)
print(f"\nÁtlagos validációs pontosság: {avg_acc:.4f} ± {std_acc:.4f}")
