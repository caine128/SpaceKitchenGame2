using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottom_Bars_Controller : HUDBarsController, IAlternative_HUDBarsController  // don not forget to DÝSABLE ENABLE THE ALTERNATE BARS 
{

    [SerializeField] protected GUI_LerpMethods_Movement[] alternativeBars_LerpScripts;
    private IEnumerator co_alternate = null;


    public sealed override void PanelControllerConfig()
    {
        base.PanelControllerConfig();

        foreach (var bar in alternativeBars_LerpScripts)
        {
            bar.gameObject.SetActive(false);
        }
    }
    protected override void SetTargetPositions()
    {
        targetPositions = new Vector2[bars.Length];
        for (int i = 0; i < bars.Length; i++)
        {
            targetPositions[i] = new Vector2(bars[i].OriginalPos.x - MathF.Abs(bars[i].Rect.sizeDelta.x), bars[i].OriginalPos.y);
        }
    }


    public void ArrangeBarsInitial()
    {

        if (co_alternate != null)
        {
            StopCoroutine(co_alternate);
        }

        Action followingAction = () =>
        {
            co_alternate = ArrangeBarsInitialRoutine();
            StartCoroutine(co_alternate);
        };

        DisplaceBars(followingAction);
    }

    IEnumerator ArrangeBarsInitialRoutine()
    {

        for (int i = 0; i < alternativeBars_LerpScripts.Length; i++)
        {
            alternativeBars_LerpScripts[i].gameObject.SetActive(true);
        
            alternativeBars_LerpScripts[i].InitialCall(Vector2.zero);

            yield return TimeTickSystem.WaitForSeconds_HUDBarsMovement;
        }
    }

    public void ArrangeBarsFinal()
    {

        if (co_alternate != null)
        {
            StopCoroutine(co_alternate);
        }

        //Action followingAction = () => PlaceBars();

        co_alternate = ArrangeBarsFinalRoutine(followingAction_IN: ()=> PlaceBars());
        StartCoroutine(co_alternate);
    }

    IEnumerator ArrangeBarsFinalRoutine(Action followingAction_IN)
    {
        for (int i = alternativeBars_LerpScripts.Length - 1; i > -1; i--)
        {
            if (i != 0 && alternativeBars_LerpScripts[i].gameObject.activeSelf == true)
            {
                alternativeBars_LerpScripts[i].FinalCall(deactivateSelf: true);
            }
            if (i == 0)
            {
                if (alternativeBars_LerpScripts[i].gameObject.activeSelf == true)
                {
                    alternativeBars_LerpScripts[i].FinalCall(deactivateSelf: true, followingAction: followingAction_IN);
                }
                else
                {
                    followingAction_IN();
                }
            }
        }


        for (int i = 0; i < alternativeBars_LerpScripts.Length; i++)
        {
            if (alternativeBars_LerpScripts[i].gameObject.activeSelf == true)
            {
                alternativeBars_LerpScripts[i].FinalCall(deactivateSelf: true);
            }

            yield return TimeTickSystem.WaitForSeconds_HUDBarsMovement;
        }

        co_alternate = null;

    }
}
