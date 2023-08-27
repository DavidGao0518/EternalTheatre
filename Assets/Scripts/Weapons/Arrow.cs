using System.Collections;
using Entity.Abstract;
using UnityEngine;

namespace Weapon {
    public class Arrow : MonoBehaviour {
        [SerializeField] private Rigidbody2D RB;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private float duration = 20;
        [SerializeField] private string layermask = "Player";
        [SerializeField] private float speed = 20;
        [SerializeField] private int damage = 1;
        [SerializeField] private float radius = 10; // Radius to search for players
        [SerializeField] private float loseHomingDistance = 3;
        [SerializeField] private bool isHoming; // Homing boolean
        [SerializeField] private float inivibleTime = 0.5f;
        private Transform _target; // Target to home towards
        private bool _isHit;
        private float _initTime;

        private void Start() {
            RB.velocity = (transform.right * speed);
            Destroy(gameObject, duration);

            if (isHoming) {
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerMask.NameToLayer(layermask));
                if (hitColliders.Length > 0) _target = hitColliders[0].transform;
            }

            _initTime = Time.time;
        }

        private void Update() {
            if (isHoming && _target != null && !_isHit) {
                Vector2 direction = (_target.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                RB.rotation = angle;
                RB.velocity = direction * speed;

                if (Vector2.Distance(transform.position, _target.position) < loseHomingDistance) _target = null;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (_isHit) return;

            if (collision.gameObject.layer == LayerMask.NameToLayer(layermask)) {
                if (collision.transform.parent.TryGetComponent(out GameEntity entity))
                    entity.OnHit(transform.position, damage);
                else if (collision.transform.parent.parent.TryGetComponent(out GameEntity closeEntity)) closeEntity.OnHit(transform.position, damage);
            }
            else if (_initTime + inivibleTime > Time.time) {
                return;
            }

            _isHit = true;
            RB.gravityScale = 1;
            StartCoroutine(TrailEndRoutine());
            Destroy(gameObject, 1);
        }

        private IEnumerator TrailEndRoutine() {
            float count = 1;
            float initialTime = trailRenderer.time;
            while (count > 0) {
                trailRenderer.time = Mathf.Lerp(initialTime, 0, 1 - count);
                count -= Time.deltaTime;
                yield return null;
            }

            trailRenderer.enabled = false;
            trailRenderer.time = 0;
        }
    }
}