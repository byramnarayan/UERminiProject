# 🌍 SUSTAINABLE FUTURE — 3D Arcade Basketball Game

[![Unity Version](https://img.shields.io/badge/Unity-6000.6.4f1%20(Unity%206)-blue.svg?logo=unity)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/Pipeline-URP%2017.6.0-green.svg)](https://unity.com/srp/universal-render-pipeline)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange.svg)](#)
[![License](https://img.shields.io/badge/License-Academic%20%2F%20Educational-lightgrey.svg)](#)

> **"SUSTAINABLE FUTURE"** is an interactive, fast-paced 3D arcade basketball game designed to teach environmental awareness. Players are challenged to rapidly categorize everyday items as **Sustainable** or **Non-Sustainable** through physical basketball shooting mechanics and recycling discards within a thrilling 30-second arcade round.

---

## 📸 Overview & Gameplay Screenshots

| Main Game Court & Challenge HUD | Arcade Stats & Evaluation |
|:---:|:---:|
| ![Court HUD](fig1.png) | ![Stats & Scoring](Fig2.png) |

---

## 🎮 How the Game Works

Each round lasts **30 seconds**. The game draws items from an environmental dataset (31 items covering renewable technology, biodegradable materials, single-use plastics, and pollutants).

```
                            ┌────────────────────────┐
                            │    30-SECOND ROUND     │
                            └──────────┬─────────────┘
                                       │
                                       ▼
                            ┌────────────────────────┐
                            │ PRESENT CHALLENGE ITEM │
                            │ (Photo + Title + Fact) │
                            └──────────┬─────────────┘
                                       │
                        ┌──────────────┴──────────────┐
                        ▼                             ▼
           [ SHOOT BASKETBALL 🏀 ]            [ PRESS DISCARD 🗑 ]
                        │                             │
                        ▼                             ▼
              [ Check Item Type ]            [ Check Item Type ]
            ┌───────────┴───────────┐      ┌───────────┴───────────┐
            ▼                       ▼      ▼                       ▼
      [ SUSTAINABLE ]       [ NON-SUSTAIN ][ NON-SUSTAIN ]  [ SUSTAINABLE ]
            │                       │      │                       │
         +10 PTS                 -10 PTS  +10 PTS                 -10 PTS
        Streak +1               Streak=0  Streak +1              Streak=0
```

### Decision Matrix

| Item Category | Player Action | Result | Points | Streak | Educational Feedback |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **🌱 Sustainable** | **Shoot Basket** | **GOAL** | **+10 PTS** | **+1** | *"Great job! Sustainable solutions power our future!"* |
| **🌱 Sustainable** | **Press Discard** | N/A | **-10 PTS** | **Reset (0)** | *"Think again! This item helps protect the planet!"* |
| **🚫 Non-Sustainable** | **Press Discard** | N/A | **+10 PTS** | **+1** | *"Well done! Discarding/avoiding waste saves resources!"* |
| **🚫 Non-Sustainable** | **Shoot Basket** | **GOAL** | **-10 PTS** | **Reset (0)** | *"Goal made, but this item harms the environment!"* |

---

## 🕹️ Controls

- **Mouse Flick / Touchscreen Swipe:** Click/touch and drag upwards towards the hoop to preview the dotted trajectory arc. Release to shoot the basketball.
- **DISCARD Button (Red Button in UI):** Click to discard and properly recycle non-sustainable waste.
- **PLAY AGAIN Button:** Appears on the final results modal to start a new 30-second round.

---

## 💻 How to Run the Project on Another PC

Follow these step-by-step instructions to clone, open, and run the project smoothly on any other PC (Windows, macOS, or Linux).

### Step 1: System Requirements & Prerequisites
Ensure the target machine has the following installed:
1. **[Git](https://git-scm.com/downloads)**: For cloning the repository.
2. **[Unity Hub](https://unity.com/download)**: To manage Unity editor versions and dependencies.
3. **Unity 6 Editor**:
   - Recommended version: **`6000.6.4f1`** (or any compatible Unity 6 LTS release).
   - Ensure the **Universal Render Pipeline (URP)** and **Windows / Mac / Linux Build Support** modules are checked during installation.

---

### Step 2: Clone the Repository
Open a terminal (Command Prompt, PowerShell, or Git Bash) and run:
```bash
git clone https://github.com/byramnarayan/UERminiProject.git
cd UERminiProject
```

---

### Step 3: Add and Open in Unity Hub
1. Launch **Unity Hub**.
2. Click the **Add** button (or **Add project from disk**).
3. Select the `UERminiProject` root folder.
4. If prompted to select an editor version, select **Unity 6 (`6000.6.4f1`)**.
   > *Note:* On first launch, Unity will import assets, generate local metadata, and compile package dependencies. This typically takes 1–3 minutes.

---

### Step 4: Open the Game Scene
Once the Unity Editor opens:
1. In the **Project** window at the bottom, navigate to:
   ```
   Assets / Scenes / SustainableFutureGame.unity
   ```
2. **Double-click** `SustainableFutureGame.unity` to load the complete arcade basketball scene.

---

### Step 5: Press Play and Test
1. Click the **Play button** ($\blacktriangleright$) at the top center of the Unity Editor.
2. The 30-second countdown will start immediately.
3. Use your mouse to drag and flick the basketball towards the hoop or click the **DISCARD** button depending on the item shown in the bottom-left challenge card!

---

## 🛠️ Automated 1-Click Scene Rebuilder (Editor Tool)

If you ever modify the scene, import new textures, or want to rebuild the entire hierarchy and UI bindings from scratch, a custom 1-click editor tool is included:

1. In the Unity top menu bar, click:
   $$\text{Sustainable Future} \longrightarrow \text{2. Setup Complete 3D Basketball Scene (1-Click)}$$
2. To refresh or auto-generate ScriptableObject item assets from raw dataset folders:
   $$\text{Sustainable Future} \longrightarrow \text{1. Auto-Generate Items from Dataset}$$

---

## 📁 Project Architecture & Key Folders

```
UERminiProject/
├── Assets/
│   ├── _Game/
│   │   ├── Materials/          # Court, ball, hoop, and net materials (URP)
│   │   ├── ScriptableObjects/  # SustainabilityItem and ItemDatabase assets
│   │   └── Scripts/
│   │       ├── Audio/          # Procedural sound synthesizer (SoundManager.cs)
│   │       ├── Core/           # GameEvents.cs, GameManager.cs, StatsManager.cs
│   │       ├── Data/           # SustainabilityItem.cs, ItemDatabase.cs
│   │       ├── Editor/         # 1-Click Scene Builder & Dataset Auto-Importer
│   │       ├── Gameplay/       # Basketball.cs, BasketballShooter.cs, HoopController.cs, ItemManager.cs
│   │       └── UI/             # UIManager.cs, UIItemCard.cs, UIResultsModal.cs
│   ├── dataset/                # Raw images for sustainable & non-sustainable items
│   ├── Scenes/
│   │   └── SustainableFutureGame.unity  # Primary active game scene
│   └── Textures/
│       └── Items/              # Court textures & background (eco.jpg)
├── functions.md                # Comprehensive documentation of all key C# functions
├── implementation.md           # Implementation blueprint and system specification
├── report.md                   # Full academic & technical report with design rationale
└── README.md                   # This project guide
```

---

## ⚙️ Key Technical Highlights

1. **Unity New Input System:** Cross-platform input handling supporting mouse flick, touch screens, and trackpads via `UnityEngine.InputSystem`.
2. **Dual-Sensor Hoop Validation:** Prevents bottom-entry score exploits using sequential upper and lower triggers with timestamped entry checks.
3. **Procedural Audio Synthesizer:** All audio effects (ball bounce, net swish, correct score ding, error buzzer) are generated mathematically at runtime using procedural waveform synthesis without heavy external audio files.
4. **Decoupled Event Bus (`GameEvents.cs`):** Components interact through static C# events, eliminating tightly coupled dependencies and `FindObjectOfType` overhead.
5. **Zero Runtime Garbage Collection:** Employs object pooling for basketballs and static ScriptableObjects to ensure steady 60+ FPS performance even on low-end school laptops.

---

## 📚 Technical Documentation & References
- For detailed docstrings and parameters of every core method, see [**`functions.md`**](functions.md).
- For complete system architecture and dataset breakdown, see [**`report.md`**](report.md).
- For full implementation specifications, see [**`implementation.md`**](implementation.md).

---

## 👥 Authors & Academic Context
- **Project:** Mini Project on Sustainability Education in Interactive 3D Media.
- **Target Audience:** Students and educators exploring interactive game-based learning.
