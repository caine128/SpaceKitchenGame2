using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFace : MonoBehaviour
{

    Camera cameraToFace;

    private void Start ()
    {
        cameraToFace = Camera.main;
    }

    private void LateUpdate ()
    {
        transform.LookAt (cameraToFace.transform);
        transform.rotation = Quaternion.LookRotation (cameraToFace.transform.forward);
    }

}
