using SustainableFuture.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SustainableFuture.UI
{
    public class UIResultsModal : MonoBehaviour
    {
        [Header("UI Text Displays")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text basketsText;
        [SerializeField] private TMP_Text streakText;
        [SerializeField] private TMP_Text avgText;
        [SerializeField] private TMP_Text titleBadgeText;
        [SerializeField] private TMP_Text motivationalText;

        [Header("Buttons")]
        [SerializeField] private Button playAgainButton;

        private void Awake()
        {
            if (playAgainButton != null)
            {
                playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            }
        }

        public void Show(int finalScore, int baskets, int streak, float accuracy)
        {
            gameObject.SetActive(true);

            if (scoreText != null) scoreText.text = $"SCORE: {finalScore}";
            if (basketsText != null) basketsText.text = $"BASKETS: {baskets}";
            if (streakText != null) streakText.text = $"STREAK: {streak}";
            if (avgText != null) avgText.text = $"ACCURACY: {accuracy:F0}%";

            // Grade feedback without missing unicode emojis
            if (titleBadgeText != null)
            {
                if (finalScore >= 80)
                    titleBadgeText.text = "ECO CHAMPION!";
                else if (finalScore >= 40)
                    titleBadgeText.text = "PLANET PROTECTOR!";
                else if (finalScore >= 10)
                    titleBadgeText.text = "ECO APPRENTICE!";
                else
                    titleBadgeText.text = "KEEP PRACTICING!";
            }

            if (motivationalText != null)
            {
                if (finalScore >= 60)
                    motivationalText.text = "Outstanding environmental awareness and shooting skills!";
                else
                    motivationalText.text = "REMEMBER: Shoot sustainable items, discard non-sustainable waste!";
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnPlayAgainClicked()
        {
            Hide();
            GameEvents.TriggerPlayAgainRequested();
        }
    }
}
