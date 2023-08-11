using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class TimeTickSystem : MonoBehaviour
{
    private static TimeTickSystem _instance;
    public static TimeTickSystem Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    public static event Action<int, bool> onTickTriggered;
    public static event Action OnLateUpdate;

    public const float TICK_TIMER_MAX = .2f;  // TODO : CHANGED FOR TEST PUTPOSES !! was .2f // for speed Im using 0.002f

    private float tickTimer;

    public static readonly WaitForSeconds WaitForSeconds_Two = new WaitForSeconds(2);
    public static readonly WaitForSeconds WaitForSeconds_One = new WaitForSeconds(1);
    public static readonly WaitForSeconds WaitForSeconds_QuarterSec = new WaitForSeconds(.25f);
    public static readonly WaitForSeconds WaitForSeconds_EighthSec = new WaitForSeconds(.125f);
    public static readonly WaitForSeconds WaitForSeconds_ContainerActivation = new WaitForSeconds(.015f);
    public static readonly WaitForSeconds WaitForSeconds_AscensionContainerActivation = new WaitForSeconds(.040f);
    public static readonly WaitForSeconds WaitForSeconds_HUDBarsMovement = new WaitForSeconds(.05f);
    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

    public const float NUMERIC_LERPDURATION = .19f; // TICK_TIMER_MAX; // CHANGED FOR TEST PUTPOSES !! was .19f
    public const float PANEL_LERPDURATION = .16f;          //Changed for test purposes was .16f;
    public const float TEXTANIM_LERPDURATION = .3f;
    public const int PERIOD_DISABLEPANEL_DEFAULT = 2;
    public const int UPDATEINTERVAL = 3;

    #region For Long Clickable Buttons
    public const float LONGCLICKTHRESHOLD_1X = .5f;
    public const float LONGCLICKTHRESHOLD_2X = 1f;
    public const float LONGCLICKTHRESHOLD_3X = 2f;

    public const float LONGCLICKFREQUENCY_1X = .2f;
    public const float LONGCLICKFREQUENCY_2X = .1f;
    public const float LONGCLICKFREQUENCY_3X = .05f;

    #endregion



    public AnimationCurve easeCurve;  //TODO : MAKE THIS PRIVATE LATER ON !!! DELETE THS ACTUALLY I HAVE THE LISS OF CURVES ALREADY !

    public enum EaseCurveType
    {
        Standard = 0,
        PropUp = 1,
        PropDown = 2,
        NudgeScale = 3,
        SmoothContinous = 4
    }
    [SerializeField]
    [LabeledArray(typeof(EaseCurveType))]
    private AnimationCurve[] easeCurves;

    public AnimationCurve GetCurve(EaseCurveType easeCurveType) => easeCurves[(int)easeCurveType];

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if(_instance == null)
                {
                    _instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if(tickTimer >= TICK_TIMER_MAX)
        {
            tickTimer -= TICK_TIMER_MAX;
            onTickTriggered?.Invoke(1, true); //fillamount int + isRefillCall bool
        }

    }

    private void LateUpdate()
    {
        if (OnLateUpdate == null)
        {
            return;
        }
        
        OnLateUpdate?.Invoke();
    }

}
