using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomerMove : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer exclamationMark;
    private void Start ()
    {
        StartCoroutine (Move ());
    }


    IEnumerator Move ()
    {
        yield return new WaitForSeconds (3f);

        float elapsedTime = 0f;
        Vector3 initialPosition = new Vector3 (-22.5f, 1, -67.5f);
        Vector3 firstStop = new Vector3 (-11, 1, -67.5f);

        while (elapsedTime < 5f)
        {
            transform.position = Vector3.Lerp (initialPosition, firstStop, (elapsedTime / 5f));
            elapsedTime += Time.deltaTime;            
            yield return null;
        }

        transform.position = firstStop;
        

        yield return new WaitForSeconds (.3f);

        

        elapsedTime = 0f;
        Vector3 secondStop = new Vector3 (-11, 1, -91);
        

        while (elapsedTime < 5f)
        {
            transform.position = Vector3.Lerp (firstStop, secondStop, (elapsedTime / 5f));
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        transform.position = secondStop;

        yield return new WaitForSeconds (.3f);        

        elapsedTime = 0f;
        Vector3 thirdStop = new Vector3 (0, 1, -91);


        while (elapsedTime < 5f)
        {
            transform.position = Vector3.Lerp (secondStop, thirdStop, (elapsedTime / 5f));
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        transform.position = thirdStop;
        exclamationMark.gameObject.SetActive (true);
    }

}
