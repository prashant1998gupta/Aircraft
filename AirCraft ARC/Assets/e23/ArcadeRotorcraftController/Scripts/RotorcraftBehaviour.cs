using System;
using System.Collections;
using UnityEngine;

namespace e23.RotorcraftController
{
    public class RotorcraftBehaviour : MonoBehaviour
    {
        public event Action<bool> onEngineEnabled;

        [Header("Components")]
        [Tooltip("Parent for the rotorcraft model.")]
        [SerializeField] private Transform _rotorcraftModel;
        [Tooltip("Assign the GameObject containing the Rrigidbody component. TIP: Use the Rotorcraft Builder window to have this auto assigned when creating a rotorcraft.")]
        [SerializeField] private Rigidbody _rotorcraftRigidbody;

        [Header("Settings")]
        [Tooltip("Create and assign a Rotorcraft Settings ScriptableObject, this object holds the rotorcraft data (Acceleration, MaxSpeed, etc). TIP: Clicking the button below, in play mode, allows you to tweak and test values at runtime.")]
        [SerializeField] private RotorcraftBehaviourSettings _rotorcraftSettings;

        private Transform container;

        private float speed, speedTarget;
        private float rotate, rollTilt;
        private float xRotation, zRotation, pitchTilt, yawTilt;
        private float rollSpeed, rollTargetSpeed, strafeTilt;
        private float throttleSpeed, throttleTarget;
        private float rayMaxDistance;

        private bool _engineEnabled, currentGroundCheck;

        private Vector3 localVelocity;

        public Transform RotorcraftModel { get { return _rotorcraftModel; } set { _rotorcraftModel = value; } }
        public Rigidbody RotorcraftRigidbody { get { return _rotorcraftRigidbody; } set { _rotorcraftRigidbody = value; } }
        public RotorcraftBehaviourSettings RotorcraftSettings { get { return _rotorcraftSettings; } set { _rotorcraftSettings = value; } }

        public float Acceleration => RotorcraftSettings.acceleration;
        public float DefaultMaxSpeed => RotorcraftSettings.maxSpeed;
        public float MaxSpeed { get; set; }
        public float BreakSpeed => RotorcraftSettings.breakSpeed;
        public float BoostSpeed => RotorcraftSettings.boostSpeed;
        public float MaxSpeedToStartReverse => RotorcraftSettings.maxSpeedToStartReverse;
        public float DefaultSteering => RotorcraftSettings.steering;
        public float Steering { get; set; }
        public float MaxRollSpeed => RotorcraftSettings.maxRollSpeed;
        public float Gravity => RotorcraftSettings.gravity;
        public float ThrottleSpeed => RotorcraftSettings.throttleSpeed;
        public float Pitch => RotorcraftSettings.pitch;
        public float Yaw => RotorcraftSettings.yaw;
        public float Roll => RotorcraftSettings.roll;
        public float RotateTarget { get; private set; }
        public bool NearGround { get; private set; }
        public bool OnGround { get; private set; }
        public LayerMask GroundMask => RotorcraftSettings.groundMask;

        public bool EngineEnabled => _engineEnabled;
        public bool IsBoosting { get; private set; }
        public float GetVelocitySqrMagnitude => RotorcraftRigidbody.velocity.sqrMagnitude;
        public Vector3 GetVelocity => RotorcraftRigidbody.velocity;

        private void Awake()
        {
            GetRequiredComponents();
            SetRotorcraftSettings();
        }

        private void Start()
        {
            if (RotorcraftSettings.engineAutoEnabled == false) { return; }

            EnableEngine(RotorcraftSettings.engineAutoEnabled);
        }

        private void GetRequiredComponents()
        {
            container = RotorcraftModel;
        }

        public void SetRotorcraftSettings()
        {
            if (RotorcraftSettings == null)
            {
                Debug.LogError("Rotorcraft is missing RotorcraftSettings asset.", gameObject);
                return;
            }

            MaxSpeed = DefaultMaxSpeed;
            Steering = DefaultSteering;

            RotorcraftRigidbody.useGravity = RotorcraftSettings.alwaysUseGravity;
            RotorcraftRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            rayMaxDistance = Vector3.Distance(transform.position, RotorcraftModel.position) + 0.5f;
        }

        private void Update()
        {
            if (_engineEnabled == false) { return; }

            Raise();
            Accelerate();
            CalculateRollSpeed();
        }

        private void OnDrawGizmos()
        {
            //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Vector3.down, Color.red);
        }

