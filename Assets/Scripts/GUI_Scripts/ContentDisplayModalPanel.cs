using System;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContentDisplayModalPanel : ContentDisplay, IPointerDownHandler, IPointerUpHandler, IPlacableRt //, ILoadable<ContentDisplayInfo_Modal>
{
    private SortableBluePrint blueprint;
    [SerializeField] private TextMeshProUGUI contentInfo;
    private GUI_LerpMethods_Int _gUI_LerpMethods_Int;

    public RectTransform RT => _rt;
    private RectTransform _rt;

    public RectTransform RTImageComponent { get => _rtsToMove[0]; }

    private (Vector2 originalSize, Vector2 originalPos) _imageComp;
    private RectTransform _rtTextComponent;
    public Vector2 OriginalSizeWideContainer { get => _originalSizeWideContainer; }
    private Vector2 _originalSizeWideContainer;
    public Vector2 OriginalPosition => _originalPosition;
    private Vector2 _originalPosition;

    private DynamicShape currentShape;

    private Mask _mask;
    private Image _maskImage;


    public RectTransform[] RTsToMove
    {
        get => _rtsToMove;
    }
    private RectTransform[] _rtsToMove;

    public enum DynamicShape
    {
        Default = 0,
        SubContainer_Wide = 1,
        Subcontainer_Square_Small = 2,
        Subcontainer_Square_Big = 3,
        Maincontainer = 4,
        SubContainer_Wide_Small = 5,
    }


    protected override void Awake()
    {
        base.Awake();
        _gUI_LerpMethods_Int = GetComponent<GUI_LerpMethods_Int>();

        _rt = GetComponent<RectTransform>();
        _mask = GetComponentInChildren<Mask>();
        _maskImage = _mask.GetComponent<Image>();

        _originalSizeWideContainer = _rt.rect.size;
        _originalPosition = _rt.anchoredPosition;
        _imageComp.originalSize = adressableImageContainers[0].rectTransform.rect.size;
        _imageComp.originalPos = adressableImageContainers[0].rectTransform.anchoredPosition;
        _rtTextComponent = contentInfo.rectTransform;

        _rtsToMove = new RectTransform[]
        {
            adressableImageContainers[0].GetComponent<RectTransform>(),
            adressableImageContainers[1].GetComponent<RectTransform>(),
            _rtTextComponent,
        };


    }



    public override void Load(ContentDisplayInfo info)
    {
        Load((ContentDisplayInfo_Modal)info);
    }

    public void Load(ContentDisplayInfo_Modal info)   // could it be privatre ????!!
    {
        blueprint = info.mainBluePrint_IN;

        ChangeDynamicShape(info.dynamicShape);

        if (contentInfo.gameObject.activeInHierarchy &&
          (string.IsNullOrEmpty(info.contentTextMain_IN) && string.IsNullOrEmpty(info.contentTextSecondary_IN)))
        {
            contentInfo.gameObject.SetActive(false);
        }

        else if (!contentInfo.gameObject.activeInHierarchy &&
          (!string.IsNullOrEmpty(info.contentTextMain_IN) || !string.IsNullOrEmpty(info.contentTextSecondary_IN)))
        {
            contentInfo.gameObject.SetActive(true);
        }

        contentInfo.text = info.contentTextSecondary_IN is not null
        ? NativeHelper.BuildString_Append(info.contentTextMain_IN,
          Environment.NewLine,
          currentShape == DynamicShape.SubContainer_Wide
            ? MethodHelper.GiveRichTextString_Color(color_IN: Color.yellow)
            : MethodHelper.GiveRichTextString_Color(color_IN: Color.white),
          info.contentTextSecondary_IN)
        : info.contentTextMain_IN;

        SelectAdressableSpritesToLoad(blueprint != null
            ? blueprint.GetAdressableImage()
            : info.spriteRef_IN);

        if (_mask.enabled != info.isMaskActive) _mask.enabled = _maskImage.enabled = info.isMaskActive;
        SetBackgroundColor(info.bgColor);
    }

    //public void ModifyText(string newText)
    //{
    //    contentInfo.text = newText;
    //}

    public async Task ModifyText(string identifier, int oldValue, int newValue, float lerpSpeedModifier)
    {
        float elapsedTime = 0;
        var stringBuilder = new StringBuilder(capacity: identifier.Length + newValue.ToString().Length);
        stringBuilder.Append(identifier);
        while (elapsedTime < TimeTickSystem.NUMERIC_LERPDURATION * lerpSpeedModifier)
        {
            stringBuilder.Remove(startIndex: identifier.Length, length: stringBuilder.Length - identifier.Length);
            stringBuilder.AppendLine();
            stringBuilder.Append(ISpendable.ToScreenFormat(Mathf.RoundToInt(Mathf.Lerp(oldValue, newValue, elapsedTime / (TimeTickSystem.NUMERIC_LERPDURATION * lerpSpeedModifier)))));//.ToString()) ;
            contentInfo.text = stringBuilder.ToString();

            elapsedTime += Time.deltaTime;

            await AsyncHelper.WaitForEndOfFrameAsync();
        }

        stringBuilder.Remove(startIndex: identifier.Length, length: stringBuilder.Length - identifier.Length);
        stringBuilder.AppendLine();

        var resizeTask = AsyncHelper.ResizeText(contentInfo, 1.2f, 1f, true);
        stringBuilder.Append(MethodHelper.GiveRichTextString_Color(Color.green)+ISpendable.ToScreenFormat(newValue));
        contentInfo.text = stringBuilder.ToString();

        await resizeTask;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (blueprint is not null)
        {
            Debug.Log(blueprint.GetName());
        }
    }



    private void ChangeDynamicShape(DynamicShape shape_IN)
    {
        switch (currentShape == shape_IN, shape_IN)
        {

            case (_, DynamicShape.Maincontainer):
                SetBackToOriginalPosition();
                break;

            case (false, DynamicShape.Subcontainer_Square_Small):
                currentShape = shape_IN;

                var size = _originalSizeWideContainer.y;
                _rt.sizeDelta = new Vector2(size, size);
                RTImageComponent.sizeDelta = _imageComp.originalSize;
                RTImageComponent.anchoredPosition = new Vector2(_rt.rect.width / 2f, _rt.rect.height / 2f);
                _rtTextComponent.sizeDelta = new Vector2(size, size);
                contentInfo.margin = new Vector4(0, 20, 0, 0);

                break;
            case (false, DynamicShape.Subcontainer_Square_Big):
                currentShape = shape_IN;

                _rt.sizeDelta = new Vector2(_originalSizeWideContainer.y * 2, _originalSizeWideContainer.y * 2);
                RTImageComponent.sizeDelta = _imageComp.originalSize * 2;
                RTImageComponent.anchoredPosition = new Vector2(_rt.rect.width / 2, 0);
                _rtTextComponent.sizeDelta = new Vector2(_originalSizeWideContainer.y, _originalSizeWideContainer.y);
                contentInfo.margin = new Vector4(0, 0, 0, 0);

                break;

            case (false, DynamicShape.SubContainer_Wide):
                currentShape = shape_IN;

                _rt.sizeDelta = _originalSizeWideContainer;
                RTImageComponent.sizeDelta = _imageComp.originalSize;
                RTImageComponent.anchoredPosition = new Vector2(_imageComp.originalPos.x, 0);
                _rtTextComponent.sizeDelta = _originalSizeWideContainer;
                contentInfo.margin = new Vector4(_originalSizeWideContainer.y, 0, 0, 0);
                break;

            case (false, DynamicShape.SubContainer_Wide_Small):
                currentShape = shape_IN;

                _rt.sizeDelta = new Vector2(_originalSizeWideContainer.x / 2, _originalSizeWideContainer.y);
                RTImageComponent.sizeDelta = _imageComp.originalSize;
                RTImageComponent.anchoredPosition = new Vector2(_imageComp.originalPos.x, 0);
                _rtTextComponent.sizeDelta = _originalSizeWideContainer;
                contentInfo.margin = new Vector4(_originalSizeWideContainer.y, 0, 0, 0);
                break;
            case (false, DynamicShape.Default):
            default:
                currentShape = shape_IN;

                break;
        }
    }

    private void SetBackgroundColor(Color color)
    {
        switch (color, adressableImageContainers[1].gameObject.activeInHierarchy)
        {
            case (var c, true) when c == default:
                adressableImageContainers[1].gameObject.SetActive(false);
                break;
            case (var c, var b) when c != default:
                if (b != true) adressableImageContainers[1].gameObject.SetActive(true);
                adressableImageContainers[1].color = c;
                break;
        }
    }

    private void SetBackToOriginalPosition()
    {
        if (_rt.anchoredPosition != this._originalPosition)
        {
            _rt.anchoredPosition = this._originalPosition;
        }
    }


    public override void Unload()
    {
        if (contentInfo.text != null) contentInfo.text = null;
        UnloadAdressableSprite();
    }
}
