using System.Collections.Generic;
using System.IO;
using SustainableFuture.Data;
using UnityEditor;
using UnityEngine;

namespace SustainableFuture.Editor
{
    [InitializeOnLoad]
    public static class AutoGenerateItemAssets
    {
        private const string DatasetSustainablePath = "Assets/dataset/sustainable";
        private const string DatasetNonSustainPath = "Assets/dataset/non-sustain";
        
        // Fallbacks for alternative naming
        private const string DatsetsSustainablePath = "Assets/datsets/sustainable";
        private const string DatsetsNonSustainPath = "Assets/datsets/non-sustain";

        private const string ItemsAssetPath = "Assets/_Game/ScriptableObjects/Items";
        private const string DatabaseAssetPath = "Assets/_Game/ScriptableObjects/Database";

        private struct ItemMeta
        {
            public string FileName;
            public string Title;
            public ItemCategory Category;
            public string Fact;
            public string Feedback;
        }

        private static readonly List<ItemMeta> ItemDefinitions = new List<ItemMeta>
        {
            // === SUSTAINABLE ITEMS (from dataset/sustainable) ===
            new ItemMeta {
                FileName = "01_bamboo_toothbrush.jpg",
                Title = "Bamboo Toothbrush",
                Category = ItemCategory.Sustainable,
                Fact = "Naturally biodegradable and prevents plastic handle landfill accumulation.",
                Feedback = "GOAL! Bamboo naturally decomposes and saves our oceans."
            },
            new ItemMeta {
                FileName = "02_canvas_tote_bag.jpg",
                Title = "Canvas Tote Bag",
                Category = ItemCategory.Sustainable,
                Fact = "Reusable thousands of times, eliminating single-use plastic carrier bags.",
                Feedback = "GOAL! Canvas bags replace hundreds of disposable plastic bags."
            },
            new ItemMeta {
                FileName = "03_stainless_steel_water_bottle.jpg",
                Title = "Stainless Steel Bottle",
                Category = ItemCategory.Sustainable,
                Fact = "Durable lifetime alternative to thousands of single-use plastic water bottles.",
                Feedback = "GOAL! Refillable steel bottles keep single-use plastic out of landfills."
            },
            new ItemMeta {
                FileName = "04_glass_storage_jars.jpg",
                Title = "Glass Storage Jars",
                Category = ItemCategory.Sustainable,
                Fact = "100% recyclable infinitely without loss of quality or chemical leaching.",
                Feedback = "GOAL! Glass is infinitely recyclable and safe for food storage."
            },
            new ItemMeta {
                FileName = "05_wooden_cooking_spoons.jpg",
                Title = "Wooden Cooking Utensils",
                Category = ItemCategory.Sustainable,
                Fact = "Renewable natural wood cookware that avoids toxic chemical leaching.",
                Feedback = "GOAL! Sustainably harvested wood is biodegradable and non-toxic."
            },
            new ItemMeta {
                FileName = "09_cardboard_box.jpg",
                Title = "Recyclable Cardboard Box",
                Category = ItemCategory.Sustainable,
                Fact = "Easily biodegradable and highly recyclable paper-based packaging material.",
                Feedback = "GOAL! Cardboard can be recycled multiple times into new products."
            },
            new ItemMeta {
                FileName = "10_jute_cord_rope.jpg",
                Title = "Natural Jute Twine",
                Category = ItemCategory.Sustainable,
                Fact = "100% biodegradable natural plant fiber with low carbon footprint.",
                Feedback = "GOAL! Jute is a rapidly renewable natural crop."
            },
            new ItemMeta {
                FileName = "11_loofah_sponge.jpg",
                Title = "Natural Loofah Sponge",
                Category = ItemCategory.Sustainable,
                Fact = "Plant-based biodegradable scrubber that sheds zero synthetic microplastics.",
                Feedback = "GOAL! Loofahs come from dried gourds and compost naturally."
            },
            new ItemMeta {
                FileName = "12_ceramic_mug.jpg",
                Title = "Ceramic Coffee Mug",
                Category = ItemCategory.Sustainable,
                Fact = "Reusable drinkware eliminating millions of disposable paper/plastic cups.",
                Feedback = "GOAL! Using a ceramic mug eliminates daily disposable coffee cups."
            },
            new ItemMeta {
                FileName = "13_aluminum_can.jpg",
                Title = "Recyclable Aluminum Can",
                Category = ItemCategory.Sustainable,
                Fact = "Highly recyclable metal saving 95% of energy compared to raw extraction.",
                Feedback = "GOAL! Aluminum recycling saves massive electrical energy."
            },
            new ItemMeta {
                FileName = "18_coconut_shell_bowl.jpg",
                Title = "Coconut Shell Bowl",
                Category = ItemCategory.Sustainable,
                Fact = "Upcycled agricultural byproduct transformed into zero-waste tableware.",
                Feedback = "GOAL! Upcycling coconut shells reduces agricultural waste burning."
            },
            new ItemMeta {
                FileName = "19_cotton_buds.jpg",
                Title = "Paper/Cotton Ear Swabs",
                Category = ItemCategory.Sustainable,
                Fact = "Organic cotton and paper stems that break down naturally in compost.",
                Feedback = "GOAL! Paper stems prevent plastic pollution in sewage systems."
            },
            new ItemMeta {
                FileName = "21_linen_napkin.jpg",
                Title = "Reusable Linen Napkin",
                Category = ItemCategory.Sustainable,
                Fact = "Washable natural fabric napkin eliminating single-use paper napkin waste.",
                Feedback = "GOAL! Linen napkins can be washed and reused for years."
            },
            new ItemMeta {
                FileName = "24_wool_dryer_balls.jpg",
                Title = "Wool Dryer Balls",
                Category = ItemCategory.Sustainable,
                Fact = "Natural, chemical-free reusable alternative to synthetic dryer sheets.",
                Feedback = "GOAL! Wool dryer balls reduce drying time and electricity consumption."
            },
            new ItemMeta {
                FileName = "25_copper_water_flask.jpg",
                Title = "Copper Water Flask",
                Category = ItemCategory.Sustainable,
                Fact = "Long-lasting antimicrobial reusable vessel that replaces plastic bottles.",
                Feedback = "GOAL! Durable copper vessels last a lifetime."
            },

            // === NON-SUSTAINABLE ITEMS (from dataset/non-sustain) ===
            new ItemMeta {
                FileName = "27_single-use_water_bottle.jpg",
                Title = "Single-Use Plastic Bottle",
                Category = ItemCategory.NonSustainable,
                Fact = "Takes 450+ years to degrade, polluting waterways with toxic microplastics.",
                Feedback = "GREAT DISCARD! Single-use bottles should be avoided and discarded."
            },
            new ItemMeta {
                FileName = "30_plastic_cutlery.jpg",
                Title = "Disposable Plastic Cutlery",
                Category = ItemCategory.NonSustainable,
                Fact = "Used for 10 minutes, persists in landfills and oceans for centuries.",
                Feedback = "GREAT DISCARD! Plastic cutlery is almost never recycled."
            },
            new ItemMeta {
                FileName = "31_disposable_coffee_cup.jpg",
                Title = "Polyethylene-Lined Cup",
                Category = ItemCategory.NonSustainable,
                Fact = "Plastic interior coating prevents conventional paper recycling.",
                Feedback = "GREAT DISCARD! Plastic-lined cups contaminate recycling batches."
            },
            new ItemMeta {
                FileName = "32_bubble_wrap.jpg",
                Title = "Non-Recyclable Bubble Wrap",
                Category = ItemCategory.NonSustainable,
                Fact = "Low-density polyethylene film that chokes standard sorting machinery.",
                Feedback = "GREAT DISCARD! Plastic wraps cannot go in standard recycling."
            },
            new ItemMeta {
                FileName = "35_plastic_cling_film.jpg",
                Title = "PVC Cling Film Wrap",
                Category = ItemCategory.NonSustainable,
                Fact = "Single-use plastic film that cannot be recycled and contaminates food chains.",
                Feedback = "GREAT DISCARD! Cling wrap contributes to microplastic pollution."
            },
            new ItemMeta {
                FileName = "39_plastic_six-pack_ring.jpg",
                Title = "Plastic Six-Pack Ring",
                Category = ItemCategory.NonSustainable,
                Fact = "Severe entanglement hazard to birds, marine turtles, and aquatic life.",
                Feedback = "GREAT DISCARD! Six-pack rings harm marine wildlife."
            },
            new ItemMeta {
                FileName = "40_single-use_coffee_pod.jpg",
                Title = "Aluminum-Plastic Pod",
                Category = ItemCategory.NonSustainable,
                Fact = "Complex composite packaging that mostly ends up in landfills.",
                Feedback = "GREAT DISCARD! Coffee pods create huge unnecessary landfill volume."
            },
            new ItemMeta {
                FileName = "41_plastic_ketchup_sachet.jpg",
                Title = "Multi-Layer Condiment Sachet",
                Category = ItemCategory.NonSustainable,
                Fact = "Unrecyclable composite film creating massive litter everywhere.",
                Feedback = "GREAT DISCARD! Sachets cannot be separated or recycled."
            },
            new ItemMeta {
                FileName = "43_synthetic_cleaning_cloth.jpg",
                Title = "Microfiber Synthetic Cloth",
                Category = ItemCategory.NonSustainable,
                Fact = "Releases millions of microplastic fibers into waterways during washing.",
                Feedback = "GREAT DISCARD! Synthetic wipes shed microplastics into oceans."
            },
            new ItemMeta {
                FileName = "44_plastic_wet_wipes.jpg",
                Title = "Plastic Wet Wipes",
                Category = ItemCategory.NonSustainable,
                Fact = "Contains plastic fibers that cause massive sewer blockages and fatbergs.",
                Feedback = "GREAT DISCARD! Wipes contain plastic and do not dissolve in water."
            },
            new ItemMeta {
                FileName = "45_synthetic_fast_fashion_shirt.jpg",
                Title = "Polyester Fast Fashion",
                Category = ItemCategory.NonSustainable,
                Fact = "Petroleum-derived fabric contributing to textile waste mountains.",
                Feedback = "GREAT DISCARD! Fast fashion creates massive carbon and textile waste."
            },
            new ItemMeta {
                FileName = "46_plastic_toothpick_flosser.jpg",
                Title = "Disposable Plastic Flosser",
                Category = ItemCategory.NonSustainable,
                Fact = "Single-use rigid plastic tool discarded after one single minute of use.",
                Feedback = "GREAT DISCARD! Single-use flossers create needless plastic trash."
            },
            new ItemMeta {
                FileName = "47_disposable_baby_diaper.jpg",
                Title = "Synthetic Disposable Diaper",
                Category = ItemCategory.NonSustainable,
                Fact = "Takes 500 years to decompose in landfills with heavy resource footprint.",
                Feedback = "GREAT DISCARD! Diapers remain in landfills for 500 years."
            },
            new ItemMeta {
                FileName = "48_plastic_party_balloon.jpg",
                Title = "Synthetic Party Balloon",
                Category = ItemCategory.NonSustainable,
                Fact = "Ingested by marine wildlife and birds mistaking fragments for food.",
                Feedback = "GREAT DISCARD! Balloons choke marine animals and birds."
            },
            new ItemMeta {
                FileName = "49_aerosol_spray_can.jpg",
                Title = "VOC Aerosol Spray Can",
                Category = ItemCategory.NonSustainable,
                Fact = "Emits volatile organic compounds that degrade urban air quality.",
                Feedback = "GREAT DISCARD! Aerosols release volatile pollutants."
            },
            new ItemMeta {
                FileName = "50_plastic_milk_jug_cap.jpg",
                Title = "Small Plastic Bottle Cap",
                Category = ItemCategory.NonSustainable,
                Fact = "Frequently escapes sorting screens and ends up as ocean debris.",
                Feedback = "GREAT DISCARD! Loose bottle caps are a top ocean beach pollutant."
            }
        };

