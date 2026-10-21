using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SustainableFuture.UI
{
    /// <summary>
    /// Animates dynamic floating score notifications (+10, -10, GOAL!, WRONG!)
    /// that float upward, pulse, and fade out smoothly over the action area.
    /// </summary>
    public class UIFloatingFeedback : MonoBehaviour
    {
        [Tooltip("TextMeshPro text display.")]
        [SerializeField] private TMP_Text tmpText;

        [Tooltip("Legacy Text fallback if TMP is absent.")]
        [SerializeField] private Text legacyText;

        [Tooltip("RectTransform animated during float up.")]
        [SerializeField] private RectTransform rectTransform;

        [Tooltip("CanvasGroup controlling opacity fade.")]
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (tmpText == null) tmpText = GetComponentInChildren<TMP_Text>();
            if (legacyText == null) legacyText = GetComponentInChildren<Text>();
        }

        /// <summary>
        /// Activates and starts the floating text animation with custom message and color.
        /// </summary>
        public void Show(string message, Color color, Vector2 startAnchoredPos)
        {
            gameObject.SetActive(true);

            if (tmpText != null)
            {
                tmpText.text = message;
                tmpText.color = color;
            }
            else if (legacyText != null)
            {
                legacyText.text = message;
                legacyText.color = color;
            }

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = startAnchoredPos;
            }

            StopAllCoroutines();
            StartCoroutine(AnimateRoutine());
        }

        /// <summary>
        /// Smoothly interpolates position, scale punch, and alpha fade out.
        /// </summary>
        private IEnumerator AnimateRoutine()
        {
            float elapsed = 0f;
            float duration = 1.1f;
            Vector2 initialPos = rectTransform != null ? rectTransform.anchoredPosition : Vector2.zero;

            if (canvasGroup != null) canvasGroup.alpha = 1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Float upward
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = initialPos + new Vector2(0f, Mathf.Lerp(0f, 75f, t));
                    float scale = Mathf.Lerp(1.3f, 1.0f, Mathf.Clamp01(t * 3f));
                    rectTransform.localScale = Vector3.one * scale;
                }

                // Fade out in second half
                if (canvasGroup != null && t > 0.5f)
                {
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, (t - 0.5f) / 0.5f);
                }

                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
