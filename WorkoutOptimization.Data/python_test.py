import json
import matplotlib.pyplot as plt

# JSON fájl betöltése
with open("json_labeled_sequences.json", "r") as f:
    sequences = json.load(f)

# Üres listák az adatokhoz
gyro_x_all = []
gyro_y_all = []
gyro_z_all = []
accel_x_all = []
accel_y_all = []
accel_z_all = []

# Végigmegyünk az összes ismétlésen
for sequence in sequences:
    sensor_data = sequence[:-1]  # utolsó elem az IsCorrect, azt most nem használjuk

    for point in sensor_data:
        gyro_x_all.append(point["GyrosX"])
        gyro_y_all.append(point["GyrosY"])
        gyro_z_all.append(point["GyrosZ"])
        accel_x_all.append(point["AccelX"])
        accel_y_all.append(point["AccelY"])
        accel_z_all.append(point["AccelZ"])

# Kirajzolás
time_steps = range(len(gyro_x_all))  # időlépések folyamatosan

plt.figure(figsize=(14, 8))

# 1. subplot: Gyro értékek
plt.subplot(2, 1, 1)
plt.plot(time_steps, gyro_x_all, label="GyrosX")
plt.plot(time_steps, gyro_y_all, label="GyrosY")
plt.plot(time_steps, gyro_z_all, label="GyrosZ")
plt.title("Gyroscope Data (All Sequences)")
plt.ylabel("Value")
plt.legend()
plt.grid(True)

# Függőleges vonalak minden ismétlés határnál
for i in range(1, len(sequences)):
    plt.axvline(x=i * 13, color='red', linestyle='--', alpha=1)

# 2. subplot: Accel értékek
plt.subplot(2, 1, 2)
plt.plot(time_steps, accel_x_all, '--', label="AccelX")
plt.plot(time_steps, accel_y_all, '--', label="AccelY")
plt.plot(time_steps, accel_z_all, '--', label="AccelZ")
plt.title("Accelerometer Data (All Sequences)")
plt.xlabel("Time Steps")
plt.ylabel("Value")
plt.legend()
plt.grid(True)

# Függőleges vonalak az accelerometer subploton is
for i in range(1, len(sequences)):
    plt.axvline(x=i * 13, color='red', linestyle='--', alpha=1)

plt.tight_layout()
plt.show()
