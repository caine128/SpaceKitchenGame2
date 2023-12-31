
using UnityEngine;

public class Grid
{
    public GridPosition GridPosition { get;  private set; }
    public readonly Vector3 GridWorldPosition;

    public bool IsBuildable { get; private set; }
    public bool IsOccupied { get => prop != null ;}
    private Prop prop = null;
    


    public Grid(GridPosition gridPosition, Vector3 gridWorldPosition, bool isBuildable)
    {
        this.GridPosition = gridPosition;
        this.GridWorldPosition = gridWorldPosition;
        this.IsBuildable = isBuildable;
    }

    public void SetBuildableStatus(bool isBuildable)
    {
        IsBuildable=isBuildable;
    }

    public void PlaceProp(Prop prop) 
    {
        if (!IsOccupied)
        {
            this.prop = prop;
        }
        else
        {
            Debug.Log($"grid at gridposition {GridPosition} is occupied with {prop.ShopUpgradeBluePrint?.GetName()}");
        }
       
    }

    public void RemoveProp()
    {
        if (IsOccupied)
        {
            this.prop = null;
        }
        else
        {
            Debug.Log($"grid at gridposition {GridPosition} has nothing to remove");
        }
    }

}
