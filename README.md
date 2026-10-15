# Parking Master 3D
🚗 Parking Master 3D — A 3D parking game where you steer, reverse, and brake through challenging levels. 🅿️ Avoid collisions, beat the timer, and earn points! 🎮

> *“Park Perfectly. Drive Carefully.”*

**Intermediate-Level College Semester Project**  
**Course:** Mobile Game Development  
**Target Platform:** Android Mobile (3D)  
**Engine:** Unity 3D (v6000.6.4f1)  
**Language:** C#  
**Package Name:** `com.adityarahate.parkingmaster3d`  
**Orientation:** Landscape  

---

## 🚀 Quick Start Guide

### Option 1: Open in Unity Editor (Recommended for Teacher/Grading)
1. Double-click `Launch_ParkingMaster_Unity.bat` in the project root or open **Unity Hub**.
2. Click **Add** -> Select the `ParkingMaster3D` directory.
3. Open the project with Unity 6 (`6000.6.4f1`).
4. In the Project window, navigate to `Assets/Scenes/` and open `MainMenu.unity` (or any level scene `Level01.unity` through `Level10.unity`).
5. Press the **Play ▶** button at the top of the Unity Editor.
6. The `GameBootstrap` system will automatically assemble the entire 3D environment, vehicle physics, audio, and UI canvas.

### Option 2: Instant Playable Web Companion
1. Open `ParkingMaster3D/WebPlayable/index.html` in any web browser (Chrome, Edge, Firefox, Safari).
2. Enjoy the complete 3D simulation with all 10 levels, 3 vehicles, sound synthesis, and touch/keyboard controls without needing any setup.

---

## 🎮 Game Controls

### Mobile Touch Controls (Android)
- **Bottom Left:** 
  - `◀` : Steer Left
  - `▶` : Steer Right
- **Bottom Right:**
  - `GAS` : Accelerate Forward
  - `STOP` : Brake / Handbrake
  - `REV` : Reverse Gear
- **Top Bar:**
  - `📷` : Toggle Camera View (Third-Person Chase vs Top-Down Aerial)
  - `⏸` : Pause Game

### Keyboard Controls (Unity Editor & Web Testing)
- `W` or `Up Arrow` : Accelerate Forward
- `S` or `Down Arrow` : Reverse Gear / Brake
- `A` or `Left Arrow` : Steer Left
- `D` or `Right Arrow` : Steer Right
- `Space` : Handbrake
- `C` : Toggle Camera View
- `Escape` : Pause / Resume

---

## 🏁 The 10 Playable Levels
1. **Level 1 – Easy Parking:** Wide asphalt roadway, large parking bay, guide cones.
2. **Level 2 – Basic Turning:** 90-degree right turn between parked cars into stall.
3. **Level 3 – Narrow Parking:** Slalom cone corridor demanding steering precision.
4. **Level 4 – Reverse Parking:** Reverse backwards into parking bay with rear barrier.
5. **Level 5 – Parallel Parking:** Parallel parking along sidewalk curb between two cars.
6. **Level 6 – Multi-Turn Parking:** S-curve chicane with concrete barriers.
7. **Level 7 – Obstacle Parking:** Cones, barrier maze, and a moving obstacle cart.
8. **Level 8 – Night Parking:** Dark environment, street lamp spotlights, and active vehicle headlights.
9. **Level 9 – Advanced Parking:** Tight parking spot crowded by multiple vehicles.
10. **Level 10 – Parking Master:** Master challenge combining tight reverse parking and moving hazards.

---

## 🚗 Vehicle Garage
- **Apex Cruiser (Starter Car):** FREE / Unlocked by default. Balanced handling and smooth acceleration.
- **Veloce GT (Sport Car):** Unlocked upon completing **Level 3**. High acceleration and sharper steering.
- **Titan Ridge 4x4 (SUV):** Unlocked upon completing **Level 7**. High braking power and heavier presence.

---

## 🏆 Scoring & Star Rating System
- **Base Score:** 1000 points
- **Time Bonus:** +10 points per remaining second
- **Parking Bonus:** +300 points for perfect angle & center alignment
- **Collision Penalty:** -100 points per collision
- **Star Rating:**
  - ⭐⭐⭐ (3 Stars): Zero collisions, completed in under 65% of time limit, high parking accuracy.
  - ⭐⭐ (2 Stars): 1 collision or medium completion time.
  - ⭐ (1 Star): 2+ collisions or completed close to the countdown expiry.

---

## ⚙ Project Architecture
```
ParkingMaster3D/
├── Assets/
│   ├── Scenes/          # MainMenu, LevelSelect, Garage, Level01 - Level10
│   ├── Scripts/
│   │   ├── Systems/     # GameBootstrap.cs, LevelData.cs, GameConstants.cs
│   │   ├── Managers/    # GameManager.cs, LevelManager.cs, ScoreManager.cs, etc.
│   │   ├── Vehicle/     # CarController.cs, CarLights.cs, WheelVisualizer.cs
│   │   ├── Gameplay/    # ParkingZone.cs, Obstacle.cs, CameraFollow.cs, etc.
│   │   ├── UI/          # UIManager.cs, MobileInputController.cs, TouchButton.cs, etc.
│   │   └── Editor/      # ParkingMasterEditorMenu.cs (Toolbar cheat & setup tools)
│   ├── Prefabs/         # Vehicles, Obstacles, Environment, UI
│   ├── Materials/       # 28 Mobile-optimized materials
│   └── Audio/           # 11 PCM .wav sound effects & BGM
├── Packages/            # Clean Unity 6 package manifest
├── ProjectSettings/     # Android landscape, input axes, quality tiers, tags
├── WebPlayable/         # Standalone 3D companion preview (index.html)
└── Launch_ParkingMaster_Unity.bat
```

---

## 📱 Android Build Configuration
1. Open **File** -> **Build Settings...** in Unity.
2. Select **Android** platform and click **Switch Platform**.
3. All 13 scenes are pre-configured in `EditorBuildSettings.asset`.
4. Verify **Player Settings**:
   - Company Name: `AdityaRahate`
   - Product Name: `Parking Master 3D`
   - Package Name: `com.adityarahate.parkingmaster3d`
   - Default Orientation: `Landscape`
5. Click **Build** to generate the Android `.apk`.
