using UnityEngine;

namespace BOCA.Core
{
    public sealed class UiAudioService : MonoBehaviour
    {
        private const int SampleRate = 44100;

        private static UiAudioService instance;

        private AudioSource audioSource;
        private AudioClip clickClip;
        private AudioClip confirmClip;
        private AudioClip completeClip;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            EnsureInstance();
        }

        public static void PlayClick()
        {
            EnsureInstance().audioSource.PlayOneShot(EnsureInstance().clickClip, 0.22f);
        }

        public static void PlayConfirm()
        {
            EnsureInstance().audioSource.PlayOneShot(EnsureInstance().confirmClip, 0.24f);
        }

        public static void PlayComplete()
        {
            EnsureInstance().audioSource.PlayOneShot(EnsureInstance().completeClip, 0.28f);
        }

        private static UiAudioService EnsureInstance()
        {
            if (instance != null)
            {
                return instance;
            }

            var serviceObject = new GameObject("BOCA UI Audio");
            instance = serviceObject.AddComponent<UiAudioService>();
            DontDestroyOnLoad(serviceObject);
            return instance;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;

            clickClip = CreateToneSequence("BOCA UI Click", new[] { 620f }, 0.055f, 0f);
            confirmClip = CreateToneSequence("BOCA UI Confirm", new[] { 480f, 720f }, 0.075f, 0.014f);
            completeClip = CreateToneSequence("BOCA Chapter Complete", new[] { 523.25f, 659.25f, 783.99f }, 0.105f, 0.018f);
        }

        private static AudioClip CreateToneSequence(
            string clipName,
            float[] frequencies,
            float toneDuration,
            float gapDuration)
        {
            var toneSamples = Mathf.Max(1, Mathf.RoundToInt(toneDuration * SampleRate));
            var gapSamples = Mathf.Max(0, Mathf.RoundToInt(gapDuration * SampleRate));
            var totalSamples = frequencies.Length * toneSamples + Mathf.Max(0, frequencies.Length - 1) * gapSamples;
            var samples = new float[totalSamples];
            var cursor = 0;

            for (var note = 0; note < frequencies.Length; note++)
            {
                var frequency = frequencies[note];
                for (var sample = 0; sample < toneSamples; sample++)
                {
                    var normalizedTime = sample / (float)toneSamples;
                    var attack = Mathf.Clamp01(normalizedTime / 0.12f);
                    var release = Mathf.Clamp01((1f - normalizedTime) / 0.42f);
                    var envelope = attack * release;
                    var phase = 2f * Mathf.PI * frequency * sample / SampleRate;
                    samples[cursor + sample] = envelope * (Mathf.Sin(phase) + 0.16f * Mathf.Sin(phase * 2f));
                }

                cursor += toneSamples + gapSamples;
            }

            var clip = AudioClip.Create(clipName, totalSamples, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
