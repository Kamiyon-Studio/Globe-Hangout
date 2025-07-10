using UnityEngine;

namespace Demo.Train.Events {
    public class Evt_ActivateArrowUI {
        public bool active;

        public Evt_ActivateArrowUI(bool active) {
            this.active = active;
        }
    }

    public class Evt_OnRightArrow { }
    public class Evt_OnLeftArrow { }

}