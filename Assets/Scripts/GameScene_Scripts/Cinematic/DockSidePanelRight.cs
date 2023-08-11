using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockSidePanelRight : MonoBehaviour
{

    IEnumerator Move ()
    {
        float elapsedTime = 0f;

        //Vector3 parkedPos = new Vector3 (-25, 0, -74);
        //Vector3 closedPos = new Vector3 (-25, 0, -74);
        Quaternion parkedPosRot = Quaternion.Euler (-90, 0, 0);
        Quaternion closedPosRot = Quaternion.Euler (0, 0, 0);

        while (elapsedTime < 3f)
        {
            //transform.position = Vector3.Lerp (parkedPos, closedPos, (elapsedTime / 3f));
            transform.rotation = Quaternion.Lerp (parkedPosRot, closedPosRot, (elapsedTime / 3f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void Start ()
    {
        StartCoroutine (Move ());
    }
}


