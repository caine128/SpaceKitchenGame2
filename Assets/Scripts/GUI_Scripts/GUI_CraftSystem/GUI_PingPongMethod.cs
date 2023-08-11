using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GUI_PingPongMethod<T_Sizeable,T_ValueToChange> : MonoBehaviour
{
    protected T_Sizeable sizeable;
    public bool CanPingPong
    {
        get => _canPingPong; 
        set => _canPingPong = value; 
    }
    protected bool _canPingPong = false;

    protected T_ValueToChange valueMaxDelta;
    protected T_ValueToChange originalValue;

    private void Awake()
    {
        sizeable = GetComponent<T_Sizeable>();
        originalValue = GetOriginalValue();
        valueMaxDelta = SetSizeMaxDelta();
    }

    private void Update()
    {
        if (_canPingPong)
        {
            SetDynamicValue();
        }
    }

    protected abstract T_ValueToChange GetOriginalValue();
    protected abstract T_ValueToChange SetSizeMaxDelta();
    protected abstract void SetDynamicValue();
    public abstract void ResetToOriginalValue();

}
