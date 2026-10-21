using System.Collections.Generic;
using SustainableFuture.Core;
using SustainableFuture.Data;
using UnityEngine;

namespace SustainableFuture.Gameplay
{
    /// <summary>
    /// Evaluates player decisions (SHOOT vs DISCARD) against the current sustainability item.
    /// Manages item queueing, Fisher-Yates deck shuffling, and dispatches score/streak delta events.
    /// </summary>
    public class ItemManager : MonoBehaviour
    {
        [Header("Database")]
        [Tooltip("The master ItemDatabase ScriptableObject asset.")]
        [SerializeField] private ItemDatabase itemDatabase;

        [Header("Point Values")]
        [Tooltip("Points awarded for correct sustainability decisions.")]
        [SerializeField] private int correctPoints = 10;

        [Tooltip("Points deducted for incorrect decisions.")]
        [SerializeField] private int incorrectPoints = -10;

        private List<SustainabilityItem> shuffledDeck = new List<SustainabilityItem>();
        private int currentDeckIndex = 0;
        private SustainabilityItem currentItem;
        private int currentStreak = 0;
        private bool isRoundActive = false;

        // --- Public Read-Only Properties ---
        public SustainabilityItem CurrentItem => currentItem;
        public int CurrentStreak => currentStreak;

        private void OnEnable()
        {
            GameEvents.OnGameStarted += HandleGameStarted;
            GameEvents.OnRoundFinished += HandleRoundFinished;
            GameEvents.OnBasketballGoalScored += HandleGoalScored;
            GameEvents.OnDiscardPressed += HandleDiscardPressed;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= HandleGameStarted;
            GameEvents.OnRoundFinished -= HandleRoundFinished;
            GameEvents.OnBasketballGoalScored -= HandleGoalScored;
            GameEvents.OnDiscardPressed -= HandleDiscardPressed;
        }

        public void SetDatabase(ItemDatabase db)
        {
            itemDatabase = db;
        }

        private void HandleGameStarted()
        {
            isRoundActive = true;
            currentStreak = 0;
            GameEvents.TriggerStreakUpdated(0);

            BuildAndShuffleDeck();
            PresentNextItem();
        }

        private void HandleRoundFinished()
        {
            isRoundActive = false;
        }

        /// <summary>
        /// Populates and randomizes the item deck using the Fisher-Yates shuffle algorithm.
        /// </summary>
        private void BuildAndShuffleDeck()
        {
            if (itemDatabase == null || itemDatabase.AllItems.Count == 0)
            {
                Debug.LogWarning("[ItemManager] ItemDatabase is missing or empty!");
                return;
            }

            shuffledDeck.Clear();
            shuffledDeck.AddRange(itemDatabase.AllItems);

            // Fisher-Yates Shuffle
            for (int i = shuffledDeck.Count - 1; i > 0; i--)
            {
                int rnd = Random.Range(0, i + 1);
                var temp = shuffledDeck[i];
                shuffledDeck[i] = shuffledDeck[rnd];
                shuffledDeck[rnd] = temp;
            }

            currentDeckIndex = 0;
        }

        /// <summary>
        /// Presents the next environmental challenge item to the player.
        /// </summary>
        public void PresentNextItem()
        {
            if (!isRoundActive) return;

            if (shuffledDeck.Count == 0 || currentDeckIndex >= shuffledDeck.Count)
            {
                BuildAndShuffleDeck();
            }

            currentItem = shuffledDeck[currentDeckIndex];
            currentDeckIndex++;

            GameEvents.TriggerItemPresented(currentItem);
        }

        /// <summary>
        /// Evaluates the outcome when a basketball GOAL is made.
        /// Sustainable + Goal = Correct (+10)
        /// Non-Sustainable + Goal = Incorrect (-10)
        /// </summary>
        private void HandleGoalScored()
        {
            if (!isRoundActive || currentItem == null) return;

            StatsManager.Instance?.RegisterBasketScored();

            if (currentItem.IsSustainable)
            {
                // Sustainable Item + Shot Scored -> Correct!
                currentStreak++;
                GameEvents.TriggerStreakUpdated(currentStreak);
                Color greenColor = new Color(0.18f, 0.85f, 0.35f, 1f);
                GameEvents.TriggerScoreChanged(correctPoints, "GOAL! +10", greenColor);
                GameEvents.TriggerItemEvaluated(currentItem, true, true);
            }
            else
            {
                // Non-Sustainable Item + Shot Scored -> Incorrect!
                currentStreak = 0;
                GameEvents.TriggerStreakUpdated(0);
                Color orangeColor = new Color(0.95f, 0.45f, 0.2f, 1f);
                GameEvents.TriggerScoreChanged(incorrectPoints, "THINK AGAIN! -10", orangeColor);
                GameEvents.TriggerItemEvaluated(currentItem, false, true);
            }

            PresentNextItem();
        }

        /// <summary>
        /// Evaluates the outcome when the DISCARD button is pressed.
        /// Non-Sustainable + Discard = Correct (+10)
        /// Sustainable + Discard = Incorrect (-10)
        /// </summary>
        private void HandleDiscardPressed()
        {
            if (!isRoundActive || currentItem == null) return;

            if (!currentItem.IsSustainable)
            {
                // Non-Sustainable Item + Discarded -> Correct!
                currentStreak++;
                GameEvents.TriggerStreakUpdated(currentStreak);
                Color greenColor = new Color(0.18f, 0.85f, 0.35f, 1f);
                GameEvents.TriggerScoreChanged(correctPoints, "DISCARDED! +10", greenColor);
                GameEvents.TriggerItemEvaluated(currentItem, true, false);
            }
            else
            {
                // Sustainable Item + Discarded -> Incorrect!
                currentStreak = 0;
                GameEvents.TriggerStreakUpdated(0);
                Color redColor = new Color(0.95f, 0.25f, 0.22f, 1f);
                GameEvents.TriggerScoreChanged(incorrectPoints, "WRONG! -10", redColor);
                GameEvents.TriggerItemEvaluated(currentItem, false, false);
            }

            PresentNextItem();
        }
    }
}
