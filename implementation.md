# "SUSTAINABLE FUTURE" — 3D Arcade Basketball Game
## Complete Implementation Specification & Architecture Plan (Updated with Exact Asset Database)

---

## 1. Project Overview

**Game Title:** SUSTAINABLE FUTURE  
**Genre:** 3D Arcade Basketball with Sustainability Educational Mechanics  
**Target Platform:** PC (Optimized for low-end hardware, 60+ FPS)  
**Background Artwork:** `Assets/Textures/Items/eco.jpg` (Used as the core environmental arcade backdrop & green color palette)  
**Item Assets:** Strict curated set from `Assets/Textures/Items/*.png`  
**Engine & Tech Stack:**
- **Unity Version:** 6 (6000.6.4f1)
- **Render Pipeline:** Universal Render Pipeline (URP 17.6.0)
- **UI Framework:** Unity UI (UGUI) with TextMeshPro (TMP) & Sprite Renderers
- **Input System:** Unity New Input System & Legacy fallback
- **Physics Engine:** Unity 3D PhysX

---

## 2. Core Educational Gameplay Loop

```
                        ┌────────────────────────┐
                        │   START 30s ROUND      │
                        │ (Background: eco.jpg)  │
                        └──────────┬─────────────┘
                                   │
                                   ▼
                        ┌────────────────────────┐
                        │  DISPLAY CURRENT ITEM  │
                        │  (Texture Sprite +     │
                        │   Title + Fact)        │
                        └──────────┬─────────────┘
                                   │
                    ┌──────────────┴──────────────┐
                    ▼                             ▼
       [ SHOOT BASKETBALL ]               [ PRESS DISCARD ]
                    │                             │
          ┌─────────┴─────────┐                   │
          ▼                   ▼                   │
       [ MISS ]            [ GOAL ]               │
          │                   │                   │
    (Keep trying/             ▼                   ▼
    reshoot item)    ┌─────────────────┐ ┌─────────────────┐
                     │ Check Item Type │ │ Check Item Type │
                     └────────┬────────┘ └────────┬────────┘
                              │                   │
               ┌──────────────┴───────┐   ┌───────┴──────────────┐
               ▼                      ▼   ▼                      ▼
        [ SUSTAINABLE ]      [ NON-SUSTAINABLE ] [ NON-SUSTAINABLE ] [ SUSTAINABLE ]
               │                      │                   │                  │
               ▼                      ▼                   ▼                  ▼
          +10 Points             -10 Points          +10 Points         -10 Points
          Streak +1              Streak Reset        Streak +1          Streak Reset
          Baskets +1             Baskets +1          (Great Discard!)   (Bad Discard!)
               │                      │                   │                  │
               └──────────────────────┼───────────────────┴──────────────────┘
                                      │
                                      ▼
                        ┌────────────────────────┐
                        │  SHOW FLOATING FEEDBACK│
                        │   LOAD NEXT ITEM       │
                        └──────────┬─────────────┘
                                   │
                                   ▼
                           [ TIME LEFT > 0? ]
                             /            \
                           YES             NO
                           /                \
                 Continue Round          [ GAME OVER SCREEN ]
                                         - Final Score
                                         - Best Baskets
                                         - Average Score
                                         - [ PLAY AGAIN ]
```

---

## 3. Decision Matrix & Scoring System

