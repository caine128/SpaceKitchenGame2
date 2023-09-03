using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

public class Worker : Character
{
    public readonly Worker_SO workerspecs;

    public override AssetReferenceAtlasedSprite GetAdressableImage() => workerspecs.characterImageRef;
    public override string GetDescription() => workerspecs.characterDescription;
    public override string GetName() => workerspecs.characterName;
    public override ToolTipInfo GetToolTipText()
        => new ToolTipInfo(header: GetName(), footer: GetDescription(),
                           bodytextAsColumns: new string[2] { "Lvl", GetLevel().ToString() });
   /* {
     


        return new string[]
                           {
                           NativeHelper.BuildString_Append(GetName(),
                                                                  Environment.NewLine,
                                                                  GetLevel().ToString())
                           };
    }*/

    public Worker(Worker_SO _workerSpecs_IN, int arbitraryLevel = 1)
    {
        currentLevel = arbitraryLevel;
        currentXP = 0;
        workerspecs = _workerSpecs_IN;

        if (arbitraryLevel > 0) ///Very important check to prevent not stored classes to be registered in the event.
        {
            Radial_CraftSlots_Crafter.Instance.onReclaimCrafted += UpdateCharacterXPEventHandler;

        }
    }

    public override int GetXpToNextLevel()
    {
        var progressionChart = CharacterManager.Instance.WorkerLevelProgressionChart.progressionCharts;
        return progressionChart[Mathf.Clamp(currentLevel, 1, progressionChart.Length - 1)].xpNeeded;
    }

    private void UpdateCharacterXPEventHandler(object sender, Radial_CraftSlots_Crafter.OnCraftingEventArgs e) // later can 
    {
        if (e.productRecipe.recipeSpecs.requiredworkers.Any(rw => rw.requiredWorker == workerspecs.workerType))
        {
            UpdateCharacterXP(e.productRecipe.recipeSpecs.workerXPAwward * 6); // Later to make FiX!! * 6 is added for speed!!
        }
    }

    // TEst Method for XP
    public void GainXPTEST(int testXP)
    {
        UpdateCharacterXP(testXP);
    }


