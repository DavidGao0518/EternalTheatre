using UnityEngine;

namespace Manager {
    public class LevelBehaviour : MonoBehaviour {
        [SerializeField] private AudioClip musicClip;
        [SerializeField] private float pitch = 1;
        [SerializeField] private float volume = 1;
        [SerializeField] private bool alwaysOverrideMusic;

        private void Start() {
            Time.timeScale = 1;
            if (musicClip) AppManager.GetInstance().musicManager.PlayMusic(musicClip,
                true, pitch, volume, alwaysOverrideMusic);
        }
    }
}