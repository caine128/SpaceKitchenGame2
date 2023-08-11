using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinType : MonoBehaviour
{
    public enum Type
    {
        FreeSpin,
        ControlledSpin,
        NoSpin,
        BackSpin,
        TargetedSpin,
    }

    //public enum SpiningObject
    //{
    //    Parent,
    //    Child,
    //}

    public enum Direction
    {
        Positive,
        Negative,
        None,
    }

    public enum Restriction
    {
        Clamped,
        Non_Clamped,
    }
}
