using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharactersPanel_Manager : ScrollablePanel<Character,CharacterType.Type,Sort.Type>, IRefreshablePanel
{
    public override int IndiceIndex => _indiceIndex;
    private readonly int _indiceIndex = 11;

    protected override void Start()
    {
        base.Start();

        activeSelection_MainType = CharacterType.Type.Worker;
        activeSelection_SubtType = Sort.Type.None;

        ExecuteEvents.Execute(mainType_Selector_Buttons[0].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
    }

    protected override void Initialize_SubSelectionButtons()
    {
        base.Initialize_SubSelectionButtons();
        foreach (SelectorButton<Sort.Type> subselectorButton in subType_SelectorButtons)
        {
            subselectorButton.SetButtonImageVisibility(isVisible: false);
        }
    }

    protected override List<Character> CreateSortList()
   {
        return CharacterManager.CharactersAvailable_Dict[activeSelection_MainType];
   }
     
   public override void ScrollToSelection(Character displayBuluPrint_In, bool markSelection)
   {
        var equalityComparer = new CharacterEqualityComparer();

        int selectedContainerIndex = 0;
        for (int i = 0; i < RequestedBluePrints.Count; i++)
        {
            if (equalityComparer.Equals(RequestedBluePrints[i], displayBuluPrint_In))
            {
                selectedContainerIndex = i;
                break;
            }
        }
        CalculateForwardPosAndScroll(selectedContainerIndex, markSelection);
   }

    public void RefreshPanel()
    {
        ArrangeAndSort(CreateSortList());
    }
}
