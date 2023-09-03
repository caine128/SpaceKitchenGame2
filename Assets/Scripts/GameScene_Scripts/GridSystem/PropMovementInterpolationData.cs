using UnityEngine;
using UnityEngine.EventSystems;

public class PropMovementInterpolationData 
{
    public readonly PointerEventData pointerEventData;
    public readonly Vector3 raycastToGroundPosition;

    public PropMovementInterpolationData(PointerEventData pointerEventData, Vector3 raycastToGroundPosition)
    {
        this.pointerEventData = pointerEventData;
        this.raycastToGroundPosition = raycastToGroundPosition;
    }

}
