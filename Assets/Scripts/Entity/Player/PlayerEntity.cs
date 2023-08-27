using System.Collections;
using System.Collections.Generic;
using Entity.Abstract;
using Entity.Player.Mechanics;
using Manager;
using UnityEngine;

namespace Entity.Player {
    public class PlayerEntity : GameEntity {
        [SerializeField] private AudioClip deathClip;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Color hitColour;

        private bool _canBeAttacked = true;

        public override void OnDeath() {
            if (IsDead) return;
            base.OnDeath();
            StartCoroutine(OnDeathRoutine());
        }

        private IEnumerator OnDeathRoutine() {
            Time.timeScale = 0.3f;

            playerController.SetBalance(false);

            AppManager.GetInstance().musicManager.PlayMusic(deathClip, false, 1, 1, true);
            yield return new WaitForSecondsRealtime(1);
            yield return new WaitForSecondsRealtime(2);

            Time.timeScale = 1f;
            GameManager.GetInstance().GameOver();
        }

        public override void OnHit(Vector3 entityPosition, int damage) {
            if (!_canBeAttacked) return;

            CurrentHealth--;
            GameManager.GetInstance().UpdateHealthUI(CurrentHealth);
            base.OnHit(entityPosition, damage);

            // ReSharper disable once Unity.NoNullPropagation - Could not care less
            CameraShakeUtil.Instance?.ShakeCamera(4, 1, 0.2f);
            hitParticleSystem.Play();
            audioSource.PlayOneShot(hitClip);
            StartCoroutine(InvulnerableRoutine());
        }

        private IEnumerator<WaitForSeconds> InvulnerableRoutine() {
            // TODO Note sure if we can do a bit better than this - Shreyas
            foreach (var bodySegment in playerController.GetBodySegments()) bodySegment.SR.color = hitColour;

            _canBeAttacked = false;
            yield return new WaitForSeconds(1.5f); // 2.5f felt too long

            foreach (var bodySegment in playerController.GetBodySegments()) bodySegment.SR.color = new Color(1, 1, 1, 1);

            _canBeAttacked = true;
        }
    }
}