using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public abstract class ScrolledObjectsVisibility<T_BluePrint> : MonoBehaviour
    where T_BluePrint : SortableBluePrint_Base
{
    [SerializeField] protected Status status = Status.Inactive;  //SERIALIZED FOR DEBUG PURPOSES !!! LATER TO REMOVE
    [SerializeField] protected ScrollRect scrollRect;
    [SerializeField] protected float worldControlPointMax = default(float);
    [SerializeField] protected float worldControlPointMin = default(float);
    protected int previousRecipeIndex;
    protected int nextRecipeIndex;

    protected ScrollablePanel<T_BluePrint> panelBelonged;
    protected bool _forceScrolled;
    public bool ForceScrolled
    {
        get
        {
            return _forceScrolled;
        }
        set
        {
            _forceScrolled = value;
            if (value == false && status==Status.Active) ForceUpdateScrolledContainerForwardScroll();
        }
    }
    protected enum Status
    {
        Active,
        Inactive,
    }
    protected void LateUpdate()
    {
        //if (Time.frameCount % 2 == 0)
        //{
        if (_forceScrolled && status == Status.Active || panelBelonged.CO is not null)
        {
            CheckVisibility();
        }
        else
        {
            if (scrollRect.horizontal && scrollRect.velocity.x == 0f || scrollRect.vertical && scrollRect.velocity.y == 0f || panelBelonged.CO is not null)
            {
                
                return;
            }
            if (Mathf.Abs(scrollRect.velocity.x) < 10f && !Mathf.Approximately(0,scrollRect.velocity.x))
            {               
                scrollRect.velocity = new Vector2(0,scrollRect.velocity.y);
                return;
            }
            if (Mathf.Abs(scrollRect.velocity.y) < 10f && !Mathf.Approximately(0, scrollRect.velocity.y))
            {
                scrollRect.velocity = new Vector2(scrollRect.velocity.x,0);
                return;
            }
            if (status == Status.Active)
            {
                scrollRect.velocity = scrollRect.vertical 
                                        ? new Vector2(scrollRect.velocity.x, Mathf.Clamp(scrollRect.velocity.y, -1500, 1500f)) 
                                        : scrollRect.velocity;
                CheckVisibility();
            }
        }
        //}
    }


    public virtual void AssignPanelBelonged(ScrollablePanel panelBelonged_IN)
    {
        panelBelonged = (ScrollablePanel<T_BluePrint>)panelBelonged_IN;
    }
    public void Configure(List<T_BluePrint> recipesToDisplay)
    {
        SetControlPoints();

        if (recipesToDisplay.Count + 1 <= panelBelonged.IndiceIndex)
        {
            status = Status.Inactive;
            return;
        }
        else
        {
            status = Status.Active;
            SetRecipeIndexes();
        }
    }

    protected virtual void SetControlPoints()
    {
        worldControlPointMax = transform.TransformPoint(panelBelonged.ContainersList[panelBelonged.IndiceIndex - 3].rt.localPosition.x, 0, 0).x;
        worldControlPointMin = transform.TransformPoint(panelBelonged.ContainersList[0].rt.localPosition.x - panelBelonged.ContainerWidth, 0, 0).x;
    }

    protected virtual void SetRecipeIndexes(int? previousRecipeIndex_IN = null, int? nextRecipeIndex_IN = null)
    {
        previousRecipeIndex = previousRecipeIndex_IN ?? -1;
        nextRecipeIndex = nextRecipeIndex_IN ?? panelBelonged.IndiceIndex;
    }

    protected virtual void CheckVisibility()
    {
        if (scrollRect.velocity.x > 0 && previousRecipeIndex > -1)
        {
            if (panelBelonged.ContainersList[panelBelonged.IndiceIndex - 3].rt.position.x >= worldControlPointMax)
            {
                UpdateContainersRightScroll();
            }
        }
        else if ((scrollRect.velocity.x < 0 || _forceScrolled) && nextRecipeIndex < panelBelonged.RequestedBluePrints.Count)
        {
            if (panelBelonged.ContainersList[0].rt.position.x <= worldControlPointMin)
            {
                UpdateContainersLeftScroll();
            }
        }
    }
    protected virtual void ForceUpdateScrolledContainerForwardScroll()
    {
        while (panelBelonged.ContainersList[0].rt.position.x <= worldControlPointMin && nextRecipeIndex < panelBelonged.RequestedBluePrints.Count)
        {
            UpdateContainersLeftScroll();
        }
    }

    protected virtual void UpdateContainersRightScroll()
    {
        Container<T_BluePrint> containerToMove = panelBelonged.ContainersList[panelBelonged.IndiceIndex - 1];
        Vector2 previousContainerPos = panelBelonged.ContainersList.FirstOrDefault().rt.localPosition;
        panelBelonged.ContainersList.RemoveAt(panelBelonged.IndiceIndex - 1);
        panelBelonged.ContainersList.Insert(0, containerToMove);
        containerToMove.rt.localPosition = new Vector2(previousContainerPos.x - panelBelonged.ContainerWidth - panelBelonged.OffsetDistance, 0);

        containerToMove.LoadContainer(panelBelonged.RequestedBluePrints[previousRecipeIndex]);
        SetRecipeIndexes(previousRecipeIndex - 1, nextRecipeIndex - 1);
    }

    protected virtual void UpdateContainersLeftScroll()
    {
        Container<T_BluePrint> containerToMove = panelBelonged.ContainersList[0];
        Vector2 previousContainerPos = panelBelonged.ContainersList.LastOrDefault().rt.localPosition;
        panelBelonged.ContainersList.RemoveAt(0);
        panelBelonged.ContainersList.Insert(panelBelonged.IndiceIndex - 1, containerToMove);
        containerToMove.rt.localPosition = new Vector2(previousContainerPos.x + panelBelonged.ContainerWidth + panelBelonged.OffsetDistance, 0);

        containerToMove.LoadContainer(panelBelonged.RequestedBluePrints[nextRecipeIndex]);
        SetRecipeIndexes(previousRecipeIndex + 1, nextRecipeIndex + 1);
    }

}



