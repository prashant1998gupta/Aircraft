using UnityEngine;

namespace e23.RotorcraftController
{
    [RequireComponent(typeof(RotorcraftBehaviour))]
    public class RotorcraftGroundFollow : MonoBehaviour
    {
        [SerializeField] private float distanceFromGround = 10f;
        [Tooltip("How quickly the rotorcraft moves up/down when the ground height changes. A higher value will follow the ground controus faster, leading to a more 1:1 ground follow.")]
        [SerializeField] private float heightChangeSpeed = 2f;
        [Tooltip("If the rotorcraft manages to get higher than the distanceFromGround value, this multiplier is used to bring it back down quicker.")]
        [SerializeField] private float descentMultiplier = 3f;

        [Header("Ground Check")]
        [SerializeField] private Vector3 forwardRay = Vector3.zero;
        [SerializeField] private Vector3 rearRay = Vector3.zero;

        private RotorcraftBehaviour rotorcraftBehaviour;
        private Transform rotorcraftParent;
        
        private RaycastHit groundFindHit;
        private float rayLength;

        private LayerMask groundMask;

        public float Distance => distanceFromGround;

        private void Awake()
        {
            GetRequiredComponents();
        }

        private void GetRequiredComponents()
        {
            rotorcraftBehaviour = GetComponent<RotorcraftBehaviour>();
            groundMask = rotorcraftBehaviour.GroundMask;

            rotorcraftParent = GetComponentInParent<Rigidbody>().transform;

            rayLength = distanceFromGround * 2f;
        }

        private void FixedUpdate()
        {
            if (rotorcraftBehaviour.EngineEnabled == false)
            {
                return;
            }

            var velocityDirection = transform.InverseTransformDirection(rotorcraftBehaviour.RotorcraftRigidbody.velocity);
            float zOffset = 0;
            
            if (velocityDirection.z > 0.5f)
            {
                zOffset = Mathf.Lerp(zOffset, forwardRay.z, 1f);
            }
            else if (velocityDirection.z < -0.5f)
            {
                zOffset = Mathf.Lerp(zOffset, rearRay.z, 1f);
            }
            else
            {
                zOffset = Mathf.Lerp(zOffset, 0f, 0.25f);
            }

            Vector3 raycastOrigin = transform.TransformPoint(new Vector3(transform.localPosition.x, transform.localPosition.y + 0.5f, transform.localPosition.z + zOffset));
            
            Physics.Raycast(raycastOrigin, Vector3.down, out groundFindHit, rayLength, groundMask);
            Debug.DrawRay(raycastOrigin, Vector2.down, Color.red);

            Vector3 updatePos;

            if (groundFindHit.transform != null)
            {
                updatePos = new Vector3(rotorcraftParent.position.x, groundFindHit.point.y + distanceFromGround, rotorcraftParent.position.z);
            }
            else
            {
                updatePos = new Vector3(rotorcraftParent.position.x, rotorcraftParent.position.y - descentMultiplier, rotorcraftParent.position.z);
            }
            
            rotorcraftParent.position = Vector3.Slerp(rotorcraftParent.position, updatePos, Time.deltaTime * heightChangeSpeed);
        }
    }
}