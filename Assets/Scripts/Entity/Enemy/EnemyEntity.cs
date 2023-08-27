using System.Collections.Generic;
using System.Linq;
using Entity.Abstract;
using Entity.Player.Mechanics;
using Manager;
using UnityEngine;
using Weapon;
using Weapons;

// Probably should bbe the ranged enemy. 
namespace Entity.Enemy {
    public class EnemyEntity : GameEntity {
        #region Fields

        private static readonly int IsWalking = Animator.StringToHash("isWalking");

        [Header("Object References")] [SerializeField]
        private Rigidbody2D torsoRB;

        [SerializeField] private FixedJoint2D pelvis;
        [SerializeField] private Animator AN;
        [SerializeField] private SpriteRenderer stringSR;

        [Header("Attack Data")] [SerializeField]
        private Weapons.Weapon weapon;

        [SerializeField] private float attackCooltime;
        private float _lastAttackTime;

        [Header("Target Data")] [SerializeField]
        private float findTargetRadius, targetDistanceToAttack, findTargetCooltime;

        [SerializeField] private bool noAnimOnAttack, rightArmTrackEnemy, flipBasedOnTarget;
        [SerializeField] private float targetDistanceToMaintainFollow = 0.8f;

        [Header("Movement Data")] [SerializeField]
        private float speed = 1f;

        [SerializeField] private List<BodySegment> bodySegments = new();
        [SerializeField] private RagdollBalance armUpperRightBalance, armLowerRightBalance;
        [SerializeField] private Transform feetPosition; //TODO also add left foot

        [SerializeField] private float watchForWallRaycastDistance, jumpSpeed;
        [SerializeField] private RagdollBalance[] rdb;

        private float _lastFindTargetTime;
        private bool _isGrounded, _needToJump;
        private GameObject _lockedTarget;
        private Vector2 _direction, _testPos;

        #endregion

        #region Initialisation

        protected override void Start() {
            base.Start();
            GameManager.GetInstance().RegisterEnemyToKillList(this);
        }

        #endregion

        #region Update

        private void Update() {
            CheckGrounded();

            if (!_lockedTarget) {
                FindTarget();
                return;
            }

            Movement();
            NavigateWalls();

            if (flipBasedOnTarget) FlipOrientation(_lockedTarget.transform.position.x < torsoRB.transform.position.x);
            if (_needToJump) TryJump();
            if (AttackCheck()) Attack();
            if (rightArmTrackEnemy) ArmMove();
        }

        #endregion

        #region Attack

        private void ArmMove() {
            _direction = (_lockedTarget.transform.position - torsoRB.transform.position).normalized;
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

            if (armLowerRightBalance) armLowerRightBalance.RotateManual(angle);
            if (armUpperRightBalance) armUpperRightBalance.RotateManual(angle);
            weapon.ManualAim(angle);
        }

        private bool AttackCheck() {
            float distanceToPlayer = Vector2.Distance(torsoRB.transform.position, _lockedTarget.transform.position);

            if (attackCooltime + _lastAttackTime <= Time.time && distanceToPlayer <= targetDistanceToAttack) {
                _lastAttackTime = Time.time;
                return true;
            }

            return false;
        }

        private void Attack() {
            if (!weapon) return;
            weapon.DoAttack();
            if (noAnimOnAttack) return;
            AN.SetTrigger((_lockedTarget.transform.position - torsoRB.transform.position).normalized.x > 0 ? "attack" : "attackLeft");
        }

        #endregion

        #region Pathfinding (sort of)

        private void FindTarget() {
            if (findTargetCooltime + _lastFindTargetTime <= Time.time) {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(torsoRB.transform.position, findTargetRadius, 1 << LayerMask.NameToLayer("Player"));

                foreach (Collider2D col in colliders) {
                    if (col.CompareTag("Torso")) _lockedTarget = col.gameObject;
                }
            }
        }

        /// <summary>
        /// isn't this just movement..???
        /// </summary>
        private void Movement() {
            if (!CheckGroundNearPlatform() && _isGrounded) {
                torsoRB.velocity = Vector2.zero;
                AN.SetBool(IsWalking, false);
                return;
            }

            if (Vector2.Distance(torsoRB.transform.position, _lockedTarget.transform.position) > targetDistanceToMaintainFollow && torsoRB.velocity != Vector2.zero) {
                AN.SetBool(IsWalking, true);

                Vector2 direction = (_lockedTarget.transform.position - torsoRB.transform.position).normalized;
                torsoRB.velocity = new Vector2(direction.x * speed, torsoRB.velocity.y);

                if (!flipBasedOnTarget) FlipOrientation(_lockedTarget.transform.position.x < torsoRB.transform.position.x);
            }
            else {
                AN.SetBool(IsWalking, false);
            }
        }

