using System.Collections;
using Entity.Abstract;
using UnityEngine;
using Weapons;

namespace Weapon {
    public class MeleeWeapon : Weapons.Weapon {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Collider2D attackColliderTrigger;
        [SerializeField] private float trailTime, trailFadeTime, attackFrameDuration, attackWindUpDuration, attackWindDownDuration;
        [SerializeField] private string targetLayer = "Player";
        [SerializeField] private int damage;
        [SerializeField] private AudioSource AS;
        [SerializeField] private AudioClip attackClip;

        private bool _isAttacking;

        private void Start() {
            attackColliderTrigger.enabled = false;
            trailRenderer.enabled = false;
        }

        public override void DoAttack() {
            StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine() {
            trailRenderer.enabled = true;
            trailRenderer.time = trailTime;
            yield return new WaitForSeconds(attackWindUpDuration);
            attackColliderTrigger.enabled = true;
            AS.PlayOneShot(attackClip);
            yield return new WaitForSeconds(attackFrameDuration);
            attackColliderTrigger.enabled = false;
            yield return new WaitForSeconds(attackWindDownDuration);

            float count = trailFadeTime;
            float initialTime = trailTime;
            while (count > 0) {
                trailRenderer.time = Mathf.Lerp(initialTime, 0, 1 - count);
                count -= Time.deltaTime;
                yield return null;
            }

            trailRenderer.enabled = false;
            trailRenderer.time = 0;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayer)) {
                if (collision.transform.parent.TryGetComponent(out GameEntity entity)) entity.OnHit(transform.position, damage);
                else if (collision.transform.parent.parent.TryGetComponent(out GameEntity _entity)) _entity.OnHit(transform.position, damage);
                else print("Fatal error: no GameEntity component found in parent " + collision.gameObject.name);
            }
        }
    }
}