using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CounterObjectScript : MonoBehaviour
{
    [SerializeField] protected SetAmount_Button[] setAmount_Buttons;
    [SerializeField] protected TextMeshProUGUI amountText;

    //private PopupPanel parentPanel;

    protected int maxAmount;

    public int CurrentAmount => currentAmount;
    protected int currentAmount = 0;

    public event Action OncounterModified;

    public virtual void Initialize<T_Blueprint>(T_Blueprint bluePrint_In)
        where T_Blueprint : IAmountable
    {
        //parentPanel = GetComponentInParent<PopupPanel>();
        maxAmount = GetMaxAmount(bluePrint_In);
        currentAmount = currentAmount == 0 ? currentAmount : 0;
        SetVisibility();
        TryChangeAmount(null);
    }

    public void Initialize(int maxAmount_IN)
    {
        maxAmount = maxAmount_IN;
        currentAmount = currentAmount == 0 ? currentAmount : 0;
        SetVisibility();
        TryChangeAmount(null);
    }

    public void TryChangeAmount(ButtonIteration.Type? buttonIterationType)
    {
        var newAmount = TrySetNewAmount(buttonIterationType);
        if(currentAmount != newAmount)
        {
            currentAmount = newAmount;
            UpdateValues(buttonIterationType);
            UpdateButtonVisibility();
            OncounterModified?.Invoke();
            //NotifyParentPanel();
        }

    }

    private void SetVisibility()
    {
        switch (maxAmount, this.gameObject.activeInHierarchy)
        {
            case ( <= 1, true): 
                this.gameObject.SetActive(false);
                break;
            case ( > 1, false): 
                this.gameObject.SetActive(true);
                break;
        }
    }

    /*private void NotifyParentPanel()
    {
        switch (parentPanel)
        {
            case DeleteItemPopupPanel deleteItemPopupPanel: deleteItemPopupPanel.ModifyIngredientCurrentAmountModifier();
                break;
        }
    }*/

    private int TrySetNewAmount(ButtonIteration.Type? buttonIterationType)
        => buttonIterationType switch
        {
            ButtonIteration.Type.Previous => Mathf.Max(1, currentAmount - 1),
            ButtonIteration.Type.Next => Mathf.Min(currentAmount + 1, maxAmount),
            null => 1,
            _ => throw new System.NotImplementedException(),
        };

    protected virtual void UpdateValues(ButtonIteration.Type? buttonIterationType)
    {
        amountText.text = currentAmount.ToString();
    }

    private void UpdateButtonVisibility()
    {
        switch (currentAmount)
        {
            case var isSingleVal when currentAmount == 1 && 1 == maxAmount:
                setAmount_Buttons[0].TryChangeVisibility(false);
                setAmount_Buttons[1].TryChangeVisibility(false);
                break;
            case var isAtMaxVal when currentAmount == maxAmount:
                setAmount_Buttons[0].TryChangeVisibility(true);
                setAmount_Buttons[1].TryChangeVisibility(false);
                break;
            case var isAtBetween when currentAmount < maxAmount && currentAmount > 1:
                setAmount_Buttons[0].TryChangeVisibility(true);
                setAmount_Buttons[1].TryChangeVisibility(true);
                break;
            case var isAtMinVal when currentAmount == 1:
                setAmount_Buttons[0].TryChangeVisibility(false);
                setAmount_Buttons[1].TryChangeVisibility(true);
                break;

        }
    }
    private int GetMaxAmount<T_Blueprint>(T_Blueprint bluePrint_In)
         where T_Blueprint : IAmountable
    {
        return bluePrint_In.GetAmount();
    }

}
