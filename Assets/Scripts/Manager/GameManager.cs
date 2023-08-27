using System.Collections.Generic;
using Entity.Enemy;
using Entity.Player.Mechanics.Scissors;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Manager {
    public class GameManager : MonoBehaviour {
        private static GameManager instance;

        public Image[] hearts;
        [SerializeField] private TextMeshProUGUI grappleText;
        [SerializeField] private TextMeshProUGUI scissorsText;
        [SerializeField] private bool infiniteGrapples;

        public bool isGameOver;
        private float _sec;
        public int GrappleCharges { get; private set; } = 5;
        public List<EnemyEntity> enemiesToKill = new();
        private ScissorMove _scissors;

        public static GameManager GetInstance() {
            return instance;
        }

        private void Awake() {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            UpdateGrappleUI();
            UpdateHealthUI(3);
        }

        public void GameOver() {
            isGameOver = true;
            AppManager.GetInstance().sceneLoader.ReloadCurrentScene();
            // TODO gameOver panel stuff
        }

        public void UpdateHealthUI(int newHealth) {
            for (int i = 0; i < hearts.Length; i++) {
                if (i < newHealth) hearts[i].enabled = true;
                else hearts[i].enabled = false;
            }
        }

        private void UpdateGrappleUI() {
            if (infiniteGrapples)
			{
                grappleText.text = "infinite";
            } else

            grappleText.text = GrappleCharges.ToString();
        }

        /// <summary>
        /// do - value to subtract
        /// </summary>
        /// <param name="amount"></param>
        public void AddGrappleCharge(int amount) {

            if (infiniteGrapples) return;

            GrappleCharges += amount;

            if (GrappleCharges >= 5)
			{
                GrappleCharges = 5;
            }

            UpdateGrappleUI();
        }


        public void UpdateScissorsUI(ScissorMove scissors) {
            _scissors = scissors;
        }

        private void Update() {
            if (_scissors) _sec = 2 - (Mathf.Floor(_scissors.seconds * 10) / 10);
            if (_sec == 0) scissorsText.text = "Ready";
            else scissorsText.text = _sec.ToString();
        }

        public void RegisterEnemyToKillList(EnemyEntity entity) {
            enemiesToKill.Add(entity);
        }

        public void EnemyEntityDead(EnemyEntity entity) {
            enemiesToKill.Remove(entity);

            if (enemiesToKill.Count <= 0) {
                //AppManager.GetInstance().sceneLoader.LoadNextScene(); // TODO ???
            }
        }
    }
}