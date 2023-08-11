using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPingPong : MonoBehaviour
{
    private new Renderer renderer;
    private Color originalColor;
    
    private void Awake ()
    {
        renderer = this.GetComponent<Renderer> ();
        originalColor = new Color (0.2627451f, 0.4509804f, 0.2117647f, .1f);
    }

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        Color intensityChange = new Color (0, 0, 0, (Mathf.PingPong (Time.unscaledTime / 10 , .25f)));

        renderer.material.SetColor ("_TintColor", originalColor + intensityChange);
        
    }
}
