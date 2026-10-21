using SustainableFuture.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SustainableFuture.UI
{
    /// <summary>
    /// Master UI coordinator that updates the Top-Left STATS panel, Top-Right ARCADE HOOPS panel,
    /// Discard action button, floating feedback notifications, and Game Over Results modal.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Stats Panel (Upper Left)")]
        [Tooltip("Displays lifetime best point score.")]
        [SerializeField] private TMP_Text bestText;

        [Tooltip("Displays baskets scored in round / lifetime.")]
        [SerializeField] private TMP_Text basketsText;

        [Tooltip("Displays total completed games / rounds.")]
        [SerializeField] private TMP_Text gamesText;

        [Tooltip("Displays current consecutive correct decisions streak.")]
        [SerializeField] private TMP_Text streakText;

        [Tooltip("Displays lifetime accuracy percentage.")]
        [SerializeField] private TMP_Text accuracyText;

        [Header("Arcade Hoops Panel (Upper Right)")]
        [Tooltip("Displays remaining countdown seconds.")]
        [SerializeField] private TMP_Text timeText;

        [Tooltip("Displays current round points score.")]
        [SerializeField] private TMP_Text scoreText;

        [Header("Item & Action Controls")]
        [Tooltip("Reference to the Item Card UI component.")]
        [SerializeField] private UIItemCard itemCard;

        [Tooltip("The Discard button used to reject non-sustainable items.")]
        [SerializeField] private Button discardButton;

        [Header("Modals & Feedback")]
        [Tooltip("Post-round game summary modal.")]
        [SerializeField] private UIResultsModal resultsModal;

        [Tooltip("Prefab for floating score popup (+10/-10).")]
        [SerializeField] private UIFloatingFeedback floatingFeedbackPrefab;

        [Tooltip("Spawn anchor point for floating feedback text.")]
        [SerializeField] private RectTransform feedbackSpawnPoint;

        private UIFloatingFeedback activeFeedback;

        private void Awake()
        {
            if (discardButton != null)
            {
                discardButton.onClick.AddListener(OnDiscardButtonClicked);
            }

            if (resultsModal != null)
            {
                resultsModal.Hide();
            }

            if (floatingFeedbackPrefab != null && feedbackSpawnPoint != null)
            {
                activeFeedback = Instantiate(floatingFeedbackPrefab, feedbackSpawnPoint);
                activeFeedback.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            GameEvents.OnGameStarted += HandleGameStarted;
            GameEvents.OnTimerTick += HandleTimerTick;
            GameEvents.OnScoreChanged += HandleScoreChanged;
            GameEvents.OnStreakUpdated += HandleStreakUpdated;
            GameEvents.OnBestBasketsUpdated += HandleBestBasketsUpdated;
            GameEvents.OnRoundFinished += HandleRoundFinished;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= HandleGameStarted;
            GameEvents.OnTimerTick -= HandleTimerTick;
            GameEvents.OnScoreChanged -= HandleScoreChanged;
            GameEvents.OnStreakUpdated -= HandleStreakUpdated;
            GameEvents.OnBestBasketsUpdated -= HandleBestBasketsUpdated;
            GameEvents.OnRoundFinished -= HandleRoundFinished;
        }

        private void Start()
        {
            UpdateAllStatsPanels();
        }

        /// <summary>
        /// Refreshes all text values on the HUD from active managers.
        /// </summary>
        public void UpdateAllStatsPanels()
        {
            if (StatsManager.Instance != null)
            {
                if (bestText != null) bestText.text = $"{StatsManager.Instance.BestScore}";
                if (basketsText != null) basketsText.text = $"{StatsManager.Instance.BestBaskets}";
                if (gamesText != null) gamesText.text = $"{StatsManager.Instance.GamesPlayed}";
                if (streakText != null) streakText.text = $"{StatsManager.Instance.CurrentStreak}";
                if (accuracyText != null) accuracyText.text = $"{StatsManager.Instance.Accuracy:F0}%";
            }

            if (GameManager.Instance != null)
            {
                if (scoreText != null) scoreText.text = $"{GameManager.Instance.CurrentScore}";
                if (timeText != null) timeText.text = $"{(int)GameManager.Instance.RemainingTime:D2}";
            }
        }

        private void HandleGameStarted()
        {
            if (resultsModal != null) resultsModal.Hide();
            if (discardButton != null) discardButton.interactable = true;

            UpdateAllStatsPanels();
        }

        /// <summary>
        /// Updates countdown clock and pulses color when time is critical (<= 5 seconds).
        /// </summary>
        private void HandleTimerTick(float remainingSeconds)
        {
            if (timeText != null)
            {
                timeText.text = $"{(int)remainingSeconds:D2}";

                if (remainingSeconds <= 5f)
                {
                    timeText.color = remainingSeconds % 2 == 0 ? new Color(1f, 0.3f, 0.25f) : new Color(1f, 0.85f, 0.2f);
                }
                else
                {
                    timeText.color = Color.white;
                }
            }
        }

        /// <summary>
        /// Updates scoreboard and spawns animated floating score text.
        /// </summary>
        private void HandleScoreChanged(int deltaPoints, string message, Color feedbackColor)
        {
            if (GameManager.Instance != null && scoreText != null)
            {
                scoreText.text = $"{GameManager.Instance.CurrentScore}";
            }

            UpdateAllStatsPanels();

            if (activeFeedback != null && feedbackSpawnPoint != null)
            {
                activeFeedback.Show(message, feedbackColor, Vector2.zero);
            }
        }

        private void HandleStreakUpdated(int currentStreak)
        {
            if (streakText != null)
            {
                streakText.text = $"{currentStreak}";
            }
        }

        private void HandleBestBasketsUpdated(int bestBaskets)
        {
            if (basketsText != null)
            {
                basketsText.text = $"{bestBaskets}";
            }
        }

        /// <summary>
        /// Displays the results modal upon round completion.
        /// </summary>
        private void HandleRoundFinished()
        {
            if (discardButton != null) discardButton.interactable = false;

            int finalScore = GameManager.Instance != null ? GameManager.Instance.CurrentScore : 0;
            int baskets = StatsManager.Instance != null ? StatsManager.Instance.BestBaskets : 0;
            int streak = StatsManager.Instance != null ? StatsManager.Instance.CurrentStreak : 0;
            float accuracy = StatsManager.Instance != null ? StatsManager.Instance.Accuracy : 0f;

            if (resultsModal != null)
            {
                resultsModal.Show(finalScore, baskets, streak, accuracy);
            }

            UpdateAllStatsPanels();
        }

        private void OnDiscardButtonClicked()
        {
            GameEvents.TriggerDiscardPressed();
        }
    }
}
