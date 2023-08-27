using Entity.Abstract;
using Entity.Player.Mechanics.Scissors;
using UnityEngine;

namespace Entity.BossEnemy {
    public class BossStringAttack : MonoBehaviour {
        [SerializeField] private StringHandler stringHandler;
        [SerializeField] private Animator AN;
        [SerializeField] private int damage = 1;
        private bool _canHit;

        public void StringStrengthen() {
            Destroy(stringHandler);
            AN.Play("StringAttackStrengthen");
            gameObject.layer = 0;
        }

        /// <summary>
        /// called by AN animation event
        /// </summary>
        private void StringAttackFrame() {
            _canHit = true;
            Destroy(gameObject, 0.5f);
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (!_canHit) return;
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            if (collision.transform.parent && collision.transform.parent.TryGetComponent(out GameEntity entity)) entity.OnHit(transform.position, damage);

            // TODO play sound - string sound
            _canHit = false;
        }
    }
}