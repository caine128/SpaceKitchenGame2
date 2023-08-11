using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static WorkerLevelProgression_SO;

public class WorkerManager : MonoBehaviour
{
    #region Singleton Syntax

    private static WorkerManager _instance;
    public static WorkerManager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    #endregion

    //[SerializeField] private Worker_List_SO workerList_SO;
    //public  WorkerLevelProgression_SO WorkerLevelProgressionChart => _workerLevelProgressionChart;
    //[SerializeField] private WorkerLevelProgression_SO _workerLevelProgressionChart;
    //public List<Worker> WorkersAvailable_List { get; private set; }

    private void Awake() // Later to take on config //
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if (Instance == null)
                {
                    _instance = this; // LATER TO ADD DONTDESTROY ON LOAD
                }
            }
        }

        //CreateWorkerList();
    }


    //private void CreateWorkerList(List<Worker> workersAvailable_List_IN = null)
    //{
    //   // WorkersAvailable_List = workersAvailable_List_IN ?? new List<Worker>(workerList_SO.listOfWorkers.Count);
    //}

    //public void AddNewWorker(Worker_SO worker_SO_In)
    //{
    //    foreach (var worker in WorkersAvailable_List)
    //    {
    //        if(worker.workerspecs == worker_SO_In)
    //        {
    //            Debug.LogError("this worker already exists");
    //            return;
    //        }
    //    }

    //    Worker newWorker = new Worker(worker_SO_In);
    //    WorkersAvailable_List.Add(newWorker);
    //}

    //public bool TryCalculateWorkerLevelUp(int workerCurrentLevel_IN, int workerCurrentXP_IN, out (int levelsAdded, int returnedXP)? levelUpInfo)
    //{
    //    if(workerCurrentLevel_IN < _workerLevelProgressionChart.progressionCharts.Length
    //           && workerCurrentXP_IN < _workerLevelProgressionChart.progressionCharts[workerCurrentLevel_IN].xpNeeded)
    //    {
    //        int amountOfLevelsToAdd = 0;

    //        while (workerCurrentLevel_IN + amountOfLevelsToAdd < _workerLevelProgressionChart.progressionCharts.Length
    //         && workerCurrentXP_IN < _workerLevelProgressionChart.progressionCharts[workerCurrentLevel_IN + amountOfLevelsToAdd].xpNeeded)
    //        {
    //            workerCurrentXP_IN -= _workerLevelProgressionChart.progressionCharts[workerCurrentLevel_IN + amountOfLevelsToAdd].xpNeeded;
    //            amountOfLevelsToAdd++;
    //        }

    //        levelUpInfo = (amountOfLevelsToAdd, workerCurrentXP_IN);
    //        return true;
    //    }

    //    else
    //    {
    //        levelUpInfo = null;
    //        return false;
    //    }    
    // }

}
