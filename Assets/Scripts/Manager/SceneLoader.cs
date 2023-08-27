using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager {
    public class SceneLoader : MonoBehaviour {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Animator AN;

        public void LoadScene(int index) {
            StartCoroutine(LoadSceneAsync(index));
        }

        private IEnumerator LoadSceneAsync(int index) {
            loadingScreen.SetActive(true);
            yield return new WaitForSecondsRealtime(1f);

            AsyncOperation async = SceneManager.LoadSceneAsync(index);
            while (!async.isDone) yield return null;

            AN.Play("CurtainOut");
            yield return new WaitForSecondsRealtime(1);

            loadingScreen.SetActive(false);
        }

        public void ReloadCurrentScene() {
            StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().buildIndex));
        }

        public void LoadNextScene() {
            int newIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (newIndex < SceneManager.sceneCountInBuildSettings) {
                StartCoroutine(LoadSceneAsync(newIndex));
                SavesManager.SetSavedLevel(newIndex);
            }
            else {
                StartCoroutine(LoadSceneAsync(0));
                SavesManager.SetSavedLevel(1);
            }
        }
    }
}