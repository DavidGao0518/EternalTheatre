using Manager;
using UnityEngine;

namespace WorldComponent {
    public class ExitDoorBehaviour : MonoBehaviour {

        [SerializeField] bool isTutorial;

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") && other.CompareTag("Torso"))
            {
                AppManager.GetInstance().sceneLoader.LoadNextScene();
                SavesManager.SetTutorialPlayed(true);
            }
        }
    }
}