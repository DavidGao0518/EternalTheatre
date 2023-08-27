using UnityEngine;
using Weapons;

namespace Weapon {
    public class RangedWeapon : Weapons.Weapon {
        [SerializeField] private GameObject arrow;
        [SerializeField] private Transform firePos;
        [SerializeField] private AudioSource AS;
        [SerializeField] private AudioClip attackClip;

        public override void DoAttack() {
            Instantiate(arrow, firePos.position, firePos.rotation);
            AS.PlayOneShot(attackClip);
        }

        public override void ManualAim(float rotation) {
            transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }
}