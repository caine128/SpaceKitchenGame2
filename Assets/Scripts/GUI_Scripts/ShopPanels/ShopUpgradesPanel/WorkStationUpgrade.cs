using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorkStationUpgrade : ShopUpgrade, IInvestable//, IRushable
{

    //public InvestmentCosts_SO.InvestmentCosts InvestmentCostCurrentLevel { get => _investmentCostsCurrentLevel; }
    private InvestmentCosts_SO.InvestmentCosts _investmentCostsCurrentLevel;

    public (int currentTickAmount, int maxTickAmount) TickAmounts { get => _tickAmounts; }
    private (int currentTickAmount, int maxTickAmount) _tickAmounts;

    //public float MaxUpgradeDuration { get => ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel[currentLevel].upgradeDuration; }
    //public float RemainingDuration { get => _remainingUpgradeDuration; }
    //private float _remainingUpgradeDuration;
    //private float _maxTimeTickCount;
    //private float _currentTimeTickCount =0;

    //public bool IsReadyToReclaim { get => _isReadyToClaimLevelUp; }
    //private bool _isReadyToClaimLevelUp = false;

    //public int TotalRushCostGem { get => ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel[currentLevel].upgradeGemCost ;}
    //public SortableBluePrint BluePrint => this;
    //public float CurrentProgress => _currentTimeTickCount/_maxTimeTickCount ;
    //public float RemainingDuration => throw new NotImplementedException();
    public override event Action<float, float> OnProgressTicked;
    public override event Action OnUpgradeTimerUpdate;

    public event Action OnInvested;
    
    

    public WorkStationUpgrade(int indexNo_IN, ShopUpgradeType.Type shopupgradeType, int arbitraryLevel = 1) 
        : base(indexNo_IN, shopupgradeType, arbitraryLevel)
    {
        if (arbitraryLevel > 0) ///Very important check to prevent not stored classes to not initiate investmentStatus
        {
            _investmentCostsCurrentLevel = GetInvestmentCostPerLevel();
            _tickAmounts = GetTickAmountsPerLevel();
        }
        
    }

    private InvestmentCosts_SO.InvestmentCosts GetInvestmentCostPerLevel()
    {
        var investMentInfo = ShopUpgradesManager.Instance.InvestmentCosts_SO.investmentCosts.FirstOrDefault(ic => ic.level == currentLevel);
        if (ValueType.Equals(investMentInfo, default))
        {
            throw new System.ArgumentException();
        }
        else
        {
            return investMentInfo;
        }
    }

    private (int currentTickAmount, int maxTickAmout) GetTickAmountsPerLevel()
       => (0,_investmentCostsCurrentLevel.requiredTickAmount);

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo[indexNo].spriteRef;
        //return ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo[indexNo].sprite;
    }

    public override string GetName()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo[indexNo].name;
    }

    public override int GetValue() // This has to be adjusted !!! There is no VALUE maybe we ca remove the interface from here totally
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel[0].upgradeGoldCost * ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo[indexNo].tierMultiplier;
    }

    public override int GetAmount()
        => ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.WorkstationUpgrades]
                    .Cast<WorkStationUpgrade>()
                    .Count(wsu => wsu.GetWorkstationType() == this.GetWorkstationType());

    public WorkstationType.Type GetWorkstationType()
        => ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo[indexNo].workstationType;


    public int GetMaxWorkerLevelCapCurrent()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel[currentLevel-1].maxWorkerLevelCap;
    }

    public (int currentLevelCap,int? nextLevelCap) GetMaxWorkerLevelCapCurrentAndNext()
    {
        var isAtMaxLevel = currentLevel == ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel.Length;
        return (ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel[currentLevel - 1].maxWorkerLevelCap,
                isAtMaxLevel 
                        ? null
                        : ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel[currentLevel].maxWorkerLevelCap);
    }

    public (WorkstationType.Type requiredWorkstationType, int requiredWorkstationLevel) GetWorkstationPrerequisite()
    {
        return (ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo[indexNo].workstationUpgradePrerequisite[0].requiredWorkstation, ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo[indexNo].workstationUpgradePrerequisite[0].requiredWorkstationLevel);
    }

    public override string GetDescription()
    {
        return ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo[indexNo].description;
    }

    private Worker_SO GetAssociatedWorker()
    {
        return CharacterManager.Instance.WorkerList_SO.listOfWorkers.First(w => w.workStationPrerequisites[0].type == this.GetWorkstationType());
    }

    public override IEnumerable<(string benefitName, string benefitValue, AssetReferenceT<Sprite> bnefitIcon)> GetDisplayableBenefits()
    {
        //var associatedWorker = CharacterManager.Instance.WorkerList_SO.listOfWorkers.First(w => w.workStationPrerequisites[0].type == this.GetWorkstationType());
        var (currentLevelCap, nextLevelCap) = GetMaxWorkerLevelCapCurrentAndNext();
        if(nextLevelCap is not null) 
            yield return ("Duration For Upgrade" ,
                      ConvertTime.ToHourMinSec(ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel[currentLevel].upgradeDuration), //.ToString(),
                      ImageManager.SelectSprite("Duration"));

        
        yield return ("Max Worker Lvl.", 
                      $"{currentLevelCap} {(nextLevelCap is null?string.Empty : ">")} {nextLevelCap}",
                      ImageManager.SelectSprite(GetAssociatedWorker().workerType.ToString()));
    }

    /*public override void TryLevelUp(ISpendable spendable)
    {
        //check that current level is not the max level
        var nextLevelInfo = ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel.Where(sbl => sbl.level == currentLevel + 1);

        if (!nextLevelInfo.Any())
        {
            Debug.Log("You are at the max level available for now");
        }

        //check if there's enough spendable either gold or gem and fire panel Event
        else if (StatsData.IsSpendableAmountEnough(requiredAmount: spendable.Amount, spendable: spendable))
        {
            StatsData.SetSpendableValue(spendable: spendable, amountDelta: spendable.Amount * -1);
            currentLevel++;
            var (currentLevelCap, nextLevelCap) = GetMaxWorkerLevelCapCurrentAndNext();

            var modalLoadData = new ModalPanel_DisplayBonuses_LoadData(mainSprite_IN: this.GetAdressableImage(),
                                                                       secondarySprite_IN: GetAssociatedWorker().characterImageRef,
                                                                       bonusExplanationStringTuple_IN: (bonusExplanationString1_IN: "Max Worker Level", bonusExplanationString2_IN: $"{currentLevelCap} {(nextLevelCap is null?string.Empty : ">")} {nextLevelCap}"),
                                                                       subheaderString_IN: $"Congratulations! {GetName()} reached Level {currentLevel}",
                                                                       modalState_IN: Information_Modal_Panel.ModalState.WorkStationUpgrade);
            PanelManager.ActivateAndLoad(invokablePanel_IN: PanelManager.InvokablePanels[typeof(Information_Modal_Panel)],
                                         preLoadAction_IN: () =>PanelManager.RemoveCurrentPanelFromNavigationStackIf(removeConditions:ipc => ipc.MainPanel is ShopUpgradesInfoPanel_Manager ),
                                         panelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: modalLoadData,
                                                                                                                   modalLoadDatas: Enumerable.Empty<ModalLoadData>()));                                                                     
        }
        else
        {
            Debug.Log("your spendable is not enough for leveling up!");
        }
    }*/

    public override void LevelUp()
    {
        if (_remainingUpgradeDuration <= 0 && _isReadyToClaimLevelUp) 
        {
            //check that current level is not the max level
            var nextLevelInfo = ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel.Where(sbl => sbl.level == currentLevel + 1);
            Debug.Log("level up script is called ");
            if (!nextLevelInfo.Any())
            {
                Debug.Log("You are at the max level available for now");
            }
            else
            {
                currentLevel++;
                _isReadyToClaimLevelUp = false;
                _investmentCostsCurrentLevel = GetInvestmentCostPerLevel();
                _tickAmounts = GetTickAmountsPerLevel();
                _currentTimeTickCount = 0;

                var (currentLevelCap, nextLevelCap) = GetMaxWorkerLevelCapCurrentAndNext();

                var modalLoadData = new ModalPanel_DisplayBonuses_LoadData(mainSprite_IN: this.GetAdressableImage(),
                                                                           secondarySprite_IN: GetAssociatedWorker().characterImageRef,
                                                                           bonusExplanationStringTuple_IN: (bonusExplanationString1_IN: "Max Worker Level", bonusExplanationString2_IN: $"{currentLevelCap} {(nextLevelCap is null ? string.Empty : ">")} {nextLevelCap}"),
                                                                           subheaderString_IN: $"Congratulations! {GetName()} reached Level {currentLevel}",
                                                                           modalState_IN: Information_Modal_Panel.ModalState.WorkStationUpgrade);
                PanelManager.ActivateAndLoad(invokablePanel_IN: PanelManager.InvokablePanels[typeof(Information_Modal_Panel)],
                                             preLoadAction_IN: () => PanelManager.RemoveCurrentPanelFromNavigationStackIf(removeConditions: ipc => ipc.MainPanel is ShopUpgradesInfoPanel_Manager),
                                             panelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: modalLoadData,
                                                                                                                       modalLoadDatas: Enumerable.Empty<ModalLoadData>()));
            }
        }
        else
        {
            Debug.LogWarning("the timer is not zeroed yet, please wait");
        }
    }

    public (int gold, int gem) GetCostsPerTick()
    {
        var tierMultiplier = ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo[indexNo].tierMultiplier;
        return (_investmentCostsCurrentLevel.baseGoldPerTick * tierMultiplier, _investmentCostsCurrentLevel.baseGemPerTick * tierMultiplier);
    }

    public void TryInvest(ISpendable spendable, int tickAmount, out int tokensToReturn)
    {
        tokensToReturn = 0;  // TODO: Implement Token Returns

        if (StatsData.IsSpendableAmountEnough(spendable: spendable, requiredAmount: spendable.Amount))
        {
            StatsData.SetSpendableValue(spendable: spendable, amountDelta: spendable.Amount * -1);

            if (_tickAmounts.currentTickAmount + tickAmount >= _tickAmounts.maxTickAmount)
            {
                _tickAmounts = (_tickAmounts.currentTickAmount + tickAmount, _tickAmounts.maxTickAmount);
                StartUpgradeTimer();
                OnInvested?.Invoke();
                PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null, unloadAction: null);
            }
            else
            {
                _tickAmounts = (_tickAmounts.currentTickAmount + tickAmount, _tickAmounts.maxTickAmount);
                OnInvested?.Invoke();
            }

        }
        else
        {
            Debug.Log($"your {spendable.GetType()} is not enough for leveling up!");
        }
    }

    private void StartUpgradeTimer()
    {
        _remainingUpgradeDuration = ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel[currentLevel].upgradeDuration;
        _maxTimeTickCount = Mathf.Ceil(_remainingUpgradeDuration * 5);

        TimeTickSystem.onTickTriggered += UpgateUpgradeTimer;

    }

    private void UpgateUpgradeTimer(int timeTick, bool _)
    {
        var valueInitial = _currentTimeTickCount / _maxTimeTickCount;
        var valueFinal = (_currentTimeTickCount + timeTick) / _maxTimeTickCount;

        _currentTimeTickCount += timeTick; //TODO: *5 multipliermodifier is for speed testing the workstation upgrades , later to FiX !
        if (_currentTimeTickCount % 5 == 0 && _currentTimeTickCount < _maxTimeTickCount)
        {
            _remainingUpgradeDuration--;
            OnUpgradeTimerUpdate?.Invoke();
        }
        else if (_currentTimeTickCount == _maxTimeTickCount)
        {
            TimeTickSystem.onTickTriggered -= UpgateUpgradeTimer;
            _remainingUpgradeDuration = 0;
            //_currentTimeTickCount = 0;
            //_maxTimeTickCount = 0;
            _isReadyToClaimLevelUp = true;

            OnUpgradeTimerUpdate?.Invoke();           
        }

        OnProgressTicked?.Invoke(valueInitial, valueFinal);
    }

    public override void Rush()
    {
        TimeTickSystem.onTickTriggered -= UpgateUpgradeTimer;
        var ticksToRush = Mathf.CeilToInt(_maxTimeTickCount - _currentTimeTickCount);
        UpgateUpgradeTimer(ticksToRush, false);
    }
}
