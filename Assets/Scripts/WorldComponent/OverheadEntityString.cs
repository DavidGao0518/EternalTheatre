using UnityEngine;

namespace WorldComponent {
    public class OverheadEntityString : MonoBehaviour {
        [SerializeField] private SpriteRenderer SR;
        public GameObject stringObj;
        private float _highlightedTime;

        private void Update() {
            if (_highlightedTime < 0) {
                SR.enabled = false;
                return;
            }

            _highlightedTime -= Time.deltaTime;
            SR.enabled = true;
            stringObj.transform.position = transform.position + new Vector3(0, 25, 0);
        }
    }
}