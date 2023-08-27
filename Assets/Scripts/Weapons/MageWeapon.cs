using UnityEngine;
using Weapons;

namespace Weapon {
    public class MageWeapon : Weapons.Weapon {
        [SerializeField] private GameObject projectile;
        [SerializeField] private Transform firePos;
        private Quaternion _aimRotation;
        [SerializeField] private AudioSource AS;
        [SerializeField] private AudioClip attackClip;

        public override void DoAttack() {
            print(firePos.rotation);
            Instantiate(projectile, firePos.position, _aimRotation);
            AS.PlayOneShot(attackClip);
        }

        public override void ManualAim(float rotation) {
            _aimRotation = Quaternion.Euler(0, 0, rotation);
        }
    }
}