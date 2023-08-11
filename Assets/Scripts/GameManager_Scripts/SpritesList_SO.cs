using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "new SpritesList", menuName = "SpritesList")]
public class SpritesList_SO : ScriptableObject
{
    [SerializeField] public List<Sprite> spritesList;

    public AssetReferenceAtlasedSprite HourGlassRef { get { return _hourGlassRef; } }
    [SerializeField] private AssetReferenceAtlasedSprite _hourGlassRef;

    public AssetReferenceAtlasedSprite CoinRef { get { return _coinRef; } }
    [SerializeField] private AssetReferenceAtlasedSprite _coinRef;

    public AssetReferenceAtlasedSprite PlusSignRef { get { return _plusSignRef; } }
    [SerializeField] private AssetReferenceAtlasedSprite _plusSignRef;

    public AssetReferenceAtlasedSprite QualityIconRef { get { return _qualityIconRef; } }
    [SerializeField] private AssetReferenceAtlasedSprite _qualityIconRef;

    public AssetReferenceAtlasedSprite MultiCraftChanceIconRef { get { return _multiCraftChanceIconRef; } }
    [SerializeField] private AssetReferenceAtlasedSprite _multiCraftChanceIconRef;

    public AssetReferenceAtlasedSprite MasteredIconRef { get { return _masteredIconRef; } }
    [SerializeField] private AssetReferenceAtlasedSprite _masteredIconRef;

    public AssetReferenceAtlasedSprite NotMasteredIconRef { get { return _notMasteredIconRef; } }
    [SerializeField] private AssetReferenceAtlasedSprite _notMasteredIconRef;

    public AssetReferenceAtlasedSprite StarIconYellow { get { return _starIconYellow; } }
    [SerializeField] private AssetReferenceAtlasedSprite _starIconYellow;

    public AssetReferenceAtlasedSprite StarIconRed { get { return _starIconRed; } }
    [SerializeField] private AssetReferenceAtlasedSprite _starIconRed;

    public AssetReferenceAtlasedSprite NotificationIconBGRef { get { return _notificationIconBGRef; } }
    [SerializeField] private AssetReferenceAtlasedSprite _notificationIconBGRef;

    public AssetReferenceAtlasedSprite TokenIcon { get { return _tokenIcon; } }
    [SerializeField] private AssetReferenceAtlasedSprite _tokenIcon;
    public AssetReferenceAtlasedSprite EnergyIcon { get { return _energyIcon; } }
    [SerializeField] private AssetReferenceAtlasedSprite _energyIcon;

    public AssetReferenceAtlasedSprite GemIcon { get { return _gemIcon; } }
    [SerializeField] private AssetReferenceAtlasedSprite _gemIcon;

    public AssetReferenceAtlasedSprite AttackIcon { get { return _attackIcon; } }
    [SerializeField] private AssetReferenceAtlasedSprite _attackIcon;

    public AssetReferenceAtlasedSprite DefenseIcon { get { return _defenseIcon; } }
    [SerializeField] private AssetReferenceAtlasedSprite _defenseIcon;

    public AssetReferenceAtlasedSprite HpIcon { get { return _hpIcon; } }
    [SerializeField] private AssetReferenceAtlasedSprite _hpIcon;

    public AssetReferenceAtlasedSprite CriticalIcon { get { return _crriticalIcon; } }
    [SerializeField] private AssetReferenceAtlasedSprite _crriticalIcon;

    public AssetReferenceAtlasedSprite EvadeIcon { get { return _evadeIcon; } }
    [SerializeField] private AssetReferenceAtlasedSprite _evadeIcon;

    public AssetReferenceAtlasedSprite RotisseurIcon => _rotisseurIcon;
    [SerializeField] private AssetReferenceAtlasedSprite _rotisseurIcon;

    public AssetReferenceAtlasedSprite EntremetienrIcon => _entremetierIcon;
    [SerializeField] private AssetReferenceAtlasedSprite _entremetierIcon;

    public AssetReferenceAtlasedSprite PoissonierIcon => _poissonierIcon;
    [SerializeField] private AssetReferenceAtlasedSprite _poissonierIcon;

    public AssetReferenceAtlasedSprite PatisserIcon => _patisserIcon;
    [SerializeField] private AssetReferenceAtlasedSprite _patisserIcon;

    public AssetReferenceAtlasedSprite FastFooderIcon => _fastFooderIcon;
    [SerializeField] private AssetReferenceAtlasedSprite _fastFooderIcon;

    public AssetReferenceAtlasedSprite LegumierIcon => _legumierIcon;
    [SerializeField] private AssetReferenceAtlasedSprite _legumierIcon;

    public AssetReferenceAtlasedSprite PotagerIcon => _potagerIcon;
    [SerializeField] private AssetReferenceAtlasedSprite _potagerIcon;

    public AssetReferenceAtlasedSprite MasterChefIcon => _masterChefIcon;
    [SerializeField] private AssetReferenceAtlasedSprite _masterChefIcon;

    public AssetReferenceT<Sprite> HirecharactersPanelBGImage => _hirecharactersPanelBGImage;
    [SerializeField] private AssetReferenceT<Sprite> _hirecharactersPanelBGImage;

}
