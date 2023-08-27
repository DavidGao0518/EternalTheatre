using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entity.Abstract;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Entity.BossEnemy {
    public class BossEntity : GameEntity {
        #region Class Fields

        private static readonly int StringAnimationHash = Animator.StringToHash("string");
        private static readonly int SummonAnimationHash = Animator.StringToHash("summon");

        [SerializeField] private Transform playerTorso;
        [SerializeField] private Animator AN;

        [Header("Movement & Attack")] [SerializeField]
        private float followSpeed;

        [SerializeField] private float maintainDistance;
        [SerializeField] private float initialAttackDelay;
        [SerializeField] private float attackCooltime;

        [FormerlySerializedAs("leftHand")] [SerializeField]
        private BossHandBehaviour leftHandBehaviour;

        [FormerlySerializedAs("rightHand")] [SerializeField]
        private BossHandRightBehaviour rightBehaviourHand;

        [SerializeField] private GameObject stringAttackPrefab;
        [SerializeField] private GameObject[] enemiesToSpawn;

        [Header("Attack UI")] 
        [SerializeField] private Image healthBarFill;
        [SerializeField] private GameObject chrisFace;

        private float _lastAttackTime;
        private Coroutine _currentCoroutine;
        private byte _attackCounter;
        private string _christSpell;

        #endregion

        #region Initialisation

        // Start is called before the first frame update
        protected override void Start() {
            base.Start();
            SetUp();
        }

        private void SetUp() {
            _lastAttackTime = Time.time - initialAttackDelay;
        }

        #endregion

        #region update

        private void Update() {
            Movement();
            AttackChoose();

            if (Input.GetKeyUp(KeyCode.K))
			{
                _christSpell += "K";

            }
            if (Input.GetKeyUp(KeyCode.L))
            {
                _christSpell += "L";

            }
            if (Input.GetKeyUp(KeyCode.U))
            {
                _christSpell += "U";

            }
            if (Input.GetKeyUp(KeyCode.G))
            {
                _christSpell += "G";

            }

            chrisFace.SetActive(_christSpell == "KLUG");
        }

        private void Movement() {
            if (Mathf.Abs(transform.position.x - playerTorso.transform.position.x) > maintainDistance) {
                Vector3 position = transform.position;
                position = Vector2.Lerp(position, new Vector2(playerTorso.transform.position.x, position.y), followSpeed * Time.deltaTime);
                transform.position = position;
            }
        }

        #endregion

        #region attack

        private void AttackChoose() {
            if (_lastAttackTime + attackCooltime <= Time.time && _currentCoroutine == null) {
                float random = Random.Range(0, 5);

                if (_attackCounter <= 4) {
                    random = _attackCounter;
                    _attackCounter++;
                }

                switch (random) {
                    case 0:
                        AN.SetTrigger(StringAnimationHash);
                        _currentCoroutine = StartCoroutine(StringAttackRoutine());
                        break;
                    case 1:
                        _currentCoroutine = StartCoroutine(SmashRoutine2());
                        break;
                    case 2:
                        _currentCoroutine = StartCoroutine(SmashRoutine());
                        break;
                    case 3:
                        AN.SetTrigger(StringAnimationHash);
                        _currentCoroutine = StartCoroutine(StringAttackRoutine());
                        break;
                    case 4:
                        AN.SetTrigger(SummonAnimationHash);
                        _currentCoroutine = StartCoroutine(SummonRoutine());
                        break;
                }

                _lastAttackTime = Time.time;
            }
        }

        private IEnumerator SmashRoutine() {
            leftHandBehaviour.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.5f);

            int random = Random.Range(1, 4);

            for (int i = 0; i < random; i++) {
                leftHandBehaviour.AttackSetUp(playerTorso);

                while (!leftHandBehaviour.Hit) yield return null;

                yield return new WaitForSeconds(2);
                _lastAttackTime = Time.time;
            }

            leftHandBehaviour.DeActivate();
            yield return new WaitForSeconds(0.5f);
            leftHandBehaviour.gameObject.SetActive(false);
            _currentCoroutine = null;
        }

        private IEnumerator SmashRoutine2() {
            rightBehaviourHand.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.5f);

            rightBehaviourHand.AttackSetUp(playerTorso);

            while (!rightBehaviourHand.IsDoneAttacking) yield return null;

            _lastAttackTime = Time.time;
            yield return new WaitForSeconds(0.5f);
            _currentCoroutine = null;

            // Note: This will be disabled in the right-hand script
        }

        private IEnumerator StringAttackRoutine() {
            List<BossStringAttack> stringList = new List<BossStringAttack>();
            int random = Random.Range(5, 10);

            for (int i = 0; i < random; i++) {
                BossStringAttack stringAttack = Instantiate(stringAttackPrefab, new Vector2(playerTorso.position.x + Random.Range(-10, 10), playerTorso.position.y + Random.Range(0, 10)),
                    Quaternion.Euler(0, 0, Random.Range(-45, 45))).GetComponent<BossStringAttack>();

                stringList.Add(stringAttack);
                yield return new WaitForSeconds(0.25f);
            }

            yield return new WaitForSeconds(0.5f);

            foreach (var strAttack in stringList.Where(strAttack => strAttack)) {
                strAttack.StringStrengthen();
            }

            _lastAttackTime = Time.time - attackCooltime / 2;
            _currentCoroutine = null;
        }

        private IEnumerator SummonRoutine() {
            yield return new WaitForSeconds(1.5f);

            int random = Random.Range(2, 3);

            for (int i = 0; i < random; i++) {
                float xPos = transform.position.x + Random.Range(-20, 20);
                Instantiate(enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)], new Vector2(Mathf.Clamp(xPos, -7, 76), 25), Quaternion.identity);
                yield return new WaitForSeconds(0.25f);
            }

            _currentCoroutine = null;
            _lastAttackTime = Time.time + random * 3;
        }

        #endregion

        #region Health

        public override void OnDeath() {
            base.OnDeath();
            AppManager.GetInstance().sceneLoader.LoadNextScene();

            // TODO Shreyas - Cut Scene
        }

        public override void OnHit(Vector3 entityPosition, int damage) {
            base.OnHit(entityPosition, damage);
            CurrentHealth--;
            healthBarFill.fillAmount = CurrentHealth / (float)maxHealth;
            audioSource.PlayOneShot(hitClip);
            //hitParticleSystem.Play();
        }

        #endregion
    }
}