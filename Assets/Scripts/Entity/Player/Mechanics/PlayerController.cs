using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entity.Abstract;
using Entity.Player.Mechanics.Scissors;
using UnityEngine;
using Manager;

// Partial Credit: https://www.youtube.com/watch?v=uVtDyPmorA0&t=180s
namespace Entity.Player.Mechanics {
    public class PlayerController : MonoBehaviour {
        #region fields

        private static readonly int IsWalking = Animator.StringToHash("isWalking");

        [Header("Movement References")] [SerializeField]
        private Rigidbody2D torsoRigidBody;

        [SerializeField] private float speed = 5;
        [SerializeField] private float jumpSpeed;
        [SerializeField] private Transform feetPosition;
        [SerializeField] private float jumpCooltime = 0.5f;
        private bool _isGrounded;
        private float _lastJumpTIme;

        [Header("Graphics References")] [SerializeField]
        private Animator animator;

        [SerializeField] private AudioSource AS;
        [SerializeField] private AudioClip launchClip, grappleClip, emptyClip;
        [SerializeField] private AudioClip[] footstepClips;

        private readonly List<ThrowablePart> _throwableParts = new();
        [SerializeField] public List<BodySegment> bodySegments = new();
        [SerializeField] private RagdollBalance[] balance;

        [Header("Scissors")] [SerializeField] private ScissorMove scissorMove;
        [SerializeField] private RagdollBalance armUpperRightBalance, armLowerRightBalance;

        [Header("Grapple")] [SerializeField] private Grapple rightArm;
        [SerializeField] private Grapple leftArm;
        private bool _isReloading;
        private Vector2 _mouseDir;
        private bool _scissorOut;

        private bool HasLeftLeg => _throwableParts.Any(x => x.gameObject.name == "Leg_Upper_L");
        private bool HasRightLeg => _throwableParts.Any(x => x.gameObject.name == "Leg_Upper_R");
        private bool HasBothLegs => HasLeftLeg && HasRightLeg;

        #endregion

        #region Initialisation

        private void Start() {
            SetUpThrowables();
        }

        private void SetUpThrowables() {
            foreach (var bodySegment in bodySegments.Where(bodySegment => bodySegment.throwablePart != null)) _throwableParts.Add(bodySegment.throwablePart);
        }

        #endregion

        #region Update Loop

        private void Update() {
            CheckGrounded();
            HandleMovement();

            _mouseDir = (Camera.main!.ScreenToWorldPoint(Input.mousePosition) - torsoRigidBody.transform.position).normalized;

            if (Input.GetMouseButtonDown(0) && !_scissorOut) Fling();
            if (Input.GetMouseButtonDown(1) && !_scissorOut) Reload();
            if (Input.GetKeyDown(KeyCode.E)) {
                if (!_scissorOut) {
                    HandleGrapple();
                }
                else {
                    //_scissorOut = false;
                    //scissorMove.gameObject.SetActive(false);
                    HandleGrapple();
                }
            }

            if (Input.GetKeyDown(KeyCode.Q) && !_scissorOut && !scissorMove.autoCut) {
                if (_throwableParts.Any(x => x.gameObject.name == "Arm_Upper_R")) {
                    _scissorOut = true;
                    scissorMove.BeginningCooldown();
                    StartCoroutine(Cooldown());
                } else
				{
                    Reload();
				}
            }

            if (_scissorOut) {
                float angle = Mathf.Atan2(_mouseDir.y, _mouseDir.x) * Mathf.Rad2Deg;

                armLowerRightBalance.RotateManual(angle);
                armUpperRightBalance.RotateManual(angle);
                scissorMove.RotateScissor(angle);

                FlipOrientation(_mouseDir.x < 0);
            }
        }

        private IEnumerator Cooldown() {
            yield return new WaitForSeconds(1.5f);
            _scissorOut = false;
        }

        public void SetBalance(bool value) {
            foreach (var t in balance) t.enabled = value;
            animator.enabled = value;
        }

        #endregion

        #region Movement

        private void HandleMovement() {
            float axisX = Input.GetAxisRaw("Horizontal");

            if (axisX != 0 && !_scissorOut) FlipOrientation(axisX < 0);
            if (_lastJumpTIme + jumpCooltime <= Time.time && _isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && HasBothLegs)
			{
                torsoRigidBody.AddForce(_throwableParts.Count * 0.25f * jumpSpeed * Vector2.up);
                _lastJumpTIme = Time.time;
			}
            if (HasBothLegs) torsoRigidBody.velocity = new Vector2(_throwableParts.Count * 0.25f * speed * axisX, torsoRigidBody.velocity.y);

            animator.SetBool(IsWalking, axisX != 0);
        }

        private void CheckGrounded() {
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(feetPosition.position, 0.15f,
                1 << LayerMask.NameToLayer("Ground")
                | 1 << LayerMask.NameToLayer("Enemy")
                | 1 << LayerMask.NameToLayer("Platform")
                | 1 << LayerMask.NameToLayer("Hand"));

            _isGrounded = collider2Ds.Length > 0;
        }

