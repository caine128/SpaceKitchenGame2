using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjects : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate (0, 0.4f, 0);
    }
}
