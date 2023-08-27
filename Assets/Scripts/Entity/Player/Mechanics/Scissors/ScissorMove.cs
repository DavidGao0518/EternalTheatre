using Manager;
using UnityEngine;

namespace Entity.Player.Mechanics.Scissors {
    public class ScissorMove : MonoBehaviour {
        private static readonly int ScissorSliceAnimationHash = Animator.StringToHash("Slice");
        private static float _timer;

        [SerializeField] private float rotateForce = 300;
        [SerializeField] private Rigidbody2D RB;
        [SerializeField] private Animator AN;
        [SerializeField] private AudioSource AS;
        [SerializeField] private AudioClip chopClip;
        [SerializeField] private ScissorCut SC;
        public bool autoCut, pulledOut;
        public float seconds;

        private void Update() {
            switch (autoCut) {
                case true: {
                    _timer += Time.deltaTime;
                    seconds = _timer % 60;
                    if (seconds > 2) autoCut = false;

                    break;
                }

                case false when pulledOut: {
                    _timer += Time.deltaTime;
                    float secs = _timer % 60;
                    if (secs > 0.5) {
                        SC.CutManage();
                        autoCut = true;
                        _timer = 0;
                        GameManager.GetInstance().UpdateScissorsUI(this);
                        AS.PlayOneShot(chopClip);
                        AN.SetTrigger(ScissorSliceAnimationHash);
                        pulledOut = false;
                    }

                    break;
                }
            }
        }

        public void RotateScissor(float angle) {
            if (RB) RB.MoveRotation(Mathf.Lerp(RB.rotation, angle, rotateForce * Time.fixedDeltaTime));
        }

        public void BeginningCooldown() {
            SC.gameObject.SetActive(true);
            pulledOut = true;
            _timer = 0;
        }
    }
}