        private void FlipOrientation(bool flip) {
            if (torsoRigidBody.velocity == Vector2.zero) return;

            foreach (BodySegment segment in bodySegments) {
                if (segment.balance) segment.balance.flipX = flip;

                segment.SR.sortingOrder = flip ? segment.flippedSortingOrder : segment.regularSortingOrder;

                if (!segment.isArm) segment.SR.flipX = flip;
            }
        }

        /// <summary>
        /// called from Animator
        /// </summary>
        private void FootStep() {
            if (_isGrounded) AS.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length - 1)]);
        }

        private void HandleGrapple() {

            if (!rightArm.Retracting && !leftArm.Retracting) {

                if (_mouseDir.x > 0)
                {
                    if (_throwableParts.Any(x => x.gameObject.name == "Arm_Upper_R"))
					{
                        if (rightArm.StartGrapple(_mouseDir)) AS.PlayOneShot(grappleClip);
                        else if (GameManager.GetInstance().GrappleCharges == 0) AS.PlayOneShot(emptyClip);
                    }
                    else
					{
                        Reload();
					}
                    
                }
                else 
                {
                    if (_throwableParts.Any(x => x.gameObject.name == "Arm_Upper_L"))
					{
                        if (leftArm.StartGrapple(_mouseDir)) AS.PlayOneShot(grappleClip);
                        else if (GameManager.GetInstance().GrappleCharges == 0) AS.PlayOneShot(emptyClip);
                    }
                    else
                    {
                        Reload();
                    }

                }
            }
            else {
                leftArm.UnGrapple();
                rightArm.UnGrapple();
            }
        }

        #endregion

        #region More Control Stuff

        private void Fling() {
            if (_isReloading) return;

            if (_throwableParts.Count > 0) {
                ThrowablePart throwSelected = _throwableParts[0];

                foreach (ThrowablePart throwable in _throwableParts) {
                    if (_mouseDir.x < 0) {
                        if (throwable.gameObject.name == "Arm_Upper_L") {
                            throwSelected = throwable;
                            break;
                        }

                        if (throwable.gameObject.name == "Leg_Upper_L") throwSelected = throwable;
                    }
                    else {
                        if (throwable.gameObject.name == "Arm_Upper_R") {
                            throwSelected = throwable;
                            break;
                        }

                        if (throwable.gameObject.name == "Leg_Upper_R") throwSelected = throwable;
                    }
                }

                _throwableParts.Remove(throwSelected);
                throwSelected.ThrowPart(_mouseDir);
                AS.PlayOneShot(launchClip);
            }
            else {
                Reload();
            }
        }

        private void Reload() {
            if (_throwableParts.Count < 4) _isReloading = true;

            foreach (var bodySegment in bodySegments.Where(bodySegment => bodySegment.throwablePart != null)) {
                if (!_throwableParts.Contains(bodySegment.throwablePart)) bodySegment.throwablePart.ReturnToBody(this);
            }
        }

        public void GetReturnBody(ThrowablePart partReturned) {
            if (!_throwableParts.Contains(partReturned))
                _throwableParts.Add(partReturned);
            else return;
            
            if (_throwableParts.Count >= 4) _isReloading = false;
        }

        #endregion

        #region Dev tools

        [ContextMenu("Auto-Setup BodySegment Refs")]
        private void AutoSetupBodySegments() {
            bodySegments = new List<BodySegment>();

            foreach (Rigidbody2D RB in GetComponentsInChildren<Rigidbody2D>()) {
                SpriteRenderer SR = RB.GetComponentInChildren<SpriteRenderer>();
                RagdollBalance balancer = RB.GetComponentInChildren<RagdollBalance>();
                ThrowablePart throwablePart = RB.GetComponentInChildren<ThrowablePart>();
                HingeJoint2D hinge = RB.GetComponentInChildren<HingeJoint2D>();

                int regularSortingOrder;

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
                    default:
                        return;
                }

                bodySegments.Add(new BodySegment(SR, balancer, throwablePart, hinge, regularSortingOrder, -regularSortingOrder, isArm));
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawWireSphere(feetPosition.position, 0.15f);
        }

        #endregion

        // get body segments
        public List<BodySegment> GetBodySegments() {
            return bodySegments;
        }
    }

    [System.Serializable]
    public class BodySegment {
        public SpriteRenderer SR;
        public RagdollBalance balance;
        public HingeJoint2D hingeJoint2D;
        public ThrowablePart throwablePart;
        public int regularSortingOrder, flippedSortingOrder;
        public bool isArm;

        public BodySegment(SpriteRenderer spriteRenderer,
            RagdollBalance balance,
            ThrowablePart throwablePart,
            HingeJoint2D hingeJoint2D,
            int regularSortingOrder,
            int flippedSortingOrder,
            bool isArm) {
            SR = spriteRenderer;
            this.balance = balance;
            this.throwablePart = throwablePart;
            this.regularSortingOrder = regularSortingOrder;
            this.flippedSortingOrder = flippedSortingOrder;
            this.isArm = isArm;
            this.hingeJoint2D = hingeJoint2D;
        }
    }
}