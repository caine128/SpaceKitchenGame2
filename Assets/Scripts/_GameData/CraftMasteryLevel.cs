using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftMasteryLevel 
{
    public enum Type
    {
        Beginner        =0,
        Novice      =1,
        Apprentice  =2,
        Journeyman  =3,
        Expert      =4,
        Master      =5,
    }

    public static Type GetNextCraftMasteryLevel(Type craftMasteryLevelIN)
    {
        switch (craftMasteryLevelIN)
        {
            case Type.Beginner:
                return Type.Novice;             
            case Type.Novice:
                return Type.Apprentice;             
            case Type.Apprentice:
                return Type.Journeyman;           
            case Type.Journeyman:
                return Type.Expert;               
            case Type.Expert:
                return Type.Master;              
            case Type.Master:
                return Type.Master;              
            default:
                return craftMasteryLevelIN;             
        }
    }

}
