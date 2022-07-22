using UnityEngine;

namespace e23.RotorcraftController.Examples
{
	public class ExampleInput : MonoBehaviour
	{
		[SerializeField] private RotorcraftBehaviour rotorcraftBehaviour;

		[Header("Controls")]
		[SerializeField] private KeyCode enableEngine = KeyCode.G;
		[SerializeField] private KeyCode engageThrottle = KeyCode.F;
		[SerializeField] private KeyCode disengageThrottle = KeyCode.V;
		[SerializeField] private KeyCode accelerate = KeyCode.W;
		[SerializeField] private KeyCode brake = KeyCode.S;
		[SerializeField] private KeyCode steerLeft = KeyCode.A;
		[SerializeField] private KeyCode steerRight = KeyCode.D;
		[SerializeField] private KeyCode strafeLeft = KeyCode.Q;
		[SerializeField] private KeyCode strafeRight = KeyCode.E;
		[SerializeField] private KeyCode boost = KeyCode.Space;
		[SerializeField] private KeyCode oneShotBoost = KeyCode.B;

		[Header("Settings")]
		[SerializeField] private float boostLength = 1f;
		
		public RotorcraftBehaviour RotorcraftBehaviour { 
			get { return rotorcraftBehaviour; } 
			set { rotorcraftBehaviour = value; } 
		}

		private void Update()
		{
			// Enable engine
			if (Input.GetKeyDown(enableEngine)) { RotorcraftBehaviour.ToggleRotors(); }

			// Throttle (raise/ lower Rotorcraft)
			if (Input.GetKey(engageThrottle)) { RotorcraftBehaviour.Throttle(1f); }
			if (Input.GetKey(disengageThrottle)) { RotorcraftBehaviour.Throttle(-1f); }

			// Acceleration
			if (Input.GetKey(accelerate)) { RotorcraftBehaviour.ControlPitchForward(); }
			if (Input.GetKey(brake)) { RotorcraftBehaviour.ControlPitchBackwards(); }

			// Steering
			if (Input.GetKey(steerLeft)) { RotorcraftBehaviour.ControlYaw(-1f); }
			if (Input.GetKey(steerRight)) { RotorcraftBehaviour.ControlYaw(1f); }

			// Strafing
			if (Input.GetKey(strafeLeft)) { RotorcraftBehaviour.ControlRoll(-1f); }
			if (Input.GetKey(strafeRight)) { RotorcraftBehaviour.ControlRoll(1f); }

			// Boost
			if (Input.GetKeyDown(boost)) { RotorcraftBehaviour.Boost(); }
			if (Input.GetKeyUp(boost)) { RotorcraftBehaviour.StopBoost(); }
			if (Input.GetKey(oneShotBoost)) { RotorcraftBehaviour.OneShotBoost(boostLength); }
		}
	}
}