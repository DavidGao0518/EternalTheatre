using Entity.Abstract;
using UnityEngine;

namespace WorldComponent {
    public class DeathBlockBehaviour : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.transform.parent && other.transform.parent.gameObject.TryGetComponent(out GameEntity gameEntity) && other.CompareTag("Torso")) gameEntity.OnDeath();
        }
    }
}