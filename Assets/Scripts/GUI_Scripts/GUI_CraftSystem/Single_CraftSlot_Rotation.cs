using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Single_CraftSlot_Rotation : MonoBehaviour
{

    [SerializeField] private RectTransform rect;
    [SerializeField] private RectTransform parent_Rect;
    [SerializeField] private GUI_LerpMethods_Rotation parent_RotationScript;
    [SerializeField] private Single_Craftslot single_Craftslot;

    //public bool isRotating = false;


    private void LateUpdate()
    {
        if(Time.frameCount % TimeTickSystem.UPDATEINTERVAL == 0)
        {
            if (parent_RotationScript.spintype == SpinType.Type.NoSpin)
            {
                SetMovingStatus(false);
                return;
            }
            else
            {
                SetMovingStatus(true);
                RotateSelf(Mathf.Abs(360 - parent_Rect.localEulerAngles.z));
            }
        }
    }

    private void RotateSelf(float angle)
    {
        rect.localEulerAngles = new Vector3(rect.localEulerAngles.x, rect.localEulerAngles.y, angle);
    }

    private void SetMovingStatus(bool isRotating)
    {
        if(single_Craftslot.containedItem != null)
        {
            single_Craftslot.containedItem.isMoving = isRotating;
        }
    }

}
