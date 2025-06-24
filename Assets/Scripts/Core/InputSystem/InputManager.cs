using System;
using UnityEngine;

using Core.EventSystem;


namespace Core.InputSystem {
    [RequireComponent(typeof(SwipeController))]
    public class InputManager : MonoBehaviour {
        public static InputManager Instance { get; private set; }

        private SwipeController swipeController;

        private void Awake() {
            swipeController = GetComponent<SwipeController>();
        }
    }
}
