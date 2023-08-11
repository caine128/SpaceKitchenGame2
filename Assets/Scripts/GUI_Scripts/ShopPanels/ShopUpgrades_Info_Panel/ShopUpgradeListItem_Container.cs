using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Quality;
using static ShopUpgrades_SO;

public class ShopUpgradeListItem_Container : Container<SortableBluePrint_ExtractedData<ShopUpgrade>>
{
    public override RectTransform rt { get => _rect; }
    [SerializeField] private RectTransform _rect;

    [SerializeField] private TextMeshProUGUI _label1;
    [SerializeField] private TextMeshProUGUI _label2;
    [SerializeField] private TextMeshProUGUI _label3;
    [SerializeField] private TextMeshProUGUI _label4;
    [SerializeField] private AdressableImage _optionalImage;

    public override void LoadContainer(SortableBluePrint_ExtractedData<ShopUpgrade> extractedData)
    {
        bluePrint = extractedData;
        switch (bluePrint.BluePrint)
        {
            case WorkStationUpgrade workStationUpgrade:
                var (charName, sprite, level, maxWorkerLevelCap) = ((string charName, AssetReferenceAtlasedSprite sprite, int level,int maxWorkerLevelCap))bluePrint.Data;
                mainImageContainer.LoadSprite(sprite);

                var isAtCurrentLevel = workStationUpgrade.GetLevel() == level;
                var isAlreadyReceived = workStationUpgrade.GetLevel() > level;

                containerImage.color = isAtCurrentLevel
                                            ? Color.yellow
                                            : isAlreadyReceived
                                                    ? Color.grey
                                                    : Color.cyan;

                _label1.text = $"Level {maxWorkerLevelCap}";
                _label2.text = $"Max Level for {charName}";              
                if (isAlreadyReceived)
                {
                    _label3.gameObject.SetActive(false);
                    _label4.gameObject.SetActive(false);
                    _optionalImage.gameObject.SetActive(true);
                    _label3.text = null;
                    _label4.text = null;
                }
                else
                {
                    _label3.gameObject.SetActive(true);
                    _label4.gameObject.SetActive(true);
                    _optionalImage.gameObject.SetActive(false);
                    _label3.text = $"Required {workStationUpgrade.GetName()} Level";
                    _label4.text = $"Level {level}";
                }                                     
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    public override void MatchContainerDynamicInfo()
    {
        Debug.LogError("shouldnt be working");
        throw new System.NotImplementedException();
    }

    public override void UnloadContainer()
    {
        mainImageContainer.UnloadSprite();
        _optionalImage.UnloadSprite();

        _label1.text = null;
        _label2.text = null;
        _label3.text = null;
        _label4.text = null;
    }
}