        private bool CheckGroundNearPlatform() {
            _testPos = new Vector2(torsoRB.position.x + ((_lockedTarget.transform.position.x - torsoRB.transform.position.x) < 0 ? -3 : 3), torsoRB.position.y - 3);

            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(_testPos, 0.2f, 1 << LayerMask.NameToLayer("Ground"));

            if (collider2Ds.Length > 0)
                return true;
            return false;

            // TODO shreyas to fix
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(_testPos, 0.2f);
        }

        /// <summary>
        /// Raycast this enemy's feet position in the direction of the walking vector.
        /// If the raycast hits the wall, the player must jump. 
        /// </summary>
        private void NavigateWalls() {
            RaycastHit2D hit = Physics2D.Raycast(feetPosition.transform.position, torsoRB.velocity.x < 0 ? Vector2.left : Vector2.right, watchForWallRaycastDistance,
                1 << LayerMask.NameToLayer("Ground"));
            if (hit) _needToJump = true;
        }

        /// <summary>
        /// if is grounded will jump
        /// </summary>
        private void TryJump() {
            if (_isGrounded) {
                torsoRB.AddForce(Vector2.up * jumpSpeed);
                _needToJump = false;
            }
        }

        /// <summary>
        /// check grounded with feed position
        /// </summary>
        private void CheckGrounded() {
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(feetPosition.position, 0.15f, 1 << LayerMask.NameToLayer("Ground"));
            _isGrounded = collider2Ds.Length > 0;
            //print(collider2Ds.Length);
        }

        private void FlipOrientation(bool flip) {
            foreach (BodySegment segment in bodySegments) {
                if (segment.balance) segment.balance.flipX = flip;

                segment.SR.sortingOrder = flip ? segment.flippedSortingOrder : segment.regularSortingOrder;

                if (!segment.isArm) segment.SR.flipX = flip;
            }
        }

        #endregion

        #region Dev Tools

        [ContextMenu("Auto-Setup BodySegment Refs")]
        private void AutoSetupBodySegments() {
            bodySegments = new List<BodySegment>();

            foreach (Rigidbody2D RB in GetComponentsInChildren<Rigidbody2D>()) {
                SpriteRenderer SR = RB.GetComponentInChildren<SpriteRenderer>();
                RagdollBalance balancer = RB.GetComponentInChildren<RagdollBalance>();
                ThrowablePart throwablePart = RB.GetComponentInChildren<ThrowablePart>();
                HingeJoint2D hinge = RB.GetComponentInChildren<HingeJoint2D>();

                int regularSortingOrder = 0;
                bool isArm = false;

                switch (RB.gameObject.name) {
                    case "Arm_Upper_L":
                        isArm = true;
                        regularSortingOrder = 5;
                        break;
                    case "Arm_Upper_R":
                        isArm = true;
                        regularSortingOrder = -5;
                        armUpperRightBalance = balancer;
                        break;
                    case "Arm_Lower_L":
                        isArm = true;
                        regularSortingOrder = 4;
                        break;
                    case "Arm_Lower_R":
                        isArm = true;
                        regularSortingOrder = -4;
                        armLowerRightBalance = balancer;
                        break;
                    case "Leg_Upper_L":
                        regularSortingOrder = 5;
                        break;
                    case "Leg_Upper_R":
                        regularSortingOrder = -5;
                        break;
                    case "Leg_Lower_L":
                        regularSortingOrder = 4;
                        break;
                    case "Leg_Lower_R":
                        regularSortingOrder = -4;
                        break;
                }

                bodySegments.Add(new BodySegment(SR, balancer, throwablePart, hinge, regularSortingOrder, -regularSortingOrder, isArm));
            }
        }

        #endregion

        #region Health

        public override void OnDeath() { //TODO player one shot bug fix
            if (IsDead) return;

            base.OnDeath();

            foreach (BodySegment segment in bodySegments.Where(bodySegment => bodySegment.balance != null)) segment.balance.enabled = false;

            GameManager.GetInstance().EnemyEntityDead(this);

            //AN.enabled = false;
            weapon.enabled = false;
            enabled = false; // TODO: disable other parts too
            stringSR.enabled = false;
            GameManager.GetInstance().AddGrappleCharge(1);

            Invoke(nameof(OnDeathRoutine), 3);
        }

        private void OnDeathRoutine()
		{
            AN.Play("CharacterDie");
            Destroy(gameObject, 1);
        }

        public void OnDeathByScissors() {
            OnDeath();
            foreach (BodySegment segment in bodySegments.Where(bodySegment => bodySegment.hingeJoint2D != null)) segment.hingeJoint2D.enabled = false;

            pelvis.enabled = false;
        }

        public override void OnHit(Vector3 enemyPosition, int damage) { // make this boolean
            CurrentHealth--;
            
            base.OnHit(enemyPosition, damage);
            audioSource.PlayOneShot(hitClip);
            hitParticleSystem.Play();
        }

        #endregion
    }
}