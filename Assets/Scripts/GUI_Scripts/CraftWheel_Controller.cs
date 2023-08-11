using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CraftWheel_Controller : HUDBarsController //MonoBehaviour, IHUDBarsController
{
    public static float Radius { get; private set; }
    public static event Action<bool> isBarVisible;


    protected override void SetTargetPositions()
    {
        targetPositions = new Vector2[bars.Length];
        for (int i = 0; i < bars.Length; i++)
        {
            var rectOffirstChild = this.transform.GetChild(index: 0).GetComponent<RectTransform>();
            targetPositions[i] = new Vector2(bars[i].OriginalPos.x + MathF.Abs(rectOffirstChild.sizeDelta.x), bars[i].OriginalPos.y);
        }
    }

    public override sealed void PanelControllerConfig()
    {
        var scroller = GetComponentInChildren<Radial_CraftSlots_Scroller>();
        Radius =  scroller.GetComponent<RectTransform>().sizeDelta.x / 3f * 2.2f;

        foreach (IConfigurablePanel configurablePanel in GetComponentsInChildren<IConfigurablePanel>())
        {
            configurablePanel.PanelConfig();
        }

        base.PanelControllerConfig();
    }


    public override sealed void DisplaceBars(Action followingAction = null)
    {
        if (co != null)
        {
            StopCoroutine(co);
        }
        isBarVisible?.Invoke(false);

        bars[0].InitialCall(targetPositions[0]);
    }

    protected sealed override IEnumerator PlaceBarsRoutine(Action followingAction_IN)
    {
        bars[0].FinalCall();
        while(bars[0].RunningCoroutine != null)
        {
            yield return null;
        }

        isBarVisible?.Invoke(true);
        co = null;
    }

}
