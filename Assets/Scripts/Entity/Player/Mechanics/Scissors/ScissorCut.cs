using UnityEngine;

namespace Entity.Player.Mechanics.Scissors {
    public class ScissorCut : MonoBehaviour {
        [SerializeField] private float stringDetectingRadius = 10;
        public bool canCut;
        private float _timer;

        private void Update() {
            FindNearbyStrings();
            if (!canCut) return;

            _timer += Time.deltaTime;
            float seconds = (_timer % 60);

            if (seconds > 0.25) {
                canCut = false;
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerStay2D(Collider2D collision) {
            if (!canCut) return;

            if (collision.gameObject.TryGetComponent(out StringHandler cutString)) cutString.Cut(); // TODO cut by mouse left click.
        }

        public void CutManage() {
            canCut = true;
            _timer = 0;
        }

        private void FindNearbyStrings() {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, stringDetectingRadius, 1 << LayerMask.NameToLayer("String"));

            foreach (Collider2D overlappedColliders in cols) {
                if (overlappedColliders.TryGetComponent(out StringHandler strings)) strings.ShowNearScissors();
            }
        }
    }
}