using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public interface IValueUpgradable : IValuable
{
     IEnumerable<(string bonusName, string bonusAmount)> ValueIncreaseModifierStrings { get; }

     public bool IsValueModified { get; }
     ToolTipInfo GetToolTipTextForValueModifiers(string header = null, string footer = null)
        => new(bodytextAsColumns: GetToolTipTextForValueModifiers(), header: header, footer: footer);
     private string[] GetToolTipTextForValueModifiers()
    {
        StringBuilder sb1 = new();
        StringBuilder sb2 = new();
        StringBuilder sb3 = new();

        foreach (var (modifier,flags) in ValueIncreaseModifierStrings.WithPositions())
        {
            sb1.Append(modifier.bonusName);
            sb2.Append(modifier.bonusAmount);
            sb3.Append("deneme");

            if((flags & FunctionalHelpers.PositionFlags.Last) != FunctionalHelpers.PositionFlags.Last)
            {
                sb1.AppendLine();
                sb2.AppendLine();
                sb3.AppendLine();
            }
        }
        /*var lastModifierItem = ValueIncreaseModifierStrings.Last();

        foreach (var modifier in ValueIncreaseModifierStrings)
        {
            sb1.Append(modifier.bonusName);
            sb2.Append(modifier.bonusAmount);
            sb3.Append("deneme");

            if (!modifier.Equals(lastModifierItem))
            {
                sb1.AppendLine();
                sb2.AppendLine();
                sb3.AppendLine();
            }

        }*/
        return new string[] { sb1.ToString(), sb2.ToString() , sb3.ToString()};
    }
}
