using UnityEngine;

namespace e23.RotorcraftController
{
    [CreateAssetMenu(fileName = nameof(AudioData), menuName = "e23/ARC/Audio Data", order = 3)]
    public class AudioData : ScriptableObject
    {
        public AudioClip rotorStart;
        public AudioClip rotorIdle;
        public AudioClip rotorThrottle;
        public AudioClip rotorStop;
        public bool is3DAudio = true;
        public Vector2 minMaxDistance = new Vector2(10f, 500f);
    }
}