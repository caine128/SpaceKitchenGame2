using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Worker Level Progression Chart", menuName = "Worker Level Progression Chart")]
public class WorkerLevelProgression_SO : ScriptableObject
{
    public ProgressionChart[] progressionCharts;

    [Serializable] public struct ProgressionChart
    {
        public int level;
        public int xpNeeded;
        public float craftTimeReduction;
    }
}
