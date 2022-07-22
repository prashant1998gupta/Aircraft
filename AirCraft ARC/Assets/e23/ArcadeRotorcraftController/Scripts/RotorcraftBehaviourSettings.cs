using UnityEngine;

namespace e23.RotorcraftController
{
    [CreateAssetMenu(fileName = nameof(RotorcraftBehaviourSettings), menuName = "e23/ARC/Rotorcraft Settings", order = 3)]
    public class RotorcraftBehaviourSettings : ScriptableObject
    {
        [Header("Parameters")]
        [Tooltip("If this is false, the engine requires turning on/off manually. See ExampleInput.cs for example of how to do this.")]
        public bool engineAutoEnabled = false;
        public bool alwaysUseGravity = false;
        [Range(1.0f, 150.0f)] public float acceleration = 50f;
        [Range(5.0f, 750.0f)] public float maxSpeed = 100f;
        [Range(1.0f, 150.0f)] public float breakSpeed = 25f;
        [Tooltip("Boost speed is in addition to MaxSpeed. Using boost will result in maxSpeed + boostSpeed.")]
        [Range(5.0f, 200.0f)] public float boostSpeed = 60f;
        [Range(1.0f, 750.0f)] public float maxSpeedToStartReverse = 150f;
        [Range(1.0f, 750.0f)] public float maxRollSpeed = 50f;
        [Range(0.0f, 100.0f)] public float steering = 100f;
        [Range(0.0f, 100.0f)] public float gravity = 10f;
        [Tooltip("Throttle - how qickly to raise higher/ descend lower.")]
        [Range(1.0f, 100.0f)] public float throttleSpeed = 40f;
        [Tooltip("Pitch tilts on the X axis, for forwards/backwards movement.\nSet to 0 for no tilt. Higher values will result in a more visible tilt.")]
        [Range(0.0f, 150f)] public float pitch = 2.5f;
        [Tooltip("Yaw rotates on the Y axis.\nWhen stationary, the rotorcraft will not tilt, set this value for a tilt when moving.")]
        [Range(0.0f, 150.0f)] public float yaw = 0f;
        [Tooltip("Roll rotates on the Z axis.\nHow much to tilt by when rolling (strafing) left/right.")]
        [Range(0.0f, 100.0f)] public float roll = 50f;
        
        [Header("Ground Layer")]
        public LayerMask groundMask = 1 << 0;
    }
}