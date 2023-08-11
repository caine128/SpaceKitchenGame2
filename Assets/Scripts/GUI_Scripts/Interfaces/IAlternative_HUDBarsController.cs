using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAlternative_HUDBarsController
{
    //void PanelControllerConfig(float lerpDuration);
    //void PlaceBars(float lerpDuration);
    //void DisplaceBars(float lerpDuration);
    //public State GetState { get; }

    //[System.Flags]
    //enum State
    //{
    //    None=0,
    //    Alternative = 1,
    //    Default = 2,
    //    Running = 4,
    //    Opened = 8,
    //    Closed = 16,
    //}

    void ArrangeBarsInitial();
    void ArrangeBarsFinal();
}

