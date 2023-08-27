using Manager;
using UnityEngine;

public class LobbyManager : MonoBehaviour {
    [SerializeField] private AudioClip lobbyMusic;
    [SerializeField] private GameObject continueButton;

    private void Start() {
        AppManager.GetInstance().musicManager.PlayMusic(lobbyMusic, true, 1, 0.7f, true);
        if (SavesManager.SavePlayerData.levelSaved == 1) continueButton.SetActive(false);
    }

    public void NewGame() {
        AppManager.GetInstance().sceneLoader.LoadScene(SavesManager.SavePlayerData.tutorialPlayed ? 2 : 1);

    }

    public void ContinueGame() {
        AppManager.GetInstance().sceneLoader.LoadScene(SavesManager.SavePlayerData.levelSaved);
    }
}