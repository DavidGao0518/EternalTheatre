using Entity.Player.Mechanics;
using UnityEngine;

namespace Entity.Abstract {
    public class RagdollBalance : MonoBehaviour {
        [SerializeField] private float force;
        public bool flipX;
        public float targetRotation;
        public Rigidbody2D RB;

        public ThrowablePart throwablePart;

        [Header("Rotate Behaviour Util")] [SerializeField]
        bool useRotationFlipMethod2;

        [SerializeField] bool noRotationFlip;
        [SerializeField] bool onlyRotateCustom;

        private bool _turnedOff;

        private void Update() {
            if ((throwablePart && throwablePart.IsThrowing) || _turnedOff || onlyRotateCustom) return;

            if (noRotationFlip)
                RB.MoveRotation(Mathf.LerpAngle(RB.rotation, targetRotation, force * Time.fixedDeltaTime));
            else if (useRotationFlipMethod2)
                RB.MoveRotation(Mathf.LerpAngle(RB.rotation, flipX ? 180 - targetRotation : 1 * targetRotation, force * Time.fixedDeltaTime));
            else
                RB.MoveRotation(Mathf.LerpAngle(RB.rotation, flipX ? -targetRotation : 1 * targetRotation, force * Time.fixedDeltaTime));
        }

        public void RotateManual(float rot) {
            RB.MoveRotation(Mathf.LerpAngle(RB.rotation, rot, force * Time.fixedDeltaTime));
        }
    }
}