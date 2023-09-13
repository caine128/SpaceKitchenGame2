using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TabPanel<T_TabType> : MonoBehaviour
    where T_TabType : System.Enum
{
    public abstract T_TabType TabType { get; }
    public abstract void LoadInfo();
    public abstract void UnloadInfo();

}







public abstract class TabPanel_Animated<T_TabType> : TabPanel<T_TabType>, IAnimatedPanelController_ManualHide
    where T_TabType : System.Enum
{

    public IEnumerator[] CO
    {
        get { return co; }
    }
    protected IEnumerator[] co = null;

    public GUI_LerpMethods PanelToAwait
    {
        get { return panelToAwait; }
    }
    [SerializeField] private GUI_LerpMethods panelToAwait;

    public abstract void DisplayContainers();
    public abstract void HideContainers();
    

    public override void UnloadInfo()
    {
        for (int i = 0; i < co.Length; i++)
        {
            if (co[i] != null)
            {
                StopCoroutine(co[i]);
                co[i] = null;
            }
        }
    }

}
