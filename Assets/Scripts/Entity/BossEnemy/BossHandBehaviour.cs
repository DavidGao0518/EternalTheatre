using System.Collections;
using Entity.Abstract;
using UnityEngine;

namespace Entity.BossEnemy {
    public class BossHandBehaviour : MonoBehaviour {
        //[SerializeField] private GameObject handBody;
        [SerializeField] private Transform defaultPosition;
        [SerializeField] private Rigidbody2D RB;
        [SerializeField] private Animator AN;
        [SerializeField] private AudioSource AS;
        [SerializeField] private AudioClip smashClip;

        [SerializeField] private float followSpeed = 1;
        [SerializeField] private float returningSpeed = 1;
        [SerializeField] private float smashSpeed = 1;
        [SerializeField] private float startAttackDistance = 1;
        [SerializeField] private float attackAreaRadius = 4;
        [SerializeField] private int smashDamage = 1;
        [SerializeField] private float maxFollowTime = 1.5f;

        public bool Hit { get; private set; }

        private Transform _target;
        private bool _returningToDefaultPos;
        private bool _isFinding;

        private float _startFindingTime;

        private void Start() {
            // Note for use of testing: AttackSetUp(FindObjectOfType<PlayerEntity>().transform); 
            _isFinding = true;
        }

        public void DeActivate() {
            AN.Play("BossHandDisappears");
        }

        private void Update() {
            if (_target) {
                if (/*Mathf.Abs(transform.position.x - _target.position.x) < startAttackDistance 
                    || */_startFindingTime + maxFollowTime <= Time.time) {
                    _target = null;
                    RB.velocity = Vector2.down * smashSpeed;
                    RB.gravityScale = 1;
                    _isFinding = false;
                }
                else {
                    RB.velocity = (new Vector2(_target.position.x, _target.position.y + 7) - (Vector2)transform.position).normalized * followSpeed;
                }
            }
            else if (_returningToDefaultPos) {
                if (Vector2.Distance(defaultPosition.position, transform.position) > 0.5f) {
                    RB.velocity = (defaultPosition.position - transform.position).normalized * returningSpeed;
                }
                else {
                    RB.velocity = Vector2.zero;
                    transform.SetParent(defaultPosition);
                    _returningToDefaultPos = false;
                }
            }
        }

        public void AttackSetUp(Transform target) {
            _target = target;
            _isFinding = true;

            RB.gravityScale = 0;
            Hit = false;
            transform.SetParent(null); 
            _startFindingTime = Time.time;
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (Hit || _isFinding) return;

            Collider2D col = Physics2D.OverlapCircle(transform.position, attackAreaRadius, 1 << LayerMask.NameToLayer("Player"));

            if (col && col.transform.parent.TryGetComponent(out GameEntity entity)) entity.OnHit(transform.position, smashDamage);

            // TODO play sound - hand smash sound
            Hit = true;
            AS.PlayOneShot(smashClip);
            //TODO when hit, go back to original pos or something
            StartCoroutine(AfterHitRoutine());
        }

        private IEnumerator AfterHitRoutine() {
            yield return new WaitForSeconds(0.5f);
            RB.gravityScale = 0;
            _returningToDefaultPos = true;
        }
    }
}