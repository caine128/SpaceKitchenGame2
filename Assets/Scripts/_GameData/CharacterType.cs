using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterType
{
    public  enum AllType
    {
        DateMostRecent = Sort.Type.DateMostRecent,        //001

        Commander = 101,
        Taskforce_Fleet = 201,
        Worker = 301,
    }

    public enum Type
    {
        Commander = AllType.Commander,
        Taskforce_Fleet = AllType.Taskforce_Fleet,
        Worker = AllType.Worker,
    }
}
