using UnityEngine;

namespace Entity.Player.Mechanics.Scissors {
    public class ScissorsControl : MonoBehaviour {
        [SerializeField] float speed = 300;
        public Rigidbody2D RB;

        private void Update() {
            Vector3 direction = Camera.main!.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            RB.MoveRotation(Mathf.Lerp(RB.rotation, angle, speed * Time.fixedDeltaTime));
        }
    }
}