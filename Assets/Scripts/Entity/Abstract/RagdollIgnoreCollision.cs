using UnityEngine;
using UnityEngine.Serialization;

namespace Entity.Abstract {
    public class RagdollIgnoreCollision : MonoBehaviour {
        [FormerlySerializedAs("targetTag")] [SerializeField]
        private Collider2D thisCollider;

        [SerializeField] private string ignoreLayer = "Player";

        private void Start() {
            if (!thisCollider) thisCollider = GetComponent<Collider2D>();
            // This is set to things do not instantly break when you guys don't assign stuff

            // Check if this object has a parent
            if (transform.parent != null)
            {
                // Get all colliders in siblings and parent
                Collider2D[] siblingColliders = transform.parent.GetComponentsInChildren<Collider2D>();
                foreach (var siblingCollider in siblingColliders)
                {
                    if (siblingCollider.gameObject.layer == LayerMask.NameToLayer(ignoreLayer))
                    {
                        Physics2D.IgnoreCollision(thisCollider, siblingCollider, true);
                    }
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D coll) {
            if (coll.gameObject.layer == LayerMask.NameToLayer(ignoreLayer)) Physics2D.IgnoreCollision(thisCollider, coll.gameObject.GetComponent<Collider2D>());
        }
    }
}