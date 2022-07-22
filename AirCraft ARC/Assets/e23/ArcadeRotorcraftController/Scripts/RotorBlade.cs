using UnityEngine;

namespace e23.RotorcraftController
{
    public class RotorBlade : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 1000f, 0f);
        [Tooltip("How fast the rotor blades slow down when the engine turns off.")]
        [SerializeField] private Vector3 rotationDrag = new Vector3(10f, 10f, 10f);

        private RotorcraftBehaviour rotorcraftBehaviour;
        private bool spinBlades = false;
        private Vector3 currentSpeed = Vector3.zero;

        private void Awake()
        {
            GetRequiredComponents();
        }

        private void OnEnable()
        {
            RegisterActions(true);
        }

        private void OnDisable()
        {
            RegisterActions(false);
        }

        private void GetRequiredComponents()
        {
            rotorcraftBehaviour = GetComponentInParent<RotorcraftBehaviour>();
        }

        private void RegisterActions(bool register)
        {
            rotorcraftBehaviour.onEngineEnabled -= SpinBlades;

            if (register == false) { return; }

            rotorcraftBehaviour.onEngineEnabled += SpinBlades;
        }

        private void SpinBlades(bool active)
        {
            spinBlades = active;
        }

        private void Update()
        {
            RotatePropeller();
        }

        private void RotatePropeller()
        {
            if (spinBlades == true)
            {
                currentSpeed = rotationSpeed * Time.deltaTime;
            }
            else if (currentSpeed.x > 0f || currentSpeed.y > 0f || currentSpeed.z > 0f)
            {
                if (currentSpeed.x > 0f) { currentSpeed.x = Mathf.SmoothStep(currentSpeed.x, currentSpeed.x - rotationDrag.x, Time.deltaTime * 2); }
                if (currentSpeed.y > 0f) { currentSpeed.y = Mathf.SmoothStep(currentSpeed.y, currentSpeed.y - rotationDrag.y, Time.deltaTime * 2); }
                if (currentSpeed.z > 0f) { currentSpeed.z = Mathf.SmoothStep(currentSpeed.z, currentSpeed.y - rotationDrag.z, Time.deltaTime * 2); }
            }
            
            transform.Rotate(currentSpeed.x, currentSpeed.y, currentSpeed.z);
        }
    }
}