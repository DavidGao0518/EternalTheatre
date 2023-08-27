using Manager;
using UnityEngine;

namespace WorldComponent {
    public class Parallax : MonoBehaviour {
        public GameObject cam;
        public SpriteRenderer SR;
        public float parallaxEffect;
        public bool followY;
        public bool noSkipEffect;
        public float startPosOffset;

        private float _length;
        private Vector2 _startPos;

        private void Start() {
            _startPos = new Vector2(transform.position.x + startPosOffset, transform.position.y);
            _length = SR.bounds.size.x;
        }

        private void Update() {
            if (GameManager.GetInstance().isGameOver) return;

            float temp = cam.transform.position.x * (1 - parallaxEffect);

            float distance = cam.transform.position.x * parallaxEffect;

            transform.position = new Vector3(_startPos.x + distance, followY ? cam.transform.position.y : transform.position.y, transform.position.z);

            if (noSkipEffect) return;

            if (temp > _startPos.x + _length) _startPos.x += _length;
            else if (temp < _startPos.x - _length) _startPos.x -= _length;
        }
    }
}