using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Single_CraftedItem_VisibilityChecker : MonoBehaviour, IConfigurablePanel
{
    public Vector2 visibilityBounds;

    //private void Start()
    //{
    //    PanelConfig();
    //}

    //private void PanelConfig()
    //{
    //    visibilityBounds = new Vector2(transform.parent.position.x, transform.parent.position.y);
    //    CheckVisibility();
    //}

    public void PanelConfig()
    {
        visibilityBounds = new Vector2(transform.parent.position.x, transform.parent.position.y);
        CheckVisibility();
    }


    public bool CheckVisibility()
    {
        if (transform.position.x > visibilityBounds.x || transform.position.y < visibilityBounds.y)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


}
