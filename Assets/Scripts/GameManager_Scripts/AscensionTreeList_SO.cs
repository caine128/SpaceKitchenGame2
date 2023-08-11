using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New AscensionTreesList", menuName ="AscensionTreesList")]
public class AscensionTreeList_SO : ScriptableObject
{
    [SerializeField] public List<AscensionTree_SO> listOfAscensionTrees;
}
