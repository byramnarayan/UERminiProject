using System.Collections;
using UnityEngine;

namespace SustainableFuture.Core
{
    /// <summary>
    /// Represents the high-level state of a game session.
    /// </summary>
    public enum GameState
    {
        Ready,
        Playing,
        RoundComplete
    }

    /// <summary>
    /// Master session manager that controls the 30-second round countdown timer,
    /// round lifecycle transitions, and active score aggregation.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Round Settings")]
        [Tooltip("Duration of a single arcade round in seconds.")]
        [SerializeField] private float roundDuration = 30.0f;

        [Tooltip("Whether to automatically kick off the first round on game launch.")]
        [SerializeField] private bool autoStartOnAwake = true;

        private GameState currentState = GameState.Ready;
        private float remainingTime;
        private int currentScore;
        private Coroutine timerCoroutine;

        // --- Public Properties ---
        public GameState CurrentState => currentState;
        public float RemainingTime => remainingTime;
        public int CurrentScore => currentScore;
        public float RoundDuration => roundDuration;

        private void Awake()
        {
            // Singleton instance enforcement
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnEnable()
        {
            GameEvents.OnScoreChanged += HandleScoreChanged;
            GameEvents.OnPlayAgainRequested += StartNewRound;
        }

        private void OnDisable()
        {
            GameEvents.OnScoreChanged -= HandleScoreChanged;
            GameEvents.OnPlayAgainRequested -= StartNewRound;
        }

        private void Start()
        {
            if (autoStartOnAwake)
            {
                StartNewRound();
            }
        }

        /// <summary>
        /// Resets the score and timer and begins a fresh 30-second round.
        /// </summary>
        public void StartNewRound()
        {
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }

            currentScore = 0;
            remainingTime = roundDuration;
            currentState = GameState.Playing;

            // Increment the lifetime games played count
            StatsManager.Instance?.IncrementGamesPlayed();

            // Broadcast round start events
            GameEvents.TriggerGameStarted();
            GameEvents.TriggerTimerTick(remainingTime);

            // Start countdown timer routine
            timerCoroutine = StartCoroutine(RoundTimerRoutine());
        }

        /// <summary>
        /// Ticks down the round timer once per second and notifies listeners.
        /// </summary>
        private IEnumerator RoundTimerRoutine()
        {
            while (remainingTime > 0f)
            {
                yield return new WaitForSeconds(1.0f);
                remainingTime -= 1.0f;

                if (remainingTime < 0f) remainingTime = 0f;

                GameEvents.TriggerTimerTick(remainingTime);
            }

            EndRound();
        }

        /// <summary>
        /// Finalizes the round when the timer reaches 00.
        /// </summary>
        private void EndRound()
        {
            currentState = GameState.RoundComplete;
            StatsManager.Instance?.AddToLifetimeScore(currentScore);

            GameEvents.TriggerRoundFinished();
        }

        /// <summary>
        /// Updates the current round score when a valid action is evaluated.
        /// </summary>
        private void HandleScoreChanged(int deltaPoints, string message, Color feedbackColor)
        {
            if (currentState != GameState.Playing) return;

            currentScore += deltaPoints;
            if (currentScore < 0) currentScore = 0; // Prevent score from dropping below zero
        }
    }
}
