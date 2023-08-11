using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AscensionLevel 
{
    public enum Type
    {
        None = 0,
        Initiate = 1,
        Divine = 2,
        Avatar = 3,
    }

    public static Type GetNextAscensionLevel(Type ascensionLevel_IN)
    {
        switch (ascensionLevel_IN)
        {
            case Type.None:
                return Type.Initiate;
      
            case Type.Initiate:
                return Type.Divine;
         
            case Type.Divine:
                return Type.Avatar;
      
            case Type.Avatar:
                return Type.Avatar;

            default:
                return ascensionLevel_IN;
     
        }
    }
}
