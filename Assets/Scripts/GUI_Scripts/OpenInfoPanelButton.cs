using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenInfoPanelButton<T_Blueprint> : MonoBehaviour
    where T_Blueprint:SortableBluePrint_Base
{
    [SerializeField] public Container<T_Blueprint> parentContainer;
}
