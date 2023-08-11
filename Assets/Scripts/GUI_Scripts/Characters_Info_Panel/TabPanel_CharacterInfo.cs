using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TabPanel_CharacterInfo : TabPanel<Tab.CharacterInfoTabs>
{
    public override Tab.CharacterInfoTabs TabType => _tabType;
    [SerializeField] private Tab.CharacterInfoTabs _tabType;

    [SerializeField] private TextMeshProUGUI characterDescription;
    [SerializeField] private TextMeshProUGUI bonusValue;
    [SerializeField] private AdressableImage bonusIcon_Adressable;
    [SerializeField] private InvokeWorkStationInfoButton invokeWorkStationInfoButton;

    public override void LoadInfo()
    {
        var selectedRecipe = CharactersInfoPanel_Manager.Instance.SelectedRecipe;
        characterDescription.text = selectedRecipe.GetDescription();

        (string retStr, AssetReferenceT<Sprite> retSprt) = selectedRecipe switch
        {
            Worker worker =>
            ($"{MethodHelper.GetValueStringPercent(worker.GetCraftTimeReduction())} == {MethodHelper.GiveRichTextString_Color(Color.green)}{MethodHelper.GetValueStringPercent(worker.GetCraftTimeReduction(worker.GetLevel() + 1))}{MethodHelper.GiveRichTextString_ClosingTagOf("color")}", ImageManager.SelectSprite(worker.workerspecs.workerType.ToString())),
            _ => throw new System.NotImplementedException()
        };
        bonusValue.text = retStr;
        bonusIcon_Adressable.LoadSprite(retSprt);

        var workStationName = MethodHelper.GetNameOfWorkStationType(((Worker)selectedRecipe).workerspecs.workStationPrerequisites[0].type);
        invokeWorkStationInfoButton.SetButtonText($"Invest in {Environment.NewLine}{workStationName}");
    }

    public override void UnloadInfo()
    {
        bonusIcon_Adressable.UnloadSprite();
    }
}
