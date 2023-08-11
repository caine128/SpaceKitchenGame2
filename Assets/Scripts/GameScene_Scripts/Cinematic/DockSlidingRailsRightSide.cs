using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockSlidingRailsRightSide : MonoBehaviour
{

    IEnumerator Move ()
    {
        float elapsedTime = 0f;

        Vector3 parkedPos = transform.position;
        Vector3 closedPos = new Vector3 (-22, 0, -65.70525f);


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
