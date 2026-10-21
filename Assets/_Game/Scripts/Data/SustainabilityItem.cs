using UnityEngine;

namespace SustainableFuture.Data
{
    /// <summary>
    /// Categorizes items into environmentally friendly (Sustainable) or single-use / polluting (NonSustainable).
    /// </summary>
    public enum ItemCategory
    {
        Sustainable,
        NonSustainable
    }

    /// <summary>
    /// ScriptableObject data model representing a single real-world environmental item.
    /// ScriptableObjects are lightweight data containers stored as Unity assets, 
    /// allowing easy balancing without modifying game logic.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "Sustainable Future/Sustainability Item")]
    public class SustainabilityItem : ScriptableObject
    {
        [Header("Display Info")]
        [Tooltip("The human-readable name of the item (e.g., 'Bamboo Toothbrush').")]
        [SerializeField] private string itemTitle;

        [Tooltip("Defines whether this item should be shot (Sustainable) or discarded (Non-Sustainable).")]
        [SerializeField] private ItemCategory category;

        [Tooltip("The raw unmodified product texture loaded from the dataset.")]
        [SerializeField] private Sprite itemSprite;

        [Tooltip("Optional emoji marker (left empty if font does not support emojis).")]
        [SerializeField] private string emojiIcon = "";

        [Header("Educational Information")]
        [Tooltip("A concise, factual educational sentence explaining why this item is or isn't eco-friendly.")]
        [TextArea(2, 4)]
        [SerializeField] private string educationalFact;

        [Tooltip("Feedback message displayed when the player makes a decision on this item.")]
        [TextArea(2, 3)]
        [SerializeField] private string actionFeedback;

        [Header("Styling")]
        [Tooltip("The theme color for the item's UI badges (Green for sustainable, Red/Amber for non-sustainable).")]
        [SerializeField] private Color badgeColor = new Color(0.18f, 0.65f, 0.35f, 1f);

        // --- Public Read-Only Properties ---
        public string ItemTitle => itemTitle;
        public ItemCategory Category => category;
        public Sprite ItemSprite => itemSprite;
        public string EmojiIcon => emojiIcon;
        public string EducationalFact => educationalFact;
        public string ActionFeedback => actionFeedback;
        public Color BadgeColor => badgeColor;
        
        /// <summary>
        /// Convenience helper to check if this item is sustainable.
        /// </summary>
        public bool IsSustainable => category == ItemCategory.Sustainable;

        /// <summary>
        /// Programmatically initializes all fields (used by AutoGenerateItemAssets).
        /// </summary>
        public void Initialize(
            string title, 
            ItemCategory cat, 
            Sprite sprite, 
            string emoji, 
            string fact, 
            string feedback, 
            Color color)
        {
            itemTitle = title;
            category = cat;
            itemSprite = sprite;
            emojiIcon = emoji;
            educationalFact = fact;
            actionFeedback = feedback;
            badgeColor = color;
        }
    }
}
