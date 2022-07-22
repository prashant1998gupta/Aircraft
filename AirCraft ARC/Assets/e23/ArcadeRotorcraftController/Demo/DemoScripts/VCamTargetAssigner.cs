using UnityEngine;
using Cinemachine;

public class VCamTargetAssigner : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private bool follow = true;
    [SerializeField] private bool lookAt = true;

    private CinemachineVirtualCamera virtualCamera;

    private void Start() 
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        
        if (target != null)
        {
            if (follow == true) { virtualCamera.Follow = target; }
            if (lookAt == true) { virtualCamera.LookAt = target; }
        }
    }
}