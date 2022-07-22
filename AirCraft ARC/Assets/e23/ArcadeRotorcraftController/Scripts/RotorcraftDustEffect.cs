using UnityEngine;

namespace e23.RotorcraftController
{
    [RequireComponent(typeof(RotorcraftBehaviour))]
    public class RotorcraftDustEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem dustParticles;
        [SerializeField] private float groundDistance = 10f;

        private RotorcraftBehaviour rotorcraftBehaviour;
        private ParticleSystem.EmissionModule dustEmmision;
        private bool grounded = false;
        private LayerMask groundMask;
        private bool engineRunning = false;
        private RaycastHit hit;

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
            dustEmmision = dustParticles.emission;
            
            rotorcraftBehaviour = GetComponent<RotorcraftBehaviour>();
            groundMask = rotorcraftBehaviour.GroundMask;
        }

        private void RegisterActions(bool register)
        {
            rotorcraftBehaviour.onEngineEnabled -= EngineEnebaled;

            if (register == false) { return; }

            rotorcraftBehaviour.onEngineEnabled += EngineEnebaled;
        }

        private void FixedUpdate()
        {
            Vector3 raycastOrigin = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            grounded = Physics.Raycast(raycastOrigin, Vector3.down, out hit, groundDistance, groundMask);

            if (hit.transform != null)
            {
                Vector3 dustPos = dustEmmision.enabled == false ? hit.point : Vector3.Slerp(dustParticles.transform.position, hit.point, Time.deltaTime * 4f);
                dustParticles.transform.position = dustPos;
            }
        }

        private void LateUpdate()
        {
            if (engineRunning == false)
            {
                ToggleParticles(false);
                dustParticles.Stop();
                return;
            }

            ToggleParticles(grounded);
        }

        private void ToggleParticles(bool enable)
        {
            if (dustParticles.isPlaying == false && enable == true) { dustParticles.Play(); }

            dustEmmision.enabled = enable;
        }

        private void EngineEnebaled(bool enable)
        {
            engineRunning = enable;
        }

        public void AssignEffect(ParticleSystem ps)
        {
            dustParticles = ps;
        }
    } 
}