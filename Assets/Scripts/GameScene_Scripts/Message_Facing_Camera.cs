using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message_Facing_Camera : MonoBehaviour
{
  

    void LateUpdate()
    {
        transform.forward = new Vector3 (Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z);
    }
}
