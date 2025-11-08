import numpy as np
import onnx
import pandas as pd
import tensorflow as tf
from tensorflow.keras import layers, models, callbacks
# import skl2onnx
# from skl2onnx.common.data_types import FloatTensorType
# import keras2onnx
# import onnx
import tf2onnx

# --- 1️⃣ Szintetikus adatgenerálás ---
ages = np.arange(15, 71)  # 15–70 évesek
resting_hr = np.arange(50, 91)  # 50–90 bpm
data = []

for age in ages:
    for rest in resting_hr:
        hr_max = 220 - age
        hr_target = rest + (hr_max - rest) / 2
        # adjunk kis véletlen zajt ±3 bpm tartományban
        hr_target_noisy = hr_target + np.random.uniform(-3, 3)
        data.append([age, rest, hr_target_noisy])

df = pd.DataFrame(data, columns=['Age', 'RestHR', 'TargetHR'])

# --- 2️⃣ Adatok normalizálása és szétosztása ---
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler

X = df[['Age', 'RestHR']].values
y = df[['TargetHR']].values

scaler_x = MinMaxScaler()
scaler_y = MinMaxScaler()

X_scaled = scaler_x.fit_transform(X)
y_scaled = scaler_y.fit_transform(y)

X_train, X_test, y_train, y_test = train_test_split(X_scaled, y_scaled, test_size=0.2, random_state=42)

# --- 3️⃣ Modell definiálása ---
model = models.Sequential([
    layers.Input(shape=(2,)),
    layers.Dense(64, activation='relu'),
    layers.Dropout(0.2),  # neuron kikapcsolás (regularizálás)
    layers.Dense(32, activation='relu'),
    layers.Dense(16, activation='relu'),
    layers.Dense(1, activation='linear')
])

model.compile(optimizer='adam', loss='mse', metrics=['mae'])

# --- 4️⃣ Early stopping callback ---
early_stop = callbacks.EarlyStopping(monitor='val_loss', patience=20, restore_best_weights=True)

# --- 5️⃣ Modell tanítása ---
history = model.fit(
    X_train, y_train,
    validation_split=0.2,
    epochs=500,
    batch_size=32,
    callbacks=[early_stop],
    verbose=1
)

# --- 6️⃣ Értékelés ---
loss, mae = model.evaluate(X_test, y_test)
scale_range = float(scaler_y.data_max_ - scaler_y.data_min_)
print(f"\nTeszt MAE: {float(mae) * scale_range:.2f} bpm")




# --- 7️⃣ Példa előrejelzés ---
test_example = np.array([[25, 70]])  # életkor=25, nyugalmi pulzus=70
test_scaled = scaler_x.transform(test_example)
pred_scaled = model.predict(test_scaled)
predicted = scaler_y.inverse_transform(pred_scaled)
print(f"Prediktált optimális pulzus: {predicted[0][0]:.2f} bpm")

# --- 8️⃣ Modell mentése ONNX formátumban ---
spec = (tf.TensorSpec((None, 2), tf.float32, name="input"),)  # 2 input feature: életkor, nyugalmi pulzus
output_path = "hr_model_tf2onnx.onnx"

model_proto, _ = tf2onnx.convert.from_keras(model, input_signature=spec, output_path=output_path)
print(f"\nA modell sikeresen elmentve: {output_path}")
