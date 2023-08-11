using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockSlidingPanels : MonoBehaviour
{

    IEnumerator Move ()
    {
        float elapsedTime = 0f;

        Vector3 parkedPos = transform.position;
        Vector3 closedPos = new Vector3 (-24.75f, 0, -69.50f);
    

        while (elapsedTime < 3f)
        {
            transform.position = Vector3.Lerp (parkedPos, closedPos, (elapsedTime / 3f));
         
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void Start ()
    {
        StartCoroutine (Move ());
    }

}
