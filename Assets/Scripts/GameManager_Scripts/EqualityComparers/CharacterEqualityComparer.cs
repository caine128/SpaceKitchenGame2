using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEqualityComparer : IEqualityComparer<Character>
{
    public bool Equals(Character character_x, Character character_y)
    {
        if (character_x == null && character_y == null)
        {
            return true;
        }
        else if (character_x == null || character_y == null)
        {
            return false;
        }
        else if (character_x.GetName().Equals(character_y.GetName(), System.StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetHashCode(Character character__IN)
    {
        return character__IN.GetName().GetHashCode();
    }
}
