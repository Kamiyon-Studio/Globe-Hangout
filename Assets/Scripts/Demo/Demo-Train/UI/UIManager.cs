using UnityEngine;

using Core.EventSystem;
using Demo.Train.Events;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject ArrowUI;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("UIManager: Instance already exists!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable() {
        EventBus.Subscribe<Evt_ActivateArrowUI>(ToggleArrowUI);
    }

    private void OnDisable() {
        EventBus.Unsubscribe<Evt_ActivateArrowUI>(ToggleArrowUI);

    }

    private void ToggleArrowUI(Evt_ActivateArrowUI e) {
        ArrowUI.SetActive(e.active);
    }
}
