using System;
using SustainableFuture.Data;
using UnityEngine;

namespace SustainableFuture.Core
{
    /// <summary>
    /// Global static Event Bus that decouples systems across the entire game.
    /// Physics, UI, Audio, and Gameplay managers communicate via these events without direct hard references.
    /// This prevents tight coupling and makes the codebase modular and easily testable.
    /// </summary>
    public static class GameEvents
    {
        // ==========================================
        // ROUND LIFECYCLE EVENTS
        // ==========================================
        public static event Action OnGameStarted;
        public static event Action<float> OnTimerTick; // remaining seconds
        public static event Action OnRoundFinished;
        public static event Action OnPlayAgainRequested;

        // ==========================================
        // ITEM & EDUCATIONAL EVALUATION EVENTS
        // ==========================================
        public static event Action<SustainabilityItem> OnItemPresented;
        public static event Action<SustainabilityItem, bool, bool> OnItemEvaluated; 
        // Parameters: (item, isCorrectDecision, wasShotAction)

        // ==========================================
        // BASKETBALL GAMEPLAY EVENTS
        // ==========================================
        public static event Action OnBasketballShot;
        public static event Action OnBasketballGoalScored;
        public static event Action OnBasketballMissed;
        public static event Action OnDiscardPressed;

        // ==========================================
        // FEEDBACK & SCORE NOTIFICATIONS
        // ==========================================
        public static event Action<int, string, Color> OnScoreChanged; // (deltaPoints, message, feedbackColor)
        public static event Action<int> OnStreakUpdated; // current streak value
        public static event Action<int> OnBestBasketsUpdated; // highest baskets in round

        // ==========================================
        // TRIGGER METHODS (Dispatched by Systems)
        // ==========================================
        public static void TriggerGameStarted() => OnGameStarted?.Invoke();
        public static void TriggerTimerTick(float remainingTime) => OnTimerTick?.Invoke(remainingTime);
        public static void TriggerRoundFinished() => OnRoundFinished?.Invoke();
        public static void TriggerPlayAgainRequested() => OnPlayAgainRequested?.Invoke();

        public static void TriggerItemPresented(SustainabilityItem item) => OnItemPresented?.Invoke(item);
        public static void TriggerItemEvaluated(SustainabilityItem item, bool isCorrect, bool wasShot) 
            => OnItemEvaluated?.Invoke(item, isCorrect, wasShot);

        public static void TriggerBasketballShot() => OnBasketballShot?.Invoke();
        public static void TriggerBasketballGoalScored() => OnBasketballGoalScored?.Invoke();
        public static void TriggerBasketballMissed() => OnBasketballMissed?.Invoke();
        public static void TriggerDiscardPressed() => OnDiscardPressed?.Invoke();

        public static void TriggerScoreChanged(int delta, string msg, Color color) 
            => OnScoreChanged?.Invoke(delta, msg, color);
        public static void TriggerStreakUpdated(int streak) => OnStreakUpdated?.Invoke(streak);
        public static void TriggerBestBasketsUpdated(int bestBaskets) => OnBestBasketsUpdated?.Invoke(bestBaskets);
    }
}
