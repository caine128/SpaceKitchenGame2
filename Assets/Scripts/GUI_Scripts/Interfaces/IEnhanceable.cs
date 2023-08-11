using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public interface IEnhanceable : IRankable,IQualitative, IValueUpgradable, ICraftable, IStatUpgradable
{
    ReadOnlyDictionary<EnhancementType.Type, Enhancement> enhancementsDict_ro { get; }
    bool isEnhanced { get; }
    bool CanEnhanceWith(EnhancementType.Type enhancementType_IN);
    Product TryEnhance(Enhancement enhancement_IN, out bool isEnhancementSuccessful);
    Product DestroyEnhancement(Enhancement enhancement_IN);
}
