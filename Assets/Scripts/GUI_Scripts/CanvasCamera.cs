using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCamera : MonoBehaviour
{

    private void OnEnable()
    {
        FindObjectOfType<SceneController>().OnSceneLoaded += SelfDestruct;
    }

    private void OnDisable()
    {
        SceneController scenecontroller = FindObjectOfType<SceneController>();
        if (scenecontroller)
        {
            scenecontroller.OnSceneLoaded -= SelfDestruct;
        }
    }

    private void SelfDestruct(object sender, SceneController.OnSceneLoadedEventArgs e)
    {
        Destroy(this.gameObject);
    }
}
