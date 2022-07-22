using UnityEngine;
using UnityEngine.EventSystems;

namespace e23.RotorcraftController.Examples
{
    public class ExampleTouchScreen : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
#pragma warning disable 0649
        [SerializeField] private RotorcraftBehaviour rotorcraftBehaviour;
        
        [Header("Altitude")]
        [SerializeField] private bool throttleUp;
        [SerializeField] private bool throttleDown;
        [Header("Acceleration")]
        [SerializeField] private bool forward;
        [SerializeField] private bool backward;
        [Header("Turning")]
        [SerializeField] private bool steerLeft;
        [SerializeField] private bool steerRight;
        [Header("Strafing")]
        [SerializeField] private bool rollLeft;
        [SerializeField] private bool rollRight;
#pragma warning restore 0649

        private bool doAction = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            doAction = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            doAction = false;
        }

        private void Update()
        {
            if (doAction)
            {
                // Craft height
                if (throttleUp) { rotorcraftBehaviour.Throttle(1); }
                if (throttleDown) { rotorcraftBehaviour.Throttle(-1); }

                // Movement forward/ back
                if (forward) { rotorcraftBehaviour.ControlPitchForward(); }
                if (backward) { rotorcraftBehaviour.ControlPitchBackwards(); }

                // Turning
                if (steerLeft) { rotorcraftBehaviour.ControlYaw(-1); }
                if (steerRight) { rotorcraftBehaviour.ControlYaw(1); }

                // Height
                if (rollLeft) { rotorcraftBehaviour.ControlRoll(-1); }
                if (rollRight) { rotorcraftBehaviour.ControlRoll(1); }
            }
        }
    }
}