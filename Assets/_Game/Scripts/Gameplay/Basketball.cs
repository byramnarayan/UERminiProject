using System;
using SustainableFuture.Core;
using UnityEngine;

namespace SustainableFuture.Gameplay
{
    /// <summary>
    /// Controls the physical properties, collision response, and lifecycle of a basketball.
    /// Supports automatic object-pooling and boundary reset.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class Basketball : MonoBehaviour
    {
        [Header("Physics Settings")]
        [Tooltip("PhysX bounciness coefficient (restitution) for authentic basketball rebounds.")]
        [SerializeField] private float bounciness = 0.72f;

        [Tooltip("Dynamic friction against backboard and rim.")]
        [SerializeField] private float dynamicFriction = 0.4f;

        [Header("Lifetime & Reset Settings")]
        [Tooltip("Delay in seconds before recycling the ball after a goal is made.")]
        [SerializeField] private float autoResetDelayAfterGoal = 1.0f;

        [Tooltip("Safety delay in seconds before resetting a missed shot if it stays in play.")]
        [SerializeField] private float autoResetDelayAfterMiss = 3.0f;

        [Tooltip("Vertical Y threshold below which a ball is considered out of bounds and reset.")]
        [SerializeField] private float outOfBoundsY = -5.0f;

        private Rigidbody rb;
        private SphereCollider col;
        private bool isShot;
        private bool hasScored;
        private float shotTime;
        private Action<Basketball> onReturnToPool;

        public bool IsShot => isShot;
        public bool HasScored => hasScored;
        public Rigidbody Rigidbody => rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<SphereCollider>();

            // Configure physics material at runtime for realistic bounce dynamics
            PhysicsMaterial ballMat = new PhysicsMaterial("BasketballPhysicsMaterial")
            {
                bounciness = bounciness,
                bounceCombine = PhysicsMaterialCombine.Maximum,
                dynamicFriction = dynamicFriction,
                staticFriction = 0.4f,
                frictionCombine = PhysicsMaterialCombine.Average
            };
            col.material = ballMat;
        }

        private void Update()
        {
            if (!isShot) return;

            // Check out of bounds
            if (transform.position.y < outOfBoundsY)
            {
                HandleMiss();
            }

            // Auto-reset timer if shot didn't score
            if (!hasScored && Time.time - shotTime > autoResetDelayAfterMiss)
            {
                HandleMiss();
            }
        }

        /// <summary>
        /// Links the ball to its parent object pool callback.
        /// </summary>
        public void Initialize(Action<Basketball> returnCallback)
        {
            onReturnToPool = returnCallback;
        }

        /// <summary>
        /// Positions and freezes the ball at the spawn point ready for the player to aim.
        /// </summary>
        public void PrepareForShot(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
            transform.rotation = Quaternion.identity;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            col.enabled = true;

            isShot = false;
            hasScored = false;
        }

        /// <summary>
        /// Applies physical impulse force and backspin to launch the basketball toward the hoop.
        /// </summary>
        public void Launch(Vector3 velocity, Vector3 angularVelocity)
        {
            rb.isKinematic = false;
            rb.linearVelocity = velocity;
            rb.angularVelocity = angularVelocity;

            isShot = true;
            hasScored = false;
            shotTime = Time.time;

            GameEvents.TriggerBasketballShot();
        }

        /// <summary>
        /// Called when the dual hoop sensors register a genuine basket.
        /// </summary>
        public void RegisterGoal()
        {
            if (hasScored) return;
            hasScored = true;

            // Dampen ball velocity slightly for a realistic net swish slowdown
            rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.3f, rb.linearVelocity.y * 0.7f, rb.linearVelocity.z * 0.3f);

            Invoke(nameof(ReturnToPool), autoResetDelayAfterGoal);
        }

        private void HandleMiss()
        {
            if (!hasScored && isShot)
            {
                GameEvents.TriggerBasketballMissed();
            }
            ReturnToPool();
        }

        /// <summary>
        /// Recycles the basketball back into the pool.
        /// </summary>
        public void ReturnToPool()
        {
            CancelInvoke();
            isShot = false;
            hasScored = false;
            rb.isKinematic = true;
            onReturnToPool?.Invoke(this);
        }
    }
}
