using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Partial Credits: https://gamedevbeginner.com/how-to-use-player-prefs-in-unity/ | https://bootcamp.uxdesign.cc/unity-feature-101-basic-saving-using-playerprefs-2fc737d1ac7b
namespace Manager {
    public class SettingsManager : MonoBehaviour {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterSlider, musicSlider, sfxSlider;

        private float _masterVolume, _musicVolume, _sfxVolume;
        private int _levelSaved;

        public void Start() {
            LoadSettings();
        }

        private void LoadSettings() {
            SavesManager.Load();

            _masterVolume = SavesManager.SavePlayerData.masterVolume;
            _musicVolume = SavesManager.SavePlayerData.musicVolume;
            _sfxVolume = SavesManager.SavePlayerData.sfxVolume;

            masterSlider.value = _masterVolume;
            musicSlider.value = _musicVolume;
            sfxSlider.value = _sfxVolume;

            audioMixer.SetFloat("MasterVolume", LinearToDecibel(_masterVolume));
            audioMixer.SetFloat("MusicVolume", LinearToDecibel(_musicVolume));
            audioMixer.SetFloat("SFXVolume", LinearToDecibel(_sfxVolume));
        }

        public void SetMasterVolume(float volume) {
            _masterVolume = volume;
            audioMixer.SetFloat("MasterVolume", LinearToDecibel(_masterVolume));
            SavesManager.SetMasterVolume(volume);
        }

        public void SetMusicVolume(float volume) {
            _musicVolume = volume;
            audioMixer.SetFloat("MusicVolume", LinearToDecibel(_musicVolume));
            SavesManager.SetMusicVolume(volume);
        }

        public void SetSFXVolume(float volume) {
            _sfxVolume = volume;
            audioMixer.SetFloat("SFXVolume", LinearToDecibel(_sfxVolume));
            SavesManager.SetSFXVolume(volume);
        }

        public void ResetPlayerData() {
            SavesManager.Reset();
            AppManager.GetInstance().ExitGame();
        }


        private float LinearToDecibel(float linear) {
            if (linear != 0) return 20.0f * Mathf.Log10(linear);
            return -80.0f;

            // Credit: OpenAI
        }
    }
}