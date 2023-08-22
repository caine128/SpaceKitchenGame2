using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridComparerByDistance : Comparer<Grid>
{
    public enum CompareDirection
    {
        ClockWise,
        CounterClockWise,
    }

    private Grid fromGrid;
    private CompareDirection compareDirection;
    public GridComparerByDistance(Grid fromGrid, CompareDirection compareDirection) : base()
    {
        this.fromGrid = fromGrid; 
        this.compareDirection = compareDirection;
    }

    public override int Compare(Grid x, Grid y)
    {
        var distanceToX = CalculateDistanceFrom(x);
        var distanceToY = CalculateDistanceFrom(y);

        if (distanceToX.CompareTo(distanceToY) != 0)
        {
            return distanceToX.CompareTo(distanceToY);
        }
        else if(CalculateDegreeFrom(x).CompareTo(CalculateDegreeFrom(y)) != 0)
        {
            return CalculateDegreeFrom(x).CompareTo(CalculateDegreeFrom(y));
        }
        else
        {
            return 0;
        }
    }

    private int CalculateDistanceFrom(Grid toGrid)
    {
        int distanceX = Mathf.Abs(toGrid.GridPosition.x - fromGrid.GridPosition.x);
        int distanceZ = Mathf.Abs(toGrid.GridPosition.z - fromGrid.GridPosition.z);

        return distanceX > distanceZ
                    ? (14 * distanceZ) + (10 * (distanceX - distanceZ))
                    : (14 * distanceX) + (10 * (distanceZ - distanceX));
    }

    private float CalculateDegreeFrom(Grid toGrid)
        => compareDirection switch
        {
            CompareDirection.CounterClockWise => Mathf.Atan2(toGrid.GridPosition.z - fromGrid.GridPosition.z, toGrid.GridPosition.x - fromGrid.GridPosition.x),
            CompareDirection.ClockWise => Mathf.Atan2(toGrid.GridPosition.x - fromGrid.GridPosition.x, toGrid.GridPosition.z - fromGrid.GridPosition.z),
            _ => throw new System.NotImplementedException(),
        };

}
