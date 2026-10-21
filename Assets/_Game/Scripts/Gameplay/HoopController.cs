using System.Collections;
using SustainableFuture.Core;
using UnityEngine;

namespace SustainableFuture.Gameplay
{
    /// <summary>
    /// Dual-sensor hoop scoring controller that validates genuine downward basket shots
    /// and triggers visual net squash/stretch animations.
    /// </summary>
    public class HoopController : MonoBehaviour
    {
        [Header("Sensors")]
        [Tooltip("Trigger collider placed above the rim.")]
        [SerializeField] private Collider upperTrigger;

        [Tooltip("Trigger collider placed below the net.")]
        [SerializeField] private Collider lowerTrigger;

        [Header("Validation Settings")]
        [Tooltip("Maximum allowed seconds between entering upper sensor and lower sensor for a valid score.")]
        [SerializeField] private float maxSensorTravelTime = 1.2f;

        [Header("Visual Feedback")]
        [Tooltip("Transform of the net mesh for animating swish squash & stretch.")]
        [SerializeField] private Transform netTransform;

        [Tooltip("Optional particle system played when a goal is scored.")]
        [SerializeField] private ParticleSystem scoreParticleFX;

        private float upperTriggerEnterTime = -1f;
        private Basketball pendingBasketball;

        private void Awake()
        {
            if (upperTrigger == null || lowerTrigger == null)
            {
                HoopSensor[] sensors = GetComponentsInChildren<HoopSensor>(true);
                foreach (var s in sensors)
                {
                    if (s.SensorType == HoopSensorType.Upper) upperTrigger = s.GetComponent<Collider>();
                    if (s.SensorType == HoopSensorType.Lower) lowerTrigger = s.GetComponent<Collider>();
                }
            }
        }

        /// <summary>
        /// Sensor 1: Triggers when the ball enters the rim from above.
        /// </summary>
        public void OnUpperSensorTriggered(Basketball ball)
        {
            if (ball == null || !ball.IsShot || ball.HasScored) return;

            // Only register downward trajectory
            if (ball.Rigidbody != null && ball.Rigidbody.linearVelocity.y <= 0.1f)
            {
                upperTriggerEnterTime = Time.time;
                pendingBasketball = ball;
            }
        }

        /// <summary>
        /// Sensor 2: Triggers when the ball exits the bottom of the net.
        /// Validates that Sensor 1 was triggered recently in the proper downward order.
        /// </summary>
        public void OnLowerSensorTriggered(Basketball ball)
        {
            if (ball == null || !ball.IsShot || ball.HasScored) return;

            // Goal validation: Upper sensor must have triggered within maxSensorTravelTime
            if (pendingBasketball == ball && (Time.time - upperTriggerEnterTime) <= maxSensorTravelTime)
            {
                upperTriggerEnterTime = -1f;
                pendingBasketball = null;

                ball.RegisterGoal();
                PlayScoreFeedback();
                GameEvents.TriggerBasketballGoalScored();
            }
        }

        /// <summary>
        /// Triggers visual particle effect and net swish animation.
        /// </summary>
        private void PlayScoreFeedback()
        {
            if (scoreParticleFX != null)
            {
                scoreParticleFX.Play();
            }

            if (netTransform != null)
            {
                StopAllCoroutines();
                StartCoroutine(AnimateNetSwish());
            }
        }

        /// <summary>
        /// Procedural squash & stretch animation for the basketball net.
        /// </summary>
        private IEnumerator AnimateNetSwish()
        {
            Vector3 originalScale = Vector3.one;
            float elapsed = 0f;
            float duration = 0.35f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float scaleY = 1.0f + Mathf.Sin(t * Mathf.PI) * 0.25f;
                float scaleXZ = 1.0f - Mathf.Sin(t * Mathf.PI) * 0.15f;

                if (netTransform != null)
                {
                    netTransform.localScale = new Vector3(scaleXZ, scaleY, scaleXZ);
                }
                yield return null;
            }

            if (netTransform != null)
            {
                netTransform.localScale = originalScale;
            }
        }
    }

    /// <summary>
    /// Distinguishes between upper (entry) and lower (exit) hoop sensors.
    /// </summary>
    public enum HoopSensorType
    {
        Upper,
        Lower
    }

    /// <summary>
    /// Sensor trigger child component attached to individual trigger colliders.
    /// </summary>
    public class HoopSensor : MonoBehaviour
    {
        [SerializeField] private HoopSensorType sensorType;
        [SerializeField] private HoopController hoopController;

        public HoopSensorType SensorType => sensorType;

        public void Setup(HoopSensorType type, HoopController controller)
        {
            sensorType = type;
            hoopController = controller;
        }

        private void OnTriggerEnter(Collider other)
        {
            Basketball ball = other.GetComponent<Basketball>();
            if (ball == null) return;

            if (hoopController == null)
            {
                hoopController = GetComponentInParent<HoopController>();
            }

            if (hoopController != null)
            {
                if (sensorType == HoopSensorType.Upper)
                {
                    hoopController.OnUpperSensorTriggered(ball);
                }
                else if (sensorType == HoopSensorType.Lower)
                {
                    hoopController.OnLowerSensorTriggered(ball);
                }
            }
        }
    }
}
