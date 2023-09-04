using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

public class StatsData : MonoBehaviour   ///// MUST BE SINGLETON  /////
{

    #region Singleton Syntax

    private static StatsData _instance;
    public static StatsData Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    #endregion

    public int CurrentLevel
    {
        get => _currentLevel;
    }
    private int _currentLevel = 1;
    private int experienceExisting = 20;
    private int experienceMax;
    private int energyExisting = 50;
    private int energyMax;
    private static readonly ISpendable goldObject = new Gold(30); 
    private static readonly ISpendable gemObject = new Gem(750);


    private const float LERP_SPEED_XP = 30f;
    private const float LERP_SPEED_ENERGYREFILL = 1/3f;

    private bool energyRefillSubscribed = false;

    public static event EventHandler<CharacterLevels.PlayerLevelledEventArgs> OnPlayerLevelled;
    public static event Action<int> OnGoldAmountChanged;
    public static event Action<int> OnGemAmountChanged;
  
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if (Instance == null)
                {
                    _instance = this; // LATER TO ADD DONTDESTROY ON LOAD
                }
            }
        }
    }

    private void OnEnable()
    {
        SceneController.Instance.OnSceneLoaded += Init;
        
    }

    private void OnDisable()
    {
        SceneController.Instance.OnSceneLoaded -= Init;
        TimeTickSystem.onTickTriggered -= SetEnergyExisting;
    }


    private void Init(object sender, SceneController.OnSceneLoadedEventArgs e)
    {        
        SetInitialStats();
    }

    private void SetInitialStats()
    {
        CharacterLevels initialValues = new CharacterLevels(_currentLevel);
        experienceMax = initialValues.nextLevelXP;
        energyMax = initialValues.newEnergyMax;

        SetCharacterLevel(levelReceived:0);
        SetExperience(experienceReceived:0);
        SetExperienceMax(initialValues.nextLevelXP);
        SetEnergyExisting(energyDelta:0);
        SetEnergyMax(initialValues.newEnergyMax, initialCall: true);
        SetSpendableValue(new Gold(), 0);
        SetSpendableValue(new Gem(), 0);
        //SetGold(gold_IN:0);
        //SetGem(gem_IN:0);
    }

    private void Update()  // FOR TEXT PURPOSES // LATER TO BE DELETED
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetExperience(20);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetExperience(40);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetEnergyExisting(-20);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetEnergyExisting(10);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            //SetEnergyMax(5);
            SetSpendableValue(new Gold(), -40);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetSpendableValue(new Gold(), 40);
            //SetGold(40);
        }
    }

    private void SetCharacterLevel(int levelReceived = 1)
    {
        _currentLevel += levelReceived;

        CharacterLevels newLevelValues = new CharacterLevels(_currentLevel);
        SetExperienceMax(newLevelValues.nextLevelXP);
        SetEnergyMax(newLevelValues.newEnergyMax);

        GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.level, _currentLevel);
        OnPlayerLevelled?.Invoke(this, newLevelValues.playerLevelledEventArgs);
    }

    private void SetExperience(int experienceReceived)
    {
        float lerpSpeedModifier;  // The default lerp speed is made for 30 units, this is keeping the lerp speed constant, 30 units can be a constant //

        int newExperienceExisting = experienceExisting + experienceReceived;
        float initalProgressAmount = (float)experienceExisting / experienceMax;
        float finalProgressAmount = (float)newExperienceExisting / experienceMax;

        while (newExperienceExisting >= experienceMax)
        {
            lerpSpeedModifier = (experienceMax - experienceExisting) / LERP_SPEED_XP;

            GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.experienceExisting, experienceExisting, experienceMax, lerpSpeedModifier, initalProgressAmount, finalProgressAmount_IN:1);
            
            experienceExisting = 0;
            newExperienceExisting -= experienceMax;

            SetCharacterLevel();

            initalProgressAmount = 0f;
            finalProgressAmount = (float) newExperienceExisting / experienceMax;       
        }

        lerpSpeedModifier = (newExperienceExisting - experienceExisting) / LERP_SPEED_XP;
        GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.experienceExisting, experienceExisting, newExperienceExisting, lerpSpeedModifier, initalProgressAmount,finalProgressAmount);
        
        experienceExisting = newExperienceExisting;
    }

    private void SetExperienceMax(int newExperienceMax)
    {
        float lerpSpeedModifier = 1f;

        GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.experienceMax, experienceMax, newExperienceMax, lerpSpeedModifier);
        experienceMax = newExperienceMax;
    }

    public bool TrySetEnergy(int energyDelta)
    {
        if (energyExisting + energyDelta >= 0)
        {
            SetEnergyExisting(energyDelta);
            return true;
        }
        else
            return false;
    }

    private void SetEnergyExisting(int energyDelta, bool isRefillCall = false)
    {
        float lerpSpeedModifier = LERP_SPEED_ENERGYREFILL;

        if (isRefillCall == false)
        {
            lerpSpeedModifier = 1f;
            TimeTickSystem.onTickTriggered -= SetEnergyExisting;
            energyRefillSubscribed = false;
        }


        int newEnergyExisting = energyExisting + energyDelta;
        float initialProgressAmount = (float)energyExisting / energyMax;
        float finalProgressAmount = (float)newEnergyExisting / energyMax;



        GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.energyExisting, energyExisting, newEnergyExisting, lerpSpeedModifier, initialProgressAmount, finalProgressAmount);

        energyExisting = newEnergyExisting;

        if (energyExisting < energyMax && isRefillCall == false)
        {
            TimeTickSystem.onTickTriggered += SetEnergyExisting;
            energyRefillSubscribed = true;
        }
        else if(energyExisting < energyMax && isRefillCall == true)
        {
            return;
        }
        else
        {
            TimeTickSystem.onTickTriggered -= SetEnergyExisting;
            energyRefillSubscribed = false;
        }
        
    }



    private void SetEnergyMax(int newEnergyMax, bool initialCall = false)
    {
        float lerpSpeedModifier = 1f;

        GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.energyMax, energyMax, newEnergyMax, lerpSpeedModifier);
        energyMax = newEnergyMax;

        if (energyExisting<energyMax && !energyRefillSubscribed)
        {
            TimeTickSystem.onTickTriggered += SetEnergyExisting;
            energyRefillSubscribed = true;
        }
        

    }

    public static bool IsSpendableAmountEnough(int requiredAmount, ISpendable spendable)
    {
        return spendable switch
        {
            Gold => requiredAmount <= goldObject.Amount,
            Gem => requiredAmount <= gemObject.Amount,
            _ => throw new NotImplementedException(),
        };
    }

    public static void SetSpendableValue(ISpendable spendable, int amountDelta)
    {
        float lerpSpeedModifier = 1f;


        switch (spendable)
        {
            case Gold:
                GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.gold, goldObject.Amount, goldObject.Amount + amountDelta, lerpSpeedModifier);
                goldObject.SetAmount(amountDelta);

                OnGoldAmountChanged?.Invoke(goldObject.Amount);
                break;
            case Gem:
                GUI_PlayerStats_Manager.Instance.SetStat(StatName.Stat.gem, gemObject.Amount, gemObject.Amount + amountDelta, lerpSpeedModifier);
                gemObject.SetAmount(amountDelta);

                OnGemAmountChanged?.Invoke(gemObject.Amount);
                break;
        }


        
    }
}