| Current Item Type | Player Action | Basketball Result | Score Effect | Streak Effect | In-Game Feedback | Educational Message |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **🌱 Sustainable** | **SHOOT** | **GOAL** | **+10 PTS** | **+1** | `"GOAL! +10"` (Green) | *"Great job! Sustainable solutions power our future!"* |
| **🌱 Sustainable** | **SHOOT** | **MISS** | **0 PTS** | No change | `"MISS!"` (Gray) | Reshoot or discard before round timer ends. |
| **🌱 Sustainable** | **DISCARD** | N/A | **-10 PTS** | **Reset to 0** | `"WRONG! -10"` (Red) | *"Think again! This item helps protect the planet!"* |
| **🚫 Non-Sustainable**| **DISCARD** | N/A | **+10 PTS** | **+1** | `"CORRECT! +10"` (Green) | *"Well done! Discarding/avoiding waste saves resources!"* |
| **🚫 Non-Sustainable**| **SHOOT** | **GOAL** | **-10 PTS** | **Reset to 0** | `"THINK AGAIN! -10"` (Orange)| *"Goal made, but this item harms the environment!"* |
| **🚫 Non-Sustainable**| **SHOOT** | **MISS** | **0 PTS** | No change | `"MISS!"` (Gray) | Ball missed; player still has opportunity to discard correctly. |

---

## 4. Curated Items Database Specification (Exact Assets in `Assets/Textures/Items/`)

### 4.1 Sustainable Items (Action: SHOOT BASKETBALL 🏀)
| # | Texture File | Display Name | Category | Educational Fact |
|---|---|---|---|---|
| 1 | `01_bamboo_toothbrush.png` | Bamboo Toothbrush | Sustainable | Naturally biodegradable and prevents plastic handle landfill accumulation. |
| 2 | `02_canvas_tote_bag.png` | Canvas Tote Bag | Sustainable | Reusable thousands of times, eliminating single-use plastic carrier bags. |
| 3 | `03_stainless_steel_water_bottle.png` | Stainless Steel Bottle | Sustainable | Durable lifetime alternative to thousands of single-use plastic water bottles. |
| 4 | `04_glass_storage_jars.png` | Glass Storage Jars | Sustainable | 100% recyclable infinitely without loss of quality or purity. |
| 5 | `05_wooden_cooking_spoons.png` | Wooden Cooking Utensils | Sustainable | Renewable, natural wood cookware that avoids toxic chemical leaching. |
| 6 | `09_cardboard_box.png` | Recyclable Cardboard Box | Sustainable | Easily biodegradable, recyclable paper-based packaging material. |
| 7 | `10_jute_cord_rope.png` | Natural Jute Twine | Sustainable | 100% biodegradable natural plant fiber with low carbon footprint. |
| 8 | `11_loofah_sponge.png` | Natural Loofah Sponge | Sustainable | Plant-based biodegradable scrubber that sheds zero microplastics. |
| 9 | `12_ceramic_mug.png` | Ceramic Coffee Mug | Sustainable | Reusable drinkware eliminating millions of disposable paper/plastic cups. |
| 10 | `13_aluminum_can.png` | Recyclable Aluminum Can | Sustainable | Highly recyclable metal saving 95% of energy compared to raw extraction. |
| 11 | `18_coconut_shell_bowl.png` | Coconut Shell Bowl | Sustainable | Upcycled agricultural byproduct transformed into zero-waste tableware. |
| 12 | `19_cotton_buds.png` | Paper/Cotton Ear Swabs | Sustainable | Organic cotton and paper stems that break down naturally. |
| 13 | `21_linen_napkin.png` | Reusable Linen Napkin | Sustainable | Washable natural fabric napkin eliminating disposable paper napkin waste. |
| 14 | `24_wool_dryer_balls.png` | Wool Dryer Balls | Sustainable | Natural, chemical-free reusable alternative to synthetic dryer sheets. |
| 15 | `25_copper_water_flask.png` | Copper Water Flask | Sustainable | Long-lasting antimicrobial reusable vessel that replaces plastic bottles. |
| 16 | `AppleCompost.png` | Fruit Scrap Compost | Sustainable | Organic food waste transformed into rich soil nutrients instead of landfill methane. |
| 17 | `BambooStraw.png` | Reusable Bamboo Straw | Sustainable | Organic reusable straw preventing marine plastic pollution. |
| 18 | `BioCoffeeCup.png` | Compostable Coffee Cup | Sustainable | Plant-based biodegradable cup designed for commercial composting. |
| 19 | `CompostSoil.png` | Organic Compost Soil | Sustainable | Restores soil biome and captures carbon naturally. |
| 20 | `CottonBag.png` | Organic Cotton Bag | Sustainable | Grown without toxic pesticides and fully biodegradable. |
| 21 | `EcoLightbulb.png` | High-Efficiency LED Bulb | Sustainable | Consumes 80% less electricity than incandescent lighting. |
| 22 | `GlassBottle.png` | Returnable Glass Bottle | Sustainable | Infinitely reusable and recyclable beverage container. |
| 23 | `MetalFlask.png` | Insulated Metal Flask | Sustainable | Eliminates disposable drink containers with long thermal retention. |
| 24 | `OrganicCotton.png` | Organic Raw Cotton | Sustainable | Sustainable crop rotation and minimal water usage. |
| 25 | `RainJar.png` | Rainwater Collection Jar | Sustainable | Harvests natural precipitation to conserve municipal drinking water. |
| 26 | `RecycledPaper.png` | 100% Recycled Paper | Sustainable | Protects virgin forests and saves freshwater during manufacturing. |
| 27 | `SolarCell.png` | Photovoltaic Solar Cell | Sustainable | Harnesses infinite solar energy with zero direct carbon emissions. |
| 28 | `WindTurbine.png` | Clean Wind Turbine | Sustainable | Generates green renewable electrical power from natural wind currents. |
| 29 | `WoodenToy.png` | Sustained Wooden Toy | Sustainable | Natural, non-toxic heirloom toy reducing petroleum-based plastic demand. |

