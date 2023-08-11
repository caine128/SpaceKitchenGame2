using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip_Panel_Manager : Panel_Base, IPointerDownHandler, IPointerUpHandler
{
    private Image backgroundImage;
    [SerializeField] private RectTransform _rT_Background;
    [SerializeField] private List<TextMeshProUGUI> tooltipTextFields;
    [SerializeField] private TextMeshProUGUI toolTipHeader;
    [SerializeField] private TextMeshProUGUI toolTipFooter;
    [SerializeField] private UnityEngine.GameObject tooltipTexField_Pf;
    private Canvas toolTipCanvas;

    //[SerializeField] TextMeshProUGUI tooltipText_Primary;
    //[SerializeField] TextMeshProUGUI tooltipText_Secondary;

    private void Awake()
    {      
        backgroundImage = GetComponent<Image>();
        backgroundImage.raycastTarget = false;
        toolTipCanvas = GetComponentInParent<Canvas>();

        if (tooltipTexField_Pf is null || tooltipTextFields is null || tooltipTextFields.Count < 2 || toolTipHeader is null || toolTipFooter is null || _rT_Background is null || toolTipCanvas is null)
        {
            throw new NullReferenceException();
        }
    }

    public void SetDynamicBGSize(ToolTipInfo toolTipInfo, RectTransform clickedObjectRT) //    string header, string[] tooltipInfoAsColumns, string footer, RectTransform clickedObjectRT)
    {
        var amountOfTextColumns = toolTipInfo.bodyTextAsColumns?.Length ?? 0;
        var missingFields = amountOfTextColumns - tooltipTextFields.Count;
        if (missingFields > 0)
            CreateAndAddNewFieds(missingFields);

        (bool isThereHeader, bool isThereFooter) = (!toolTipInfo.header.IsNullOrWhiteSpaceOrEmpty(), !toolTipInfo.footer.IsNullOrWhiteSpaceOrEmpty());


        ///Getting The Message WidthAndHeight
        ///And Setting The Text And SizeDelta
        Vector2 totalFieldsSize = Vector2.zero;
        float totalHorizontalOffset = 0f;
        for (int i = 0; i < amountOfTextColumns; i++)
        {
            var preferredFieldSize = tooltipTextFields[i].GetPreferredValues(toolTipInfo.bodyTextAsColumns[i]);         
            totalFieldsSize = new Vector2(totalFieldsSize.x + preferredFieldSize.x, MathF.Max(totalFieldsSize.y, preferredFieldSize.y));

            tooltipTextFields[i].rectTransform.sizeDelta = new Vector2(preferredFieldSize.x, totalFieldsSize.y);
            tooltipTextFields[i].SetText(sourceText: toolTipInfo.bodyTextAsColumns[i]);
        }
        ///Clearing Unused Fields
        foreach (var unusedField in tooltipTextFields.Where((field, index) => index >= amountOfTextColumns))
        {
            unusedField.SetText(string.Empty);
        }
        if (!isThereHeader) toolTipHeader.SetText(string.Empty);
        if (!isThereFooter) toolTipFooter.SetText(string.Empty);
        ///Getting The HeaderAndFooter Height
        ///And Setting The Text And SizeDelta
        if (isThereHeader)
        {
            var preferredFieldSize = toolTipHeader.GetPreferredValues(text: toolTipInfo.header);
            //var preferredHeaderHeight = toolTipHeader.GetPreferredValues(text: toolTipInfo.header, width: totalFieldsSize.x - toolTipHeader.margin.x*2, height:0).y;
            totalHorizontalOffset = preferredFieldSize.x - totalFieldsSize.x;
            totalFieldsSize = new Vector2(Mathf.Max(totalFieldsSize.x, preferredFieldSize.x), totalFieldsSize.y + preferredFieldSize.y);

            toolTipHeader.rectTransform.sizeDelta = new Vector2(totalFieldsSize.x, preferredFieldSize.y);
            toolTipHeader.SetText(sourceText: toolTipInfo.header);

           
        }
        if (isThereFooter)
        {
            var preferredFooterHeight = toolTipFooter.GetPreferredValues(text: toolTipInfo.footer, width: totalFieldsSize.x - toolTipHeader.margin.x * 2, height: 0).y;
            totalFieldsSize = new Vector2(totalFieldsSize.x, totalFieldsSize.y + preferredFooterHeight);

            toolTipFooter.rectTransform.sizeDelta = new Vector2(totalFieldsSize.x, preferredFooterHeight);
            toolTipFooter.SetText(sourceText: toolTipInfo.footer);
        }

        var offsetForEach = totalHorizontalOffset > 0 ? totalHorizontalOffset / (amountOfTextColumns - 1) : 0;

        if (isThereHeader) toolTipHeader.rectTransform.anchoredPosition = Vector2.zero;
        tooltipTextFields[0].rectTransform.anchoredPosition = new Vector2 (0f, isThereHeader ? -toolTipHeader.rectTransform.rect.height : 0f);       
        for (int i = 1; i < amountOfTextColumns; i++)
        {
            tooltipTextFields[i].rectTransform.anchoredPosition = new Vector2(offsetForEach + tooltipTextFields[i - 1].rectTransform.anchoredPosition.x + tooltipTextFields[i - 1].rectTransform.rect.width, tooltipTextFields[0].rectTransform.anchoredPosition.y);
        }
        if (isThereFooter) toolTipFooter.rectTransform.anchoredPosition = new Vector2(x:0f, 
                                                                                      y: amountOfTextColumns > 0
                                                                                            ? tooltipTextFields[toolTipInfo.bodyTextAsColumns.Length-1].rectTransform.anchoredPosition.y - tooltipTextFields[toolTipInfo.bodyTextAsColumns.Length-1].rectTransform.rect.height
                                                                                            : isThereHeader 
                                                                                                 ? -toolTipHeader.rectTransform.rect.height
                                                                                                 : 0f);


        _rT_Background.sizeDelta = totalFieldsSize;
        _rT_Background.position = FindTooltipPosition(clickedObjectRT); ;
        backgroundImage.raycastTarget = true;


        ////var totalTextBoxSize = Vector2.zero;
        //float totalTextBoxWidth = 0;
        //for (int i = 0; i < tooltipInfoAsColumns.Length; i++)
        //{
        //    tooltipTextFields[i].SetText(tooltipInfoAsColumns[i]);
        //    tooltipTextFields[i].ForceMeshUpdate();
        //    var textSize = tooltipTextFields[i].GetRenderedValues();

        //    tooltipTextFields[i].rectTransform.sizeDelta = new Vector2(textSize.x + (tooltipTextFields[i].margin.x * 2), textSize.y + (tooltipTextFields[i].margin.y * 2));
        //    //tooltipTextFields[i].rectTransform.anchoredPosition = i > 0 ? new Vector2(tooltipTextFields[i - 1].rectTransform.anchoredPosition.x + tooltipTextFields[i - 1].rectTransform.rect.width, 0) : Vector2.zero;

        //    //totalTextBoxSize = new Vector2(totalTextBoxSize.x + tooltipTextFields[i].rectTransform.rect.width, MathF.Max(totalTextBoxSize.y, tooltipTextFields[i].rectTransform.rect.height));
        //    totalTextBoxWidth += tooltipTextFields[i].rectTransform.rect.width;
        //}

        ///*for (int i = tooltipInfoAsColumns.Length; i < tooltipTextFields.Count; i++)
        //{
        //    tooltipTextFields[i].SetText(string.Empty);
        //}*/

        //var totalTextBoxHeight = PlaceFieldsAndGetVerticalSize();
        //Vector2 totalTextBoxSize = new Vector2(totalTextBoxWidth, totalTextBoxHeight);
        ///*tooltipText_Primary.SetText(tooltipText_IN.text1);
        //tooltipText_Secondary.SetText(tooltipText_IN.text2);
        //tooltipText_Primary.ForceMeshUpdate();

        //var text1Size = tooltipText_Primary.GetRenderedValues(onlyVisibleCharacters: false);
        //var text2Size = tooltipText_Secondary.GetRenderedValues(onlyVisibleCharacters: false);
        //var totalTextBoxSize = new Vector2(text1Size.x + text2Size.x + tooltipText_Primary.margin.x * 2, MathF.Max(text1Size.y, text2Size.y) + tooltipText_Primary.margin.y * 2);

        //tooltipText_Primary.SetText(tooltipText_IN);
        //tooltipText_Primary.ForceMeshUpdate();
        //var textSize = tooltipText_Primary.GetRenderedValues(onlyVisibleCharacters: false) + new Vector2(tooltipText_Primary.margin.x * 2, tooltipText_Primary.margin.y * 2);

        //var (position, pivots) = FindTooltipPosition(clickedObjectRT);*/

        //_rT_Background.sizeDelta = totalTextBoxSize;
        //_rT_Background.position = FindTooltipPosition(clickedObjectRT); ;
        //backgroundImage.raycastTarget = true;

        /*_rT_Background.pivot = pivots;
        gameObject.SetActive(true);*/

        /*
        float PlaceFieldsAndGetVerticalSize()
        {
            float currentVerticalPos = 0f;
            if (!string.IsNullOrEmpty(header) && !string.IsNullOrWhiteSpace(header))
            {
                SetHeaderFooter(header, totalTextBoxWidth);
                toolTipHeader.rectTransform.anchoredPosition = Vector2.zero;
                currentVerticalPos -= toolTipHeader.rectTransform.rect.height;
            }

            float currentHorizontalPos = 0f;
            for (int i = 0; i < tooltipInfoAsColumns.Length; i++)
            {
                tooltipTextFields[i].rectTransform.anchoredPosition = new Vector2(currentVerticalPos, currentHorizontalPos);
                currentHorizontalPos += tooltipTextFields[i].rectTransform.rect.width;
            }

            currentVerticalPos -= tooltipTextFields.Max(field => field.rectTransform.rect.height);

            if (!string.IsNullOrEmpty(footer) && !string.IsNullOrWhiteSpace(footer))
            {
                SetHeaderFooter(footer, totalTextBoxWidth);
                toolTipFooter.rectTransform.anchoredPosition = new Vector2(0f, currentVerticalPos);
                currentVerticalPos -= toolTipFooter.rectTransform.rect.height;
            }

            return Mathf.Abs(currentVerticalPos);
        }
        */

    }
    
    private void CreateAndAddNewFieds(int missingFields)
    {
        for (int i = 0; i < missingFields; i++)
        {
            var newTextField = UnityEngine.GameObject.Instantiate(tooltipTexField_Pf).GetComponent<TextMeshProUGUI>();
            newTextField.transform.SetParent(_rT_Background);
            newTextField.rectTransform.localScale = Vector3.one;
            newTextField.name = $"textField{tooltipTextFields.Count + 1 + i}";
            tooltipTextFields.Add(newTextField);
        }
    }
    /*
    private void SetHeaderFooter(string headerAndOrFooter, float totalTextBoxWidth)
    {
        toolTipHeader.rectTransform.sizeDelta = new Vector2(totalTextBoxWidth, 0);
        toolTipHeader.SetText(headerAndOrFooter);
        toolTipHeader.ForceMeshUpdate();
        toolTipHeader.rectTransform.sizeDelta = new Vector2(toolTipHeader.rectTransform.sizeDelta.x, toolTipHeader.GetRenderedValues().y);
    }*/


    private Vector2 FindTooltipPosition(RectTransform clickedObjectRT)
    {
        float verticalOffset = 5f;
        var corners = new Vector3[4];
        clickedObjectRT.GetWorldCorners(corners);

        var clickedObjectPos = clickedObjectRT.position;
        float midPoint = (corners[1].x + corners[2].x) / 2;
        (bool isPastCenterX, bool isPastCenterY) screenBoundsCheck = (Screen.width / 2 < clickedObjectPos.x, Screen.height / 2 < clickedObjectPos.y);

        return screenBoundsCheck switch
        {
            (true, true) => new Vector2(midPoint - (_rT_Background.sizeDelta.x / 2) * toolTipCanvas.scaleFactor, corners[0].y - (verticalOffset + _rT_Background.sizeDelta.y / 2) * toolTipCanvas.scaleFactor),
            (true, false) => new Vector2(midPoint - (_rT_Background.sizeDelta.x / 2) * toolTipCanvas.scaleFactor, corners[1].y + (verticalOffset + _rT_Background.sizeDelta.y / 2) * toolTipCanvas.scaleFactor),
            (false, false) => new Vector2(midPoint + (_rT_Background.sizeDelta.x / 2) * toolTipCanvas.scaleFactor, corners[2].y + (verticalOffset + _rT_Background.sizeDelta.y / 2) * toolTipCanvas.scaleFactor),
            (false, true) => new Vector2(midPoint + (_rT_Background.sizeDelta.x / 2) * toolTipCanvas.scaleFactor, corners[3].y - (verticalOffset + _rT_Background.sizeDelta.y / 2) * toolTipCanvas.scaleFactor),
        };
    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        backgroundImage.raycastTarget = false;
        PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null);
    }
}


