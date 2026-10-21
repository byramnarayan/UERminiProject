using System.Collections.Generic;
using UnityEngine;

namespace SustainableFuture.Data
{
    /// <summary>
    /// Master ScriptableObject database that aggregates all sustainable and non-sustainable items.
    /// Acts as the single source of truth for the ItemManager during gameplay rounds.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Sustainable Future/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [Header("Item Collections")]
        [Tooltip("Collection of eco-friendly items that should be scored by shooting the basketball.")]
        [SerializeField] private List<SustainabilityItem> sustainableItems = new List<SustainabilityItem>();

        [Tooltip("Collection of polluting / single-use items that should be discarded using the Discard button.")]
        [SerializeField] private List<SustainabilityItem> nonSustainableItems = new List<SustainabilityItem>();

        [Tooltip("Unified list containing all items in the database.")]
        [SerializeField] private List<SustainabilityItem> allItems = new List<SustainabilityItem>();

        // Public Read-Only Accessors
        public IReadOnlyList<SustainabilityItem> SustainableItems => sustainableItems;
        public IReadOnlyList<SustainabilityItem> NonSustainableItems => nonSustainableItems;
        public IReadOnlyList<SustainabilityItem> AllItems => allItems;

        /// <summary>
        /// Total number of registered items across all categories.
        /// </summary>
        public int TotalItemCount => allItems.Count;

        /// <summary>
        /// Rebuilds the collections and unifies them into allItems.
        /// Called automatically during the asset generation phase.
        /// </summary>
        public void SetItems(List<SustainabilityItem> sustainable, List<SustainabilityItem> nonSustainable)
        {
            sustainableItems = new List<SustainabilityItem>(sustainable);
            nonSustainableItems = new List<SustainabilityItem>(nonSustainable);
            
            allItems = new List<SustainabilityItem>();
            allItems.AddRange(sustainableItems);
            allItems.AddRange(nonSustainableItems);
        }

        /// <summary>
        /// Returns a random item from the full database.
        /// </summary>
        public SustainabilityItem GetRandomItem()
        {
            if (allItems == null || allItems.Count == 0) return null;
            return allItems[Random.Range(0, allItems.Count)];
        }

        /// <summary>
        /// Returns a random item from a specific preferred category.
        /// </summary>
        public SustainabilityItem GetRandomBalancedItem(ItemCategory preferredCategory)
        {
            if (preferredCategory == ItemCategory.Sustainable && sustainableItems.Count > 0)
            {
                return sustainableItems[Random.Range(0, sustainableItems.Count)];
            }
            if (preferredCategory == ItemCategory.NonSustainable && nonSustainableItems.Count > 0)
            {
                return nonSustainableItems[Random.Range(0, nonSustainableItems.Count)];
            }
            return GetRandomItem();
        }
    }
}
