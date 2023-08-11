using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeallocatable 
{
    public void UnloadAndDeallocate();
}


public interface IQuickUnloadable
{
    void QuickUnload();
}
