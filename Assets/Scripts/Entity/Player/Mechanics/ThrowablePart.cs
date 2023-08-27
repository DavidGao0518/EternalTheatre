using System.Collections;
using Entity.Abstract;
using UnityEngine;

namespace Entity.Player.Mechanics {
    public class ThrowablePart : MonoBehaviour {
        public bool IsThrowing { get; private set; }

        [SerializeField] private HingeJoint2D joint;
        [SerializeField] private Rigidbody2D rigidBody2D, lowerRigidBodyCollider;
        [SerializeField] private CapsuleCollider2D upperCollider, lowerCollider;
        [SerializeField] private Transform torso;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private float returningSpeed = 40f;
        [SerializeField] private int damage;

        private PlayerController _playerController;
        private bool _isReturning, _hasCollidedOnce;
        private Coroutine _endCoroutine;

        private static IEnumerator FixStuckResizeRoutine(CapsuleCollider2D col) {
            Vector2 originalSize = col.size;
            Vector2 smallSize = new Vector2(0.01f, 0.01f);
            float duration = 0.5f;

            // Set to small size
            col.size = smallSize;

            // Enlarge to original size
            float time = 0;
            while (time < duration) {
                col.size = Vector2.Lerp(smallSize, originalSize, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            // Ensure that the size is exactly as it should be at the end
            col.size = originalSize;
        }

        private void FixedUpdate() {
            // Note: Involves physics, so FixedUpdate is used
            if (!_isReturning) return;

            rigidBody2D.velocity = Vector2.zero;
            transform.position = Vector2.MoveTowards(transform.position, torso.position, Time.deltaTime * returningSpeed);

            if (Vector2.Distance(transform.position, torso.position) < 0.5f) {
                joint.enabled = true;
                _isReturning = false;


                upperCollider.gameObject.layer = LayerMask.NameToLayer("Player");
                lowerCollider.gameObject.layer = LayerMask.NameToLayer("Player");
                upperCollider.enabled = true;
                FixStuck(upperCollider, rigidBody2D, false);
                lowerCollider.enabled = true;
                FixStuck(lowerCollider, lowerRigidBodyCollider, true);
                _endCoroutine = StartCoroutine(TrailEndRoutine()); //TODO rename and clean stuff
            }
        }

        private void FixStuck(CapsuleCollider2D col, Rigidbody2D rb, bool alsoResize) {
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(col.transform.position, 0.35f, 1 << LayerMask.NameToLayer("Ground"));

            if (collider2Ds.Length > 0) {
                rb.AddForce(Vector2.up * 1000);
                if (alsoResize) StartCoroutine(FixStuckResizeRoutine(col));
            }
        }

        public void ThrowPart(Vector3 vectorToThrow) {
            if (IsThrowing) return;

            IsThrowing = true;
            joint.enabled = false;
            rigidBody2D.AddForce(vectorToThrow * 4000);
            upperCollider.gameObject.layer = 2;
            lowerCollider.gameObject.layer = 2;

            if (_endCoroutine != null) StopCoroutine(_endCoroutine);
        }

        public void ReturnToBody(PlayerController owningController) {
            if (!_playerController) _playerController = owningController;
            _isReturning = true;
            rigidBody2D.velocity = Vector2.zero;

            trailRenderer.enabled = true;
            trailRenderer.time = 0.5f;
            upperCollider.enabled = false;
            lowerCollider.enabled = false;

            _hasCollidedOnce = false;
        }

        private IEnumerator TrailEndRoutine() {
            float count = 0.5f;
            float initialTime = trailRenderer.time;
            while (count > 0) {
                trailRenderer.time = Mathf.Lerp(initialTime, 0, 0.5f - count);
                count -= Time.deltaTime;
                yield return null;
            }

            trailRenderer.enabled = false;
            trailRenderer.time = 0;
            _playerController.GetReturnBody(this);
            _endCoroutine = null;
            IsThrowing = false;
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (_hasCollidedOnce || !IsThrowing) return;

            if (!other.gameObject.CompareTag("Enemy")) return;

            if (other.transform.parent && other.transform.parent.gameObject.TryGetComponent(out GameEntity gameEntity)) {
                gameEntity.OnHit(transform.position, damage);
                _hasCollidedOnce = true;
            }
            else if (other.transform.TryGetComponent(out GameEntity hitEntity)) {
                hitEntity.OnHit(transform.position, damage);
                _hasCollidedOnce = true;
            }
        }
    }
}