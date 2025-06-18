import json
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from scipy.signal import find_peaks

def load_data(file_path):
    with open(file_path, 'r') as f:
        data = json.load(f)
    return pd.DataFrame(data)

def detect_repetitions_bottom_peaks(gyro_z, distance, prominence):
    # min
    peaks_min, _ = find_peaks(-gyro_z, distance=distance, prominence=prominence)
    return peaks_min

def plot_sensor_data(df, rep_peaks, title="Gyorsulás és giroszkóp adatok"):
    fig, axs = plt.subplots(2, 1, figsize=(14, 8), sharex=True)
    
    axs[0].plot(df.index, df['AccelX'], label='Accel X')
    axs[0].plot(df.index, df['AccelY'], label='Accel Y')
    axs[0].plot(df.index, df['AccelZ'], label='Accel Z')
    axs[0].set_title('Gyorsulás (accelerometer)')
    axs[0].legend()
    axs[0].grid(True)
    
    axs[1].plot(df.index, df['GyrosZ'], label='Gyros Z')
    axs[1].plot(df.index, df['GyrosX'], label='Gyros X', alpha=0.7)
    axs[1].plot(df.index, df['GyrosY'], label='Gyros Y', alpha=0.7)
    axs[1].set_title('Szögsebesség (gyroscope)')
    axs[1].legend()
    axs[1].grid(True)
    
    # Ismétlések jelölése
    for peak in rep_peaks:
        axs[0].axvline(x=peak, color='red', linestyle='--', alpha=0.8, linewidth=2)
        axs[1].axvline(x=peak, color='red', linestyle='--', alpha=0.8, linewidth=2)
    
    plt.xlabel('Minták indexe (100ms/minta @ 10Hz)')
    plt.suptitle(title)
    plt.tight_layout()
    plt.show()

def run_parameter_search(df, target_reps=None, min_reps=6, max_reps=12):
    """
    Paraméterkereső edzés ismétlések detektálásához (6-14 tartomány)
    target_reps: ha konkrét ismétlésszámot tudunk, azt preferálja
    min_reps, max_reps: elfogadható ismétlés tartomány
    """
    if target_reps:
        print(f"Paraméterhangolás fut {target_reps} ismétlésre (elfogadható: {min_reps}-{max_reps})...\n")
    else:
        print(f"Paraméterhangolás fut {min_reps}-{max_reps} ismétlés tartományra...\n")
    
    results = []
    
    for distance in range(8, 31, 2):  # 8-30 lépésköz 2 (finomabb hangolás)
        for prominence in [0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.2, 1.5]:
            peaks = detect_repetitions_bottom_peaks(df['GyrosZ'], distance, prominence)
            count = len(peaks)
            
            # Pontozási rendszer
            score = 0
            status = "❌"
            
            if min_reps <= count <= max_reps:
                status = "✅"
                # Alappontszám ha a tartományban van
                score = 100
                
                # Bónusz pontok ha közel van a célhoz
                if target_reps:
                    distance_from_target = abs(count - target_reps)
                    score += max(0, 50 - distance_from_target * 10)
                
                # Bónusz a közepes paraméterekért (stabilabb detektálás)
                if 15 <= distance <= 25:
                    score += 10
                if 0.2 <= prominence <= 0.4:
                    score += 10
            
            print(f"{status} distance={distance:2}, prominence={prominence:.2f} → {count:2} ismétlés (score: {score:3})")
            
            results.append({
                'distance': distance,
                'prominence': prominence,
                'count': count,
                'peaks': peaks,
                'score': score
            })
    
    # Legjobb eredmény kiválasztása pontszám alapján
    if results:
        best_result = max(results, key=lambda x: x['score'])
        if best_result['score'] > 0:
            return (best_result['distance'], best_result['prominence'], best_result['peaks'])
    
    return None


def resample_rep(df, target_len=14):
    old_len = len(df)
    if old_len == target_len:
        return df.round(5)
    
    old_indices = np.linspace(0, 1, old_len)
    new_indices = np.linspace(0, 1, target_len)
    
    new_df = pd.DataFrame()
    for col in df.columns:
        new_df[col] = np.interp(new_indices, old_indices, df[col])
    return new_df.round(5)


