using UnityEngine;

namespace Demo {
    public class DemoSpeedManager : MonoBehaviour {
        public static DemoSpeedManager Instance { get; private set; }

        [Header("Speed Settings")]
        [SerializeField] private float citySpeed = 3f;
        [SerializeField] private float vechicleSpeed = 3f;
        [SerializeField] private float movingVechicleSpeed = 3f;


        // Lifecycle

        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("DemoSpeedManager: Instance already exists");
                Destroy(gameObject);
                return;
            }

            Instance = this;

        }


        // Setters
        public void SetCitySpeed(float newSpeed) => citySpeed = newSpeed;
        public void SetVechicleSpeed(float newSpeed) => vechicleSpeed = newSpeed;
        public void SetMovingVechicleSpeed(float newSpeed) => movingVechicleSpeed = newSpeed;



        // Getters

        public float GetCitySpeed() => citySpeed;
        public float GetVechicleSpeed() => vechicleSpeed;
        public float GetMovingVechicleSpeed() => movingVechicleSpeed;
    }
}
