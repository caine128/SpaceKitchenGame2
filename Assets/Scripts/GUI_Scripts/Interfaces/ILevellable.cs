using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevellable : IRankable
{
    public bool isAtMaxLevel { get; }

}