def export_repetitions_to_json(df, rep_peaks, correct=True, filename="labeled_reps.json", target_len=14):
    reps_data = []

    for i in range(len(rep_peaks) - 1):
        start_idx = rep_peaks[i]
        end_idx = rep_peaks[i + 1]
        rep_df = df.iloc[start_idx:end_idx]

        rep_df_subset = rep_df[['GyrosX', 'GyrosY', 'GyrosZ', 'AccelX', 'AccelY', 'AccelZ']]
        rep_df_resampled = resample_rep(rep_df_subset, target_len=target_len)

        rep_dict = {
            "index_range": [int(start_idx), int(end_idx)],
            "sensor_data": rep_df_resampled.to_dict(orient="records"),
            "correct": correct
        }

        reps_data.append(rep_dict)

    with open(filename, "w") as f:
        json.dump(reps_data, f, indent=2)

    print(f"✅ {len(reps_data)} ismétlés elmentve a(z) {filename} fájlba.")

def plot_all_resampled_reps(filename="labeled_reps.json"):
    with open(filename, "r") as f:
        data = json.load(f)

    all_reps = []

    for rep in data:
        sensor_data = rep["sensor_data"]
        df = pd.DataFrame(sensor_data)
        df = df.apply(pd.to_numeric, errors='coerce')
        all_reps.append(df)

    full_df = pd.concat(all_reps, ignore_index=True)

    # Elkülönítjük az oszlopokat
    gyro_cols = ['GyrosX', 'GyrosY', 'GyrosZ']
    accel_cols = ['AccelX', 'AccelY', 'AccelZ']

    fig, axs = plt.subplots(2, 1, figsize=(14, 8), sharex=True)


        # Accelerometer subplot
    for col in accel_cols:
        axs[0].plot(full_df[col], label=col)
    axs[0].set_title("📈 Accelerometer adatok (összefűzött ismétlések)")
    axs[0].set_xlabel("Idő (összefűzött minták)")
    axs[0].set_ylabel("Érték")
    axs[0].legend()
    axs[0].grid(True)

    # Gyroscope subplot
    for col in gyro_cols:
        axs[1].plot(full_df[col], label=col)
    axs[1].set_title("📈 Gyroscope adatok (összefűzött ismétlések)")
    axs[1].set_ylabel("Érték")
    axs[1].legend()
    axs[1].grid(True)


    plt.tight_layout()
    plt.show()



if __name__ == "__main__":
    df = load_data("adatok.json")
    
    # Kidobjuk a dátumot, ha van
    if 'Date' in df.columns:
        df = df.drop(columns=['Date'])
    # Kiszűrjük azokat a sorokat, ahol mind a 6 oszlop értéke 0
    sensor_cols = ['AccelX', 'AccelY', 'AccelZ', 'GyrosX', 'GyrosY', 'GyrosZ']
    df = df[~(df[sensor_cols] == 0).all(axis=1)]
   
    df = df.dropna(subset=['GyrosZ'])
    print(f"Összes mintaszám: {len(df)} (kb {len(df)/10:.1f} másodperc @ 10Hz)\n")
    
    # Ha tudod hány ismétlést csináltál, add meg itt:
    known_reps = 8  # Változtasd meg vagy állítsd None-ra automatikus detektáláshoz
    
    if known_reps:
        # Kalibrálás ismert ismétlésszámmal
        print(f"🎯 Kalibrálás {known_reps} ismétlésre...")
        best = run_parameter_search(df, target_reps=known_reps)
        
        if best:
            distance, prominence, rep_peaks = best
            rep_peaks = rep_peaks[1:]
            print(f"\n🎯 Kalibrált paraméterek: distance={distance}, prominence={prominence}")
            print(f"📊 Detektált ismétlések: {len(rep_peaks)}")
            plot_sensor_data(df, rep_peaks, 
                           title=f"Kalibrált detektálás - {len(rep_peaks)} ismétlés")
            export_repetitions_to_json(df, rep_peaks, correct=True)
            plot_all_resampled_reps("labeled_reps.json")