        [MenuItem("Sustainable Future/1. Generate Item Database & Assets")]
        public static void GenerateAllItemAssets()
        {
            EnsureDirectoriesExist();
            ConfigureDatasetTextureImporters();

            List<SustainabilityItem> sustainableList = new List<SustainabilityItem>();
            List<SustainabilityItem> nonSustainableList = new List<SustainabilityItem>();

            Color sustainableColor = new Color(0.12f, 0.65f, 0.32f, 1f);
            Color nonSustainableColor = new Color(0.85f, 0.25f, 0.22f, 1f);

            foreach (var meta in ItemDefinitions)
            {
                string targetFolder = meta.Category == ItemCategory.Sustainable ? GetSustainableFolderPath() : GetNonSustainFolderPath();
                string texturePath = $"{targetFolder}/{meta.FileName}";

                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
                if (sprite == null)
                {
                    // Check fallback with .png if .jpg isn't found
                    string pngPath = texturePath.Replace(".jpg", ".png");
                    sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
                }

                if (sprite == null)
                {
                    Debug.LogWarning($"[AutoGenerateItemAssets] Could not load sprite from dataset at: {texturePath}");
                }

                string assetName = Path.GetFileNameWithoutExtension(meta.FileName);
                string assetPath = $"{ItemsAssetPath}/{assetName}.asset";

                SustainabilityItem itemAsset = AssetDatabase.LoadAssetAtPath<SustainabilityItem>(assetPath);
                if (itemAsset == null)
                {
                    itemAsset = ScriptableObject.CreateInstance<SustainabilityItem>();
                    AssetDatabase.CreateAsset(itemAsset, assetPath);
                }

                Color badgeCol = meta.Category == ItemCategory.Sustainable ? sustainableColor : nonSustainableColor;
                itemAsset.Initialize(
                    meta.Title,
                    meta.Category,
                    sprite,
                    "",
                    meta.Fact,
                    meta.Feedback,
                    badgeCol
                );

                EditorUtility.SetDirty(itemAsset);

                if (meta.Category == ItemCategory.Sustainable)
                {
                    sustainableList.Add(itemAsset);
                }
                else
                {
                    nonSustainableList.Add(itemAsset);
                }
            }

            string dbPath = $"{DatabaseAssetPath}/ItemDatabase.asset";
            ItemDatabase database = AssetDatabase.LoadAssetAtPath<ItemDatabase>(dbPath);
            if (database == null)
            {
                database = ScriptableObject.CreateInstance<ItemDatabase>();
                AssetDatabase.CreateAsset(database, dbPath);
            }

            database.SetItems(sustainableList, nonSustainableList);
            EditorUtility.SetDirty(database);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"<color=#2ecc71><b>[Sustainable Future]</b></color> Successfully loaded original images from Assets/dataset! Total items: {database.TotalItemCount} (Sustainable: {sustainableList.Count}, Non-Sustainable: {nonSustainableList.Count})");
        }

