using System;
using UnityEngine;

namespace BOCA.Core
{
    public static class GameSettingsService
    {
        private const string MasterVolumeKey = "boca.settings.master-volume";
        private const float DefaultMasterVolume = 0.8f;

        public static event Action<float> MasterVolumeChanged;

        public static float MasterVolume => PlayerPrefs.GetFloat(MasterVolumeKey, DefaultMasterVolume);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ApplySavedSettings()
        {
            AudioListener.volume = MasterVolume;
        }

        public static void SetMasterVolume(float volume)
        {
            var clamped = Mathf.Clamp01(volume);
            AudioListener.volume = clamped;
            PlayerPrefs.SetFloat(MasterVolumeKey, clamped);
            PlayerPrefs.Save();
            MasterVolumeChanged?.Invoke(clamped);
        }
    }
}
