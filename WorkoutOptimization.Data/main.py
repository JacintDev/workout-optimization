import json
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from scipy.signal import find_peaks
from scipy.interpolate import interp1d

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

def run_parameter_search(df, target_reps=None, min_reps=25, max_reps=32):
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


def print_repetition_details(df, rep_peaks):
    print("\n📋 Részletes ismétlés adatok:")
    for i in range(len(rep_peaks) - 1):
        start = rep_peaks[i]
        end = rep_peaks[i + 1]
        rep_df = df.iloc[start:end]
        print(f"\n📌 Ismétlés {i+1}: index [{start}:{end}]")
        print(rep_df[['GyrosX', 'GyrosY', 'GyrosZ']].to_string(index=True))

def normalize_repetition_length_smart(df, rep_peaks, target_length=40):
    """
    Fix hosszúságú ismétlések generálása, úgy hogy:
    - az első és utolsó adat megmarad
    - downsamplingnél fontos jellemzők ne vesszenek el
    - upsamplingnél a görbe mentén interpolál
    """
    normalized_reps = []

    for i in range(len(rep_peaks) - 1):
        start = rep_peaks[i]
        end = rep_peaks[i + 1]
        rep_df = df.iloc[start:end].reset_index(drop=True)
        current_len = len(rep_df)

        # Ha pont jó a hossz
        if current_len == target_length:
            normalized = rep_df.values

        # 🔽 DOWNsampling – több minta van, mint kellene
        elif current_len > target_length:
            # Mindig megtartjuk az első és utolsó sort
            keep_idxs = [0, current_len - 1]

            # A maradékból egyenletesen kiválasztunk szükséges mennyiséget
            remaining = target_length - 2
            step = (current_len - 2) / (remaining + 1)
            additional_idxs = [int(round(step * j + 1)) for j in range(remaining)]

            selected_idxs = sorted(set(keep_idxs + additional_idxs))
            downsampled_df = rep_df.iloc[selected_idxs]
            normalized = downsampled_df.values

        # 🔼 UPsampling – kevesebb minta van, mint kellene
        else:
            # Új indexek: [0, 1, ..., target_length - 1]
            interp_idxs = np.linspace(0, current_len - 1, target_length)
            interp_df = pd.DataFrame()

            for col in rep_df.columns:
                f = interp1d(np.arange(current_len), rep_df[col], kind='cubic', fill_value="extrapolate")
                interp_df[col] = f(interp_idxs)

            normalized = interp_df.values

        normalized_reps.append(normalized)

    return np.array(normalized_reps)


def save_normalized_repetitions_to_json(normalized_data, filename, valid):
    json_data = []
    for repetition in normalized_data:
        rep_list = []
        for row in repetition:
            entry = {
                "GyrosX": float(row[0]),
                "GyrosY": float(row[1]),
                "GyrosZ": float(row[2]),
                "AccelX": float(row[3]),
                "AccelY": float(row[4]),
                "AccelZ": float(row[5])
            }
            rep_list.append(entry)
        rep_list.append({"IsCorrect": valid})  # Minden ismétlés helyesnek van jelölve
        json_data.append(rep_list)
    

    with open(filename, 'w') as f:
        json.dump(json_data, f, indent=2)

def plot_all_reps_combined(filename, rep_length=13):
    with open(filename, 'r') as f:
        data = json.load(f)

    # Összefűzött listák
    gyros_x, gyros_y, gyros_z = [], [], []
    acc_x, acc_y, acc_z = [], [], []

    for repetition in data:
        gyros_x.extend([point["GyrosX"] for point in repetition])
        gyros_y.extend([point["GyrosY"] for point in repetition])
        gyros_z.extend([point["GyrosZ"] for point in repetition])
        
        acc_x.extend([point["AccelX"] for point in repetition])
        acc_y.extend([point["AccelY"] for point in repetition])
        acc_z.extend([point["AccelZ"] for point in repetition])

    # Ábrázolás
    plt.figure(figsize=(12, 6))
    plt.suptitle("Összes ismétlés – Gyroscope & Accelerometer", fontsize=16)

    # Gyroscope
    plt.subplot(2, 1, 1)
    plt.plot(gyros_x, label="GyrosX")
    plt.plot(gyros_y, label="GyrosY")
    plt.plot(gyros_z, label="GyrosZ")
    plt.title("Gyroscope")
    plt.ylabel("°/s")
    plt.legend()
    plt.grid(True)

    # Accelerometer
    plt.subplot(2, 1, 2)
    plt.plot(acc_x, label="AccelX")
    plt.plot(acc_y, label="AccelY")
    plt.plot(acc_z, label="AccelZ")
    plt.title("Accelerometer")
    plt.ylabel("g")
    plt.xlabel("Idő (frame index)")
    plt.legend()
    plt.grid(True)

    # Függőleges vonalak az ismétlések határánál (minden rep_length mintánál)
    length = len(gyros_x)  # összes adat hossza (minden tengelynek ugyanannyi van)
    peaks = range(rep_length, length, rep_length)
    for peak in peaks:
        plt.subplot(2, 1, 1)
        plt.axvline(x=peak, color='red', linestyle='--', alpha=0.8, linewidth=1)
        plt.subplot(2, 1, 2)
        plt.axvline(x=peak, color='red', linestyle='--', alpha=0.8, linewidth=1)

    plt.tight_layout(rect=[0, 0.03, 1, 0.95])
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
    known_reps = 10# Változtasd meg vagy állítsd None-ra automatikus detektáláshoz
    
    if known_reps:
        # Kalibrálás ismert ismétlésszámmal
        print(f"🎯 Kalibrálás {known_reps} ismétlésre...")
        best = run_parameter_search(df, target_reps=known_reps, min_reps=known_reps-3, max_reps=known_reps+3)
        
        if best:
            distance, prominence, rep_peaks = best
            rep_peaks = rep_peaks[1:]
            print(f"\n🎯 Kalibrált paraméterek: distance={distance}, prominence={prominence}")
            print(f"📊 Detektált ismétlések: {len(rep_peaks)}")
            plot_sensor_data(df, rep_peaks, 
                           title=f"Kalibrált detektálás - {len(rep_peaks)} ismétlés")
            print(rep_peaks)
            print(distance)
            normalized_data = normalize_repetition_length_smart(df, rep_peaks, target_length=13)
            save_normalized_repetitions_to_json(normalized_data, "normalized_reps.json", valid=False)
            # plot_all_reps_combined("normalized_reps.json", rep_length=13)