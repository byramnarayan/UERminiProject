using System.Collections.Generic;
using SustainableFuture.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SustainableFuture.Gameplay
{
    /// <summary>
    /// Arcade flick and swipe shooting controller with real-time trajectory line calculation,
    /// dynamic force curve mapping, and low-end PC object pooling.
    /// Compatible with Unity's New Input System (Mouse & Touchscreen).
    /// </summary>
    public class BasketballShooter : MonoBehaviour
    {
        [Header("Shooting Setup")]
        [Tooltip("Prefab instantiated into the ball pool.")]
        [SerializeField] private Basketball basketballPrefab;

        [Tooltip("Transform position where the next ready ball is placed.")]
        [SerializeField] private Transform ballSpawnPoint;

        [Tooltip("Transform position of the hoop rim used as aiming target reference.")]
        [SerializeField] private Transform hoopTargetPoint;

        [Tooltip("Size of the pre-allocated basketball object pool.")]
        [SerializeField] private int poolSize = 4;

        [Header("Shooting Physics Tuning")]
        [Tooltip("Minimum swipe distance in pixels required to register a shot.")]
        [SerializeField] private float minSwipeDistance = 25f;

        [Tooltip("Maximum swipe distance in pixels that maps to 100% force.")]
        [SerializeField] private float maxSwipeDistance = 400f;

        [Tooltip("Base forward force applied to minimum valid shots.")]
        [SerializeField] private float baseForwardForce = 5.6f;

        [Tooltip("Maximum forward force applied to full-power shots.")]
        [SerializeField] private float maxForwardForce = 8.8f;

        [Tooltip("Base upward arc force.")]
        [SerializeField] private float baseUpwardForce = 6.4f;

        [Tooltip("Maximum upward arc force for high-arcing swishes.")]
        [SerializeField] private float maxUpwardForce = 10.2f;

        [Tooltip("Horizontal deflection sensitivity based on swipe angle.")]
        [SerializeField] private float lateralSensitivity = 0.008f;

        [Tooltip("Rotational backspin applied to the ball upon release.")]
        [SerializeField] private float spinForce = 18f;

        [Header("Trajectory Preview")]
        [Tooltip("LineRenderer used to visualize predicted ball arc during drag.")]
        [SerializeField] private LineRenderer trajectoryLine;

        [Tooltip("Number of preview sample points along the parabola.")]
        [SerializeField] private int trajectoryPointCount = 22;

        [Tooltip("Physics simulation step size for trajectory drawing.")]
        [SerializeField] private float trajectoryTimeStep = 0.045f;

        private List<Basketball> ballPool = new List<Basketball>();
        private Basketball activeBall;
        private Vector2 dragStartPos;
        private float dragStartTime;
        private bool isDragging;
        private bool isInputEnabled = true;

        private void Awake()
        {
            InitializePool();
        }

        private void OnEnable()
        {
            GameEvents.OnGameStarted += HandleGameStarted;
            GameEvents.OnRoundFinished += HandleRoundFinished;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= HandleGameStarted;
            GameEvents.OnRoundFinished -= HandleRoundFinished;
        }

        private void Start()
        {
            SpawnNextBall();
        }

        private void Update()
        {
            if (!isInputEnabled) return;

            HandleShootingInput();
        }

        /// <summary>
        /// Pre-instantiates balls into the pool to eliminate runtime memory allocations (zero GC).
        /// </summary>
        private void InitializePool()
        {
            if (basketballPrefab == null) return;

            for (int i = 0; i < poolSize; i++)
            {
                Basketball ball = Instantiate(basketballPrefab, transform);
                ball.gameObject.SetActive(false);
                ball.Initialize(ReturnBallToPool);
                ballPool.Add(ball);
            }
        }

        /// <summary>
        /// Activates the next available ball from the pool and positions it at the spawn point.
        /// </summary>
        public void SpawnNextBall()
        {
            if (ballSpawnPoint == null) return;

            foreach (var ball in ballPool)
            {
                if (ball != null && !ball.gameObject.activeInHierarchy)
                {
                    activeBall = ball;
                    activeBall.gameObject.SetActive(true);
                    activeBall.PrepareForShot(ballSpawnPoint.position);
                    return;
                }
            }

            // Fallback: Instantiate an extra ball if pool is fully exhausted
            if (basketballPrefab != null)
            {
                Basketball newBall = Instantiate(basketballPrefab, transform);
                newBall.Initialize(ReturnBallToPool);
                ballPool.Add(newBall);
                activeBall = newBall;
                activeBall.PrepareForShot(ballSpawnPoint.position);
            }
        }

        /// <summary>
        /// Object pool callback when a ball is recycled.
        /// </summary>
        private void ReturnBallToPool(Basketball ball)
        {
            if (ball != null)
            {
                ball.gameObject.SetActive(false);
            }

            if (activeBall == null || !activeBall.gameObject.activeInHierarchy || activeBall.IsShot)
            {
                SpawnNextBall();
            }
        }

        /// <summary>
        /// Polls mouse and touch inputs from Unity's New Input System.
        /// </summary>
        private void HandleShootingInput()
        {
            bool pointerDown = false;
            bool pointerHeld = false;
            bool pointerUp = false;
            Vector2 pointerPos = Vector2.zero;

            // Read New Input System Mouse / Touchscreen states
            if (Mouse.current != null)
            {
                pointerDown = Mouse.current.leftButton.wasPressedThisFrame;
                pointerHeld = Mouse.current.leftButton.isPressed;
                pointerUp = Mouse.current.leftButton.wasReleasedThisFrame;
                pointerPos = Mouse.current.position.ReadValue();
            }
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                pointerDown = Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
                pointerHeld = Touchscreen.current.primaryTouch.press.isPressed;
                pointerUp = Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;
                pointerPos = Touchscreen.current.primaryTouch.position.ReadValue();
            }

            // Pointer Down Event
            if (pointerDown)
            {
                // Prevent shooting if clicked directly over UI buttons (like Discard)
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                dragStartPos = pointerPos;
                dragStartTime = Time.time;
                isDragging = true;
            }

            // Pointer Dragging Event (Update Trajectory)
            if (isDragging && pointerHeld)
            {
                Vector2 dragDelta = pointerPos - dragStartPos;

                if (dragDelta.y > 10f && trajectoryLine != null)
                {
                    Vector3 predictedVelocity = CalculateVelocity(dragDelta, Time.time - dragStartTime);
                    DrawTrajectory(predictedVelocity);
                }
            }

            // Pointer Up Event (Release Shot)
            if (isDragging && pointerUp)
            {
                isDragging = false;
                if (trajectoryLine != null) trajectoryLine.enabled = false;

                Vector2 dragDelta = pointerPos - dragStartPos;
                float swipeDuration = Mathf.Max(0.06f, Time.time - dragStartTime);

                if (dragDelta.y >= minSwipeDistance && activeBall != null && !activeBall.IsShot)
                {
                    ExecuteShot(dragDelta, swipeDuration);
                }
            }
        }

        /// <summary>
        /// Launches the active basketball with calculated physics velocity and backspin.
        /// </summary>
        private void ExecuteShot(Vector2 dragDelta, float duration)
        {
            Vector3 shootVelocity = CalculateVelocity(dragDelta, duration);
            Vector3 angularVelocity = new Vector3(-spinForce, 0f, 0f);

            Basketball shotBall = activeBall;
            activeBall = null;

            shotBall.Launch(shootVelocity, angularVelocity);

            // Queue up next ball automatically
            Invoke(nameof(SpawnNextBall), 0.35f);
        }

        /// <summary>
        /// Calculates the 3D launch velocity vector based on 2D screen swipe distance, angle, and flick velocity.
        /// </summary>
        private Vector3 CalculateVelocity(Vector2 dragDelta, float duration)
        {
            float clampedY = Mathf.Clamp(dragDelta.y, minSwipeDistance, maxSwipeDistance);
            float normalizedPower = (clampedY - minSwipeDistance) / (maxSwipeDistance - minSwipeDistance);

            // Speed bonus for fast flicks
            float speedBonus = Mathf.Clamp01(1.0f - (duration - 0.06f) / 0.35f) * 0.35f;
            float totalPower = Mathf.Clamp01(normalizedPower + speedBonus);

            float forward = Mathf.Lerp(baseForwardForce, maxForwardForce, totalPower);
            float upward = Mathf.Lerp(baseUpwardForce, maxUpwardForce, totalPower);
            float lateral = dragDelta.x * lateralSensitivity;

            // Target direction vector toward hoop
            Vector3 aimDir = Vector3.forward;
            if (hoopTargetPoint != null && ballSpawnPoint != null)
            {
                Vector3 toTarget = (hoopTargetPoint.position - ballSpawnPoint.position).normalized;
                aimDir = new Vector3(toTarget.x, 0f, toTarget.z).normalized;
            }

            Vector3 rightDir = Vector3.Cross(Vector3.up, aimDir).normalized;
            return (aimDir * forward) + (Vector3.up * upward) + (rightDir * lateral);
        }

        /// <summary>
        /// Calculates and visualizes the ballistic arc using kinematic formulas.
        /// </summary>
        private void DrawTrajectory(Vector3 launchVelocity)
        {
            if (trajectoryLine == null || ballSpawnPoint == null) return;

            trajectoryLine.enabled = true;
            trajectoryLine.positionCount = trajectoryPointCount;

            Vector3 currentPos = ballSpawnPoint.position;
            Vector3 currentVel = launchVelocity;

            for (int i = 0; i < trajectoryPointCount; i++)
            {
                trajectoryLine.SetPosition(i, currentPos);
                currentVel += Physics.gravity * trajectoryTimeStep;
                currentPos += currentVel * trajectoryTimeStep;
            }
        }

        private void HandleGameStarted()
        {
            isInputEnabled = true;
            SpawnNextBall();
        }

        private void HandleRoundFinished()
        {
            isInputEnabled = false;
            isDragging = false;
            if (trajectoryLine != null) trajectoryLine.enabled = false;
        }

        public void SetInputEnabled(bool enabled)
        {
            isInputEnabled = enabled;
            if (!enabled && isDragging)
            {
                isDragging = false;
                if (trajectoryLine != null) trajectoryLine.enabled = false;
            }
        }
    }
}
