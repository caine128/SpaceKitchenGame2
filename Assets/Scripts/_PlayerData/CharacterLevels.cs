using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterLevels 
{
    //public int currentLevel;
    public readonly int nextLevelXP;
    public readonly int newEnergyMax;
    public readonly PlayerLevelledEventArgs playerLevelledEventArgs;
    private readonly int _currentLevel;
    public readonly struct PlayerLevelledEventArgs
    {
        public readonly IEnumerable<Character_SO> charactersUnlocked;
        public PlayerLevelledEventArgs(IEnumerable<Character_SO> charactersUnlocked_IN)
        {
            charactersUnlocked = charactersUnlocked_IN;
        }   
    }

    public CharacterLevels(int currentLevel)
    {
        _currentLevel = currentLevel;
        nextLevelXP = GetXPTonextLevel();
        newEnergyMax = GetEnergyMaxNextLevel();
        playerLevelledEventArgs = GetNewLevelUnlocks();
    }

    private int GetXPTonextLevel()
    {
        switch (_currentLevel)
        {
            case 1: 
                return 100;              
            case 2: 
                return 130;
            case 3:
                return 180;
            case 4:
                return 250;
            case 5:
                return 320;

            default: return 0 ;

        }
    }

    private int GetEnergyMaxNextLevel()
    {
        switch (_currentLevel)
        {
            case 1:
                return 60;
            case 2:
                return 120;
            case 3:
                return 190;
            case 4:
                return 210;
            case 5:
                return 230;

            default: return 0;

        }
    }

    private PlayerLevelledEventArgs GetNewLevelUnlocks()
    {
        var charactersUnlocked = CharacterManager.Instance.WorkerList_SO.listOfWorkers.Where(cso => cso.requiredLevel <= _currentLevel && cso.requiredLevel != 1);
        return new PlayerLevelledEventArgs(charactersUnlocked);
    }

}
