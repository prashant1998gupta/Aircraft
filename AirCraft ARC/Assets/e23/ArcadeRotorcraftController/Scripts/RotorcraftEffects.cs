using UnityEngine;

namespace e23.RotorcraftController
{
    public class RotorcraftEffects : MonoBehaviour
    {
        [Tooltip("If true, the trail renderer(s) will always emit, even when the engine is off.")]
        [SerializeField] private bool alwaysTrail = false; 

        private RotorcraftBehaviour rotorcraftBehaviour;
        private ParticleSystem[] particleSystems;
        private TrailRenderer[] trailRenderers;
        private bool shouldEmmit = false;

        private void Awake()
        {
            GetRequiredComponents();
        }

        private void Update()
        {
            Effects();
        }

        private void LateUpdate()
        {
            UpdateEmitting();
        }

        private void GetRequiredComponents()
        {
            rotorcraftBehaviour = GetComponent<RotorcraftBehaviour>();
            particleSystems = GetComponentsInChildren<ParticleSystem>();
            trailRenderers = GetComponentsInChildren<TrailRenderer>();
        }

        private void Effects()
        {
            Particles();

            for (int i = 0; i < trailRenderers.Length; i++)
            {
                Trail(trailRenderers[i]);
            }
        }

        private void UpdateEmitting()
        {
            shouldEmmit = rotorcraftBehaviour.EngineEnabled || alwaysTrail;
        }

        private void Particles()
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                ParticleSystem.EmissionModule emission = particleSystems[i].emission;
                emission.enabled = shouldEmmit;
            }
        }

        private void Trail(TrailRenderer trail)
        {
            trail.emitting = shouldEmmit;
        }
    }
}