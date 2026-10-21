using SustainableFuture.Core;
using UnityEngine;

namespace SustainableFuture.Audio
{
    /// <summary>
    /// Lightweight procedural audio synthesizer and sound dispatcher.
    /// Synthesizes arcade audio clips at runtime using mathematical waveforms (sine, square, FM, noise),
    /// eliminating the need for heavy external audio files and providing zero-overhead sound on low-end PCs.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;

        [Header("Custom Clips (Optional)")]
        [Tooltip("Optional custom audio clips. If unassigned, procedural clips are used automatically.")]
        [SerializeField] private AudioClip bounceClip;
        [SerializeField] private AudioClip swooshClip;
        [SerializeField] private AudioClip goalClip;
        [SerializeField] private AudioClip correctClip;
        [SerializeField] private AudioClip wrongClip;
        [SerializeField] private AudioClip buzzerClip;

        // Cached procedural synthesized clips
        private AudioClip synthBounce;
        private AudioClip synthSwoosh;
        private AudioClip synthGoal;
        private AudioClip synthCorrect;
        private AudioClip synthWrong;
        private AudioClip synthBuzzer;

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

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.spatialBlend = 0f; // 2D flat stereo arcade sound
            }

            // Generate procedural clips into memory
            GenerateProceduralAudioClips();
        }

        private void OnEnable()
        {
            GameEvents.OnBasketballShot += PlaySwoosh;
            GameEvents.OnBasketballGoalScored += PlayGoal;
            GameEvents.OnScoreChanged += HandleScoreSound;
            GameEvents.OnRoundFinished += PlayBuzzer;
        }

        private void OnDisable()
        {
            GameEvents.OnBasketballShot -= PlaySwoosh;
            GameEvents.OnBasketballGoalScored -= PlayGoal;
            GameEvents.OnScoreChanged -= HandleScoreSound;
            GameEvents.OnRoundFinished -= PlayBuzzer;
        }

        private void HandleScoreSound(int deltaPoints, string msg, Color col)
        {
            if (deltaPoints > 0)
            {
                PlayCorrect();
            }
            else
            {
                PlayWrong();
            }
        }

        public void PlayBounce() => PlayClip(bounceClip != null ? bounceClip : synthBounce, 0.6f);
        public void PlaySwoosh() => PlayClip(swooshClip != null ? swooshClip : synthSwoosh, 0.7f);
        public void PlayGoal() => PlayClip(goalClip != null ? goalClip : synthGoal, 0.9f);
        public void PlayCorrect() => PlayClip(correctClip != null ? correctClip : synthCorrect, 0.85f);
        public void PlayWrong() => PlayClip(wrongClip != null ? wrongClip : synthWrong, 0.8f);
        public void PlayBuzzer() => PlayClip(buzzerClip != null ? buzzerClip : synthBuzzer, 1.0f);

        private void PlayClip(AudioClip clip, float volume = 1f)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, volume);
            }
        }

        #region Procedural Audio Synthesizer
        /// <summary>
        /// Generates clean arcade audio sound effects into RAM using PCM float buffers.
        /// </summary>
        private void GenerateProceduralAudioClips()
        {
            // 1. Basketball Ground / Rim Bounce
            synthBounce = CreateSynthClip("SynthBounce", 0.08f, (t) => {
                float freq = Mathf.Lerp(160f, 60f, t);
                return Mathf.Sin(2 * Mathf.PI * freq * t) * (1f - t);
            });

            // 2. Ball Throw Swoosh (Wind / Speed)
            synthSwoosh = CreateSynthClip("SynthSwoosh", 0.18f, (t) => {
                float noise = (Random.value * 2f - 1f) * 0.4f;
                float sine = Mathf.Sin(2 * Mathf.PI * Mathf.Lerp(300f, 600f, t) * t) * 0.6f;
                return (noise + sine) * Mathf.Sin(t * Mathf.PI);
            });

            // 3. Basket Net Swish / Chime (Major Chord)
            synthGoal = CreateSynthClip("SynthGoal", 0.4f, (t) => {
                float freq1 = 523.25f; // C5
                float freq2 = 659.25f; // E5
                float freq3 = 783.99f; // G5
                float tone = Mathf.Sin(2 * Mathf.PI * freq1 * t) * 0.3f +
                             Mathf.Sin(2 * Mathf.PI * freq2 * t) * 0.3f +
                             Mathf.Sin(2 * Mathf.PI * freq3 * t) * 0.4f;
                return tone * (1f - t);
            });

            // 4. Correct Decision Ping (Ascending D5 -> A5)
            synthCorrect = CreateSynthClip("SynthCorrect", 0.25f, (t) => {
                float freq = t < 0.12f ? 587.33f : 880.0f;
                return Mathf.Sin(2 * Mathf.PI * freq * t) * (1f - t);
            });

            // 5. Wrong Decision Buzz (Descending Saw/Square)
            synthWrong = CreateSynthClip("SynthWrong", 0.3f, (t) => {
                float freq = Mathf.Lerp(220f, 110f, t);
                float square = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * freq * t)) * 0.4f;
                return square * (1f - t);
            });

            // 6. Round End Buzzer (Low Frequency 150Hz Buzz)
            synthBuzzer = CreateSynthClip("SynthBuzzer", 0.8f, (t) => {
                float freq = 150f;
                float square = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * freq * t)) * 0.5f;
                return square * Mathf.Clamp01((0.8f - t) / 0.8f);
            });
        }

        /// <summary>
        /// Allocates an AudioClip and fills sample data with mathematical wave functions.
        /// </summary>
        private AudioClip CreateSynthClip(string name, float length, System.Func<float, float> waveFunc)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * length);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                data[i] = waveFunc(t);
            }

            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
        #endregion
    }
}
