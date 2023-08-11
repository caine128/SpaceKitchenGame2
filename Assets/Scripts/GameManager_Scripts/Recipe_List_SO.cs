using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RecipeList", menuName = "RecipeList")]
public class Recipe_List_SO : ScriptableObject
{
    [SerializeField] public List<Recipes_SO> listOfRecipes;
}
