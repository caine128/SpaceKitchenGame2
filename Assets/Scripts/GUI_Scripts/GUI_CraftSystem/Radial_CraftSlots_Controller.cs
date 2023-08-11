using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radial_CraftSlots_Controller : MonoBehaviour, IConfigurablePanel
{
    #region Singleton Syntax
    private static Radial_CraftSlots_Controller _instance;
    public static Radial_CraftSlots_Controller Instance { get { return _instance; } }
    private static readonly object _lock = new object();
    #endregion

    [SerializeField] private float radius;   
    [SerializeField] private Single_Craftslot[] single_Craftslots;

    [SerializeField] private GUI_LerpMethods_Rotation parent_RotationScript;
    
    private const int TOTAL_CRAFTSLOTS = 12;
    private float startingAngle = 165f;
    private float offsetAngle;

    private IEnumerator runningCoroutine = null;
    private Queue<IEnumerator> queue = new Queue<IEnumerator>();
    public bool cr_Running { get; private set; } = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if (Instance == null)
                {
                    _instance = this;
                }
            }
        }
    }



    public void PanelConfig()
    {
        radius = CraftWheel_Controller.Radius;
        offsetAngle = 360f / TOTAL_CRAFTSLOTS;
        SetCraftSlot_Holders();
    }


    private void SetCraftSlot_Holders()
    {
        float angle = startingAngle;
        foreach (Single_Craftslot single_CraftSlot in single_Craftslots)
        {
            single_CraftSlot.GetComponent<RectTransform>().anchoredPosition = SetPositionFromAngle(angle);
            angle -= offsetAngle;
        }
    }

    public void SetRotation(SpinType.Type spintypeIN, float angleDelta = 0)
    {
        parent_RotationScript.SetSpinType(spintypeIN, angleDelta, angularDirectionIN: -1);

    }

    public void SetRotationTargeted(SpinType.Type spintypeIN, Single_Craftslot craftSlotIN)
    {
        int craftSlotInNo = 0;

        for (int i = single_Craftslots.Length -1; i > -1; i--)
        {
            if (single_Craftslots[i] == craftSlotIN)
            {
                craftSlotInNo = i;
                continue;
            }

            if (single_Craftslots[i].containedItem != null && single_Craftslots[i].containedItem.isVisible)
            {
                int craftSlotTargetNo = i;

                parent_RotationScript.SetSpinType(spintypeIN, angularVelocityIN: 0 ,angularDirectionIN: -1, targetSpinOffset : (craftSlotTargetNo - craftSlotInNo) * offsetAngle);

                return;
            }
        }
    }
   
 
    private Vector2 SetPositionFromAngle(float angle)
    {
        Vector2 pos = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        pos *= radius;
        return pos;
    }



    public void RearrangeCraftSlots(int slotNo, Single_CraftedItem itemHolderToAwait)
    {
       
        if(runningCoroutine == null && cr_Running == false)
        {
            runningCoroutine = RearrangeCraftSlotsRoutine(slotNo, itemHolderToAwait);
            StartCoroutine(runningCoroutine);
        }
        else
        {
            queue.Enqueue(RearrangeCraftSlotsRoutine(slotNo, itemHolderToAwait));
        }
    }

    IEnumerator RearrangeCraftSlotsRoutine(int slotNo, Single_CraftedItem itemHolderToAwait)
    {
        cr_Running = true;
        int lastSlotNo = 0;

        while (itemHolderToAwait!=null && itemHolderToAwait.cr_Running)
        {
            yield return null;
        }

        for (int i = slotNo+1 ; i < single_Craftslots.Length; i++)
        {
            if (single_Craftslots[i].containedItem != null)
            {
                Single_CraftedItem itemToMove = single_Craftslots[i].containedItem;
                single_Craftslots[i].RemoveItem();
                single_Craftslots[i - 1].DropItem(itemToMove,isSlerping:true);

                lastSlotNo = i;
                yield return null;
            }
        }
        while (single_Craftslots[lastSlotNo].cr_Running)
        {
            yield return null;
        }
        cr_Running = false;
        Dequeue();
    }

    private void Dequeue()
    {
        runningCoroutine = null;
        if (queue.Count > 0)
        {
            Debug.Log("dequeuing");
            runningCoroutine = queue.Dequeue();
            StartCoroutine(runningCoroutine);
        }
        else
        {
            SetRotation(SpinType.Type.BackSpin);
        }
    }
}
