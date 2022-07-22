using System.Collections.Generic;
using UnityEngine;

public class E23CameraTrigger : MonoBehaviour
{
    [SerializeField] private GameObject vCamera = null;
    [SerializeField] private List<GameObject> vCameras = null;

    private void OnTriggerEnter(Collider other)
    {
        vCamera.SetActive(true);

        vCameras.ForEach(cam => cam.SetActive(false));
    }
}