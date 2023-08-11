using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "New Worker", menuName = "Worker")]
public class Worker_SO : Character_SO
{
    public WorkerType.Type workerType;

    public WorkStationPrerequisite[] workStationPrerequisites;
    public int goldCostForHire;
    public int gemCostForHire;
    public Recipes_SO[] unlockRecipes;
    public DialoguePhrases[] allDialoguePhrases;


    [Serializable] public struct WorkStationPrerequisite
    {
        public WorkstationType.Type type;
        public int level;
    }

    [Serializable] public struct DialoguePhrases
    {
        [TextArea]public string[] phrases;
    }
}
