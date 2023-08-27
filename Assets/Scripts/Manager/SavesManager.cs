using UnityEngine;
using UnityEngine.Serialization;

namespace Manager {
    public static class SavesManager {
        public static SavePlayerData SavePlayerData { get; private set; }

        private static void Save() {
            PlayerPrefs.SetString("SavedPlayer", JsonUtility.ToJson(SavePlayerData));
            PlayerPrefs.Save();
        }

        public static void Load() {
            SavePlayerData = JsonUtility.FromJson<SavePlayerData>(PlayerPrefs.GetString("SavedPlayer"));

            if (SavePlayerData == null) {
                SavePlayerData = new SavePlayerData(0.5f, 0.5f, 0.5f, 1, false);
                Save();
            }
        }

        public static void Reset() {
            SavePlayerData = new SavePlayerData(0.5f, 0.5f, 0.5f, 1, false);
            Save();
        }

        /// <summary>
        /// set volume to value and also save the entire savePlayerData
        /// </summary>
        /// <param name="value"></param>
        public static void SetMasterVolume(float value) {
            SavePlayerData = new SavePlayerData(value,
                SavePlayerData.musicVolume,
                SavePlayerData.sfxVolume,
                SavePlayerData.levelSaved,
                SavePlayerData.tutorialPlayed);
            Save();
        }

        public static void SetMusicVolume(float value) {
            SavePlayerData = new SavePlayerData(SavePlayerData.masterVolume, value, 
                SavePlayerData.sfxVolume, SavePlayerData.levelSaved, SavePlayerData.tutorialPlayed);
            Save();
        }

        public static void SetSFXVolume(float value) {
            SavePlayerData = new SavePlayerData(SavePlayerData.masterVolume,
                SavePlayerData.musicVolume,
                value,
                SavePlayerData.levelSaved,
                SavePlayerData.tutorialPlayed);
            Save();
        }

        public static void SetSavedLevel(int value) {
            SavePlayerData = new SavePlayerData(SavePlayerData.masterVolume, SavePlayerData.musicVolume, 
                SavePlayerData.sfxVolume, value, SavePlayerData.tutorialPlayed);
            Save();
        }

        public static void SetTutorialPlayed(bool value)
		{
            SavePlayerData = new SavePlayerData(SavePlayerData.masterVolume, SavePlayerData.musicVolume,
                SavePlayerData.sfxVolume, SavePlayerData.levelSaved, value);
            Save();
        }
    }

    [System.Serializable]
    public class SavePlayerData {
        [FormerlySerializedAs("masterVolue")] public float masterVolume;
        public float musicVolume, sfxVolume;
        public int levelSaved;
        public bool tutorialPlayed;

        public SavePlayerData(float masterVolume, float musicVolume, float sfxVolume, int levelSaved, bool tutorialPlayed)
		{
			this.masterVolume = masterVolume;
			this.musicVolume = musicVolume;
			this.sfxVolume = sfxVolume;
			this.levelSaved = levelSaved;
			this.tutorialPlayed = tutorialPlayed;
		}
	}
}