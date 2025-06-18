import json
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense
# 1) Betöltés
with open('normalized_reps.json', 'r') as f:
    data = json.load(f)  # data: lista ismétlésekből, mind 14 elem (13 adat + 1 IsCorrect)

X = []
y = []

for ism in data:
    # ism egy lista 14 dict-tel
    # Első 13 elem az adat:
    seq_np = np.array([[p['GyrosX'], p['GyrosY'], p['GyrosZ'], p['AccelX'], p['AccelY'], p['AccelZ']] for p in ism[:13]])
    X.append(seq_np)
    
    # A 14. elem a címke:
    label = ism[13].get('IsCorrect', False)  # alapértelmezetten False, ha nincs ott
    y.append(1 if label else 0)

X = np.array(X)  # (példányszám, 13, 6)
y = np.array(y)  # (példányszám,)

print(f'Adat méret: {X.shape}')
print(f'Címke méret: {y.shape}')




# Modell felépítése
model = Sequential([
    LSTM(32, input_shape=(13, 6)),  # 13 időlépés, 6 jellemző
    Dense(1, activation='sigmoid')  # Bináris kimenet (helyes/rossz)
])

model.compile(optimizer='adam',
              loss='binary_crossentropy',
              metrics=['accuracy'])

model.summary()

# Betanítás
history = model.fit(X, y, epochs=30, batch_size=2, validation_split=0.2)

# Például egy új ismétlés (13x6 formában) – ezt cseréld ki a sajátodra:
sample = np.array([
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,0,0,0,0],
])

# 1 példányt vizsgálunk, tehát bővítsük ki a dimenzióval (1, 13, 6)
sample = np.expand_dims(sample, axis=0)

# Előrejelzés
prediction = model.predict(sample)

# Eredmény
print("Valószínűség, hogy helyes:", prediction[0][0])
print("Helyes gyakorlat?" , prediction[0][0] > 0.5)