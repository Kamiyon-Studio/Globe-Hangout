using UnityEngine;

public class VechicleMove : MonoBehaviour {
    [Header("Speed")]
    [SerializeField] private float speed;

    private void Update() {
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }
}
