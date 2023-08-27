using Entity.Enemy;
using UnityEngine;

namespace Entity.Player.Mechanics.Scissors {
    public class StringHandler : MonoBehaviour {
        [SerializeField] private EnemyEntity enemyEntity;
        [SerializeField] private Rigidbody2D RB;
        [SerializeField] private SpriteRenderer SR;
        [SerializeField] private Transform headObj;
        [SerializeField] private bool isStringStatic;
        [SerializeField] private Color disabledColor;
        private float _highlightedTime;

        public void Cut() {
            if (enemyEntity && enemyEntity.IsDead) return;
            if (enemyEntity) enemyEntity.OnDeathByScissors();
            if (RB) RB.isKinematic = false;

            Destroy(gameObject);
        }

        public void ShowNearScissors() {
            _highlightedTime = 0.05f;
        }

        private void Update() {
            if (!isStringStatic) transform.position = headObj.transform.position + new Vector3(0, 25, 0);

            if (_highlightedTime < 0 || (enemyEntity && enemyEntity.IsDead)) {
                if (SR) SR.color = disabledColor;
                return;
            }

            _highlightedTime -= Time.deltaTime;
            if (SR) SR.color = Color.white;
        }
    }
}