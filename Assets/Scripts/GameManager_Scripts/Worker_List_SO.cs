using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WorkerList", menuName = "WorkerList")]
public class Worker_List_SO : ScriptableObject
{
    [SerializeField] public List<Worker_SO> listOfWorkers;
}
