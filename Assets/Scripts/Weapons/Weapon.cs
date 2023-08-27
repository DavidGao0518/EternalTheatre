using UnityEngine;

namespace Weapons {
    public class Weapon : MonoBehaviour {
        public virtual void DoAttack() {
            throw new System.NotImplementedException("DoAttack() should be overriden in a child class! (WeaponBase.cs)");
        }

        public virtual void ManualAim(float rotation) {
        }
    }
}