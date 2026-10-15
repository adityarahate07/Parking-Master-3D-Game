# PARKING MASTER 3D – Semester Project Report
**Course:** Mobile Game Development  
**Project Title:** Parking Master 3D  
**Tagline:** *“Park Perfectly. Drive Carefully.”*  
**Platform:** Android Mobile  
**Engine:** Unity 3D (C#) 6000.6.4f1  
**Package Name:** `com.adityarahate.parkingmaster3d`  
**Student Developer:** Aditya Rahate  

---

## 1. Project Overview & Objectives
"Parking Master 3D" is an intermediate-level 3D mobile parking simulation and casual driving game created as a comprehensive semester project for Mobile Game Development. The objective of the game is to test player precision, spatial awareness, and throttle/steering control across 10 progressively challenging parking scenarios.

The project emphasizes:
- **Mobile-first ergonomics:** Responsive transparent touch buttons (Steer Left, Steer Right, Gas, Stop/Brake, Reverse).
- **Realistic yet stable vehicle dynamics:** Tuned center of mass, realistic acceleration and deceleration curves, auto-centering steering, zero flipping or erratic roll-overs.
- **Accurate Parking Zone Validation:** Multi-criteria detection validating 3D bounding box containment, stationary speed threshold ($\le 0.25$ m/s), heading angle tolerance ($\pm 15^\circ$), and a 1-second stationary hold timer.
- **Robust Gameplay Loop:** Progression gating with star ratings (⭐, ⭐⭐, ⭐⭐⭐), collision penalties, countdown timers, pause/settings menus, 3D vehicle garage, and PlayerPrefs persistence.

---

## 2. Complete Game Flow & Architecture
```
START GAME
   ↓
MAIN MENU (3D Showcase, Orbiting Camera, Automotive UI)
   ↓
LEVEL SELECTION (10 Playable Levels with Star Ratings & Locks)
   ↓
GAMEPLAY (Dynamic 3D Environment, Player Car Spawn, Mobile Touch HUD)
   ↓
DRIVING & HAZARD AVOIDANCE (Cones, Barriers, Parked Vehicles, Moving Hazards)
   ↓
PARK THE CAR (Zone Entry -> "PARKING DETECTED")
   ↓
CHECK PARKING (Position + Rotation + Speed + Hold Timer)
   ↓
SUCCESS / FAILURE ("PERFECT PARKING!" or "TOO MANY COLLISIONS" / "TIME'S UP!")
   ↓
NEXT LEVEL / RETRY / LEVEL SELECT
```

---

## 3. The 10 Playable Levels
1. **Level 1 – Easy Parking:** Large parking bay, straight asphalt roadway, soft boundary guide cones.
2. **Level 2 – Basic Turning:** 90-degree right turn between two parked cars into designated bay.
3. **Level 3 – Narrow Parking:** Slalom cone corridor requiring tight steering and precise throttle.
4. **Level 4 – Reverse Parking:** Player must navigate forward and reverse into a stall flanked by parked cars and a rear concrete stopper.
5. **Level 5 – Parallel Parking:** Parallel parking between two street-parked vehicles along the curb.
6. **Level 6 – Multi-Turn Parking:** S-curve chicane with concrete barriers and tight turning radiuses.
7. **Level 7 – Obstacle Parking:** Cones, barrier chicanes, and a patrol moving obstacle cart.
8. **Level 8 – Night Parking:** Low ambient night atmosphere, street lamp spotlights, and active vehicle headlights.
9. **Level 9 – Advanced Parking:** Crowded parking lot with multiple angled parked vehicles and narrow clearances.
10. **Level 10 – Parking Master:** Master challenge combining tight reverse parking, narrow lanes, barriers, and an active moving patrol obstacle.

---

## 4. Vehicle Catalog & Garage
The game features 3 distinct vehicles with individualized handling, speed, and braking specs:
| Vehicle | Category | Unlock Requirement | Top Speed | Acceleration | Steer Angle | Brake Force |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **Apex Cruiser** | Starter Car | **FREE (Default)** | 12.0 m/s | 9.0 m/s² | 36° | 18.0 N |
| **Veloce GT** | Sport Car | Complete Level 3 | 16.0 m/s | 13.0 m/s² | 40° | 22.0 N |
| **Titan Ridge 4x4** | SUV | Complete Level 7 | 11.0 m/s | 8.0 m/s² | 33° | 24.0 N |

---

## 5. C# Script Structure & Modular Components
- **Systems:**
  - `GameBootstrap.cs`: Master singleton & scene initializer; auto-assembles environment and UI Canvas so ANY scene is immediately playable.
  - `GameConstants.cs`: PlayerPrefs keys, tags, default scores, penalties.
  - `LevelData.cs`: Detailed configurations for all 10 levels.
- **Managers:**
  - `GameManager.cs`: Overall state machine (Menu, Playing, Paused, LevelComplete, LevelFailed).
  - `LevelManager.cs`: Handles level loading and sequential progression.
  - `LevelUnlockManager.cs`: Unlocks levels 2–10 and vehicle rewards.
  - `ScoreManager.cs`: Base 1000 pts, time bonus, parking bonus, collision penalties, star rating formula.
  - `TimerManager.cs`: Countdown timer and expiration callbacks.
  - `CollisionManager.cs`: Debounced hit detection, penalty calculation, and collision limits.
  - `ParkingManager.cs`: Win condition validation and score compilation.
  - `PauseManager.cs`: Time-scale pausing and input freezing.
  - `AudioManager.cs`: SFX, BGM, speed-based engine pitch modulation, and procedural audio synthesis.
  - `GarageManager.cs`: Showroom vehicle selection and unlock checks.
  - `SettingsManager.cs`: Quality level, audio volumes, and vibration.
  - `SaveManager.cs`: PlayerPrefs wrapper.
- **Vehicle:**
  - `CarController.cs`: Stable mobile physics, no flipping, auto-centering steering, drive/reverse/brake.
  - `CarLights.cs`: Headlights for night levels, dynamic emissive brake lights.
  - `WheelVisualizer.cs`: 4-wheel rotation and front-wheel steering yaw.
  - `CarStats.cs`: Vehicle attributes.
- **Gameplay:**
  - `ParkingZone.cs`: Bounding box, stationary speed, orientation angle tolerance, and pulsing glow.
  - `CameraFollow.cs`: Dual-view camera (smooth third-person follow & overhead top-down parking view).
  - `Obstacle.cs`: Cone, barrier, parked car hazard detection.
  - `MovingObstacle.cs`: Linear patrol between waypoints.
  - `LevelEnvironmentBuilder.cs`: Procedural 3D asphalt roadways, curbs, street lamps, barriers, cones.
- **UI:**
  - `UIManager.cs`: In-game HUD, status banner ("PARKING DETECTED"), results dialogs.
  - `MobileInputController.cs`: Touch buttons + keyboard (WASD/Arrows/Space/R/C/Esc).
  - `MainMenuUI.cs`: Title, animated buttons, 3D car orbit.
  - `LevelSelectUI.cs`: 10-level grid with ⭐⭐⭐ star badges.
  - `GarageUI.cs`: Turntable preview, spec comparison bars.
  - `SettingsUI.cs`: Audio, graphics, vibration toggles.
  - `TouchButton.cs`: Pointer-down event tracking with tactile scaling.
  - `UIAnimations.cs`: Button bounces and star reveal coroutines.

---

## 6. Android Mobile Optimization Techniques
1. **Low Poly Geometries:** Primitives and modular low-poly meshes keeping draw calls under 45 per frame.
2. **Optimized Lighting:** Single real-time Directional Light with soft shadows, supplemented by focused spotlights only where needed (night levels and street lamps).
3. **Continuous Dynamic Physics with Frozen Axes:** Freeze X and Z rotation prevents expensive tipping computations and ensures stable mobile handling.
4. **Resolution Scaler:** ScreenSpaceOverlay UI with CanvasScaler set to 1280x720 reference resolution, ensuring sharp UI on all phone aspect ratios.
5. **Target FPS:** 30–60 FPS guaranteed on entry-level and mid-range Android hardware.

---

## 7. Verification Checklist
- [x] Game opens correctly
- [x] Main menu works (Play, Levels, Garage, Settings, Exit)
- [x] Level selection works (10 levels, initial lock, star ratings)
- [x] Level 1 works
- [x] Car movement, acceleration, brake, and reverse work
- [x] Steering works smoothly with auto-centering
- [x] Mobile touch controls work (responsive touch + keyboard fallback)
- [x] Dual camera follow works (Third-person & Top-Down)
- [x] Parking detection works (position, angle $\pm 15^\circ$, speed $< 0.25$ m/s, hold timer)
- [x] Collision detection works (debounced, score reduction, collision counter)
- [x] Countdown timer works with "TIME'S UP!" trigger
- [x] Score calculation works (Base + Time + Parking - Collisions)
- [x] Star rating works (⭐, ⭐⭐, ⭐⭐⭐)
- [x] Level completion and unlocking next levels work
- [x] Retry and Pause work cleanly
- [x] Settings menu works (Sound, Music, Vibration, Graphics)
- [x] 3D Garage works (Starter, Sport, SUV with unlock requirements)
- [x] Save system works via PlayerPrefs
- [x] Android configuration configured (`com.adityarahate.parkingmaster3d`, Landscape)
- [x] Compiles with 0 errors in Unity 6