---

### 4.2 Non-Sustainable Items (Action: PRESS DISCARD 🗑)
| # | Texture File | Display Name | Category | Educational Fact |
|---|---|---|---|---|
| 30 | `27_single-use_water_bottle.png` | Single-Use Plastic Bottle | Non-Sustainable | Takes 450+ years to degrade, polluting waterways with microplastics. |
| 31 | `30_plastic_cutlery.png` | Disposable Plastic Cutlery | Non-Sustainable | Used for 10 minutes, persists in landfills and oceans for centuries. |
| 32 | `31_disposable_coffee_cup.png` | Polyethylene-Lined Cup | Non-Sustainable | Plastic interior coating prevents conventional paper recycling. |
| 33 | `32_bubble_wrap.png` | Non-Recyclable Bubble Wrap | Non-Sustainable | Low-density polyethylene wrapping that chokes recycling machinery. |
| 34 | `35_plastic_cling_film.png` | PVC Cling Film Wrap | Non-Sustainable | Single-use plastic wrap that cannot be recycled and contaminates food chains. |
| 35 | `39_plastic_six-pack_ring.png` | Plastic Six-Pack Ring | Non-Sustainable | Severe entanglement hazard to birds, marine turtles, and fish. |
| 36 | `40_single-use_coffee_pod.png` | Aluminum-Plastic Coffee Pod | Non-Sustainable | Complex composite packaging that mostly ends up in landfills. |
| 37 | `41_plastic_ketchup_sachet.png` | Multi-Layer Condiment Sachet| Non-Sustainable | Unrecyclable composite film creating massive litter everywhere. |
| 38 | `43_synthetic_cleaning_cloth.png`| Microfiber Synthetic Cloth | Non-Sustainable | Releases millions of microplastic fibers into waterways during washing. |
| 39 | `44_plastic_wet_wipes.png` | Plastic Wet Wipes | Non-Sustainable | Contains plastic fibers that cause massive sewer blockages ("fatbergs"). |
| 40 | `45_synthetic_fast_fashion_shirt.png`| Polyester Fast Fashion Shirt| Non-Sustainable | Petroleum-derived fabric contributing to textile waste mountains. |
| 41 | `46_plastic_toothpick_flosser.png`| Disposable Plastic Flosser | Non-Sustainable | Single-use rigid plastic tool discarded after one single minute of use. |
| 42 | `47_disposable_baby_diaper.png` | Synthetic Disposable Diaper | Non-Sustainable | Takes 500 years to decompose in landfills with heavy resource footprint. |
| 43 | `48_plastic_party_balloon.png` | Synthetic Party Balloon | Non-Sustainable | Ingested by marine wildlife and birds mistaking fragments for food. |
| 44 | `49_aerosol_spray_can.png` | VOC Aerosol Spray Can | Non-Sustainable | Emits volatile organic compounds that degrade urban air quality. |
| 45 | `50_plastic_milk_jug_cap.png` | Small Plastic Bottle Cap | Non-Sustainable | Frequently escapes sorting screens and ends up as ocean debris. |
| 46 | `AerosolCan.png` | Chemical Aerosol Can | Non-Sustainable | Hazardous pressurized waste releasing greenhouse propellants. |
| 47 | `CandyWrapper.png` | Metallized Candy Wrapper | Non-Sustainable | Multi-layer plastic/foil packaging that cannot be separated or recycled. |
| 48 | `CigaretteButt.png` | Cellulose Acetate Butt | Non-Sustainable | Toxic filter fibers that leach arsenic and heavy metals into waterways. |
| 49 | `CircuitBoard.png` | Toxic E-Waste Circuit | Non-Sustainable | Contains toxic lead and mercury requiring dedicated hazardous e-waste recycling. |
| 50 | `CoalLump.png` | Raw Fossil Coal | Non-Sustainable | High-carbon fossil fuel that drives severe atmospheric global warming. |
| 51 | `DisposableDiaper.png` | Disposable Diaper Waste | Non-Sustainable | Heavy landfill burden containing superabsorbent polymers. |
| 52 | `EWasteBattery.png` | Discarded Lithium Battery | Non-Sustainable | Toxic chemical fire and heavy metal soil contamination hazard if not recycled. |
| 53 | `MicroplasticJar.png` | Microplastic Particles | Non-Sustainable | Ubiquitous indestructible particles penetrating global food and water supplies. |
| 54 | `OilCanister.png` | Petroleum Motor Oil Can | Non-Sustainable | Fossil byproduct whose improper disposal contaminates thousands of liters of groundwater. |
| 55 | `PlasticBag.png` | Lightweight Polyethylene Bag | Non-Sustainable | Suffocates wildlife and breaks into harmful microplastics. |
| 56 | `PlasticBottle.png` | Crushed Plastic Bottle | Non-Sustainable | Excessive single-use petroleum plastic packaging. |
| 57 | `PlasticCutlery.png` | Plastic Fork and Knife | Non-Sustainable | Non-recyclable single-use dining waste. |
| 58 | `PlasticStraw.png` | Disposable Plastic Straw | Non-Sustainable | Lethal obstruction to sea turtles and ocean marine life. |
| 59 | `StyrofoamBox.png` | Expanded Polystyrene Box | Non-Sustainable | Brittle synthetic foam that persists in nature for over 500 years. |
| 60 | `ToxicDrum.png` | Industrial Toxic Chemical Drum | Non-Sustainable | Hazardous industrial waste destroying aquatic ecosystems. |

