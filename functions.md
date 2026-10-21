# 🏀 "SUSTAINABLE FUTURE" — Key Functions Reference Guide
## Comprehensive Doc-Type Function Documentation & Architectural Explanation

This reference document explains the **most important functions** in the project across physics, game loops, scoring logic, data management, UI, and procedural audio.

---

## 📑 Table of Contents
1. [Core Shooting & Physics Engine](#1-core-shooting--physics-engine)
   - [`BasketballShooter.HandleShootingInput()`](#11-basketballshooterhandleshootinginput)
   - [`BasketballShooter.CalculateShootVelocity()`](#12-basketballshootercalculateshootvelocity)
   - [`BasketballShooter.RenderTrajectory()`](#13-basketballshooterrendertrajectory)
   - [`Basketball.Launch()`](#14-basketballlaunch)
   - [`HoopController.OnUpperSensorTriggered()` & `OnLowerSensorTriggered()`](#15-hoopcontroller-dual-sensor-validation)
2. [Game Loop & Sustainability Decision Matrix](#2-game-loop--sustainability-decision-matrix)
   - [`ItemManager.BuildAndShuffleDeck()`](#21-itemmanagerbuildandshuffledeck)
   - [`ItemManager.HandleGoalScored()`](#22-itemmanagerhandlegoalscored)
   - [`ItemManager.HandleDiscardPressed()`](#23-itemmanagerhandlediscardpressed)
   - [`GameManager.StartGame()` & `EndGame()`](#24-gamemanagerstartgame--endgame)
3. [Player Statistics & Accuracy Tracking](#3-player-statistics--accuracy-tracking)
   - [`StatsManager.HandleItemEvaluated()`](#31-statsmanagerhandleitemevaluated)
   - [`StatsManager.SaveStats()` & `LoadStats()`](#32-statsmanagersavestats--loadstats)
4. [User Interface & Visual Feedback](#4-user-interface--visual-feedback)
   - [`UIItemCard.DisplayItem()`](#41-uiitemcarddisplayitem)
   - [`UIManager.ShowFloatingPopup()`](#42-uimanagershowfloatingpopup)
   - [`UIResultsModal.ShowResults()`](#43-uiresultsmodalshowresults)
5. [Procedural Audio Synthesis](#5-procedural-audio-synthesis)
   - [`SoundManager.GenerateToneClip()`](#51-soundmanagergeneratetoneclip)
   - [`SoundManager.PlayProceduralSound()`](#52-soundmanagerplayproceduralsound)
6. [Global Decoupled Event Bus](#6-global-decoupled-event-bus)
   - [`GameEvents.Trigger...()`](#61-gameevents-static-event-triggers)

---

## 1. Core Shooting & Physics Engine

---

### 1.1 `BasketballShooter.HandleShootingInput()`
📁 **File:** [`BasketballShooter.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Gameplay/BasketballShooter.cs#L143-L195)

```csharp
/// <summary>
/// Detects drag start, drag progress, and finger/mouse release using Unity's New Input System.
/// Updates the real-time parabolic trajectory line and launches the ball on release.
/// </summary>
private void HandleShootingInput()
```

- **Parameters:** None (reads from `UnityEngine.InputSystem.Mouse.current` and `Touchscreen.current`).
- **Returns:** `void`
- **What It Does:**
  1. Checks if the player clicks or touches the screen (ignoring UI clicks).
  2. Tracks the drag vector $\Delta = \text{CurrentPosition} - \text{StartPosition}$.
  3. Draws a curved trajectory preview line in real-time.
  4. On release, if the swipe distance is greater than `minSwipeDistance` ($25\text{ px}$), it calculates velocity and launches the basketball.
- **In Plain English:** This function translates the player's flick gesture on screen into the ball's flight path.

---

### 1.2 `BasketballShooter.CalculateShootVelocity()`
📁 **File:** [`BasketballShooter.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Gameplay/BasketballShooter.cs#L225-L255)

```csharp
/// <summary>
/// Converts a 2D screen drag vector into a 3D physical velocity vector pointing toward the hoop.
/// </summary>
/// <param name="dragDelta">The 2D pixel delta between swipe start and swipe end points.</param>
/// <param name="dragDuration">The time elapsed (in seconds) during the swipe.</param>
/// <returns>A 3D Vector3 velocity containing forward, vertical arc, and lateral deflection forces.</returns>
private Vector3 CalculateShootVelocity(Vector2 dragDelta, float dragDuration)
```

- **Parameters:**
  - `Vector2 dragDelta`: The distance and direction the player swiped on the screen.
  - `float dragDuration`: How fast the swipe occurred.
- **Returns:** `Vector3` (Initial linear velocity in world space $\text{m/s}$).
- **What It Does:**
  1. Clamps swipe length between `minSwipeDistance` ($25\text{ px}$) and `maxSwipeDistance` ($400\text{ px}$).
  2. Calculates normalized force factor $t \in [0, 1]$.
  3. Interpolates forward force ($5.6\text{ to }8.8\text{ m/s}$) and upward arc force ($6.4\text{ to }10.2\text{ m/s}$).
  4. Adds horizontal tilt ($X$-axis) based on the swipe angle.
- **In Plain English:** Decides how hard, how high, and in what direction the basketball is thrown based on your finger flick.

---

### 1.3 `BasketballShooter.RenderTrajectory()`
📁 **File:** [`BasketballShooter.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Gameplay/BasketballShooter.cs#L257-L278)

```csharp
/// <summary>
/// Computes and renders a 3D parabolic trajectory curve using the standard physics equation of motion.
/// </summary>
/// <param name="startPos">The starting 3D world position of the basketball.</param>
/// <param name="initialVelocity">The calculated launch velocity vector.</param>
private void RenderTrajectory(Vector3 startPos, Vector3 initialVelocity)
```

- **Parameters:**
  - `Vector3 startPos`: Launch point in world space.
  - `Vector3 initialVelocity`: 3D velocity vector.
- **Returns:** `void`
- **What It Does:**
  Uses the kinematic formula for 22 sample points along time $t$:
  $$\vec{P}(t) = \vec{P}_0 + \vec{V}_0 t + \frac{1}{2}\vec{g}t^2$$
  Feeds these 3D points directly into `LineRenderer` to draw a dotted aiming line.
- **In Plain English:** Shows the player the predicted flight arc *before* releasing the ball.

---

### 1.4 `Basketball.Launch()`
📁 **File:** [`Basketball.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Gameplay/Basketball.cs#L75-L88)

```csharp
/// <summary>
/// Activates ball physics, applies initial velocity, and adds backspin rotation.
/// </summary>
/// <param name="velocity">The 3D linear velocity applied to the Rigidbody.</param>
/// <param name="spinForce">The angular rotational speed (backspin) applied around the X-axis.</param>
public void Launch(Vector3 velocity, float spinForce)
```

- **Parameters:**
  - `Vector3 velocity`: Launch velocity vector.
  - `float spinForce`: Backspin torque multiplier.
- **Returns:** `void`
- **What It Does:**
  1. Sets `Rigidbody.isKinematic = false`.
  2. Sets `Rigidbody.linearVelocity = velocity`.
  3. Adds angular velocity `Vector3.right * spinForce` for realistic aerodynamic backspin.
  4. Starts an auto-reset countdown timer ($4.0\text{ s}$) to recycle the ball back into the pool.
- **In Plain English:** Throws the physical ball into the world and applies basketball spin.

---

### 1.5 `HoopController` Dual-Sensor Validation
📁 **File:** [`HoopController.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Gameplay/HoopController.cs#L50-L80)

```csharp
/// <summary>
/// Step 1: Detects ball entry through the top of the rim.
/// </summary>
public void OnUpperSensorTriggered(Basketball ball)

/// <summary>
/// Step 2: Validates exit through the bottom within 1.2s of Step 1, registering a genuine score.
/// </summary>
public void OnLowerSensorTriggered(Basketball ball)
```

- **What It Does:**
  - `OnUpperSensorTriggered`: Checks if the ball is moving downwards ($V_y \le 0.1$). Records entry timestamp `upperTriggerEnterTime = Time.time`.
  - `OnLowerSensorTriggered`: Confirms the same ball exited within $1.2\text{ s}$. If valid, triggers `GameEvents.TriggerBasketballGoalScored()`, spawns goal particle FX, and animates net swish.
- **In Plain English:** Prevents cheating by ensuring the ball went **top $\rightarrow$ bottom** through the hoop instead of flying up from underneath.

---

## 2. Game Loop & Sustainability Decision Matrix

---

### 2.1 `ItemManager.BuildAndShuffleDeck()`
📁 **File:** [`ItemManager.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Gameplay/ItemManager.cs#L74-L95)

```csharp
/// <summary>
/// Populates the item list from ItemDatabase and randomizes it using the Fisher-Yates shuffle algorithm.
/// </summary>
private void BuildAndShuffleDeck()
```

- **Parameters:** None
- **Returns:** `void`
- **What It Does:**
  1. Copies all 31 items from `ItemDatabase.asset`.
  2. Performs an $O(n)$ in-place Fisher-Yates swap from last index to first index:
     $$j = \text{Random}(0, i+1); \quad \text{Swap}(\text{deck}[i], \text{deck}[j])$$
  3. Resets `currentDeckIndex = 0`.
- **In Plain English:** Shuffles all 31 eco-items like a deck of cards so every game round has a randomized order without repeating until the deck ends.

---

### 2.2 `ItemManager.HandleGoalScored()`
📁 **File:** [`ItemManager.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Gameplay/ItemManager.cs#L125-L151)

```csharp
/// <summary>
/// Evaluates the educational outcome when the player makes a basket.
/// </summary>
private void HandleGoalScored()
```

- **Parameters:** None (invoked by `GameEvents.OnBasketballGoalScored`).
- **Returns:** `void`
- **What It Does:**
  - If current item is **Sustainable** (Correct choice to keep/shoot):
    $$\text{Score} \mathrel{+}= 10, \quad \text{Streak} \mathrel{+}= 1, \quad \text{Trigger Correct Audio/Visuals}$$
  - If current item is **Non-Sustainable** (Mistake — should have discarded!):
    $$\text{Score} \mathrel{-}= 10, \quad \text{Streak} = 0, \quad \text{Trigger Penalty Audio/Visuals}$$
  - Advances to the next item via `PresentNextItem()`.
- **In Plain English:** Rewards you $+10$ points for shooting good green items, but penalizes you $-10$ points if you shoot single-use plastic/pollution items.

---

### 2.3 `ItemManager.HandleDiscardPressed()`
📁 **File:** [`ItemManager.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Gameplay/ItemManager.cs#L153-L178)

```csharp
/// <summary>
/// Evaluates the educational outcome when the player clicks the DISCARD button.
/// </summary>
private void HandleDiscardPressed()
```

- **Parameters:** None (invoked by `GameEvents.OnDiscardPressed`).
- **Returns:** `void`
- **What It Does:**
  - If current item is **Non-Sustainable** (Correct choice to discard):
    $$\text{Score} \mathrel{+}= 10, \quad \text{Streak} \mathrel{+}= 1, \quad \text{Trigger Discard Audio & Success Popup}$$
  - If current item is **Sustainable** (Mistake — discarded an eco-friendly solution!):
    $$\text{Score} \mathrel{-}= 10, \quad \text{Streak} = 0, \quad \text{Trigger Penalty Audio & Popup}$$
  - Advances to the next item via `PresentNextItem()`.
- **In Plain English:** Rewards you $+10$ points for discarding trash/pollutants into recycling, but penalizes you if you throw away sustainable items like solar panels or bicycles.

---

### 2.4 `GameManager.StartGame()` & `EndGame()`
📁 **File:** [`GameManager.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Core/GameManager.cs#L70-L125)

```csharp
/// <summary>
/// Resets round score, initializes 30-second timer, and broadcasts GameStarted event.
/// </summary>
public void StartGame()

/// <summary>
/// Stops input, triggers buzzer audio, saves statistics, and displays final results modal.
/// </summary>
public void EndGame()
```

- **In Plain English:** Controls the 30-second arcade timer, round start, and game over screen.

---

## 3. Player Statistics & Accuracy Tracking

---

### 3.1 `StatsManager.HandleItemEvaluated()`
📁 **File:** [`StatsManager.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Core/StatsManager.cs#L135-L162)

```csharp
/// <summary>
/// Records decision results and recalculates lifetime stats and accuracy percentage.
/// </summary>
/// <param name="item">The item that was evaluated.</param>
/// <param name="wasCorrect">True if player made the correct educational choice.</param>
/// <param name="pointsDelta">Point change (+10 or -10).</param>
private void HandleItemEvaluated(SustainabilityItem item, bool wasCorrect, int pointsDelta)
```

- **Parameters:**
  - `SustainabilityItem item`: Item evaluated.
  - `bool wasCorrect`: Success flag.
  - `int pointsDelta`: Points awarded or deducted.
- **Returns:** `void`
- **What It Does:**
  1. Increments `TotalAttempts++`.
  2. If correct, increments `TotalCorrect++`.
  3. Updates dynamic accuracy:
     $$\text{Accuracy} = \left(\frac{\text{TotalCorrect}}{\text{TotalAttempts}}\right) \times 100\%$$
  4. Updates current round baskets count if points were gained through a shot.
  5. Updates all UI stats via `GameEvents.TriggerStatsRefreshed()`.
- **In Plain English:** Keeps a real-time ledger of how many items you answered correctly to compute your percentage accuracy.

---

### 3.2 `StatsManager.SaveStats()` & `LoadStats()`
📁 **File:** [`StatsManager.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Core/StatsManager.cs#L75-L105)

```csharp
/// <summary>
/// Reads lifetime bests, games played, and accuracy stats from PlayerPrefs on startup.
/// </summary>
private void LoadStats()

/// <summary>
/// Writes updated high scores, best baskets, and lifetime totals to persistent storage.
/// </summary>
private void SaveStats()
```

- **In Plain English:** Ensures your high scores, best streaks, and total baskets are remembered even when you close and reopen the game.

---

## 4. User Interface & Visual Feedback

---

### 4.1 `UIItemCard.DisplayItem()`
📁 **File:** [`UIItemCard.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/UI/UIItemCard.cs#L60-L95)

```csharp
/// <summary>
/// Populates the lower-left vertical challenge card with item photo, name, category badge, and fact.
/// </summary>
/// <param name="item">The SustainabilityItem ScriptableObject data model to display.</param>
public void DisplayItem(SustainabilityItem item)
```

- **Parameters:** `SustainabilityItem item` (ScriptableObject containing Sprite, name, type, and fact string).
- **Returns:** `void`
- **What It Does:**
  1. Assigns `itemImage.sprite = item.ItemImage`.
  2. Sets `itemNameText.text = item.ItemName.ToUpper()`.
  3. Sets category badge text & color:
     - Sustainable: Emerald Green (`#2ecc71`) with text `"SUSTAINABLE ITEM"`.
     - Non-Sustainable: Coral Red (`#e74c3c`) with text `"NON-SUSTAINABLE ITEM"`.
  4. Sets educational fact description text.
- **In Plain English:** Updates the card on the bottom-left of the screen to show the current item you need to judge.

---

### 4.2 `UIManager.ShowFloatingPopup()`
📁 **File:** [`UIManager.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/UI/UIManager.cs#L145-L165)

```csharp
/// <summary>
/// Spawns an animated floating text popup (+10 PTS in green or -10 PTS in red) over the screen.
/// </summary>
/// <param name="text">The message string (e.g. "+10 SUSTAINABLE!").</param>
/// <param name="color">The text color (Green for positive, Red for negative).</param>
public void ShowFloatingPopup(string text, Color color)
```

- **In Plain English:** Shows an arcade-style pop-up text that floats up and fades away when you score or make a mistake.

---

### 4.3 `UIResultsModal.ShowResults()`
📁 **File:** [`UIResultsModal.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/UI/UIResultsModal.cs#L50-L85)

```csharp
/// <summary>
/// Displays the game-over dialog showing final score, baskets made, accuracy %, and a custom message.
/// </summary>
/// <param name="finalScore">Total points earned during the round.</param>
/// <param name="baskets">Total successful baskets scored.</param>
/// <param name="accuracy">Calculated accuracy percentage.</param>
/// <param name="isNewBest">True if the player broke their previous high score.</param>
public void ShowResults(int finalScore, int baskets, float accuracy, bool isNewBest)
```

- **In Plain English:** Opens the round-end summary window showing how well you did with a "Play Again" button.

---

## 5. Procedural Audio Synthesis

---

### 5.1 `SoundManager.GenerateToneClip()`
📁 **File:** [`SoundManager.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Audio/SoundManager.cs#L125-L165)

```csharp
/// <summary>
/// Synthesizes an uncompressed procedural AudioClip in memory without requiring external audio files.
/// </summary>
/// <param name="name">Name identifier for the generated clip.</param>
/// <param name="frequency">Base tone frequency in Hertz (e.g. 520 Hz for high ping, 140 Hz for buzz).</param>
/// <param name="duration">Duration of the sound in seconds.</param>
/// <param name="waveType">Waveform type: Sine (smooth), Square (arcade 8-bit), or Noise (swish/swoosh).</param>
/// <returns>A fully playable Unity AudioClip generated purely via math.</returns>
private AudioClip GenerateToneClip(string name, float frequency, float duration, WaveType waveType)
```

- **Parameters:**
  - `string name`: Clip name.
  - `float frequency`: Pitch in $\text{Hz}$.
  - `float duration`: Length in seconds.
  - `WaveType waveType`: Wave equation (`Sine`, `Square`, `Noise`, `Chirp`).
- **Returns:** `AudioClip`
- **What It Does:**
  Iterates through $N = \text{duration} \times 44100$ audio samples, calculates mathematical waveforms $A \sin(2\pi f t) \cdot e^{-kt}$, and creates a runtime `AudioClip.Create()`.
- **In Plain English:** Creates sound effects (swish, goal ding, buzzers, and swooshes) directly in code without needing `.wav` or `.mp3` files on disk!

---

## 6. Global Decoupled Event Bus

---

### 6.1 `GameEvents` Static Event Triggers
📁 **File:** [`GameEvents.cs`](file:///C:/Users/Dell/UERminiProject/Assets/_Game/Scripts/Core/GameEvents.cs#L1-L70)

| Event Trigger Method | Listeners | Purpose |
| :--- | :--- | :--- |
| `TriggerGameStarted()` | `GameManager`, `ItemManager`, `UIManager`, `BasketballShooter` | Starts 30s timer, shuffles deck, enables shooting input. |
| `TriggerBasketballGoalScored()` | `ItemManager`, `SoundManager`, `HoopController` | Validates shot against current item and plays swish sound. |
| `TriggerDiscardPressed()` | `ItemManager`, `SoundManager` | Validates discard against current item. |
| `TriggerItemEvaluated(item, isCorrect, pts)` | `StatsManager`, `UIManager`, `SoundManager` | Updates accuracy, floating text, and triggers score sounds. |
| `TriggerRoundFinished(score)` | `GameManager`, `UIManager`, `BasketballShooter` | Disables input, stops timer, shows results modal. |

- **In Plain English:** A radio broadcasting system. When something happens in the game (like a goal scored), it shouts it out once, and all other systems react without needing messy direct connections.

---
*Created for the **"SUSTAINABLE FUTURE"** 3D Arcade Basketball Game.*
