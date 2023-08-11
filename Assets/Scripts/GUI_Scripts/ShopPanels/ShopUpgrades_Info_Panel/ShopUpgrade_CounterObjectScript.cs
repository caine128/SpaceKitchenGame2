using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpgrade_CounterObjectScript : CounterObjectScript
{
    protected int originalAmount;
    [SerializeField] private GUI_LerpMethods_Float[] progressBars;

    private void InitializeAtMaxLevel()
    {
        for (int i = 0; i < progressBars.Length; i++)
        {
            progressBars[i].ClearQueue();
        }
        progressBars[0].UpdateBarCall(initialValue: 0, finalValue: 1, lerpSpeedModifier: 1, queueRequest: true);
        amountText.text = NativeHelper.BuildString_Append(
                            MethodHelper.GiveRichTextString_Color(Color.green),
                            "Max Investment",
                            MethodHelper.GiveRichTextString_ClosingTagOf("color"));
        foreach (var button in setAmount_Buttons)
        {
            button.TryChangeVisibility(false);
        }
    }

    public sealed override void Initialize<T_Blueprint>(T_Blueprint bluePrint_In)
    {
        if(((ILevellable)bluePrint_In).isAtMaxLevel) // Early Exit if at max level
        {
            InitializeAtMaxLevel();
        }
        else
        {
            for (int i = 0; i < progressBars.Length; i++)
            {
                progressBars[i].ClearQueue();
            }

            var (currentTickAmount, maxTickAmount) = ((IInvestable)bluePrint_In).TickAmounts;
            currentAmount = 0;
            originalAmount = currentTickAmount;
            maxAmount = maxTickAmount - originalAmount;

            progressBars[0].UpdateBarCall(initialValue: 0f,
                                       finalValue: (float)originalAmount / (float)(maxAmount + originalAmount),
                                       lerpSpeedModifier: 1, // TODO: LATER TO CHANGE
                                       queueRequest: true);
            TryChangeAmount(null);
        }
    }

    protected sealed override void UpdateValues(ButtonIteration.Type? buttonIterationType)
    {
        amountText.text = NativeHelper.BuildString_Append(
                            MethodHelper.GiveRichTextString_Color(Color.white),
                            (originalAmount).ToString(),
                            MethodHelper.GiveRichTextString_ClosingTagOf("color"),
                            "(+", currentAmount.ToString(),")",
                            MethodHelper.GiveRichTextString_Color(Color.green),
                            "/",(maxAmount+originalAmount).ToString(),
                            MethodHelper.GiveRichTextString_ClosingTagOf("color"));

        progressBars[1].UpdateBarCall(initialValue: buttonIterationType == ButtonIteration.Type.Next || buttonIterationType==null
                                                        ? Mathf.Max(0, originalAmount + currentAmount - 1) / (float)(maxAmount+ originalAmount)
                                                        : Mathf.Max(0, originalAmount + currentAmount + 1) / (float)(maxAmount + originalAmount),
                                      finalValue: (float)(originalAmount + currentAmount) / (float)(maxAmount+originalAmount),
                                      lerpSpeedModifier: 1, // TODO: LATER TO CHANGE
                                      queueRequest: false);  
    }

    public void UpdateValuesOnInvested()
    {
        var initialOriginalAmount = originalAmount;
        originalAmount += currentAmount;
        maxAmount -= currentAmount;
        currentAmount = 0;

        progressBars[0].UpdateBarCall(initialValue: initialOriginalAmount / (float)(maxAmount + originalAmount),
                                      finalValue: Mathf.Max(0, originalAmount + currentAmount)/ (float)(maxAmount + originalAmount),
                                      lerpSpeedModifier: 1f,
                                      queueRequest: true);
        TryChangeAmount(null);
    }
}
