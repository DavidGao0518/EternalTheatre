using System.Collections;
using Manager;
using UnityEngine;

namespace Entity.Player.Mechanics {
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(DistanceJoint2D))]
    public class Grapple : MonoBehaviour {
        [SerializeField] private Rigidbody2D RB;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private LayerMask grappleMask;

        [SerializeField] private float maxDistance = 1000f;
        [SerializeField] private float grapplingSpeed = 10f;
        [SerializeField] private float grappleShootSpeed = 20f;

        public bool Retracting { get; private set; } // Retracting is when the player moves towards the grapple point
        private bool IsGrappling { get; set; }

        private Transform _grappleFollowObj, _grappleTargetObj;
        private Rigidbody2D _selectedBodyForMove;
        private Vector2 _grappleHitPositionOffset;
        private Vector2 FinalTargetPos => (Vector2)_grappleTargetObj.transform.position - _grappleHitPositionOffset;

        private void FixedUpdate() {
            if (Retracting) lineRenderer.SetPosition(0, transform.position);
            Pull();
        }

        private void Pull() {
            if (!Retracting) return;

            lineRenderer.SetPosition(0, _grappleFollowObj.position);
            lineRenderer.SetPosition(1, FinalTargetPos);
            Vector2 dir = FinalTargetPos - (Vector2)_grappleFollowObj.position;
            dir.Normalize();

            float distance = Vector2.Distance(FinalTargetPos, _grappleFollowObj.position);

            _selectedBodyForMove.velocity = new Vector2(dir.x * grapplingSpeed, dir.y * grapplingSpeed / 2);

            if (distance < 1f || !_grappleTargetObj.gameObject.activeInHierarchy) {
                _selectedBodyForMove.velocity = Vector2.zero;
                Retracting = false;
                IsGrappling = false;
                lineRenderer.enabled = false;
                //GameManager.GetInstance().AddGrappleCharge(-1);
            }
        }

        public void UnGrapple() {
            if (!IsGrappling) return;

            _selectedBodyForMove.velocity = Vector2.zero;
            Retracting = false;
            IsGrappling = false;
            lineRenderer.enabled = false;
        }

        public bool StartGrapple(Vector2 direction) {
            if (!IsGrappling && GameManager.GetInstance().GrappleCharges > 0) {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappleMask);

                Debug.DrawRay(transform.position, direction, Color.blue);

                if (hit.collider != null) {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") && hit.transform.TryGetComponent(out Rigidbody2D rb)) {
                        _selectedBodyForMove = rb;
                        _grappleFollowObj = hit.collider.transform;
                        _grappleTargetObj = transform;
                        _grappleHitPositionOffset = (Vector2)hit.collider.transform.position - hit.point;
                    }
                    else {
                        _selectedBodyForMove = RB;
                        _grappleFollowObj = transform;
                        _grappleTargetObj = hit.transform;
                        _grappleHitPositionOffset = (Vector2)hit.collider.transform.position - hit.point;
                    }

                    IsGrappling = true;
                    GameManager.GetInstance().AddGrappleCharge(-1);
                    lineRenderer.enabled = true;
                    lineRenderer.positionCount = 2;
                    StartCoroutine(Grappler());
                    Vector2.Distance(_grappleFollowObj.position, FinalTargetPos);
                    return true;
                }
            }
            return false;
        }

        private IEnumerator Grappler() {
            float t = 0;
            float time = 10;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
            Vector2 newPos;
            for (; t < time; t += grappleShootSpeed * Time.deltaTime) {
                newPos = Vector2.Lerp(transform.position, FinalTargetPos, t / time);
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, newPos);
                yield return null;
            }

            lineRenderer.SetPosition(1, FinalTargetPos);
            Retracting = true;
        }
    }
}