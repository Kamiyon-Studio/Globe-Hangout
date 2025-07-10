using Core.EventSystem;
using Core.InputSystem.Events;
using Demo.Train.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Demo.Train.UI {
    public class ArrowUI : MonoBehaviour {
        [SerializeField] private Image leftArrow;
        [SerializeField] private Image rightArrow;

        private Animator animator;

        private void Start() {
            gameObject.SetActive(false);
            animator = GetComponent<Animator>();
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnSwipeLeft>(OnLeftArrow);
            EventBus.Subscribe<Evt_OnSwipeRight>(OnRightArrow);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnSwipeLeft>(OnLeftArrow);
            EventBus.Unsubscribe<Evt_OnSwipeRight>(OnRightArrow);
        }

        private void OnLeftArrow(Evt_OnSwipeLeft e) {
            animator.SetTrigger("LeftArrow");
        }

        private void OnRightArrow(Evt_OnSwipeRight e) {
            animator.SetTrigger("RightArrow");
        }
    }

}