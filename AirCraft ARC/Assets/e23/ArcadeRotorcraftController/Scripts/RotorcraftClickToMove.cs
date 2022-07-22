using UnityEngine;

namespace e23.RotorcraftController
{
    [RequireComponent(typeof(RotorcraftBehaviour))]
    public class RotorcraftClickToMove : MonoBehaviour
    {
        [Tooltip("Can be left empty.")]
        [SerializeField] private Transform targetVisual = null;
        [Tooltip("If left empty Camera.main will be used in Awake().\nUse the Camera property to assign a camera from an external class.")]
        [SerializeField] private Camera _camera = null;
        [Tooltip("How far away the target position needs to be before the rotorcraft will move.")]
        [SerializeField] private float minDistanceToMove = 10f;
        [Tooltip("The minimum angle required between the rotorcraft forward and target position, tweak this value if the rotorcraft is 'shaking' as it heads to a target position.")]
        [SerializeField] private float minAngleToTurn = 10f;

        private RotorcraftBehaviour rotorcraftBehaviour;
        private bool atTargetPosition = true;
        private Vector3 targetPosition;

        public Camera Camera 
        { 
            get { return _camera; } 
            set { _camera = value; } 
        }

        private void Awake()
        {
            GetRequiredComponents();
        }

        private void GetRequiredComponents()
        {
            if (Camera == null) 
            { 
                Camera = Camera.main;            
            }

            rotorcraftBehaviour = GetComponent<RotorcraftBehaviour>();
        }

        private void Update()
        {
            if (rotorcraftBehaviour.EngineEnabled == false) { return; }
            if (Input.GetMouseButtonDown(0) == true)
            {
                atTargetPosition = false;
                targetVisual.gameObject.SetActive(true);
                PerformClick(Input.mousePosition);
            }

            if (CheckDistance() == false) { atTargetPosition = true; }

            SteerToWaypoint();

            if (atTargetPosition == true) 
            {
                targetVisual.gameObject.SetActive(false);
                return; 
            }

            if (CheckDistance() == true && CheckToStop() == false)
            {
                rotorcraftBehaviour.ControlPitchForward();
            }
        }

        private void PerformClick(Vector3 mousePosition)
        {
            RaycastHit hit;
            var ray = _camera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;

                if (targetVisual == null) { return; }

                targetVisual.position = hit.point;
                targetVisual.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
        }

        private bool CheckDistance()
        {
            Vector3 correctForY = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            float distance = Vector3.Distance(transform.position, correctForY);

            return distance >= minDistanceToMove ? true : false;
        }

        private bool CheckToStop()
        {
            Vector3 correctForY = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            float distance = Vector3.Distance(transform.position, correctForY);
            
            return distance < minDistanceToMove * 2f ? true : false;
        }

        private void SteerToWaypoint()
        {
            Vector3 targetVector = targetPosition - transform.position;
            targetVector.y = transform.localPosition.y;

            Vector3 transformForwardPlane = transform.forward;
            transformForwardPlane.y = transform.localPosition.y;

            Vector3 cross = Vector3.Cross(transformForwardPlane, targetVector);            
            float angleBetween = Vector3.Angle(transformForwardPlane, targetVector);
            
            if (angleBetween <= minAngleToTurn) { return; }
            var newDirection = cross.y >= 0 ? 1 : -1;
            
            rotorcraftBehaviour.ControlYaw(newDirection);
        }
    }
}