public abstract class ScrolledObjectsVisibility<T_BluePrint, T_MainSelector, T_SubSelector> : ScrolledObjectsVisibility<T_BluePrint>
    where T_BluePrint : SortableBluePrint
    where T_MainSelector : System.Enum
    where T_SubSelector : System.Enum
{
    //protected ScrollablePanel<T_BluePrint, T_MainSelector, T_SubSelector> panelBelonged;
    //[SerializeField] protected float worldControlPointMax = default(float);
    //[SerializeField] protected float worldControlPointMin = default(float);
    //protected int previousRecipeIndex;               
    //protected int nextRecipeIndex;                     
    /*[SerializeField] protected ScrollRect scrollRect;  */                        //SERIALIZED FOR DEBUG PURPOSES !!! LATER TO REMOVE
    /*[SerializeField] protected Status status = Status.Inactive;*/          //SERIALIZED FOR DEBUG PURPOSES !!! LATER TO REMOVE
                                                                             //protected IEnumerator routine= null;

    //public override void AssignPanelBelonged(ScrollablePanel panelBelonged_IN)
    //{
    //    panelBelonged = (ScrollablePanel<T_BluePrint, T_MainSelector, T_SubSelector>) panelBelonged_IN;
    //}
    //public void AssignPanelBelonged( ScrollablePanel<T_BluePrint, T_MainSelector, T_SubSelector> panelBelonged_IN)
    //{
    //    panelBelonged = panelBelonged_IN;
    //}


    //protected void Update()   
    //{
    //    if(Time.frameCount % 2 == 0)
    //    {
    //        if (scrollRect.velocity == Vector2.zero)
    //        {
    //            return;
    //        }
    //        if (Mathf.Abs(scrollRect.velocity.x) < 10f)
    //        {
    //            scrollRect.velocity = Vector2.zero;
    //        }
    //        if (status == Status.Active)
    //        {
    //            CheckVisibility();
    //        }
    //    }
    //}
    //protected enum Status
    //{
    //    Active,
    //    Inactive,
    //}

    //public override void Configure(List<T_BluePrint> recipesToDisplay)
    //{
    //    SetControlPoints();

    //    if(recipesToDisplay.Count+1 <= panelBelonged.IndiceIndex )
    //    {
    //        status = Status.Inactive;
    //        return;
    //    }
    //    else
    //    {
    //        status = Status.Active;
    //        SetRecipeIndexes(-1, panelBelonged.IndiceIndex);
    //    }
    //}

    //protected void SetControlPoints()
    //{
    //    worldControlPointMax = transform.TransformPoint(panelBelonged.ContainersList[panelBelonged.IndiceIndex - 3].rt.localPosition.x, 0, 0).x;
    //    worldControlPointMin = transform.TransformPoint(panelBelonged.ContainersList[0].rt.localPosition.x - panelBelonged.CardWidth, 0, 0).x;
    //}


    //protected void CheckVisibility()
    //{

    //    if (scrollRect.velocity.x > 0 && previousRecipeIndex > -1)
    //    {
    //        if(panelBelonged.ContainersList[panelBelonged.IndiceIndex - 3].rt.position.x >= worldControlPointMax)
    //        {
    //            Container<T_BluePrint> containerToMove = panelBelonged.ContainersList[panelBelonged.IndiceIndex - 1];
    //            Vector2 previousContainerPos = panelBelonged.ContainersList.FirstOrDefault().rt.localPosition;
    //            panelBelonged.ContainersList.RemoveAt(panelBelonged.IndiceIndex - 1);
    //            panelBelonged.ContainersList.Insert(0, containerToMove);
    //            containerToMove.rt.localPosition = new Vector2(previousContainerPos.x - panelBelonged.CardWidth - panelBelonged.OffsetDistance, 0);

    //            containerToMove.LoadContainer(panelBelonged.RequestedBluePrints[previousRecipeIndex]);
    //            SetRecipeIndexes(previousRecipeIndex - 1, nextRecipeIndex - 1);


    //        }
    //    }
    //    else if (scrollRect.velocity.x < 0 && nextRecipeIndex < panelBelonged.RequestedBluePrints.Count)
    //    {
    //        if (panelBelonged.ContainersList[0].rt.position.x <= worldControlPointMin)
    //        {
    //            Container<T_BluePrint> containerToMove = panelBelonged.ContainersList[0];
    //            Vector2 previousContainerPos = panelBelonged.ContainersList.LastOrDefault().rt.localPosition;
    //            panelBelonged.ContainersList.RemoveAt(0);
    //            panelBelonged.ContainersList.Insert(panelBelonged.IndiceIndex - 1, containerToMove);
    //            containerToMove.rt.localPosition = new Vector2(previousContainerPos.x + panelBelonged.CardWidth + panelBelonged.OffsetDistance, 0);

    //            containerToMove.LoadContainer(panelBelonged.RequestedBluePrints[nextRecipeIndex]);
    //            SetRecipeIndexes(previousRecipeIndex + 1, nextRecipeIndex + 1);
    //        }
    //    }
    //}

    //protected void SetRecipeIndexes(int previousRecipeIndex_IN, int nextRecipeIndex_IN)
    //{
    //    previousRecipeIndex = previousRecipeIndex_IN;
    //    nextRecipeIndex = nextRecipeIndex_IN;
    //}


}
