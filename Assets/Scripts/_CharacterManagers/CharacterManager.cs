using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    #region Singleton Syntax

    private static CharacterManager _instance;
    public static CharacterManager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    #endregion

    public Worker_List_SO WorkerList_SO
    {
        get => _workerList_SO;
    }
    [SerializeField] private Worker_List_SO _workerList_SO;
    public WorkerLevelProgression_SO WorkerLevelProgressionChart => _workerLevelProgressionChart;
    [SerializeField] private WorkerLevelProgression_SO _workerLevelProgressionChart;
    public static Dictionary<CharacterType.Type, List<Character>> CharactersAvailable_Dict { get; private set; }
    public static int MaxLevel;

    private void Awake() // Later to take on config //
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

        MaxLevel = _workerLevelProgressionChart.progressionCharts.Max(pc => pc.level);
        CreateCharacterDict();
    }

    private void OnEnable()
    {
        StatsData.OnPlayerLevelled += UnlockNewCharacter;
    }

    private void Update() //FOR TEST PUTPOSES LATER TO DELETE 
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            var selectedWorker = _workerList_SO.listOfWorkers.Where(w => w.workerType == WorkerType.Type.Krixath_The_Rotisseur);
            foreach (var item in selectedWorker)
            {
                Debug.Log(item.name);
            }
            AddNewCharacter(selectedWorker);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {

            AddNewCharacter(_workerList_SO.listOfWorkers);

            //var selectedWorker = _workerList_SO.listOfWorkers.Find(w => w.workerType == WorkerType.Type.Trugmil_The_Legumier);
            //AddNewCharacter(selectedWorker);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            var selectedWorker = CharactersAvailable_Dict[CharacterType.Type.Worker].First(cw => cw.isHired == false);
            selectedWorker.TryHireCharacter(new Gold((selectedWorker as Worker).workerspecs.goldCostForHire));
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedWorker = CharactersAvailable_Dict[CharacterType.Type.Worker].First();
            ((Worker)selectedWorker).GainXPTEST(360);
        }
    }

    private void CreateCharacterDict(Dictionary<CharacterType.Type, List<Character>> charactersAvailable_Dict_IN = null)
    {
        CharactersAvailable_Dict = charactersAvailable_Dict_IN ?? new Dictionary<CharacterType.Type, List<Character>>(Enum.GetValues(typeof(CharacterType.Type)).Length);

        foreach (var type in Enum.GetValues(typeof(CharacterType.Type)))
        {
            var characterTypeEnum = (CharacterType.Type)type;
            CharactersAvailable_Dict.Add(characterTypeEnum, new List<Character>());
        }

    }

    private void UnlockNewCharacter(object sender, CharacterLevels.PlayerLevelledEventArgs e)
    {
        //if (e.charactersUnlocked.GetEnumerator().MoveNext())
        //{
        AddNewCharacter(e.charactersUnlocked);
        //}      
    }

    private void AddNewCharacter<T_Character_SO>(IEnumerable<T_Character_SO> characters_SO_IN)
        where T_Character_SO : Character_SO
    {
        List<ModalPanel_DisplayBonuses_UnlockCharacter> modalLoadDataList = new List<ModalPanel_DisplayBonuses_UnlockCharacter>();
        //ModalPanel_DisplayBonuses_UnlockCharacter modalLoadData;
        foreach (var character_SO in characters_SO_IN)
        {
            var charType = character_SO.characterType;
            switch (charType)
            {
                case CharacterType.Type.Commander:
                    Debug.LogWarning("To Be implemented ");
                    break;
                case CharacterType.Type.Taskforce_Fleet:
                    Debug.LogWarning("To Be implemented ");
                    break;
                case CharacterType.Type.Worker when !CharactersAvailable_Dict[charType].Any(w => ((Worker)w).workerspecs == character_SO):

                    Worker newWorker = new Worker(character_SO as Worker_SO);
                    CharactersAvailable_Dict[charType].Add(newWorker);
                    WorkerEventsDict[newWorker.workerspecs.workerType]?.Invoke();
                    RecipeManager.Instance.UpdateLockedItemsDict(newWorker);
                    modalLoadDataList.Add(new ModalPanel_DisplayBonuses_UnlockCharacter(character_IN: newWorker, modalState_IN: Information_Modal_Panel.ModalState.Char_Unlocked));

                    Debug.Log($"{newWorker.GetName()} is added as a worker");
                    break;

                default:
                    Debug.Log($"{character_SO.characterName} is already recruited before !");
                    break;
            }
        }

        if (modalLoadDataList.Count > 0)
        {
            var panelToInvoke = PanelManager.InvokablePanels[typeof(Information_Modal_Panel)];

            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToInvoke,
                                         panelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: modalLoadDataList[0],
                                                                                                                   modalLoadDatas: modalLoadDataList.Skip(1)),
                                         alternativeLoadAction_IN: () =>
                                         {
                                             var panel = (Information_Modal_Panel)panelToInvoke.MainPanel;
                                             panel.ModalLoadDataQueue.Enqueue(modalLoadDataList);
                                         });

        }
    }


    public static event Action OnKrixathModified;
    public static event Action OnChoptuModified;
    public static event Action OnEkolModified;
    public static event Action OnQindrekModified;
    public static event Action OnTrilqeoxModified;
    public static event Action OnTrugmilModified;
    public static event Action OnXadenModified;
    public static event Action OnMasterChefModified;

    public static readonly Dictionary<WorkerType.Type, Action> WorkerEventsDict = new Dictionary<WorkerType.Type, Action>
    {
        {WorkerType.Type.Krixath_The_Rotisseur, OnKrixathModified },
        {WorkerType.Type.Chophu_The_Patissier, OnChoptuModified },
        {WorkerType.Type.Ekol_The_Potager, OnEkolModified },
        {WorkerType.Type.Qindrek_The_Poissonier, OnQindrekModified },
        {WorkerType.Type.Trilqeox_The_Entremetier, OnTrilqeoxModified },
        {WorkerType.Type.Trugmil_The_Legumier, OnTrugmilModified },
        {WorkerType.Type.Xaden_The_Fast_Fooder, OnXadenModified },
        {WorkerType.Type.Master_Chef, OnMasterChefModified },
    };
}