    protected override void UpdateCharacterXP(int gainedXP)
    {
        if (!isAtMaxLevel)
        {
            var currentTotalXP = currentXP + gainedXP;

            Int16 levelIncerements = 0;

            while (!isAtMaxLevel     //Is at max level here is necessary, because overflowing experience causes to re-check the progress chart in while loop
                    && currentTotalXP >= CharacterManager.Instance.WorkerLevelProgressionChart.progressionCharts[currentLevel].xpNeeded)
            {
                var mainWorkStation = (WorkStationUpgrade)ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.WorkstationUpgrades]
                                            .FirstOrDefault(su => ((WorkStationUpgrade)su).GetWorkstationType() == workerspecs.workStationPrerequisites[0].type);

                if (mainWorkStation?.GetMaxWorkerLevelCapCurrent() > currentLevel)
                {
                    currentTotalXP -= CharacterManager.Instance.WorkerLevelProgressionChart.progressionCharts[currentLevel].xpNeeded;
                    currentLevel++;
                    levelIncerements++;
                    CharacterManager.WorkerEventsDict[workerspecs.workerType]?.Invoke();
                }
                else
                {
                    currentTotalXP = CharacterManager.Instance.WorkerLevelProgressionChart.progressionCharts[currentLevel].xpNeeded;
                    Debug.LogError("this worker cannot level up becuase of equipment prereq");
                    break;
                }

            }
            currentXP = currentTotalXP;

            if (levelIncerements > 0)
            {
                var panelToLoad = PanelManager.InvokablePanels[typeof(Information_Modal_Panel)];

                var modalLoadData = new ModalPanel_DisplayBonuses_LevelUpCharacter(
                                      unlocks_IN: RecipeManager.RecipesAvailable_List.Where(ar => ar.recipeSpecs.requiredworkers.Any(rw => rw.requiredWorker == workerspecs.workerType && rw.requiredWorkerLevel == currentLevel)),
                                      character_IN: this,
                                      modalState_IN: Information_Modal_Panel.ModalState.Char_Levelled);

                PanelManager.ActivateAndLoad(invokablePanel_IN: panelToLoad,
                                                       panelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: modalLoadData,
                                                                                                                                 modalLoadDatas: Enumerable.Empty<ModalLoadData>()),
                                                       alternativeLoadAction_IN: () =>
                                                       {
                                                           var panel = ((Information_Modal_Panel)panelToLoad.MainPanel);
                                                           panel.ModalLoadDataQueue.Enqueue(modalLoadData);
                                                       });
            }
        }
        else
        {
            Debug.Log("worker is already at max level");
        }

    }

    public float GetCraftTimeReduction(int? workerLevel = null) // IMPLEMENT DURATION REDUCTION INTO ACTUAL CRAFTING ALGORYTHM
    {
        var progressionCharts = CharacterManager.Instance.WorkerLevelProgressionChart.progressionCharts;
        return workerLevel is not null
                                    ? progressionCharts[Mathf.Clamp(workerLevel.Value - 1, 0, progressionCharts.Length - 1)].craftTimeReduction
                                    : progressionCharts[currentLevel - 1].craftTimeReduction;

    }

    public IEnumerable<ProductRecipe> GetWorkerRecipes()  // IS RESEARCHED CAN BE ADDED TO TEST THE RECEIPES OF A WORKER IN WORKERINFO PANEL
    {
        return RecipeManager.RecipesAvailable_List.Where(ra => Enum.IsDefined(typeof(EquipmentType.Type), (EquipmentType.Type)workerspecs.workStationPrerequisites[0].type)
                                                         && ra.recipeSpecs.requiredEquipment == (EquipmentType.Type)workerspecs.workStationPrerequisites[0].type
                                                  && ra.IsResearched())
                                                  .OrderBy(ra => ra.GetLevel());
    }

    public (string retStr, float progVal) GetProgressionStatus()
          => (isHired,
             currentLevel >= CharacterManager.MaxLevel,
             (WorkStationUpgrade)ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.WorkstationUpgrades]
                                                .Where(su => ((WorkStationUpgrade)su).GetWorkstationType() == workerspecs.workStationPrerequisites[0].type)
                                                .DefaultIfEmpty(null)
                                                .First())

             switch
          {
              (false, _, _) => ($"Hire {GetName()}", 0f),
              (true, true, _) => ("Max Level", 100f),
              (true, false, var workStation) => workStation switch
              {
                  null => ($"Buy {workerspecs.workStationPrerequisites[0].type}", 0f),
                  { } => workStation.GetMaxWorkerLevelCapCurrent() switch
                  {
                      var maxLevelCap when maxLevelCap <= currentLevel => ($"Requires {workerspecs.workStationPrerequisites[0].type} lv. {workStation.GetLevel() + 1}", 0f),
                      var maxLevelCap when maxLevelCap > currentLevel => ($"% {currentXP * 100 / CharacterManager.Instance.WorkerLevelProgressionChart.progressionCharts[currentLevel].xpNeeded}",
                                                                               currentXP * 100 / CharacterManager.Instance.WorkerLevelProgressionChart.progressionCharts[currentLevel].xpNeeded),
                      _ => throw new System.NotImplementedException(),
                  }
              },
          };

    private IEnumerable<(SortableBluePrint, int)> GetMissingWorkStations() // TODO : CAN BE OPTIMIZED BY IFALLMET? BOOL ON THE WORKER LIKE PRODUCRECIPE
    {
        foreach (var r_wtl in workerspecs.workStationPrerequisites)
        {
            var workStationToReturn = (WorkStationUpgrade)ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.WorkstationUpgrades]
                                                .FirstOrDefault(ws => ((WorkStationUpgrade)ws).GetWorkstationType() == r_wtl.type);

            if (workStationToReturn?.GetLevel() >= r_wtl.level)
                continue;

            yield return workStationToReturn switch
            {
                null => (new WorkStationUpgrade(
                                        indexNo_IN: ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.baseInfo.Select((bi, index) => (bi, index)).First(x => x.bi.workstationType == r_wtl.type).index,
                                        shopupgradeType: ShopUpgradeType.Type.WorkstationUpgrades,
                                        arbitraryLevel: 0), r_wtl.level),
                { } => (workStationToReturn, r_wtl.level),
            };
        }
    }

    public override void TryHireCharacter(ISpendable spendable)
    {
        /// Check if existingworkstations are of required type and minimum required level, 
        /// which are specified in workerprerequisites   
        var missingWorkSations = GetMissingWorkStations();
        if (missingWorkSations.Any())
        {
            foreach (var sation in missingWorkSations)
            {
                Debug.LogWarning(sation.Item1.GetName());
            }

            var missingWorkStationsList = missingWorkSations.ToList();
            PanelManager.ActivateAndLoad(invokablePanel_IN: PanelManager.InvokablePanels[typeof(MissingRequirementsPopupPanel)],
                                         panelLoadAction_IN: () => MissingRequirementsPopupPanel.Instance.LoadPanel(new PanelLoadDatas(
                                             mainLoadInfo: this,
                                             panelHeader: missingWorkStationsList.Count > 1
                                                                ? "Missing Workstations"
                                                                : "Missing Workstation",
                                             tcs_IN: null,
                                             bluePrintsToLoad: missingWorkStationsList)));
            return;
        }


        else if (StatsData.IsSpendableAmountEnough(requiredAmount: spendable.Amount, spendable: spendable))
        {
            StatsData.SetSpendableValue(spendable: spendable, amountDelta: spendable.Amount * -1);
            isHired = true;
            CharacterManager.WorkerEventsDict[workerspecs.workerType]?.Invoke();


            var panelToLoad = PanelManager.InvokablePanels[typeof(Information_Modal_Panel)];

            var hiredWorkerData = new ModalPanel_DisplayBonuses_HireCharacter(character_IN: this,
                                                                              unlocks_IN: workerspecs.unlockRecipes,// RecipeManager.RecipesAvailable_List.Where(ar => workerspecs.unlockRecipes.Any(rtu => rtu == ar.recipeSpecs)),
                                                                              modalState_IN: Information_Modal_Panel.ModalState.Char_Hired);


            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToLoad,
                                         preLoadAction_IN: () => PanelManager.RemoveCurrentPanelFromNavigationStackIf(removeConditions: ipc => ipc.MainPanel is HireCharacter_Panel),
                                         panelLoadAction_IN: () =>
                                         {
                                             Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: hiredWorkerData,
                                                                                                                   modalLoadDatas: Enumerable.Empty<ModalLoadData>());
                                         },

                                         alternativeLoadAction_IN: () =>
                                         {
                                             var panel = ((Information_Modal_Panel)panelToLoad.MainPanel);
                                             panel.ModalLoadDataQueue.Enqueue(hiredWorkerData);
                                         }); ;

            /// Taken Down becuase we  dont want the recipes modal info to fire before the worker model info          
            foreach (var recipe_SO in workerspecs.unlockRecipes)
            {
                RecipeManager.Instance.AddNewRecipe(recipe_SO);
            }
            /// This is removed becuase it is already being called from Recipemanager
            //RecipeManager.Instance.UpdateLockedItemsDict(this);

            Debug.Log(workerspecs.name + " is hired");
        }
        else
        {
            Debug.Log($"There is not enough {spendable.GetType()}s to activate {workerspecs.name}");
        }

    }


}
