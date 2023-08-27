using System.Collections;
using Entity.Abstract;
using UnityEngine;

namespace Entity.BossEnemy {
    public class BossHandRightBehaviour : MonoBehaviour {
        [SerializeField] private Rigidbody2D RB;
        [SerializeField] private Transform defaultPosition;
        [SerializeField] private Animator AN;
        [SerializeField] private AudioSource AS;
        [SerializeField] private AudioClip smashClip;
        [SerializeField] private float speed, smashSpeed;
        [SerializeField] private float attackAreaRadius = 4;
        [SerializeField] private int smashDamage = 1;

        private bool _hasDestination, _hit;
        private Vector2 _destination;
        private Transform _target;

		private void Start()
		{
            _hit = true;
        }

		public bool IsDoneAttacking { get; private set; }

        private void Update() {
            if (!_hasDestination) return;

            if (Vector2.Distance(_destination, transform.position) > 0.5f) {
                RB.velocity = (_destination - (Vector2)transform.position).normalized * speed;
            }
            else {
                RB.velocity = Vector2.zero;
                _destination = Vector2.zero;
                _hasDestination = false;
                _hit = true;
            }
        }

        public void AttackSetUp(Transform target) {
            _target = target;
            transform.SetParent(null);
            RB.gravityScale = 0;
            StartCoroutine(AttackRoutine());
            IsDoneAttacking = false;
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (_hit) return;

            Collider2D col = Physics2D.OverlapCircle(transform.position, attackAreaRadius, 1 << LayerMask.NameToLayer("Player"));

            if (col && col.transform.parent.TryGetComponent(out GameEntity entity)) entity.OnHit(transform.position, smashDamage);
            AS.PlayOneShot(smashClip);
            // TODO play sound - hand smash sound
            _hit = true;
        }

        private IEnumerator AttackRoutine() {
            RB.gravityScale = 0;
            _hit = true;
            _hasDestination = true;
            
            var position = _target.position;
            _destination = new Vector2(position.x, position.y + 10);
            yield return new WaitForSeconds(0.5f);
            AN.SetTrigger("Attack");
            _hasDestination = false;
            _hit = false;
            RB.velocity = Vector2.down * smashSpeed;
            RB.gravityScale = 1;
            yield return new WaitForSeconds(1);
            RB.gravityScale = 0;
            AN.SetTrigger("Attack");
            _hit = false;
            _hasDestination = true;
            _destination = new Vector2(transform.position.x - 10, transform.position.y);
            yield return new WaitForSeconds(2f);
            AN.SetTrigger("Attack");
            _hit = false;
            _hasDestination = true;
            _destination = new Vector2(position.x + 10, transform.position.y);
            yield return new WaitForSeconds(1.5f);
            _hit = true; // Note: this is set to true to prevent the hand from hitting the player when it returns to its default position
            _hasDestination = true;
            _destination = defaultPosition.position;
            IsDoneAttacking = true;
            yield return new WaitForSeconds(0.5f);

            AN.Play("BossHandDisappears");

            yield return new WaitForSeconds(0.5f);
            transform.SetParent(defaultPosition);
            transform.position = defaultPosition.position;
            gameObject.SetActive(false);
        }
    }
}