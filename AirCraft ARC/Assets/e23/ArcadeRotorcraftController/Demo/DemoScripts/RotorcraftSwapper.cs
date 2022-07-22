using System;
using System.Collections.Generic;
using UnityEngine;

namespace e23.RotorcraftController.Demo
{
    public class RotorcraftSwapper : MonoBehaviour
    {
        public event Action onRotorcraftSwapped;

        [SerializeField] private List<SwapData> rotorcraftsToSwap = null;
        [SerializeField] private GameObject aiCamera = null;
        [SerializeField] private GameObject mobileControls = null;
        [SerializeField] private int startRotorcraft = 0;

        private int activeRotorcraft = 0;

        public RotorcraftBehaviour ActiveRotorcraft => rotorcraftsToSwap[activeRotorcraft].rotorcraftBehaviour;

        private void Awake() 
        {
            for (int i = 0; i < rotorcraftsToSwap.Count; i++)
            {
                Transform vehcileTransform = rotorcraftsToSwap[i].input.transform;
                rotorcraftsToSwap[i].ResetPos = vehcileTransform.position;
            }
        }

        private void Start() 
        {
            if (aiCamera != null) { aiCamera.SetActive(false); }
            
            for (int i = 0; i < rotorcraftsToSwap.Count; i++)
            {
                ToggleCameraType(rotorcraftsToSwap[i]);
            }
            
            SwapRotorcraft(startRotorcraft);
        }

        private void Update()
        {
           /* if (Input.GetKeyDown(KeyCode.C))
            {
                rotorcraftsToSwap[activeRotorcraft].isFollowCam = !rotorcraftsToSwap[activeRotorcraft].isFollowCam;
                ToggleCameraType(rotorcraftsToSwap[activeRotorcraft]);
            }*/

          /*  if (Input.GetKeyDown(KeyCode.T))
            {
                ToggleAI();
            }*/

           /* if (Input.GetKeyDown(KeyCode.R))
            {
                ResetRotorcraftPosition();
            }*/

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void SwapRotorcraft(int index)
        {
            for (int i = 0; i < rotorcraftsToSwap.Count; i++)
            {
                if (i == index)
                {
                    activeRotorcraft = index;
                    SetDataState(rotorcraftsToSwap[i], true);

                    onRotorcraftSwapped?.Invoke();
                }
                else
                {
                    SetDataState(rotorcraftsToSwap[i], false);
                }
            }

            if (mobileControls != null) { mobileControls.SetActive(true); }
        }

        public void ToggleAI()
        {
            aiCamera.SetActive(!aiCamera.activeSelf);

            if (mobileControls != null) { mobileControls.SetActive(!mobileControls.activeSelf); }
        }

        private void SetDataState(SwapData swapData, bool active)
        {
            swapData.cameraParent.SetActive(active);
            swapData.input.enabled = active;
        }

        private void ToggleCameraType(SwapData swapData)
        {
            if (swapData.isFollowCam == false)
            {
                swapData.topDownCamera.SetActive(true);
                swapData.followCamera.SetActive(false);
            }
            else
            {
                swapData.topDownCamera.SetActive(false);
                swapData.followCamera.SetActive(true);
            }
        }

        private void ResetRotorcraftPosition()
        {
            rotorcraftsToSwap[activeRotorcraft].rotorcraftBehaviour.SetPosition(rotorcraftsToSwap[activeRotorcraft].ResetPos, Quaternion.identity);
        }

        [System.Serializable]
        public class SwapData
        {
            public Examples.ExampleInput input;
            public bool isFollowCam = false;
            public GameObject cameraParent;
            public GameObject topDownCamera;
            public GameObject followCamera;

            private Vector3 startingPosition;
            public RotorcraftBehaviour rotorcraftBehaviour { get { return input.GetComponent<RotorcraftBehaviour>(); } }
            public Vector3 ResetPos { get { return startingPosition; } set { startingPosition = value; } }
        }
    }
}