        private void FixedUpdate()
        {
            if (_engineEnabled == true)
            {
                Turn();
                BodyTiltOnMovement();
                RollCraft();
            }
            else
            {
                GroundRotorcraft();
            }

            RaycastHit hitNear;
            RaycastHit hitOn;

            Vector3 raycastOrigin = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            OnGround = Physics.Raycast(raycastOrigin, Vector3.down, out hitOn, rayMaxDistance, GroundMask);
            NearGround = Physics.Raycast(raycastOrigin, Vector3.down, out hitNear, rayMaxDistance + 1f, GroundMask);

            ToggleConstraints();

            RotorcraftModel.up = Vector3.Lerp(RotorcraftModel.up, hitNear.normal, Time.deltaTime * 8.0f);
            RotorcraftModel.Rotate(0, transform.eulerAngles.y, 0);

            if (_engineEnabled == true)
            {
                RotorcraftRigidbody.AddForce(transform.forward * speedTarget, ForceMode.Acceleration);
                RotorcraftRigidbody.AddForce(transform.right * rollTargetSpeed, ForceMode.Acceleration);
                RotorcraftRigidbody.AddForce(transform.up * throttleTarget, ForceMode.Acceleration);
            }

            if (OnGround == true && _engineEnabled == false)
            {
                ResetTargets();
            }

            localVelocity = transform.InverseTransformVector(RotorcraftRigidbody.velocity);

            if (Math.Abs(speedTarget) <= 0.5f)
            {
                speedTarget = 0;
                speed = 0;
            }
        }

        private void ToggleConstraints()
        {
            RotorcraftRigidbody.constraints = (EngineEnabled == false || OnGround == true) ? ~RigidbodyConstraints.FreezeAll : RigidbodyConstraints.FreezeRotation;
        }

        private void ResetTargets()
        {
            speedTarget = 0f;
            rollTargetSpeed = 0f;
            throttleTarget = 0f;
            rotate = 0f;
            RotateTarget = 0f;

            RotorcraftRigidbody.velocity = Vector3.zero;
            RotorcraftRigidbody.angularVelocity = Vector3.zero;
        }

        private void EnableEngine(bool active)
        {
            _engineEnabled = active;
            onEngineEnabled?.Invoke(active);

            ToggleGravity();
        }

        private void ToggleGravity()
        {
            if (RotorcraftSettings.alwaysUseGravity == true) { return; }

            RotorcraftRigidbody.useGravity = !_engineEnabled;
        }

        private void Raise()
        {
            throttleTarget = Mathf.SmoothStep(throttleTarget, throttleSpeed, Time.deltaTime * Acceleration);
            throttleSpeed = 0;
        }

        private void Accelerate()
        {
            speedTarget = Mathf.SmoothStep(speedTarget, speed, Time.deltaTime * Acceleration);
            speedTarget = Mathf.Round(speedTarget * 100f) / 100f;
            speed = 0f;
        }

        private void Turn()
        {
            RotateTarget = Mathf.Lerp(RotateTarget, rotate, Time.deltaTime * 4f);

            float yRotation = speedTarget < 0 ? RotorcraftRigidbody.transform.eulerAngles.y - RotateTarget : RotorcraftRigidbody.transform.eulerAngles.y + RotateTarget;

            RotorcraftRigidbody.transform.rotation = Quaternion.Slerp(RotorcraftRigidbody.transform.rotation, Quaternion.Euler(new Vector3(0, yRotation, 0)), Time.deltaTime * 2.0f);
        }

        private void CalculateRollSpeed()
        {
            rollTargetSpeed = Mathf.SmoothStep(rollTargetSpeed, rollSpeed, Time.deltaTime * Acceleration);
            rollSpeed = 0;
        }

        private void RollCraft()
        {
            rollTilt = Mathf.Lerp(rollTilt, strafeTilt, Time.deltaTime * 4f);
            strafeTilt = 0;

            container.localRotation = Quaternion.Slerp(container.localRotation, Quaternion.Euler(0, RotateTarget / 8, -rollTilt), Time.deltaTime * 10f);
        }
                
        private void BodyTiltOnMovement()
        {
            xRotation = Mathf.Lerp(xRotation, pitchTilt, Time.deltaTime * 4f);

            if (GetVelocitySqrMagnitude > 0 && rotate != 0 && Mathf.Abs(localVelocity.z) > 0.1f)
            {
                zRotation = Mathf.Lerp(zRotation, yawTilt, Time.deltaTime * 4f);
            }
            else
            {
                yawTilt = 0f;
                zRotation = Mathf.Lerp(zRotation, yawTilt, Time.deltaTime * 4f);
            }

            pitchTilt = 0f;
            yawTilt = 0f;
            rotate = 0f;

            _rotorcraftModel.localRotation = Quaternion.Slerp(_rotorcraftModel.localRotation, Quaternion.Euler(new Vector3(0, 0, zRotation)), Time.deltaTime * 4f);
            container.localRotation = Quaternion.Slerp(container.localRotation, Quaternion.Euler(new Vector3(xRotation, 0, 0)), Time.deltaTime * 10f);
        }

