using System.Collections;
using UnityEngine;

namespace e23.RotorcraftController
{
    [RequireComponent(typeof(RotorcraftBehaviour))]
    [RequireComponent(typeof(AudioSource))]
    public class RotorcraftAudio : MonoBehaviour
    {
        [Tooltip("Audio data can be created via right mouse clicking in the Project Window -> Create -> e23 -> ARC -> Audio Data")]
        [SerializeField] private AudioData audioData = null;

        private RotorcraftBehaviour rotorcraftBehaviour;
        private AudioSource audioSource;
        private Coroutine playAfterIdleCoroutine;

        private void Awake()
        {
            GetRequireComponents();
        }

        private void OnEnable()
        {
            RegisterActions(true);
        }

        private void OnDestroy()
        {
            RegisterActions(false);
        }

        private void GetRequireComponents()
        {
            rotorcraftBehaviour = GetComponent<RotorcraftBehaviour>();
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;

            if (audioData == null) 
            { 
                Debug.LogWarning($"No AudioData assigned to {gameObject.name}", gameObject); 
                return; 
            }

            float spatial = audioData.is3DAudio == true ? 1f : 0f;
            audioSource.spatialBlend = spatial;
            audioSource.minDistance = audioData.minMaxDistance.x;
            audioSource.maxDistance = audioData.minMaxDistance.y;
        }

        private void RegisterActions(bool register)
        {
            rotorcraftBehaviour.onEngineEnabled -= PlaySound;

            if (register == false) { return; }

            rotorcraftBehaviour.onEngineEnabled += PlaySound;
        }

        private void PlaySound(bool engineEnabled)
        {
            if (audioData == null) 
            { 
                DisableComponent("No audio data has been assigned");
                return; 
            }

            if (audioData.rotorStart == null) 
            {
                DisableComponent("Rotor start audio clip has not been assigned to the Audio Data. Disabling component.");
                return; 
            }

            if (audioData.rotorStop == null) 
            {
                DisableComponent("Rotor stop audio clip has not been assigned to the Audio Data. Disabling component.");
                return; 
            }

            if (playAfterIdleCoroutine != null) { StopCoroutine(playAfterIdleCoroutine); }

            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = engineEnabled == true ? audioData.rotorStart : audioData.rotorStop;            
            audioSource.Play();

            if (engineEnabled == true)
            {
                playAfterIdleCoroutine = StartCoroutine(PlayIdleAfterStart(audioSource.clip.length));
            }

            void DisableComponent(string message)
            {
                Debug.LogWarning(message, gameObject);
                RegisterActions(false);
                this.enabled = false;
            }
        }
                
        private IEnumerator PlayIdleAfterStart(float wait)
        {
            yield return new WaitForSeconds(wait);

            audioSource.clip = audioData.rotorIdle;
            audioSource.loop = true;

            audioSource.Play();
        }

        public void SetAudioData(AudioData data)
        {
            audioData = data;
        }
    }
}