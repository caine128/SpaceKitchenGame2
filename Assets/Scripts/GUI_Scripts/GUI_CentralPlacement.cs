
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public static class GUI_CentralPlacement
{
    //public static void PlaceRectTransForms(this IReadOnlyList<RectTransform> rects, int? amountToShift_IN =null, float? offsetDistance_IN = null)
    //{
    //    float offsetDistance = offsetDistance_IN ?? rects[0].rect.height / 4;
    //    float totalOffsetAmount = (rects.Count - 1) * offsetDistance;
    //    float totalContainerWidth = 0;
    //    for (int i = 0; i < rects.Count; i++)
    //    {
    //        totalContainerWidth += rects[i].rect.width;
    //    }
    //    float totalWidth = totalContainerWidth + totalOffsetAmount;
    //}



    public static void PlaceContainers<T_ContentDisplay>(this IReadOnlyList<T_ContentDisplay> contentDisplays_IN, int requiredAmount, float containerWidth, bool isHorizontalPlacement, float? crossPosition_IN = null, float? offsetDistance_IN = null, float shiftDistance = 0)
       where T_ContentDisplay : ContentDisplay, IPlacableRt
    {
        float offsetDistance = offsetDistance_IN ?? containerWidth / 4;
        int firstPivot;
        ActivateContainers(requiredAmount, contentDisplays_IN);

        if (requiredAmount % 2 == 0)
        {

            firstPivot = requiredAmount / 2;         /// Keep in mind that first pivot equates to the firstpivotValue+1 in the array of contentdisplays !!
            for (int i = 0; i < requiredAmount; i++)
            {
                var j = i - firstPivot;
                var modifiedIndex = isHorizontalPlacement ? i : requiredAmount - 1 - i;

                float crossPosition = crossPosition_IN ?? (isHorizontalPlacement == true ? contentDisplays_IN[modifiedIndex].RT.anchoredPosition.y : contentDisplays_IN[modifiedIndex].RT.anchoredPosition.x);

                float newPosition = (j * (containerWidth + offsetDistance)) + (offsetDistance / 2) - shiftDistance;
                contentDisplays_IN[modifiedIndex].RT.pivot = isHorizontalPlacement ? new Vector2(0, .5f) : new Vector2(.5f, 0);
                contentDisplays_IN[modifiedIndex].RT.anchoredPosition = isHorizontalPlacement ? new Vector2(newPosition, crossPosition) : new Vector2(crossPosition, newPosition);
            }


            /* firstPivot = requiredAmount / 2;         /// Keep in mind that first pivot equates to the firstpivotValue+1 in the array of contentdisplays !!
             for (int i = firstPivot; i < requiredAmount; i++)
             {

                 float crossPosition = crossPosition_IN ?? (isHorizontalPlacement == true ? contentDisplays_IN[i].RT.anchoredPosition.y : contentDisplays_IN[i].RT.anchoredPosition.x);

                 float newPosition = ((i - firstPivot) * (containerWidth + offsetDistance)) + (offsetDistance / 2) - shiftDistance;
                 //float newPosition = ((i - firstPivot) * (containerWidth + offsetDistance)) + ((i - firstPivot + 1) * (offsetDistance / 2)) - shiftDistance;
                 contentDisplays_IN[i].RT.pivot = isHorizontalPlacement ? new Vector2(0, .5f) : new Vector2(.5f, 0);
                 contentDisplays_IN[i].RT.anchoredPosition = isHorizontalPlacement ? new Vector2(newPosition, crossPosition) : new Vector2(crossPosition, newPosition);
             }

             for (int i = firstPivot - 1; i > -1; i--)
             {
                 float crossPosition = crossPosition_IN ?? (isHorizontalPlacement == true ? contentDisplays_IN[i].RT.anchoredPosition.y : contentDisplays_IN[i].RT.anchoredPosition.x);

                 float newPosition = ((i - firstPivot) * (containerWidth + offsetDistance)) + (offsetDistance / 2) - shiftDistance;
                 // float newPosition = ((i - firstPivot) * (containerWidth + (offsetDistance / 2))) - shiftDistance;
                 contentDisplays_IN[i].RT.pivot = isHorizontalPlacement ? new Vector2(0, .5f) : new Vector2(.5f, 0);
                 contentDisplays_IN[i].RT.anchoredPosition = isHorizontalPlacement ? new Vector2(newPosition, crossPosition) : new Vector2(crossPosition, newPosition);
             }*/
        }
        else
        {
            firstPivot = Mathf.FloorToInt(requiredAmount / 2);   /// Keep in mind that first pivot equates to the firstpivotValue+1 in the array of contentdisplays !! 

            for (int i = 0; i < requiredAmount; i++)
            {
                var j = i - firstPivot;
                var modifiedIndex = isHorizontalPlacement ? i : requiredAmount - 1 - i;

                float crossPosition = crossPosition_IN ?? (isHorizontalPlacement == true ? contentDisplays_IN[modifiedIndex].RT.anchoredPosition.y : contentDisplays_IN[modifiedIndex].RT.anchoredPosition.x);

                float newPosition = (j * (containerWidth + offsetDistance)) - shiftDistance;
                contentDisplays_IN[modifiedIndex].RT.pivot = isHorizontalPlacement ? new Vector2(0.5f, .5f) : new Vector2(.5f, .5f);
                contentDisplays_IN[modifiedIndex].RT.anchoredPosition = isHorizontalPlacement ? new Vector2(newPosition, crossPosition) : new Vector2(crossPosition, newPosition);
            }

            /*for (int i = firstPivot; i < requiredAmount; i++)
            {
                float crossPosition = crossPosition_IN ?? (isHorizontalPlacement == true ? contentDisplays_IN[i].RT.anchoredPosition.y : contentDisplays_IN[i].RT.anchoredPosition.x);

                float newPosition = ((i - firstPivot) * (containerWidth + offsetDistance)) - shiftDistance;
                contentDisplays_IN[i].RT.pivot = isHorizontalPlacement ? new Vector2(0.5f, .5f) : new Vector2(.5f, .5f);
                contentDisplays_IN[i].RT.anchoredPosition = isHorizontalPlacement ? new Vector2(newPosition, crossPosition) : new Vector2 (crossPosition,newPosition);
            }
            for (int i = firstPivot - 1; i > -1; i--)
            {
                float crossPosition = crossPosition_IN ?? (isHorizontalPlacement == true ? contentDisplays_IN[i].RT.anchoredPosition.y : contentDisplays_IN[i].RT.anchoredPosition.x);

                float newPosition = ((i - firstPivot) * (containerWidth + offsetDistance)) - shiftDistance;
                contentDisplays_IN[i].RT.pivot = isHorizontalPlacement ? new Vector2(0.5f, .5f) : new Vector2(.5f, .5f);
                contentDisplays_IN[i].RT.anchoredPosition = isHorizontalPlacement ? new Vector2(newPosition, crossPosition) : new Vector2(crossPosition,newPosition);
            }*/
        }

        DeactivateUnusedContainers(requiredAmount, contentDisplays_IN);
    }


    public static void PlaceContainersMatrix<T_ContentDisplay>(this IReadOnlyList<T_ContentDisplay> contentDisplays_IN, int requiredAmount, RectTransform containingPanel_RT,float crossPosition = 0f)
        where T_ContentDisplay : ContentDisplayPopup_Generic
    {

        RectTransform contentDisplayRect =default; //= contentDisplays_IN[0].RT;
        int amountOfRequiredRows; /*= requiredAmount switch
        {
            <= 4 => 2,
            >4 and <=6 => 3,
            > 6 => throw new InvalidDataException(),
        };*/




        float horizontalOffsetDistance = default;// = ContentDisplayPopup_Generic.TextContainrWidth * 1.2f;
        float verticalOffsetDistance = default; // = (containingPanel_RT.rect.height - (amountOfRequiredRows * contentDisplays_IN[0].RT.rect.height)) / (amountOfRequiredRows - 1 + 2);
        Vector2 previousPosition = default; // = new Vector2(-containingPanel_RT.rect.width/2 + )
        
        //float verticalOffsetDistance = contentDisplays_IN[0].RT.rect.height * 2 / 3;
        //wfloat originalPositionY = contentDisplays_IN[0].OriginalPosition.y;

        /*int columnNumber = 0;
        float horizontalPos = 0;*/

        ActivateContainers(requiredAmount, contentDisplays_IN);

        for (int i = 0; i < requiredAmount; i++)
        {
            
            if (i == 0)
            {
                amountOfRequiredRows = requiredAmount switch
                {
                    <= 4 => 2,
                    > 4 and <= 6 => 3,
                    > 6 => throw new InvalidDataException(),
                };

                contentDisplayRect = contentDisplays_IN[i].RT;
                horizontalOffsetDistance = ContentDisplayPopup_Generic.TextContainrWidth * 1.2f;
                verticalOffsetDistance = (containingPanel_RT.rect.height - (amountOfRequiredRows * contentDisplayRect.rect.height)) / (amountOfRequiredRows - 1 + 2);
                var horizontalMargin = (containingPanel_RT.rect.width - horizontalOffsetDistance - (2 * contentDisplayRect.rect.width)) / 2;
                previousPosition = new Vector2((-containingPanel_RT.rect.width / 2) + (contentDisplayRect.rect.width / 2) + horizontalMargin, (containingPanel_RT.rect.height / 2) - (contentDisplayRect.rect.height / 2) - verticalOffsetDistance);

                contentDisplays_IN[i].RT.anchoredPosition = previousPosition;
                Debug.LogWarning("-containingPanel_RT.rect.width / 2" + -containingPanel_RT.rect.width / 2);
                Debug.LogWarning("containingPanel_RT.rect.width" + containingPanel_RT.rect.width);
                Debug.LogWarning("horizontalmargin : " + horizontalMargin);
            }
            Debug.LogWarning("startingposition for :"+ i + previousPosition + "contentdisplaywidth for " + i + contentDisplayRect.rect.width);
            Debug.LogWarning("containing panlel width " + containingPanel_RT.rect.width);
           

            if (contentDisplays_IN[i].RT.pivot.x != .5f || contentDisplays_IN[i].RT.pivot.y != .5f) contentDisplays_IN[i].RT.pivot = new Vector2(.5f, .5f);

            else if( i != 0)
            {
                //Debug.Log("enumerator is on index" + i);
                contentDisplays_IN[i].RT.anchoredPosition = i % 2 == 1
                                                            ? new Vector2(previousPosition.x + contentDisplayRect.rect.width + horizontalOffsetDistance, previousPosition.y)
                                                            : new Vector2(previousPosition.x - contentDisplayRect.rect.width - horizontalOffsetDistance, previousPosition.y - contentDisplayRect.rect.height - verticalOffsetDistance);
                previousPosition = contentDisplays_IN[i].RT.anchoredPosition;
            }

           /* if (i % 2 == 0)
            {
                horizontalPos = columnNumber * horizontalOffsetDistance;
                columnNumber++;

                contentDisplays_IN[i].RT.anchoredPosition = new Vector2(horizontalPos, originalPositionY + verticalOffsetDistance);
            }
            else if (i % 2 == 1)
            {
                contentDisplays_IN[i].RT.anchoredPosition = new Vector2(horizontalPos, originalPositionY - verticalOffsetDistance);
            }*/

        }

        DeactivateUnusedContainers(requiredAmount, contentDisplays_IN);
    }

    public static void PlaceContainersMatrix<T_ContentDisplay>(this IReadOnlyList<T_ContentDisplay> contentDisplay, Vector2[] positions)
    where T_ContentDisplay : ContentDisplay,IPlacableRt  //ContentDisplayPopup_Generic
    {
        //if (contentDisplay.Count != positions.Length)
        //{
        //    throw new ArgumentOutOfRangeException();
        //}
        var requiredAmount = positions.Length;
        ActivateContainers(requiredAmount, contentDisplay);

        for (int i = 0; i < requiredAmount; i++)
        {
            contentDisplay[i].RT.anchoredPosition = positions[i];
        }

        DeactivateUnusedContainers(requiredAmount, contentDisplay);
    }

    public static (Vector2[] positions, float containerHeight) MatrixPlacementCalculation(RectTransform containingPanel_RT, RectTransform container_RT, int requiredAmount, Vector2? customContainerSize = null)
    {
        var containerWidth = customContainerSize?.x?? container_RT.rect.width;
        var containerHeight = customContainerSize?.y?? container_RT.rect.height;
        var offsetdistance = containerWidth / 4;

        if (containingPanel_RT.rect.width < containerWidth + (offsetdistance * 2))
        {
            throw new ArgumentOutOfRangeException();
        }

        int amountOfContainersForARow = requiredAmount;
        //int remainingAmountOfContainers = requiredAmount;

        Vector2[] positionsArray = new Vector2[requiredAmount];
        float edgeDistancesTotal;
        //int requiredRowAmount;

        while (!AreEnoughEdgeDistances())
        {
            amountOfContainersForARow--;
            continue;
        }

        var (startingYpoint, ContainingPanelRequiredHeight) = CalculateRequiredRowAmount();

        for (int i = 0; i < requiredAmount; i++)
        {
            var rowNo = Mathf.FloorToInt(i / amountOfContainersForARow);

            positionsArray[i] = i % amountOfContainersForARow == 0
                                                        ? new Vector2(-containingPanel_RT.rect.width / 2 + edgeDistancesTotal / 2 + containerWidth/2,
                                                                       startingYpoint - (rowNo* containerHeight) - (rowNo*offsetdistance))
                                                        : new Vector2(positionsArray[i - 1].x + containerWidth  + offsetdistance,
                                                                      positionsArray[i - 1].y);
        }
        return (positionsArray, ContainingPanelRequiredHeight);


        bool AreEnoughEdgeDistances()
        {          
            edgeDistancesTotal = containingPanel_RT.rect.width - ((amountOfContainersForARow * containerWidth) + ((amountOfContainersForARow - 1) * offsetdistance));
            return edgeDistancesTotal/2 >= offsetdistance;
        }

        (float,float) CalculateRequiredRowAmount()
        {
           
            var rowAmount = Mathf.CeilToInt((float)requiredAmount / amountOfContainersForARow);
            var requiredContainerHeight = (rowAmount * containerHeight) + ((rowAmount - 1) * offsetdistance) + (offsetdistance * 2);

            var upperMargin = (requiredContainerHeight - (rowAmount * containerHeight) - ((rowAmount - 1) * offsetdistance)) / 2;
            return (requiredContainerHeight / 2 - upperMargin - containerHeight / 2, requiredContainerHeight);


           /*return containingPanel_RT.rect.height >= requiredContainerHeight
                                        ? (startingYpoint: 0, requiredCcontainerHeight: containingPanel_RT.rect.height)
                                        : (startingYpoint: requiredContainerHeight / 2 - offsetdistance - container_RT.rect.height/2, requiredCcontainerHeight: requiredContainerHeight);
           */
        }

    }


    private static void ActivateContainers<T_ContentDisplay>(int requiredAmount_IN, IReadOnlyList<T_ContentDisplay> contentDisplays)
        where T_ContentDisplay : ContentDisplay
    {
        for (int i = 0; i < requiredAmount_IN; i++)
        {
            if (contentDisplays[i].gameObject.activeSelf != true)
            {
                contentDisplays[i].gameObject.SetActive(true);
            }
        }
    }

    public static void LoadContainers<T_ContentDisplay>(this IReadOnlyList<T_ContentDisplay> contentDisplays_IN, SortableBluePrint bluePrint_IN, int requiredAmount_IN, bool hideAtInit)
      where T_ContentDisplay : ContentDisplay
    {
        for (int i = 0; i < requiredAmount_IN; i++)
        {
            var contentDisplayInfo = new ContentDisplayInfo_ContentDisplay_WithText(bluePrint_IN: bluePrint_IN, indexNo_IN: i);
            contentDisplays_IN[i].Load(contentDisplayInfo);    //bluePrint_IN, i);
            if (hideAtInit) contentDisplays_IN[i].ScaleDirect(isVisible: false,
                                                              finalValueOperations: null);
        }
    }

    public static void LoadContainers<T_ContentDisplay, T_ContentDisplayLoadData>(this IReadOnlyList<T_ContentDisplay> contentDisplays_IN,
                                                                                  IEnumerable<T_ContentDisplayLoadData> loadData_IN,
                                                                                  bool hideAtInit)
    where T_ContentDisplay : ContentDisplay
    where T_ContentDisplayLoadData : ContentDisplayInfo
    {
        var contentDisplayCounter = 0;
        foreach (var loadData in loadData_IN)
        {
            contentDisplays_IN[contentDisplayCounter].Load(loadData);
            if (hideAtInit) contentDisplays_IN[contentDisplayCounter].ScaleDirect(isVisible: false,
                                                                                  finalValueOperations: null);
            contentDisplayCounter++;
        }
    }

    public static void LoadContainers<T_ContentDisplay, T_BlueprintType>(this IReadOnlyList<T_ContentDisplay> contentDisplays_IN,
                                                                         List<(T_BlueprintType bluePrintToLoad, int amountToLoad)> bluePrints_IN,
                                                                         bool hideAtInit)
        where T_ContentDisplay : ContentDisplayFrame
        where T_BlueprintType : SortableBluePrint
    {
        for (int i = 0; i < bluePrints_IN.Count; i++)
        {
            var contentDisplayInfo = new ContentDisplayInfo_ConentDisplayFrame(bluePrint_IN: bluePrints_IN[i].bluePrintToLoad, bluePrints_IN[i].amountToLoad);
            contentDisplays_IN[i].Load(contentDisplayInfo);             //bluePrints_IN[i].bluePrintToLoad, bluePrints_IN[i].amountToLoad);
            if (hideAtInit) contentDisplays_IN[i].ScaleDirect(isVisible: false,
                                                              finalValueOperations: null);
        }
    }

    public static void LoadContainers<T_ContentDisplay>(this IReadOnlyList<T_ContentDisplay> contentDisplays_IN,
        IEnumerable<(UnityEngine.AddressableAssets.AssetReferenceT<Sprite> spriteRef, string enhancmentBonus)> enhancementBonuses, bool hideAtInit, ContentDisplayModalPanel.DynamicShape dynamicShape_IN, bool isMaskActive = true, Color bgColor = default)
        where T_ContentDisplay : ContentDisplayModalPanel
    {

        var contentDisplayCounter = 0;
        foreach (var (spriteRef, enhancmentBonus) in enhancementBonuses)
        {
            var contentDisplaInfo = new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                spriteRef_IN: spriteRef,
                                                                contentTextMain_IN: enhancmentBonus,
                                                                contentTextSecondary_IN: null,
                                                                dynamicShape_IN: dynamicShape_IN,
                                                                isMaskActive: isMaskActive,
                                                                bgColor: bgColor);
            contentDisplays_IN[contentDisplayCounter].Load(contentDisplaInfo);
            if (hideAtInit) contentDisplays_IN[contentDisplayCounter].ScaleDirect(isVisible: false,
                                                                                  finalValueOperations: null);

            contentDisplayCounter++;
        }
    }


    public static void HideContainers<T_ContentDisplay>(this IReadOnlyList<T_ContentDisplay> contentDisplays_IN)
        where T_ContentDisplay : MonoBehaviour, IGUI_Animatable
    {
        for (int i = 0; i < contentDisplays_IN.Count; i++)
        {
            if (contentDisplays_IN[i].gameObject.activeSelf == false)
            {
                break;
            }
            else
            {
                contentDisplays_IN[i].ScaleDirect(isVisible: false, finalValueOperations: null);
            }
        }
    }





    public static void DeactivateUnusedContainers<T_ContentDisplay>(int requiredAmount_IN, in IReadOnlyList<T_ContentDisplay> contentDisplay)
        where T_ContentDisplay : ContentDisplay
    {
        for (int i = requiredAmount_IN; i < contentDisplay.Count; i++)
        {
            contentDisplay[i].Unload();
            if (contentDisplay[i].gameObject.activeSelf != false) contentDisplay[i].gameObject.SetActive(false);
        }
    }

    public static void SortContainers<T_ContentDisplay, T_ParentPanel>(this IReadOnlyList<T_ContentDisplay> contentDisplays_IN,
                                                                      Vector3?[] customInitialValues,
                                                                      (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)?[] secondaryInterpolations,
                                                                      int amountToSort_IN,
                                                                      int enumeratorIndex,
                                                                      T_ParentPanel parentPanel_IN,
                                                                      float[] lerpSpeedModifiers,
                                                                      WaitForSeconds customWaitInterval = null,
                                                                      Action followingAction = null)

        where T_ContentDisplay : MonoBehaviour, IGUI_Animatable
        where T_ParentPanel : MonoBehaviour, IAnimatedPanelController
    {
        if (parentPanel_IN.CO[enumeratorIndex] != null)
        {
            parentPanel_IN.StopCoroutine(parentPanel_IN.CO[enumeratorIndex]);
            parentPanel_IN.CO[enumeratorIndex] = null;
        }
        parentPanel_IN.CO[enumeratorIndex] = SortContainersRoutine(contentDisplays_IN,
                                                                   customInitialValues,
                                                                   secondaryInterpolations: secondaryInterpolations,
                                                                   amountToSort_IN,
                                                                   enumeratorIndex,
                                                                   parentPanel_IN,
                                                                   customWaitInterval is not null
                                                                      ? customWaitInterval
                                                                      : TimeTickSystem.WaitForSeconds_ContainerActivation,
                                                                   lerpSpeedmodifiers: lerpSpeedModifiers,
                                                                   followingAction: followingAction);
        parentPanel_IN.StartCoroutine(parentPanel_IN.CO[enumeratorIndex]);
    }

    private static IEnumerator SortContainersRoutine<T_ContentDisplay, T_ParentPanel>(IReadOnlyList<T_ContentDisplay> contentDisplays_IN,
                                                                                      Vector3?[] customInitialValues,
                                                                                      (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)?[] secondaryInterpolations,
                                                                                      int amountToSort_IN,
                                                                                      int enumeratorIndex,
                                                                                      T_ParentPanel parentPanel_IN,
                                                                                      WaitForSeconds customWaitInterval,
                                                                                      float[] lerpSpeedmodifiers,
                                                                                      Action followingAction)

        where T_ContentDisplay : MonoBehaviour, IGUI_Animatable
        where T_ParentPanel : MonoBehaviour, IAnimatedPanelController
    {
        while (parentPanel_IN.PanelToAwait.RunningCoroutine != null)
        {
            yield return null;
        }

        for (int i = 0; i < amountToSort_IN; i++)
        {
            contentDisplays_IN[i].gameObject.SetActive(true);

            contentDisplays_IN[i].AnimateWithRoutine(customInitialValue: customInitialValues is not null
                                                            ? customInitialValues[i]
                                                            : null,
                                                            secondaryInterpolation: secondaryInterpolations is not null
                                                            ? secondaryInterpolations[i]
                                                            : null,
                                                     isVisible: true,
                                                     lerpSpeedModifier: lerpSpeedmodifiers is not null
                                                         ? lerpSpeedmodifiers[i]
                                                         : 1f,
                                                     followingAction_IN: i == amountToSort_IN - 1
                                                         ? followingAction
                                                         : null);
            yield return customWaitInterval;
        }

        parentPanel_IN.CO[enumeratorIndex] = null;
    }



    public static void PlaceImageWithText(TextMeshProUGUI textToplace, RectTransform textToplaceRect, RectTransform imageToPlaceRect, bool isImageOnLeft, float? customSpacing = null)
    {

        var spacingBetween = customSpacing.HasValue ? customSpacing.Value : 5f;
        var textWdth = textToplace.GetPreferredValues(textToplace.text);
        textToplaceRect.sizeDelta = new Vector2(textWdth.x, textToplaceRect.sizeDelta.y);
        var totalAmount = textToplaceRect.sizeDelta.x + spacingBetween + imageToPlaceRect.sizeDelta.x;
        var diff = totalAmount - textToplaceRect.sizeDelta.x;

        if (isImageOnLeft)
        {
            textToplaceRect.anchoredPosition = new Vector2(diff / 2, textToplaceRect.anchoredPosition.y);
            imageToPlaceRect.anchoredPosition = new Vector2((textToplaceRect.anchoredPosition.x) - (textToplaceRect.rect.width / 2) - spacingBetween, imageToPlaceRect.anchoredPosition.y);
        }
        else
        {
            textToplaceRect.anchoredPosition = new Vector2(-(diff / 2), textToplaceRect.anchoredPosition.y);
            imageToPlaceRect.anchoredPosition = new Vector2((textToplaceRect.anchoredPosition.x) + (textToplaceRect.rect.width / 2) + spacingBetween, imageToPlaceRect.anchoredPosition.y);
        }
    }
}

