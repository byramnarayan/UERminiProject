using SustainableFuture.Core;
using UnityEngine;

namespace SustainableFuture.Core
{
    /// <summary>
    /// Manages persistent player statistics and calculates lifetime accuracy, best baskets, and scores.
    /// Data is saved persistently using Unity's PlayerPrefs system.
    /// </summary>
    public class StatsManager : MonoBehaviour
    {
        // PlayerPrefs Storage Keys
        private const string KeyBestScore = "SF_BEST_SCORE";
        private const string KeyBestBaskets = "SF_BEST_BASKETS";
        private const string KeyGamesPlayed = "SF_GAMES_PLAYED";
        private const string KeyLifetimeScore = "SF_LIFETIME_SCORE";
        private const string KeyLifetimeBaskets = "SF_LIFETIME_BASKETS";
        private const string KeyBestStreak = "SF_BEST_STREAK";
        private const string KeyTotalAttempts = "SF_TOTAL_ATTEMPTS";
        private const string KeyTotalCorrect = "SF_TOTAL_CORRECT";

        public static StatsManager Instance { get; private set; }

        // --- Persistent Stats Properties ---
        public int BestScore { get; private set; }
        public int BestBaskets { get; private set; }
        public int GamesPlayed { get; private set; }
        public int CurrentStreak { get; private set; }
        public int BestStreak { get; private set; }
        public int LifetimeScore { get; private set; }
        public int LifetimeBaskets { get; private set; }
        public int CurrentRoundBaskets => currentRoundBaskets;

        // --- Accuracy Calculation ---
        public int TotalAttempts { get; private set; }
        public int TotalCorrect { get; private set; }
        public float Accuracy => TotalAttempts > 0 ? ((float)TotalCorrect / TotalAttempts) * 100f : 0f;

        private int currentRoundBaskets;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            LoadStats();
        }

        private void OnEnable()
        {
            GameEvents.OnGameStarted += HandleGameStarted;
            GameEvents.OnStreakUpdated += HandleStreakUpdated;
            GameEvents.OnRoundFinished += HandleRoundFinished;
            GameEvents.OnItemEvaluated += HandleItemEvaluated;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= HandleGameStarted;
            GameEvents.OnStreakUpdated -= HandleStreakUpdated;
            GameEvents.OnRoundFinished -= HandleRoundFinished;
            GameEvents.OnItemEvaluated -= HandleItemEvaluated;
        }

        /// <summary>
        /// Loads saved stats from PlayerPrefs storage.
        /// </summary>
        private void LoadStats()
        {
            BestScore = PlayerPrefs.GetInt(KeyBestScore, 0);
            BestBaskets = PlayerPrefs.GetInt(KeyBestBaskets, 0);
            GamesPlayed = PlayerPrefs.GetInt(KeyGamesPlayed, 0);
            LifetimeScore = PlayerPrefs.GetInt(KeyLifetimeScore, 0);
            LifetimeBaskets = PlayerPrefs.GetInt(KeyLifetimeBaskets, 0);
            BestStreak = PlayerPrefs.GetInt(KeyBestStreak, 0);
            TotalAttempts = PlayerPrefs.GetInt(KeyTotalAttempts, 0);
            TotalCorrect = PlayerPrefs.GetInt(KeyTotalCorrect, 0);
            CurrentStreak = 0;
        }

        /// <summary>
        /// Saves current statistics to disk.
        /// </summary>
        private void SaveStats()
        {
            PlayerPrefs.SetInt(KeyBestScore, BestScore);
            PlayerPrefs.SetInt(KeyBestBaskets, BestBaskets);
            PlayerPrefs.SetInt(KeyGamesPlayed, GamesPlayed);
            PlayerPrefs.SetInt(KeyLifetimeScore, LifetimeScore);
            PlayerPrefs.SetInt(KeyLifetimeBaskets, LifetimeBaskets);
            PlayerPrefs.SetInt(KeyBestStreak, BestStreak);
            PlayerPrefs.SetInt(KeyTotalAttempts, TotalAttempts);
            PlayerPrefs.SetInt(KeyTotalCorrect, TotalCorrect);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Increments the total games/rounds played count.
        /// </summary>
        public void IncrementGamesPlayed()
        {
            GamesPlayed++;
            currentRoundBaskets = 0;
            SaveStats();
        }

        /// <summary>
        /// Records a scored basket in the current round and updates lifetime records.
        /// </summary>
        public void RegisterBasketScored()
        {
            currentRoundBaskets++;
            LifetimeBaskets++;

            if (currentRoundBaskets > BestBaskets)
            {
                BestBaskets = currentRoundBaskets;
                GameEvents.TriggerBestBasketsUpdated(BestBaskets);
            }
            SaveStats();
        }

        private void HandleGameStarted()
        {
            CurrentStreak = 0;
            currentRoundBaskets = 0;
        }

        /// <summary>
        /// Tracks total player decisions to compute overall accuracy.
        /// </summary>
        private void HandleItemEvaluated(Data.SustainabilityItem item, bool isCorrect, bool wasShot)
        {
            TotalAttempts++;
            if (isCorrect) TotalCorrect++;
            SaveStats();
        }

        private void HandleStreakUpdated(int streak)
        {
            CurrentStreak = streak;
            if (CurrentStreak > BestStreak)
            {
                BestStreak = CurrentStreak;
            }
            SaveStats();
        }

        private void HandleRoundFinished()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentScore > BestScore)
            {
                BestScore = GameManager.Instance.CurrentScore;
            }
            SaveStats();
        }

        public void AddToLifetimeScore(int score)
        {
            LifetimeScore += score;
            if (score > BestScore)
            {
                BestScore = score;
            }
            SaveStats();
        }

        /// <summary>
        /// Developer utility to reset all statistics back to zero.
        /// </summary>
        public void ResetAllStats()
        {
            PlayerPrefs.DeleteKey(KeyBestScore);
            PlayerPrefs.DeleteKey(KeyBestBaskets);
            PlayerPrefs.DeleteKey(KeyGamesPlayed);
            PlayerPrefs.DeleteKey(KeyLifetimeScore);
            PlayerPrefs.DeleteKey(KeyLifetimeBaskets);
            PlayerPrefs.DeleteKey(KeyBestStreak);
            PlayerPrefs.DeleteKey(KeyTotalAttempts);
            PlayerPrefs.DeleteKey(KeyTotalCorrect);
            PlayerPrefs.Save();
            LoadStats();
        }
    }
}
