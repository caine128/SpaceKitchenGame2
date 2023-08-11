using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ships : MonoBehaviour
{
    
    

    IEnumerator Move ()
    {
        yield return new WaitForSeconds (4f);

        float elapsedTime = 0f;        

        Vector3 parkedPos = new Vector3 (-39, 1, -67);                
        Vector3 onAirPos = new Vector3 (-39, 5, -67);
        Quaternion parkedPosRot = Quaternion.Euler (0, -60, 0);
        Quaternion onAirPosRot = Quaternion.Euler (0, -90, 0);

        while (elapsedTime < 3f)
        {
            transform.position = Vector3.Lerp (parkedPos, onAirPos, (elapsedTime / 3f));
            transform.rotation = Quaternion.Lerp (parkedPosRot, onAirPosRot, (elapsedTime / 3f));
            elapsedTime += Time.deltaTime;            
            yield return null;
        }
        transform.position = onAirPos;
        transform.rotation = onAirPosRot;

        yield return new WaitForSeconds (3f);

        elapsedTime = 0f;
        while (elapsedTime < 3f)
        {
            transform.position = Vector3.Lerp (onAirPos, new Vector3(-90, 5, -67) , (elapsedTime / 3f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
    }

    private void Start ()
    {
        StartCoroutine (Move());
    }

}
