using System.Collections;
using UnityEngine;

public abstract class GUI_LerpMethods : MonoBehaviour
{
    protected virtual float LerpDuration  => TimeTickSystem.PANEL_LERPDURATION;
 
    public IEnumerator RunningCoroutine
    {
        get { return runningCoroutine; }
        protected set { runningCoroutine = value; }
    }

    protected IEnumerator runningCoroutine = null;

    public abstract void PanelConfig();
 
}
