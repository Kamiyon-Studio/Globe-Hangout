using UnityEngine;

namespace Demo {
    public class CityMovement : MonoBehaviour {
        private float speed;

        private void Start() {
            speed = DemoSpeedManager.Instance.GetCitySpeed();
        }

        private void Update() {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
    }
}