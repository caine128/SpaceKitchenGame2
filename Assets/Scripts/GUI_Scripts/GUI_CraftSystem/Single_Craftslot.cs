using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Single_Craftslot : MonoBehaviour
{
    public Single_CraftedItem containedItem { get; private set; } 
    [SerializeField] private Transform craftSlots_TempHolder;
    public bool cr_Running { get; private set; } = false;
    [SerializeField] private AnimationCurve easeCurve;
    [SerializeField] private float lerpDuration = .2f;

    public void DropItem(Single_CraftedItem craftedItem, bool isSlerping = false)
    {
        containedItem = craftedItem;

        RectTransform rtCraftedItem = craftedItem.GetComponent<RectTransform>();        

        if (isSlerping )
        {
            rtCraftedItem.SetParent(craftSlots_TempHolder);
            StartCoroutine(PlaceItemRoutine(craftedItem, rtCraftedItem, rtCraftedItem.position));
        }
        else
        {
            rtCraftedItem.SetParent(gameObject.transform);
            rtCraftedItem.anchoredPosition = Vector2.zero;
            rtCraftedItem.localEulerAngles = Vector3.zero;
            rtCraftedItem.SetAsLastSibling();
        }   
    }

    IEnumerator PlaceItemRoutine(Single_CraftedItem craftedItem, RectTransform rtCraftedItem, Vector2 startPosition, float lerpSpeedmodifier = 1)
    {
        cr_Running = true;
        craftedItem.isMoving = true;

        float elapsedTime = 0f;
        while (elapsedTime < lerpDuration*lerpSpeedmodifier)
        {
            float easeFactor = elapsedTime / (lerpDuration * lerpSpeedmodifier);
            easeFactor = easeCurve.Evaluate(easeFactor);

            craftedItem.transform.position = Vector3.SlerpUnclamped(startPosition, transform.TransformPoint(Vector2.zero), easeFactor);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        craftedItem.isMoving = false;
        rtCraftedItem.SetParent(gameObject.transform);
        rtCraftedItem.anchoredPosition = Vector2.zero;
        rtCraftedItem.localEulerAngles = Vector3.zero;
        rtCraftedItem.SetAsLastSibling();

        cr_Running = false;
    }


    public void RemoveItem()
    {
        containedItem = null;
    }
}
