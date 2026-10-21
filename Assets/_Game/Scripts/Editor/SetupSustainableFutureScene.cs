using System.IO;
using SustainableFuture.Audio;
using SustainableFuture.Core;
using SustainableFuture.Data;
using SustainableFuture.Gameplay;
using SustainableFuture.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace SustainableFuture.Editor
{
    public static class SetupSustainableFutureScene
    {
        private const string ScenePath = "Assets/Scenes/SustainableFutureGame.unity";
        private const string PrefabsPath = "Assets/_Game/Prefabs";
        private const string MaterialsPath = "Assets/_Game/Materials";
        private const string DatabaseAssetPath = "Assets/_Game/ScriptableObjects/Database/ItemDatabase.asset";
        private const string EcoBgPath = "Assets/Textures/Items/eco.jpg";
        private const string RobotoFontPath = "Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Roboto-Bold SDF.asset";
        private const string FallbackFontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";

        private static TMP_FontAsset cachedFont;

        // Visual Palette matching reference style
        private static readonly Color PanelBgColor = new Color(0.067f, 0.247f, 0.184f, 0.96f); // #113f2f (Deep Forest Green)
        private static readonly Color PanelBorderYellow = new Color(0.96f, 0.72f, 0.18f, 1f); // #f5b82e (Vibrant Golden Yellow)
        private static readonly Color MintTextColor = new Color(0.78f, 0.98f, 0.88f, 1f); // #c6f6d5 (Bright Mint)
        private static readonly Color GoldTitleColor = new Color(0.96f, 0.72f, 0.18f, 1f); // Golden Yellow Header

        [MenuItem("Sustainable Future/2. Setup Complete 3D Basketball Scene (1-Click)")]
        public static void BuildCompleteScene()
        {
            // 1. Ensure Item Database is generated first
            AutoGenerateItemAssets.GenerateAllItemAssets();

            // 2. Ensure directories exist
            EnsureFolders();

            cachedFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(RobotoFontPath);
            if (cachedFont == null)
            {
                cachedFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FallbackFontPath);
            }

            // 3. Create or open new Scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 4. Create Lighting & Camera
            Camera mainCam = SetupCameraAndLighting();

            // 5. Generate Green Patterned Court Texture & Material
            Material courtMat = CreatePatternedGreenCourtMaterial();
            Material backboardMat = GetOrCreateMaterial("Mat_Backboard", new Color(0.96f, 0.96f, 0.96f));
            Material rimMat = GetOrCreateMaterial("Mat_Rim", new Color(0.95f, 0.45f, 0.1f));
            Material netMat = GetOrCreateMaterial("Mat_Net", new Color(0.88f, 0.88f, 0.88f, 0.65f), true);
            Material ballMat = GetOrCreateMaterial("Mat_Basketball", new Color(0.94f, 0.44f, 0.12f));

            // 6. Create eco.jpg Background Backdrop
            SetupBackgroundBackdrop();

            // 7. Create 3D Basketball Hoop & Court
            HoopController hoop = SetupHoop(backboardMat, rimMat, netMat);
            SetupCourt(courtMat);

            // 8. Create Basketball Prefab
            Basketball ballPrefab = CreateBasketballPrefab(ballMat);

            // 9. Create Core Game Loop Hierarchy
            GameObject gameRoot = new GameObject("_GameControllers");
            GameManager gameManager = gameRoot.AddComponent<GameManager>();
            StatsManager statsManager = gameRoot.AddComponent<StatsManager>();
            ItemManager itemManager = gameRoot.AddComponent<ItemManager>();
            SoundManager soundManager = gameRoot.AddComponent<SoundManager>();

            ItemDatabase db = AssetDatabase.LoadAssetAtPath<ItemDatabase>(DatabaseAssetPath);
            itemManager.SetDatabase(db);

            // 10. Create Basketball Shooter
            GameObject shooterObj = new GameObject("BasketballShooter");
            shooterObj.transform.position = new Vector3(0f, 0.5f, -4.2f);
            BasketballShooter shooter = shooterObj.AddComponent<BasketballShooter>();

            GameObject spawnPoint = new GameObject("BallSpawnPoint");
            spawnPoint.transform.parent = shooterObj.transform;
            spawnPoint.transform.position = new Vector3(0f, 0.75f, -3.8f);

            LineRenderer trajectory = shooterObj.AddComponent<LineRenderer>();
            SetupTrajectoryLine(trajectory);

            SetPrivateField(shooter, "basketballPrefab", ballPrefab);
            SetPrivateField(shooter, "ballSpawnPoint", spawnPoint.transform);
            SetPrivateField(shooter, "hoopTargetPoint", hoop.transform);
            SetPrivateField(shooter, "trajectoryLine", trajectory);

            // 11. Create UI System with Refined Clear Styling
            UIManager uiManager = SetupUI();

            // 12. Save Scene
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"<color=#2ecc71><b>[Sustainable Future]</b></color> Scene successfully created with crystal-clear UI at: {ScenePath}!");
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
            if (!AssetDatabase.IsValidFolder("Assets/_Game")) AssetDatabase.CreateFolder("Assets", "_Game");
            if (!AssetDatabase.IsValidFolder(PrefabsPath)) AssetDatabase.CreateFolder("Assets/_Game", "Prefabs");
            if (!AssetDatabase.IsValidFolder(MaterialsPath)) AssetDatabase.CreateFolder("Assets/_Game", "Materials");
        }

        private static Camera SetupCameraAndLighting()
        {
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            Camera cam = camObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.06f, 0.12f, 0.08f);
            cam.fieldOfView = 50f;
            camObj.transform.position = new Vector3(0f, 2.05f, -5.6f);
            camObj.transform.rotation = Quaternion.Euler(11f, 0f, 0f);
            camObj.AddComponent<AudioListener>();

            var uac = camObj.AddComponent<UniversalAdditionalCameraData>();
            uac.renderPostProcessing = false;

            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1.0f, 0.98f, 0.92f);
            light.intensity = 1.3f;
            lightObj.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            return cam;
        }

        private static Material CreatePatternedGreenCourtMaterial()
        {
            string texturePath = $"{MaterialsPath}/Tex_GreenCourtPattern.png";
            int width = 512;
            int height = 512;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, true);

            Color darkGreen = new Color(0.08f, 0.28f, 0.16f); // #144729
            Color lightGreen = new Color(0.13f, 0.38f, 0.22f); // #216138
            Color courtLine = new Color(0.85f, 0.95f, 0.88f); // Soft Court White

            int stripeWidth = 64;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isStripe = ((x / stripeWidth) % 2 == 0);
                    Color col = isStripe ? darkGreen : lightGreen;

                    if (x % stripeWidth == 0 || y % stripeWidth == 0)
                    {
                        col = Color.Lerp(col, darkGreen * 0.7f, 0.4f);
                    }

                    if (x == 4 || x == width - 5 || y == 4 || y == height - 5)
                    {
                        col = courtLine;
                    }

                    tex.SetPixel(x, y, col);
                }
            }
            tex.Apply();

            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(texturePath, bytes);
            AssetDatabase.ImportAsset(texturePath);

            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer != null)
            {
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            Texture2D savedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

            string matPath = $"{MaterialsPath}/Mat_PatternedGreenCourt.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, matPath);
            }

            mat.mainTexture = savedTex;
            mat.mainTextureScale = new Vector2(4f, 6f);
            mat.color = Color.white;
            EditorUtility.SetDirty(mat);

            return mat;
        }

        private static void SetupBackgroundBackdrop()
        {
            Texture2D ecoTex = AssetDatabase.LoadAssetAtPath<Texture2D>(EcoBgPath);
            if (ecoTex == null) return;

            GameObject bgQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bgQuad.name = "Background_EcoBackdrop";
            bgQuad.transform.position = new Vector3(0f, 3.2f, 3.8f);
            bgQuad.transform.localScale = new Vector3(14.5f, 9.0f, 1f);

            Object.DestroyImmediate(bgQuad.GetComponent<Collider>());

            Material bgMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            bgMat.mainTexture = ecoTex;
            bgMat.color = new Color(0.92f, 0.92f, 0.92f, 1f);
            AssetDatabase.CreateAsset(bgMat, $"{MaterialsPath}/Mat_EcoBackdrop.mat");

            bgQuad.GetComponent<MeshRenderer>().sharedMaterial = bgMat;
        }

        private static HoopController SetupHoop(Material backboardMat, Material rimMat, Material netMat)
        {
            GameObject hoopRoot = new GameObject("BasketballHoop");
            hoopRoot.transform.position = new Vector3(0f, 2.6f, 0f);
            HoopController controller = hoopRoot.AddComponent<HoopController>();

            // Backboard
            GameObject backboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backboard.name = "Backboard";
            backboard.transform.parent = hoopRoot.transform;
            backboard.transform.localPosition = new Vector3(0f, 0.55f, 0.45f);
            backboard.transform.localScale = new Vector3(2.4f, 1.6f, 0.08f);
            backboard.GetComponent<MeshRenderer>().sharedMaterial = backboardMat;

            // Support Pole
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "SupportPole";
            pole.transform.parent = hoopRoot.transform;
            pole.transform.localPosition = new Vector3(0f, -1.4f, 0.6f);
            pole.transform.localScale = new Vector3(0.15f, 2.0f, 0.15f);
            pole.GetComponent<MeshRenderer>().sharedMaterial = backboardMat;

            // Rim
            GameObject rim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rim.name = "Rim";
            rim.transform.parent = hoopRoot.transform;
            rim.transform.localPosition = new Vector3(0f, 0f, 0f);
            rim.transform.localScale = new Vector3(1.1f, 0.04f, 1.1f);
            rim.GetComponent<MeshRenderer>().sharedMaterial = rimMat;
            Object.DestroyImmediate(rim.GetComponent<Collider>());

            // Rim Colliders
            CreateRimCollider(hoopRoot.transform, new Vector3(0.5f, 0f, 0f));
            CreateRimCollider(hoopRoot.transform, new Vector3(-0.5f, 0f, 0f));
            CreateRimCollider(hoopRoot.transform, new Vector3(0f, 0f, 0.5f));
            CreateRimCollider(hoopRoot.transform, new Vector3(0f, 0f, -0.5f));

            // Net Mesh
            GameObject net = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            net.name = "Net";
            net.transform.parent = hoopRoot.transform;
            net.transform.localPosition = new Vector3(0f, -0.35f, 0f);
            net.transform.localScale = new Vector3(0.9f, 0.35f, 0.9f);
            net.GetComponent<MeshRenderer>().sharedMaterial = netMat;
            Object.DestroyImmediate(net.GetComponent<Collider>());

            // Upper Sensor
            GameObject upperObj = new GameObject("UpperGoalSensor");
            upperObj.transform.parent = hoopRoot.transform;
            upperObj.transform.localPosition = new Vector3(0f, 0.15f, 0f);
            BoxCollider upperCol = upperObj.AddComponent<BoxCollider>();
            upperCol.isTrigger = true;
            upperCol.size = new Vector3(0.85f, 0.15f, 0.85f);
            HoopSensor upperSensor = upperObj.AddComponent<HoopSensor>();
            upperSensor.Setup(HoopSensorType.Upper, controller);

            // Lower Sensor
            GameObject lowerObj = new GameObject("LowerGoalSensor");
            lowerObj.transform.parent = hoopRoot.transform;
            lowerObj.transform.localPosition = new Vector3(0f, -0.55f, 0f);
            BoxCollider lowerCol = lowerObj.AddComponent<BoxCollider>();
            lowerCol.isTrigger = true;
            lowerCol.size = new Vector3(0.85f, 0.15f, 0.85f);
            HoopSensor lowerSensor = lowerObj.AddComponent<HoopSensor>();
            lowerSensor.Setup(HoopSensorType.Lower, controller);

            SetPrivateField(controller, "upperTrigger", upperCol);
            SetPrivateField(controller, "lowerTrigger", lowerCol);
            SetPrivateField(controller, "netTransform", net.transform);

            return controller;
        }

        private static void CreateRimCollider(Transform parent, Vector3 localPos)
        {
            GameObject rimCol = new GameObject("RimCollider");
            rimCol.transform.parent = parent;
            rimCol.transform.localPosition = localPos;
            SphereCollider col = rimCol.AddComponent<SphereCollider>();
            col.radius = 0.08f;
        }

        private static void SetupCourt(Material courtMat)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "CourtFloor";
            floor.transform.position = new Vector3(0f, -0.1f, 0f);
            floor.transform.localScale = new Vector3(14f, 0.2f, 20f);
            floor.GetComponent<MeshRenderer>().sharedMaterial = courtMat;
        }

        private static Basketball CreateBasketballPrefab(Material ballMat)
        {
            string prefabPath = $"{PrefabsPath}/Basketball.prefab";
            Basketball existing = AssetDatabase.LoadAssetAtPath<Basketball>(prefabPath);
            if (existing != null) return existing;

            GameObject ballObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ballObj.name = "Basketball";
            ballObj.transform.localScale = Vector3.one * 0.44f;
            ballObj.GetComponent<MeshRenderer>().sharedMaterial = ballMat;

            Rigidbody rb = ballObj.AddComponent<Rigidbody>();
            rb.mass = 0.62f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            Basketball ball = ballObj.AddComponent<Basketball>();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(ballObj, prefabPath);
            Object.DestroyImmediate(ballObj);

            return prefab.GetComponent<Basketball>();
        }

        private static void SetupTrajectoryLine(LineRenderer line)
        {
            line.enabled = false;
            line.useWorldSpace = true;
            line.startWidth = 0.05f;
            line.endWidth = 0.015f;
            Material trajMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            trajMat.color = new Color(0.2f, 0.95f, 0.45f, 0.8f);
            AssetDatabase.CreateAsset(trajMat, $"{MaterialsPath}/Mat_TrajectoryLine.mat");
            line.sharedMaterial = trajMat;
        }

        private static UIManager SetupUI()
        {
            EventSystem existingEs = Object.FindFirstObjectByType<EventSystem>();
            if (existingEs == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<InputSystemUIInputModule>();
            }
            else
            {
                var legacyModule = existingEs.GetComponent<StandaloneInputModule>();
                if (legacyModule != null) Object.DestroyImmediate(legacyModule);

                if (existingEs.GetComponent<InputSystemUIInputModule>() == null)
                {
                    existingEs.gameObject.AddComponent<InputSystemUIInputModule>();
                }
            }

            // Canvas
            GameObject canvasObj = new GameObject("Canvas_GameUI");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            UIManager uiManager = canvasObj.AddComponent<UIManager>();

            // =========================================================
            // 1. TOP-LEFT STATS PANEL
            // =========================================================
            GameObject statsPanel = CreateBorderedCard(canvasObj.transform, "StatsPanel", 
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -40f), new Vector2(330f, 275f));

            CreateAnchoredTMPText(statsPanel.transform, "Title", "STATS", 26, FontStyles.Bold, TextAlignmentOptions.Center,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -40f), new Vector2(0f, 40f), GoldTitleColor);

            TMP_Text bestTxt = CreateSeparatedStatRow(statsPanel.transform, "BEST", "0", -75f);
            TMP_Text basketsTxt = CreateSeparatedStatRow(statsPanel.transform, "BASKETS", "0", -110f);
            TMP_Text gamesTxt = CreateSeparatedStatRow(statsPanel.transform, "GAMES", "0", -145f);
            TMP_Text streakTxt = CreateSeparatedStatRow(statsPanel.transform, "STREAK", "0", -180f);
            TMP_Text accuracyTxt = CreateSeparatedStatRow(statsPanel.transform, "ACCURACY", "0%", -215f);

            // =========================================================
            // 2. TOP-RIGHT ARCADE HOOPS PANEL
            // =========================================================
            GameObject hoopsPanel = CreateBorderedCard(canvasObj.transform, "ArcadeHoopsPanel", 
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -40f), new Vector2(330f, 185f));

            CreateAnchoredTMPText(hoopsPanel.transform, "Title", "ARCADE HOOPS", 24, FontStyles.Bold, TextAlignmentOptions.Center,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -40f), new Vector2(0f, 40f), GoldTitleColor);

            TMP_Text timeTxt = CreateSeparatedStatRow(hoopsPanel.transform, "TIME", "30", -80f, new Color(1f, 0.45f, 0.4f));
            TMP_Text scoreTxt = CreateSeparatedStatRow(hoopsPanel.transform, "SCORE", "0", -122f, new Color(0.35f, 0.98f, 0.6f));

            // =========================================================
            // 3. LOWER-LEFT VERTICAL ITEM CARD (Unmodified Dataset Images)
            // =========================================================
            GameObject itemCardObj = CreateBorderedCard(canvasObj.transform, "UIItemCard", 
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(40f, 40f), new Vector2(350f, 460f));
            UIItemCard itemCard = itemCardObj.AddComponent<UIItemCard>();

            // 1. Top: Item Image Box (Centered at top)
            GameObject iconFrame = new GameObject("IconFrame");
            iconFrame.transform.parent = itemCardObj.transform;
            RectTransform iconFrameRect = iconFrame.AddComponent<RectTransform>();
            iconFrameRect.anchorMin = new Vector2(0.5f, 1f);
            iconFrameRect.anchorMax = new Vector2(0.5f, 1f);
            iconFrameRect.pivot = new Vector2(0.5f, 1f);
            iconFrameRect.anchoredPosition = new Vector2(0f, -20f);
            iconFrameRect.sizeDelta = new Vector2(175f, 175f);
            Image iconFrameBg = iconFrame.AddComponent<Image>();
            iconFrameBg.color = new Color(0.04f, 0.16f, 0.11f, 0.98f);
            Outline iconOutline = iconFrame.AddComponent<Outline>();
            iconOutline.effectColor = PanelBorderYellow;
            iconOutline.effectDistance = new Vector2(3f, 3f);

            GameObject iconObj = new GameObject("ItemIcon");
            iconObj.transform.parent = iconFrame.transform;
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(6f, 6f);
            iconRect.offsetMax = new Vector2(-6f, -6f);
            Image iconImg = iconObj.AddComponent<Image>();
            iconImg.preserveAspect = true;

            // 2. Below Image: Item Name (Centered Bold Title)
            TMP_Text itemTitle = CreateAnchoredTMPText(itemCardObj.transform, "ItemTitle", "REUSABLE BOTTLE", 22, FontStyles.Bold, TextAlignmentOptions.Center,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -215f), new Vector2(0f, 38f), Color.white);
            itemTitle.enableAutoSizing = true;
            itemTitle.fontSizeMin = 16;
            itemTitle.fontSizeMax = 22;

            // 3. Below Name: Category Pill Badge
            GameObject badgeBox = new GameObject("CategoryBadge");
            badgeBox.transform.parent = itemCardObj.transform;
            RectTransform badgeRect = badgeBox.AddComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(0.5f, 1f);
            badgeRect.anchorMax = new Vector2(0.5f, 1f);
            badgeRect.pivot = new Vector2(0.5f, 1f);
            badgeRect.anchoredPosition = new Vector2(0f, -260f);
            badgeRect.sizeDelta = new Vector2(280f, 36f);
            Image badgeBg = badgeBox.AddComponent<Image>();
            badgeBg.color = new Color(0.12f, 0.65f, 0.35f, 1f);

            TMP_Text badgeTxt = CreateAnchoredTMPText(badgeBox.transform, "BadgeText", "SUSTAINABLE ITEM", 15, FontStyles.Bold, TextAlignmentOptions.Center,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Color.white);

            // 4. Below Badge: Educational Fact (Clear Large Text)
            TMP_Text itemFact = CreateAnchoredTMPText(itemCardObj.transform, "ItemFact", "Durable alternative that saves thousands of single-use plastics.", 16, FontStyles.Normal, TextAlignmentOptions.Center,
                new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f), MintTextColor);
            
            RectTransform factRect = itemFact.GetComponent<RectTransform>();
            factRect.anchorMin = new Vector2(0f, 0f);
            factRect.anchorMax = new Vector2(1f, 1f);
            factRect.offsetMin = new Vector2(18f, 16f);
            factRect.offsetMax = new Vector2(-18f, -305f);
            itemFact.textWrappingMode = TextWrappingModes.Normal;
            itemFact.lineSpacing = 12f;

            SetPrivateField(itemCard, "itemIconImage", iconImg);
            SetPrivateField(itemCard, "itemCategoryBadgeText", badgeTxt);
            SetPrivateField(itemCard, "itemCategoryBadgeBg", badgeBg);
            SetPrivateField(itemCard, "itemTitleText", itemTitle);
            SetPrivateField(itemCard, "itemFactText", itemFact);

            // =========================================================
            // 4. DISCARD BUTTON (Matching Style, Lower Right)
            // =========================================================
            GameObject discardObj = CreateBorderedCard(canvasObj.transform, "DiscardButton", 
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-40f, 40f), new Vector2(290f, 105f));
            
            Button discardBtn = discardObj.AddComponent<Button>();
            ColorBlock cb = discardBtn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1.25f, 1.25f, 1.25f);
            cb.pressedColor = new Color(0.85f, 0.85f, 0.85f);
            discardBtn.colors = cb;

            CreateAnchoredTMPText(discardObj.transform, "DiscardLabel", "DISCARD", 28, FontStyles.Bold, TextAlignmentOptions.Center,
                new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0f, 8f), new Vector2(0f, 40f), PanelBorderYellow);

            CreateAnchoredTMPText(discardObj.transform, "DiscardSub", "[ NON-SUSTAINABLE ]", 14, FontStyles.Bold, TextAlignmentOptions.Center,
                new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0f, -22f), new Vector2(0f, 25f), MintTextColor);

            // =========================================================
            // 5. FLOATING FEEDBACK
            // =========================================================
            GameObject feedbackPoint = new GameObject("FeedbackSpawnPoint");
            feedbackPoint.transform.parent = canvasObj.transform;
            RectTransform fbRect = feedbackPoint.AddComponent<RectTransform>();
            fbRect.anchorMin = new Vector2(0.5f, 0.5f);
            fbRect.anchorMax = new Vector2(0.5f, 0.5f);
            fbRect.pivot = new Vector2(0.5f, 0.5f);
            fbRect.anchoredPosition = new Vector2(0f, 90f);

            UIFloatingFeedback fbInstance = CreateFloatingFeedbackPrefab(feedbackPoint.transform);

            // =========================================================
            // 6. RESULTS MODAL
            // =========================================================
            UIResultsModal resultsModal = SetupResultsModal(canvasObj.transform);
            resultsModal.gameObject.SetActive(false);

            // Link UIManager fields
            SetPrivateField(uiManager, "bestText", bestTxt);
            SetPrivateField(uiManager, "basketsText", basketsTxt);
            SetPrivateField(uiManager, "gamesText", gamesTxt);
            SetPrivateField(uiManager, "streakText", streakTxt);
            SetPrivateField(uiManager, "accuracyText", accuracyTxt);

            SetPrivateField(uiManager, "timeText", timeTxt);
            SetPrivateField(uiManager, "scoreText", scoreTxt);
            SetPrivateField(uiManager, "itemCard", itemCard);
            SetPrivateField(uiManager, "discardButton", discardBtn);
            SetPrivateField(uiManager, "resultsModal", resultsModal);
            SetPrivateField(uiManager, "floatingFeedbackPrefab", fbInstance);
            SetPrivateField(uiManager, "feedbackSpawnPoint", fbRect);

            return uiManager;
        }

        private static GameObject CreateBorderedCard(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
        {
            GameObject panel = new GameObject(name);
            panel.transform.parent = parent;
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = anchorMin;
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            Image img = panel.AddComponent<Image>();
            img.color = PanelBgColor;

            Outline outline = panel.AddComponent<Outline>();
            outline.effectColor = PanelBorderYellow;
            outline.effectDistance = new Vector2(4f, 4f);

            return panel;
        }

        private static TMP_Text CreateSeparatedStatRow(Transform parent, string label, string defaultValue, float yOffsetFromTop, Color? valColor = null)
        {
            GameObject rowObj = new GameObject($"Row_{label}");
            rowObj.transform.parent = parent;
            RectTransform rowRect = rowObj.AddComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(1f, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.anchoredPosition = new Vector2(0f, yOffsetFromTop);
            rowRect.sizeDelta = new Vector2(0f, 32f);

            GameObject labelObj = new GameObject("Label");
            labelObj.transform.parent = rowObj.transform;
            RectTransform labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(0.65f, 1f);
            labelRect.offsetMin = new Vector2(24f, 0f);
            labelRect.offsetMax = new Vector2(0f, 0f);

            TMP_Text labelTxt = labelObj.AddComponent<TextMeshProUGUI>();
            if (cachedFont != null) labelTxt.font = cachedFont;
            labelTxt.extraPadding = true;
            labelTxt.text = label;
            labelTxt.fontSize = 19;
            labelTxt.fontStyle = FontStyles.Bold;
            labelTxt.alignment = TextAlignmentOptions.Left;
            labelTxt.color = MintTextColor;

            GameObject valObj = new GameObject("Value");
            valObj.transform.parent = rowObj.transform;
            RectTransform valRect = valObj.AddComponent<RectTransform>();
            valRect.anchorMin = new Vector2(0.65f, 0f);
            valRect.anchorMax = new Vector2(1f, 1f);
            valRect.offsetMin = new Vector2(0f, 0f);
            valRect.offsetMax = new Vector2(-24f, 0f);

            TMP_Text valTxt = valObj.AddComponent<TextMeshProUGUI>();
            if (cachedFont != null) valTxt.font = cachedFont;
            valTxt.extraPadding = true;
            valTxt.text = defaultValue;
            valTxt.fontSize = 21;
            valTxt.fontStyle = FontStyles.Bold;
            valTxt.alignment = TextAlignmentOptions.Right;
            valTxt.color = valColor ?? Color.white;

            return valTxt;
        }

        private static TMP_Text CreateAnchoredTMPText(Transform parent, string name, string text, float fontSize, FontStyles style, TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size, Color color)
        {
            GameObject txtObj = new GameObject(name);
            txtObj.transform.parent = parent;
            RectTransform rect = txtObj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            TMP_Text tmp = txtObj.AddComponent<TextMeshProUGUI>();
            if (cachedFont != null) tmp.font = cachedFont;
            tmp.extraPadding = true;
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.alignment = alignment;
            tmp.color = color;
            return tmp;
        }

        private static UIFloatingFeedback CreateFloatingFeedbackPrefab(Transform parent)
        {
            GameObject fbObj = new GameObject("FloatingFeedback");
            fbObj.transform.parent = parent;
            RectTransform rect = fbObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(450f, 80f);
            fbObj.AddComponent<CanvasGroup>();

            TMP_Text tmp = fbObj.AddComponent<TextMeshProUGUI>();
            if (cachedFont != null) tmp.font = cachedFont;
            tmp.extraPadding = true;
            tmp.fontSize = 42;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;

            UIFloatingFeedback feedback = fbObj.AddComponent<UIFloatingFeedback>();
            return feedback;
        }

        private static UIResultsModal SetupResultsModal(Transform parent)
        {
            GameObject modalObj = CreateBorderedCard(parent, "UIResultsModal", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(620f, 580f));
            modalObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            modalObj.GetComponent<Image>().color = new Color(0.04f, 0.16f, 0.11f, 0.98f);

            UIResultsModal modal = modalObj.AddComponent<UIResultsModal>();

            CreateAnchoredTMPText(modalObj.transform, "Header", "ROUND COMPLETE", 32, FontStyles.Bold, TextAlignmentOptions.Center,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -48f), new Vector2(0f, 45f), PanelBorderYellow);

            TMP_Text badgeTxt = CreateAnchoredTMPText(modalObj.transform, "TitleBadge", "ECO CHAMPION!", 24, FontStyles.Bold, TextAlignmentOptions.Center,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -98f), new Vector2(0f, 35f), GoldTitleColor);

            TMP_Text scoreTxt = CreateAnchoredTMPText(modalObj.transform, "Score", "SCORE: 0", 28, FontStyles.Bold, TextAlignmentOptions.Center,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -145f), new Vector2(0f, 38f), Color.white);

            TMP_Text basketsTxt = CreateAnchoredTMPText(modalObj.transform, "Baskets", "BASKETS: 0", 22, FontStyles.Normal, TextAlignmentOptions.Center,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -190f), new Vector2(0f, 30f), MintTextColor);

            TMP_Text streakTxt = CreateAnchoredTMPText(modalObj.transform, "Streak", "STREAK: 0", 22, FontStyles.Normal, TextAlignmentOptions.Center,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -225f), new Vector2(0f, 30f), MintTextColor);

            TMP_Text avgTxt = CreateAnchoredTMPText(modalObj.transform, "Avg", "ACCURACY: 0%", 22, FontStyles.Normal, TextAlignmentOptions.Center,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -260f), new Vector2(0f, 30f), MintTextColor);

            // Large, highly visible Remember Motivational Text
            TMP_Text motivTxt = CreateAnchoredTMPText(modalObj.transform, "Motiv", "REMEMBER: Shoot sustainable items, discard non-sustainable waste!", 20, FontStyles.Bold, TextAlignmentOptions.Center,
                new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -325f), new Vector2(560f, 65f), new Color(0.98f, 0.88f, 0.35f));
            motivTxt.textWrappingMode = TextWrappingModes.Normal;
            motivTxt.lineSpacing = 10f;

            // Play Again Button (Anchored near bottom)
            GameObject btnObj = CreateBorderedCard(modalObj.transform, "PlayAgainButton", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 50f), new Vector2(330f, 65f));
            btnObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            btnObj.GetComponent<Image>().color = new Color(0.12f, 0.55f, 0.28f, 1f);
            Button playAgainBtn = btnObj.AddComponent<Button>();

            CreateAnchoredTMPText(btnObj.transform, "Label", "PLAY AGAIN", 24, FontStyles.Bold, TextAlignmentOptions.Center,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Color.white);

            SetPrivateField(modal, "scoreText", scoreTxt);
            SetPrivateField(modal, "basketsText", basketsTxt);
            SetPrivateField(modal, "streakText", streakTxt);
            SetPrivateField(modal, "avgText", avgTxt);
            SetPrivateField(modal, "titleBadgeText", badgeTxt);
            SetPrivateField(modal, "motivationalText", motivTxt);
            SetPrivateField(modal, "playAgainButton", playAgainBtn);

            return modal;
        }

        private static Material GetOrCreateMaterial(string name, Color color, bool transparent = false)
        {
            string path = $"{MaterialsPath}/{name}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                mat = new Material(shader);
                mat.color = color;
                if (transparent)
                {
                    mat.SetFloat("_Surface", 1);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = 3000;
                }
                AssetDatabase.CreateAsset(mat, path);
            }
            return mat;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
            }
        }
    }
}
