using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDatable 
{

    public DateTime DateLastCrafted { get; }
    DateTime GetLastCraftedDate();

    void SetLastCraftedTime();
}