---

## 5. System Architecture & Class Hierarchy

```
Assets/
├── _Game/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs              # Round state machine & 30s timer controller
│   │   │   ├── GameEvents.cs               # Event aggregator (decoupled messaging)
│   │   │   └── StatsManager.cs             # High scores, streak, games played persistence
│   │   ├── Data/
│   │   │   ├── SustainabilityItem.cs       # ScriptableObject for environmental items
│   │   │   └── ItemDatabase.cs             # Collection of sustainable & non-sustainable items
│   │   ├── Gameplay/
│   │   │   ├── BasketballShooter.cs        # Drag/Swipe aiming & shooting physics controller
│   │   │   ├── Basketball.cs               # Ball physics, ground/rim bounce, auto-reset logic
│   │   │   ├── HoopController.cs           # Dual-sensor top/bottom goal verification
│   │   │   └── ItemManager.cs              # Item queue, shuffle, and evaluation logic
│   │   ├── UI/
│   │   │   ├── UIManager.cs                # Main UI driver (Stats, Arcade Hoops, Results)
│   │   │   ├── UIItemCard.cs               # Current Item badge/card animation with Sprite icon
│   │   │   ├── UIFloatingFeedback.cs       # Animated +10/-10 popups & streak alerts
│   │   │   └── UIResultsModal.cs           # Post-round score summary & replay button
│   │   ├── Audio/
│   │   │   └── SoundManager.cs             # Procedural & synthesized SFX for all game events
│   │   └── Editor/
│   │       ├── AutoGenerateItemAssets.cs   # Automatically builds ScriptableObjects from Assets/Textures/Items
│   │       └── SetupSustainableFutureScene.cs # 1-Click scene generator & automatic wiring
│   ├── ScriptableObjects/
│   │   ├── Items/                          # Individual item assets generated from Textures/Items
│   │   └── Database/                       # Main ItemDatabase asset linking all items
│   ├── Materials/                          # URP lightweight materials (Court, Rim, Net, Ball)
│   ├── Prefabs/                            # Ball, Hoop, UI components
│   └── Scenes/
│       └── SustainableFutureGame.unity     # Production scene
```

