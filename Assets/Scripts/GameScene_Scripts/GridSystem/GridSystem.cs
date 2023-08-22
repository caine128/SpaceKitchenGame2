using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class GridSystem
{
    private readonly int width;
    private readonly int height;
    private readonly float cellSize;

    private readonly Grid[,] grid;
    public readonly Grid CenterGrid;   //=> grid[shopCenter.x, shopCenter.z];

    //private readonly (int x, int z) shopCenter;

    public GridSystem(int width, int height, float cellSize, (int x, int z) shopSize, Transform tile_PF, Transform debugTextPrefab)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.grid = new Grid[width, height];

        int tilePerGrid = 5;

        int xAxisOffset = Mathf.FloorToInt((width - shopSize.x) / 2f);
        int zAxisOffset = Mathf.FloorToInt((height - shopSize.z) / 2f);

        var shopCenter = (xAxisOffset + Mathf.CeilToInt(shopSize.x / 2f), zAxisOffset + Mathf.CeilToInt(shopSize.z / 2f));


        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var isWithinShopSize = x <= xAxisOffset + shopSize.x && x > xAxisOffset && z <= zAxisOffset + shopSize.z && z > zAxisOffset;

                GridPosition gridPosition = new(x: x, z: z);

                var newGrid = new Grid(gridPosition, FromGridPosToWorldPos(gridPosition), isBuildable: isWithinShopSize);
                if ((x, z) == shopCenter) CenterGrid = newGrid;
                grid[x, z] = newGrid;

                /*if (isWithinShopSize)
                {
                    var debugTextGO = UnityEngine.Object.Instantiate(debugTextPrefab);
                    var tmComponent = debugTextGO.GetComponent<TextMeshPro>();
                    tmComponent.text = $"{x} , {z}";
                    if((x,z) == shopCenter) tmComponent.color = Color.yellow;  // TODO : Later to remove it's or debug purposes 
                    debugTextGO.position = new Vector3(x + .5f, 0.2f, z + .5f);
                    
                }*/

                if (x % tilePerGrid == 0 && z % tilePerGrid == 0)
                {
                    var tile_Go = UnityEngine.Object.Instantiate(tile_PF);
                    tile_Go.eulerAngles = new Vector3(90, 0, 0);
                    tile_Go.localScale = new Vector3(cellSize * 5, cellSize * 5, 1);
                    tile_Go.position = new Vector3(x + (float)tilePerGrid / 2f, 0, z + (float)tilePerGrid / 2f);
                }
            }
        }      
    }

    // TODO : Later to do private when onpointerdown instantiation of props are removed from the buildinggrid script
    public Vector3 FromGridPosToWorldPos(GridPosition gridPosition)
    {
        var x = new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
        return x;
    }

    // TODO : Later to do private when onpointerdown instantiation of props are removed from the buildinggrid script
    public GridPosition FromWorldPosToGridPos(Vector3 worldPosition)
    {
        GridPosition gridPosition = new(Mathf.FloorToInt(worldPosition.x / cellSize), Mathf.FloorToInt(worldPosition.z / cellSize));
        return gridPosition;
    }

    public IEnumerable<Grid> GetGrids(Predicate<Grid> predicate)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (predicate(grid[x,z]))
                    yield return grid[x,z];
            }
        } 
    }

    public Grid GetGrid(GridPosition gridPosition)
    {
        (int x, int z) rectifiedGridPositions;

        rectifiedGridPositions.x = gridPosition.x < 0
                                        ? 0
                                        : gridPosition.x >= width
                                                ? width - 1
                                                : gridPosition.x;
        rectifiedGridPositions.z = gridPosition.z < 0
                                        ? 0
                                        : gridPosition.z >= height
                                                ? height - 1
                                                : gridPosition.z;

        return this.grid[rectifiedGridPositions.x, rectifiedGridPositions.z];
    }

    public Grid GetGrid(Vector3 worldPosition)
    {
        return GetGrid(FromWorldPosToGridPos(worldPosition));
    }

    private enum IterationDirection
    {
        DownWards = 0,
        UpWards = 1,
    }


    /*public Grid GetCenterGridAdjustedToPropSize(Prop prop)
    {

        (int x, int z) offsets = (0, 0);
        IterationDirection iterationDirection = IterationDirection.DownWards;
        int verticalFullIterationAmount = 0;
 
        var shouldIterate = true;


        while (shouldIterate)
        {
            shouldIterate = false;

            foreach (var gridpos in OffsettedGridpositions())
            {


                var grid = GetGrid(gridpos);
                if (grid.isBuildable)
                {
                    if (grid.IsOccupied)
                    {
                        offsets = iterationDirection switch
                        {
                            IterationDirection.DownWards => (offsets.x, offsets.z - 1),
                            IterationDirection.UpWards => (offsets.x, offsets.z + 1),
                            _ => throw new NotImplementedException(),
                        };
                        shouldIterate = true;
                        break;
                    }
                }
                else
                {
                    switch (iterationDirection)
                    {

                        case IterationDirection.DownWards when gridpos.z <= currentLowerPoint :
                            currentLowerPoint = gridpos.z;
                            iterationDirection = IterationDirection.UpWards;
                            offsets = (offsets.x, +1);
                            shouldIterate = true;
                            break;

                        case IterationDirection.DownWards when gridpos.z > currentLowerPoint:
                            shouldIterate = true;
                            break;

                        case IterationDirection.UpWards when gridpos.z >= currentUpperPoint:
                            currentUpperPoint = gridpos.z;
                            verticalFullIterationAmount++;
                            offsets = verticalFullIterationAmount % 2 != 0
                                            ? (offsets.x - verticalFullIterationAmount, 0)
                                            : (offsets.x + verticalFullIterationAmount, 0);
                            iterationDirection = IterationDirection.DownWards;                  
                            shouldIterate = true;
                            break;

                        case IterationDirection.UpWards when gridpos.z < currentUpperPoint:
                            currentUpperPoint = gridpos.z;
                            verticalFullIterationAmount++;
                            offsets = verticalFullIterationAmount % 2 != 0
                                            ? (offsets.x - verticalFullIterationAmount, 0)
                                            : (offsets.x + verticalFullIterationAmount, 0);
                            iterationDirection = IterationDirection.DownWards;
                            shouldIterate = true;
                            break;
                    }

                }
            }


        }



        return GetGrid(OffsettedGridpositions().First());



        IEnumerable<GridPosition> OffsettedGridpositions()
        {
            foreach (var gridPos in prop.GetCurrentGridPositions())
            {

                yield return new GridPosition(x: gridPos.x + offsets.x, z: gridPos.z + offsets.z);
                Debug.Log("yielding grippos :" + new GridPosition(x: gridPos.x + offsets.x, z: gridPos.z + offsets.z));
            }
        }

    }*/


    public bool IsWithinEffectiveRange(Vector3 worldPosition)
    {
        return worldPosition.x % cellSize > 0.05f
            && worldPosition.x % cellSize < 0.95f
            && worldPosition.y % cellSize > 0.05f
            && worldPosition.y % cellSize < 0.95f;
    }

}
