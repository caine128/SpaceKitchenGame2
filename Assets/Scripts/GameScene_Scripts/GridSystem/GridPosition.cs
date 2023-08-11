using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct GridPosition : IEquatable<GridPosition>
{
    public readonly int x;
    public readonly int z;

    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition other && this.Equals(other);
    }

    public bool Equals(GridPosition other)
    {
        return x == other.x && z == other.z;
    }

    public override int GetHashCode()
    {
        return (x, z).GetHashCode();
    }

    public static bool operator ==(GridPosition first, GridPosition second)
    {
        return first.Equals(second);
    }

    public static bool operator !=(GridPosition first, GridPosition second)
    {
        return !(first == second);
    }

    public override string ToString() => $"x: {x}, z: {z}";
  
}