        private static string GetSustainableFolderPath()
        {
            if (Directory.Exists(DatasetSustainablePath)) return DatasetSustainablePath;
            if (Directory.Exists(DatsetsSustainablePath)) return DatsetsSustainablePath;
            return DatasetSustainablePath;
        }

        private static string GetNonSustainFolderPath()
        {
            if (Directory.Exists(DatasetNonSustainPath)) return DatasetNonSustainPath;
            if (Directory.Exists(DatsetsNonSustainPath)) return DatsetsNonSustainPath;
            return DatasetNonSustainPath;
        }

        private static void EnsureDirectoriesExist()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Game"))
                AssetDatabase.CreateFolder("Assets", "_Game");

            if (!AssetDatabase.IsValidFolder("Assets/_Game/ScriptableObjects"))
                AssetDatabase.CreateFolder("Assets/_Game", "ScriptableObjects");

            if (!AssetDatabase.IsValidFolder(ItemsAssetPath))
                AssetDatabase.CreateFolder("Assets/_Game/ScriptableObjects", "Items");

            if (!AssetDatabase.IsValidFolder(DatabaseAssetPath))
                AssetDatabase.CreateFolder("Assets/_Game/ScriptableObjects", "Database");
        }

        private static void ConfigureDatasetTextureImporters()
        {
            string[] searchPaths = new string[] {
                GetSustainableFolderPath(),
                GetNonSustainFolderPath()
            };

            bool hasChanged = false;

            foreach (var path in searchPaths)
            {
                if (!Directory.Exists(path)) continue;

                string[] files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    if (file.EndsWith(".meta")) continue;

                    string relativePath = file.Replace("\\", "/");
                    TextureImporter importer = AssetImporter.GetAtPath(relativePath) as TextureImporter;
                    if (importer != null)
                    {
                        bool needReimport = false;
                        if (importer.textureType != TextureImporterType.Sprite)
                        {
                            importer.textureType = TextureImporterType.Sprite;
                            importer.spriteImportMode = SpriteImportMode.Single;
                            importer.mipmapEnabled = false;
                            importer.alphaIsTransparency = true;
                            needReimport = true;
                        }

                        if (needReimport)
                        {
                            importer.SaveAndReimport();
                            hasChanged = true;
                        }
                    }
                }
            }

            if (hasChanged)
            {
                AssetDatabase.Refresh();
            }
        }
    }
}
