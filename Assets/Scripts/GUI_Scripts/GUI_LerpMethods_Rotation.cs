using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_LerpMethods_Rotation : MonoBehaviour, IConfigurablePanel
{
    //public static event Action<bool> onWheelRotating;


    private RectTransform rect;
    public IEnumerator co_BackSpin { get; private set; }
    public IEnumerator co_TargetSpin { get; private set; }
    [SerializeField] private AnimationCurve easeCurve;

    public SpinType.Type spintype = SpinType.Type.NoSpin; //serialize is for visualising delete it after setting the  wheel !!     // LATER TO MAKE AUTOPROPERTY
    public SpinType.Direction spindirection = SpinType.Direction.None;    // LATER TO MAKE AUTOPROPERTY
    private float angularVelocity;   //serialize is for visualising delete it after setting the  wheel !!
    private float _elasticity = 1;//serialize is for visualising delete it after setting the  wheel !!

    private float lerpSpeedModifier = 1f;
    private int angularDirection;
    private bool isBackSpinning = false;
    private bool isTargetSpinning = false;
    private Vector3 targetSpinPosition = new Vector3();

    private (float, float) clampedAngle;

    private void OnDisable()
    {
        if (Radial_CraftSlots_Crafter.Instance != null)
        {
            Radial_CraftSlots_Crafter.Instance.onStartCrafting -= SetClampAngle;
            Radial_CraftSlots_Crafter.Instance.onReclaimCrafted -= SetClampAngle;
        }
    }

    private void Start()
    {
        PanelConfig(); // THIS ONE HAS TO MOVE UPPER LEVEL LIKE THE OTHER GUI_LERPS
    }

    public void PanelConfig()
    {
        rect = this.GetComponent<RectTransform>();
        Radial_CraftSlots_Crafter.Instance.onStartCrafting += SetClampAngle;
        Radial_CraftSlots_Crafter.Instance.onReclaimCrafted += SetClampAngle;
    }

    private void LateUpdate()
    {
        if (spintype == SpinType.Type.NoSpin) // || isSlotsRearranging)  /// If there is no spin movement, Stop The LateUpdate from running every frame.
        {
            return;
        }

        if (spintype == SpinType.Type.ControlledSpin)
        {
            if (co_BackSpin != null)
            {
                StopCoroutine(co_BackSpin);
                co_BackSpin = null;
                spintype = SpinType.Type.ControlledSpin;
                isBackSpinning = false;
            }

            if (!CheckClampedAngle())
            {
                SetElasticity();
            }

            spindirection = angularVelocity > 0 ? SpinType.Direction.Positive : (angularVelocity < 0 ? SpinType.Direction.Negative : spindirection);
            rect.localEulerAngles = new Vector3(rect.localEulerAngles.x, rect.localEulerAngles.y, rect.localEulerAngles.z + (angularVelocity * angularDirection * _elasticity));
        }
        else if (spintype == SpinType.Type.FreeSpin)
        {
            spindirection = angularVelocity > 0 ? SpinType.Direction.Positive : (angularVelocity < 0 ? SpinType.Direction.Negative : spindirection);

            if (!CheckClampedAngle())
            {
                SetElasticity();
            }

            rect.localEulerAngles = new Vector3(rect.localEulerAngles.x, rect.localEulerAngles.y, rect.localEulerAngles.z + (angularVelocity * Time.deltaTime * angularDirection * _elasticity));
            angularVelocity = angularVelocity > 0 ? angularVelocity - 1 : angularVelocity + 1;

            if (Mathf.Abs(angularVelocity * _elasticity) <= 1)
            {
                angularVelocity = 0;
                spintype = SpinType.Type.BackSpin;
            }
        }
        else if (spintype == SpinType.Type.TargetedSpin && isTargetSpinning == false)   /// SPINDIRECTION YET TO BE ADDED
        {
            _elasticity = 1;
            isTargetSpinning = true;
            RotateBackToPosition(.4f);
        }

        else if (spintype == SpinType.Type.BackSpin && isBackSpinning == false) // this targetspinninbool might be unnecesary
        {
            _elasticity = 1;
            spindirection = spindirection == SpinType.Direction.Positive ? SpinType.Direction.Negative : SpinType.Direction.Positive;
            isBackSpinning = true;
            RotateBackToPosition(.4f); /// IT WAS .4 I arranged to see clearly
        }
    }


    private void SetElasticity()
    {
        if (rect.localEulerAngles.z < 360 && rect.localEulerAngles.z > 330 && spindirection == SpinType.Direction.Negative)
        {
            _elasticity = 1;
            return;
        }
        else if (rect.localEulerAngles.z < 360 && rect.localEulerAngles.z > 330 && spindirection == SpinType.Direction.Positive)
        {
            float angleDifference = 360 - rect.localEulerAngles.z;
            _elasticity = Mathf.Lerp(1, 0, Mathf.Clamp(angleDifference, 0, 20) / 20);
        }
        else if (spindirection == SpinType.Direction.Positive)
        {
            _elasticity = 1;
            return;
        }
        else
        {
            float angleDifference = rect.localEulerAngles.z - clampedAngle.Item2;
            _elasticity = Mathf.Lerp(1, 0, Mathf.Clamp(angleDifference, 0, 20) / 20);
        }
    }

    private void RotateBackToPosition(float lerpDuration)
    {

        if (spintype == SpinType.Type.TargetedSpin)
        {

            if (co_BackSpin != null || co_TargetSpin != null)
            {
                StopAllCoroutines();
            }
            Vector3 targetRotateposition = targetSpinPosition;
            co_TargetSpin = RotateBackToPositionRoutine(targetRotateposition, lerpDuration, needExtraBackSpin: true);
            StartCoroutine(co_TargetSpin);
        }

        else if (spintype == SpinType.Type.BackSpin)
        {
            if (co_TargetSpin != null) return;
            if (co_BackSpin != null)
            {
                StopCoroutine(co_BackSpin);
            }
            Vector3 targetRotateposition = GetBackSpinPosition();
            co_BackSpin = RotateBackToPositionRoutine(targetRotateposition, lerpDuration);
            StartCoroutine(co_BackSpin);
        }
    }


    private Vector3 GetBackSpinPosition()
    {
        if (!CheckClampedAngle())
        {
            //if (spinningObject == SpinType.SpiningObject.Parent)
            //{
            if (rect.localEulerAngles.z < 360 && rect.localEulerAngles.z > 330)
            {
                return new Vector3(rect.localEulerAngles.x, rect.localEulerAngles.y, clampedAngle.Item1);
            }
            else if (rect.localEulerAngles.z <= 330 && rect.localEulerAngles.z >= 300)
            {
                Debug.Log("working");
                return new Vector3(rect.localEulerAngles.x, rect.localEulerAngles.y, 0);
            }
            else
            {
                return new Vector3(rect.localEulerAngles.x, rect.localEulerAngles.y, clampedAngle.Item2);
            }
        }

        float angleDifference = (rect.localEulerAngles.z % 30f);

        if (Mathf.Abs(angleDifference) <= 15)
        {
            return new Vector3(rect.localEulerAngles.x, rect.localEulerAngles.y, rect.localEulerAngles.z - (angleDifference));
        }
        else
        {
            return new Vector3(rect.localEulerAngles.x, rect.localEulerAngles.y, rect.localEulerAngles.z + (30 - angleDifference));
        }
    }

    private bool CheckClampedAngle()
    {
        if (rect.localEulerAngles.z >= clampedAngle.Item1 && rect.localEulerAngles.z <= clampedAngle.Item2 || Mathf.Approximately(rect.localEulerAngles.z, 0)) return true;
        else return false;
    }

    private void SetClampAngle(object sender, Radial_CraftSlots_Crafter.OnCraftingEventArgs e)
    {
        //if (spinningObject == SpinType.SpiningObject.Parent)
        //{
        switch (e.activeCraftAmount)
        {
            case 1:
            case 2:
            case 3:
                clampedAngle = (0f, 0f);
                break;
            case 4:
                clampedAngle = (0f, 30f);
                break;
            case 5:
                clampedAngle = (0f, 60f);
                break;
            case 6:
                clampedAngle = (0f, 90f);
                break;
            case 7:
                clampedAngle = (0f, 120f);
                break;
            case 8:
                clampedAngle = (0f, 150f);
                break;
            case 9:
                clampedAngle = (0f, 180f);
                break;
            case 10:
                clampedAngle = (0f, 210f);
                break;
            case 11:
                clampedAngle = (0f, 240f);
                break;
            case 12:
                clampedAngle = (0f, 360f);
                break;
        }

    }

    public void SetSpinType(SpinType.Type spinTypeIN, float angularVelocityIN, int angularDirectionIN, float targetSpinOffset = default)
    {

        if (spintype != spinTypeIN || spinTypeIN == SpinType.Type.TargetedSpin)
        {

            if (spintype == SpinType.Type.TargetedSpin && spinTypeIN == SpinType.Type.TargetedSpin)
            {
                targetSpinPosition = new Vector3(targetSpinPosition.x, targetSpinPosition.y, (targetSpinPosition.z - targetSpinOffset));
                isTargetSpinning = false;
            }
            else if (spintype != SpinType.Type.TargetedSpin && spinTypeIN == SpinType.Type.TargetedSpin)
            {
                targetSpinPosition = new Vector3(rect.localEulerAngles.x, rect.localEulerAngles.y, (rect.localEulerAngles.z - targetSpinOffset));
            }
            else if (spintype == SpinType.Type.TargetedSpin && spinTypeIN != SpinType.Type.TargetedSpin)
            {
                return;
            }
            spintype = spinTypeIN;
        }
        angularVelocity = angularVelocityIN;
        angularDirection = angularDirectionIN;
    }

    IEnumerator RotateBackToPositionRoutine(Vector3 finalPosition, float lerpDuration, bool needExtraBackSpin = false)
    {

        float elapsedTime = 0f;
        Vector3 currentPos = rect.localEulerAngles;
        Quaternion currentPosQ = rect.rotation;
        Quaternion finalPositionQ = rect.rotation * Quaternion.Euler(0, 0, finalPosition.z - currentPos.z);


        while (elapsedTime < lerpDuration * lerpSpeedModifier)
        {
            float easeFactor = elapsedTime / (lerpDuration * lerpSpeedModifier);
            easeFactor = easeCurve.Evaluate(easeFactor);

            rect.rotation = Quaternion.LerpUnclamped(currentPosQ, finalPositionQ, easeFactor);
            //rect.localEulerAngles = new Vector3(rect.localEulerAngles.x, rect.localEulerAngles.y, Mathf.LerpAngle(currentPos.z, finalPosition.z, easeFactor));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rect.localEulerAngles = finalPosition;

        if (finalPosition.z == 360) // to counter strange unity backrotation number 1E-10
        {
            rect.localEulerAngles = new Vector3(rect.localEulerAngles.x, rect.localEulerAngles.y, 0);
        }

        yield return null;  // IMPORTANT FOR THE CHILD TO BE ABLE TO MAKE THE LAST EQUALISATION

        isBackSpinning = isTargetSpinning = false;
        spintype = SpinType.Type.NoSpin;
        spindirection = SpinType.Direction.None;
        co_BackSpin = co_TargetSpin = null;

        if (needExtraBackSpin)
        {
            spintype = SpinType.Type.BackSpin;
        }
    }

}
