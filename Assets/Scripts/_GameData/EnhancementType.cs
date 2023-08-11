using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnhancementType 
{
    public enum AllType
    {
        DateMostRecent = Sort.Type.DateMostRecent,    //001
        Quantity = Sort.Type.Quantity,                //003
        Level =Sort.Type.Level,                       //005

        Runestone_Enhancement = 402,
        Elemental_Enhancement = 702,
        Spirit_Enhancement = 502,

    }

    public enum Type
    {
        Runestone_Enhancement=AllType.Runestone_Enhancement,
        Elemental_Enhancement=AllType.Elemental_Enhancement,
        Spirit_Enhancement=AllType.Spirit_Enhancement,
    }

}