---

## 6. Implementation Roadmap

```
Phase 1: Architecture, Data Layer & Sprite Importer
├── [Task 1.1] Create Core Game Events & Enums
├── [Task 1.2] Create SustainabilityItem & ItemDatabase ScriptableObjects
└── [Task 1.3] Auto-generate ScriptableObjects linking all PNG sprites in Assets/Textures/Items/

Phase 2: 3D Court, Basketball & Hoop Physics
├── [Task 2.1] Implement Basketball.cs & Physics Material
├── [Task 2.2] Implement HoopController.cs with Dual-Trigger Goal Sensor
└── [Task 2.3] Implement BasketballShooter.cs (Arcade Flick & Trajectory)

Phase 3: Gameplay Loop & Educational Logic
├── [Task 3.1] Implement GameManager.cs (30s Timer & State Machine)
├── [Task 3.2] Implement ItemManager.cs (Queue, Shuffle, Decision Evaluation)
└── [Task 3.3] Implement StatsManager.cs (Persistent Stats & Averages)

Phase 4: Arcade UI & Visual Theme (Using eco.jpg)
├── [Task 4.1] Build UIManager.cs with Top Stats & Arcade Hoops Panels
├── [Task 4.2] Implement UIItemCard with texture icon, title, category, and educational tip
├── [Task 4.3] Implement Floating Score Popups (+10/-10) & Streak Banner
├── [Task 4.4] Implement Game Over / Results Modal & Replay Flow
└── [Task 4.5] Configure eco.jpg backdrop and green arcade palette

Phase 5: Audio, Polish & Low-End Optimization
├── [Task 5.1] Implement SoundManager.cs (Procedural/Clean Audio FX)
├── [Task 5.2] Create 1-Click Scene Setup Editor Tool
└── [Task 5.3] Optimization check: SRP Batching, Draw Call minimization

Phase 6: Verification & Scene Validation
├── [Task 6.1] Execute 1-Click Scene Builder in Unity
└── [Task 6.2] Validate full game loop in Play Mode
```

---

## 7. Low-End PC Optimization Strategy

1. **Sprite Atlas / Efficient Texture Formats:**
   - Texture import settings configured to ASTC/DXT compression with max size 512 for snappy loading.
2. **Zero Garbage Collection During Gameplay:**
   - Pre-allocated object pools for basketballs, score popup text, and sound sources.
3. **URP SRP Batching:**
   - Simple Lit/Unlit shaders for court geometry; single-pass UI canvas rendering.
4. **Physics Determinism:**
   - Fixed Timestep at 0.02s (50Hz) with simple primitive colliders for backboard and rim.
