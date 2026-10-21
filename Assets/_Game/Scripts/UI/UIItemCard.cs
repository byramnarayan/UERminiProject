using System.Collections;
using SustainableFuture.Core;
using SustainableFuture.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SustainableFuture.UI
{
    public class UIItemCard : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image itemIconImage;
        [SerializeField] private TMP_Text itemCategoryBadgeText;
        [SerializeField] private Image itemCategoryBadgeBg;
        [SerializeField] private TMP_Text itemTitleText;
        [SerializeField] private TMP_Text itemFactText;
        [SerializeField] private RectTransform cardRect;

        [Header("Colors")]
        [SerializeField] private Color sustainableBadgeColor = new Color(0.12f, 0.65f, 0.35f, 1f);
        [SerializeField] private Color nonSustainableBadgeColor = new Color(0.85f, 0.25f, 0.22f, 1f);

        private Coroutine popCoroutine;

        private void Awake()
        {
            if (cardRect == null) cardRect = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            GameEvents.OnItemPresented += DisplayItem;
        }

        private void OnDisable()
        {
            GameEvents.OnItemPresented -= DisplayItem;
        }

        public void DisplayItem(SustainabilityItem item)
        {
            if (item == null) return;

            if (itemIconImage != null)
            {
                itemIconImage.sprite = item.ItemSprite;
                itemIconImage.enabled = item.ItemSprite != null;
                itemIconImage.preserveAspect = true;
            }

            if (itemCategoryBadgeText != null)
            {
                itemCategoryBadgeText.text = item.IsSustainable ? "SUSTAINABLE ITEM" : "NON-SUSTAINABLE ITEM";
                itemCategoryBadgeText.color = Color.white;
            }

            if (itemCategoryBadgeBg != null)
            {
                itemCategoryBadgeBg.color = item.IsSustainable ? sustainableBadgeColor : nonSustainableBadgeColor;
            }

            if (itemTitleText != null)
            {
                itemTitleText.text = item.ItemTitle.ToUpper();
                itemTitleText.color = item.IsSustainable ? new Color(0.72f, 0.98f, 0.82f) : new Color(1f, 0.75f, 0.75f);
            }

            if (itemFactText != null)
            {
                itemFactText.text = item.EducationalFact;
            }

            // Animate card punch
            if (gameObject.activeInHierarchy)
            {
                if (popCoroutine != null) StopCoroutine(popCoroutine);
                popCoroutine = StartCoroutine(AnimateCardPop());
            }
        }

        private IEnumerator AnimateCardPop()
        {
            if (cardRect == null) yield break;

            float elapsed = 0f;
            float duration = 0.22f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float scale = 0.94f + Mathf.Sin(t * Mathf.PI) * 0.1f;
                cardRect.localScale = Vector3.one * scale;
                yield return null;
            }

            cardRect.localScale = Vector3.one;
        }
    }
}
