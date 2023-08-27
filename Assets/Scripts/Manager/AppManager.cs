using TMPro;
using UnityEngine;

namespace Manager {
    public class AppManager : MonoBehaviour {
        private static AppManager instance;

        public SceneLoader sceneLoader;
        public MusicManager musicManager;
        public SettingsManager settingsManager;

        [SerializeField] private GameObject settingPanel;
        [SerializeField] private TMP_Text exitGameText;
    
        public static AppManager GetInstance() {
            return instance;
        }

        private void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
                Debug.LogWarning("Instance already initialised");
                return;
            }
            SavesManager.Load();
        }

        private void Update() {

            if (Input.GetKeyDown(KeyCode.V))
            {
                ScreenCapture.CaptureScreenshot("SomeLevel.png");
            }

            if (!Input.GetKeyDown(KeyCode.Escape)) return;
        
            settingPanel.SetActive(!settingPanel.activeInHierarchy);

            Time.timeScale = settingPanel.activeInHierarchy ? 0 : 1;
        }

        public void ExitGame() {
            Time.timeScale = 1;
            settingPanel.SetActive(false);
            sceneLoader.LoadScene(0);
        }
    }
}