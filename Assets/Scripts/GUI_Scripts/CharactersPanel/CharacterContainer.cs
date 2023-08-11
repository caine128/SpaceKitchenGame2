using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainer : Container<Character>
{
    public override RectTransform rt => _rect;
    [SerializeField] private RectTransform _rect;

    public TextMeshProUGUI Name => _name;
    [SerializeField] TextMeshProUGUI _name;

    public TextMeshProUGUI Level => _level;
    [SerializeField] TextMeshProUGUI _level;

    [SerializeField] private TextMeshProUGUI descriptionHeaderText;

    [SerializeField] private ContentDisplay_JustSprite descriptionSprite;

    [SerializeField] private TextMeshProUGUI descriptionText;
    private RectTransform descriptionTextRect;

    [SerializeField] private TextMeshProUGUI progressBarText;
    [SerializeField] private Image progressBarBG;
    [SerializeField] private Image progressBarFG;

    private void Awake()
    {
        descriptionTextRect = descriptionText.GetComponent<RectTransform>();
    }

    public override void LoadContainer(Character newChar_IN)
    {
        bluePrint = newChar_IN;

        mainImageContainer.LoadSprite(newChar_IN.GetAdressableImage());
        _name.text = newChar_IN.GetName();

        var charLevel = newChar_IN.GetLevel();
        _level.text = charLevel >= CharacterManager.MaxLevel ? "Max Level" : charLevel.ToString();

        switch (newChar_IN)
        {
            case Commander:
                break;
            case Worker worker:
                
                descriptionHeaderText.text = MethodHelper.GetNameOfWorkerType(worker.workerspecs.workerType);
                
                var workerProfessionSprite = ImageManager.SelectSprite(worker.workerspecs.workerType.ToString());
                descriptionSprite.Load(new ContentDisplayInfo_JustSprite(workerProfessionSprite));
                
                switch (worker.isHired)
                {
                    case true:
                        if (descriptionText.gameObject.activeInHierarchy != true) descriptionText.gameObject.SetActive(true);
                        if (containerImage.color != Color.white) containerImage.color = Color.white;
                        descriptionText.text = MethodHelper.GetValueStringPercent(worker.GetCraftTimeReduction());
                        GUI_CentralPlacement.PlaceImageWithText(descriptionText, descriptionTextRect, descriptionSprite.RT, true);
                        break;

                    case false:
                        if (descriptionText.gameObject.activeInHierarchy != false) descriptionText.gameObject.SetActive(false);
                        if (containerImage.color != Color.grey) containerImage.color = Color.grey;
                        descriptionSprite.RT.anchoredPosition = new Vector2(descriptionSprite.RT.rect.width / 2, descriptionSprite.RT.anchoredPosition.y);

                        break;
                }

                //descriptionText.text = MethodHelper.GetValueStringPercent(worker.GetCraftTimeReduction());
                //GUI_CentralPlacement.PlaceImageWithText(descriptionText, descriptionTextRect, descriptionSprite.RT, true);

                var returnTuple = worker.GetProgressionStatus();
                progressBarText.text = returnTuple.retStr;
                progressBarFG.fillAmount *= returnTuple.progVal;

                break;
        }

    }

    public override void MatchContainerDynamicInfo()
    {
        throw new System.NotImplementedException();
    }

    public override void UnloadContainer()
    {
        mainImageContainer.UnloadSprite();
        _name.text = null;
        _level.text = null;

        descriptionSprite.Unload();
        descriptionText.text = null;
    }
}