        private void GroundRotorcraft()
        {
            if (_engineEnabled == false && OnGround == false)
            {
                RotorcraftRigidbody.velocity += Vector3.up * Physics.gravity.y * Gravity * Time.deltaTime;
            }
        }

        /// <summary>
        /// Change the MaxSpeed of the Rotorcraft. Use DefaultMaxSpeed to return to the original MaxSpeed
        /// </summary>
        /// <param name="speedPenalty"></param>
        public void MovementPenalty(float speedPenalty)
        {
            MaxSpeed = speedPenalty;
        }

        /// <summary>
        /// Change the Steering speed of the Rotorcraft. Use DefaultSteering to return to the original Steering
        /// </summary>
        /// <param name="steerPenalty"></param>
        public void SteeringPenalty(float steerPenalty)
        {
            Steering = steerPenalty;
        }

        // Input controls	

        /// <summary>
        /// Turn the Rotorcraft engine on/off
        /// </summary>
        public void ToggleRotors()
        {
            EnableEngine(!_engineEnabled);
        }

        /// <summary>
        /// Turn the engine on and start the rotor blades spinning
        /// </summary>
        public void EnableRotors()
        {
            EnableEngine(true);
        }
        
        /// <summary>
        ///  Turn the engine off and stop the rotor blades spinning
        /// </summary>
        public void DisableRotors()
        {
            EnableEngine(false);
        }

        /// <summary>
        /// Raise (float 1) or lower (float -1) the Rotorcraft
        /// </summary>
        /// <param name="direction"></param>
        public void Throttle(float direction)
        {
            throttleSpeed = ThrottleSpeed * (direction * 2);
        }

        /// <summary>
        /// Move the Rotorcraft foward
        /// </summary>
        public void ControlPitchForward()
        {
            if (!IsBoosting) 
            { 
                speed = MaxSpeed;
            }
            else 
            {
                speed = MaxSpeed + BoostSpeed; 
            }
            
            pitchTilt = Pitch;
        }

        /// <summary>
        /// Slow down and reverse
        /// </summary>
        public void ControlPitchBackwards()
        {
            if (GetVelocitySqrMagnitude > MaxSpeedToStartReverse && localVelocity.z > 0)
            {
                speed -= BreakSpeed;
            }
            else
            {
                speed = -MaxSpeed;
            }

            pitchTilt = -Pitch;
        }

        /// <summary>
        /// Turn left (float -1) or right (float 1). 
        /// </summary>
        /// <param name="direction"></param>
        public void ControlYaw(float direction)
        {
            if (OnGround == false)
            {
                rotate = Steering * (direction * 1.5f);
                yawTilt = Yaw * -direction;
            }
        }

        /// <summary>
        /// Move sideways, left (float -1) or right (float 1)
        /// </summary>
        /// <param name="direction"></param>
        public void ControlRoll(float direction)
        {
            rollSpeed = MaxRollSpeed * (direction * 2);
            strafeTilt = Roll * direction;
        }

        /// <summary>
        /// Sets isBoosting to true. Set your boost speed in the RotorcraftSettings asset
        /// </summary>
        public void Boost()
        {
            IsBoosting = true;
        }

        /// <summary>
        /// Performs a timed boost, pass in a float for how long the boost should last in seconds
        /// </summary>
        /// <param name="boostLength"></param>
        public void OneShotBoost(float boostLength)
        {
            if (IsBoosting == false)
            {
                StartCoroutine(BoostTimer(boostLength));
            }
        }

        private IEnumerator BoostTimer(float boostLength)
        {
            Boost();

            yield return new WaitForSeconds(boostLength);

            StopBoost();
        }

        /// <summary>
        /// Sets isBoosting to false
        /// </summary>
        public void StopBoost()
        {
            IsBoosting = false;
        }

        /// <summary>
        /// Set the position and rotation of the Rotorcraft. This will also set the speed and turning to 0
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SetPosition(Vector3 position, Quaternion rotation)
        {
            RotorcraftRigidbody.velocity = Vector3.zero;
            RotorcraftRigidbody.angularVelocity = Vector3.zero;
            RotorcraftRigidbody.position = position;

            speed = speedTarget = rotate = 0.0f;

            RotorcraftRigidbody.Sleep();
            transform.SetPositionAndRotation(position, rotation);
            RotorcraftRigidbody.WakeUp();
        }
    }
}