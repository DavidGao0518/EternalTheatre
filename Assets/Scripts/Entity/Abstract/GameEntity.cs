using UnityEngine;

namespace Entity.Abstract {
    public abstract class GameEntity : MonoBehaviour {
        [SerializeField] protected int maxHealth;
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected AudioClip hitClip;
        [SerializeField] protected ParticleSystem hitParticleSystem;
        protected int CurrentHealth;
        public bool IsDead { get; private set; }

        protected virtual void Start() {
            CurrentHealth = maxHealth;
        }

        public virtual void OnDeath() {
            IsDead = true;
        }

        public virtual void OnHit(Vector3 entityPosition, int damage) {
            if (CurrentHealth <= 0) OnDeath();
        }

        public int GetCurrentHealth() {
            return CurrentHealth;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.CompareTag("DeathBlock")) OnDeath();
        }
    }
}