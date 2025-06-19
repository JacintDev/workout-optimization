import json
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense
from tensorflow.keras.callbacks import EarlyStopping, ModelCheckpoint
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
    [105.9261, 159.8105, -150.3793, 0.764807, 0.558656, 0.0],
    [84.5711919335899, 143.84911124081674, -148.9160464706417, 0.6956400136821047, 0.6575745315740832, 0.0],
    [69.65711909322776, 112.10006231458102, -139.01391641511958, 0.6228802826875413, 0.7478405919515393, 0.0],
    [46.79487596435616, 91.72893203508079, -86.34624372132333, 0.5812871172797972, 0.8244285859529148, 0.0],
    [24.052214362416706, 61.355688669387945, -18.202957978833165, 0.5485982041347139, 0.8850784900072927, 0.0],
    [8.103849945447955, 41.52685042750308, 35.84500796167725, 0.5394272190834439, 0.8910828380756373, 0.0],
    [-1.3055023205188674, 29.44535447004717, 74.23750243042454, 0.5934992858490566, 0.852322121462264, 0.0],
    [-9.315706626931611, 17.21664993968989, 98.60432828171677, 0.6601580726814933, 0.7794338614570322, 0.0],
    [-13.63516922097715, 12.909334139976156, 95.06290935199877, 0.6727581940483748, 0.6602675113903315, 0.0],
    [-7.701362085582571, 8.073237061735224, 55.15375238641768, 0.6875510129088822, 0.5634525278442548, 0.0],
    [2.546164310336188, 4.87955049132394, 0.3961806219679494, 0.7405643671202855, 0.5530593309170843, 0.0],
    [12.294896597404612, 11.350939484416934, -43.0052221969312, 0.741159647352574, 0.5725632952297255, 0.0],
    [22.67217, 19.28003, -72.07409, 0.717876, 0.633565, 0.0]
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
