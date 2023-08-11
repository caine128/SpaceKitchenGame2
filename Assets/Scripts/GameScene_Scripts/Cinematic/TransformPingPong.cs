using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPingPong : MonoBehaviour
{
    private new Transform transform;
    //private Transform originalTransform;

    private void Awake ()
    {
        transform = this.GetComponent<Transform> ();         
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3 (-8.278f ,1.27f + (Mathf.PingPong(Time.unscaledTime / 10, .7f)), -93.49f);
    }
}
