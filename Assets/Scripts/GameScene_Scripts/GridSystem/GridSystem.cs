using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GridSystem
{
    private readonly int width;
    private readonly int height;
    private readonly float cellSize;

    private readonly Grid[,] grid;

    public GridSystem(int width, int height, float cellSize , (int x, int z) shopSize , Transform tile_PF, Transform debugTextPrefab)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.grid = new Grid[width, height];

        int tilePerGrid = 5;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var isWithinShopSize = x < 20 + shopSize.x && x > 20 && z < 20 + shopSize.z && z > 20;

                GridPosition gridPosition = new(x: x, z: z);
                grid[x, z] = new Grid(gridPosition, FromGridPosToWorldPos(gridPosition), isBuildable: isWithinShopSize ? true : false);

                if(isWithinShopSize)
                {
                    var debugTextGO = UnityEngine.GameObject.Instantiate<Transform>(debugTextPrefab);
                    var tmComponent = debugTextGO.GetComponent<TextMeshPro>();
                    tmComponent.text = $"{x} , {z}";
                    debugTextGO.position = new Vector3(x + .5f, 0.2f, z + .5f);
                }
              

                if (x% tilePerGrid == 0 && z% tilePerGrid == 0)
                {
                    var tile_Go = Object.Instantiate(tile_PF);
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

    public Grid GetGrid(GridPosition gridPosition)//, out Grid grid)
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

        /*if (gridPosition.x >= 0 && gridPosition.z >= 0 && gridPosition.x <width && gridPosition.z < height)
        {
            grid = this.grid[gridPosition.x, gridPosition.z];
            return true;
        }
        else
        {
            Debug.Log($"there is no grid to get at position : {gridPosition}");
            grid = null;
            return false;
        }*/

        return this.grid[rectifiedGridPositions.x, rectifiedGridPositions.z];
    }

    public Grid GetGrid(Vector3 worldPosition)//, out Grid grid)
    {
        return GetGrid(FromWorldPosToGridPos(worldPosition));//, out Grid grid_Out);
        //grid = grid_Out;
        //return canReturn;
    }

    public bool IsWithinEffectiveRange(Vector3 worldPosition)
    {
        return worldPosition.x % cellSize > 0.05f
            && worldPosition.x % cellSize < 0.95f
            && worldPosition.y % cellSize > 0.05f
            && worldPosition.y % cellSize < 0.95f;
    }

   /*public bool ValidateGrids(GridPosition gridPosition, (int x,int z) propSize)
    {
        if (gridPosition.x >= 0 && gridPosition.z >= 0 && gridPosition.x < width && gridPosition.z < height 
            && gridPosition.x + propSize.x <width && gridPosition.z + propSize.z < height)
        {
            for (int x = gridPosition.x; x < gridPosition.x + propSize.x; x++)
            {
                for (int z = gridPosition.z; z < gridPosition.z + propSize.z; z++)
                {
                    if (grid[x, z].IsOccupied)
                    {
                        Debug.Log("grid(s) are occupied");
                        return false;
                    }                      
                }
            }
            return true;
        }
        else
        {
            Debug.Log($"there is no grid to get at position : {gridPosition}");
            return false;
        }

    }*/

}
