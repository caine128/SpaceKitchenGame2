using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductType  
{
    public enum AllType
    {
        DateMostRecent = Sort.Type.DateMostRecent,    //001
        Value = Sort.Type.Value,                      //002
        Quantity = Sort.Type.Quantity,                //003
        Type = Sort.Type.Type,                        //004
        Level = Sort.Type.Level,                      //005
        Quality = Sort.Type.Quality,                  //006

        Burger = 101,
        Ribs = 102,
        Beef = 103,
        Chicken = 104,
        Pork = 105,
        Lamb = 106,
        Venison = 107,
        Pasta = 201,
        Grains = 202,
        Rice = 203,
        Noodles = 204,
        WholeGrain = 205,
        Salmon = 301,
        Crab = 302,
        Cake = 401,
        Sandwich = 501,
        Leaves = 601,
        Roots = 602,
        Flowers = 603,
        Seeds = 604,
        Fungi = 605,
        Tubers = 606,
        Mixed_Soup = 701,
        Spirit_Enhancement = 502,
        Elemental_Enhancement = 702,
        Runestone_Enhancement = 402,

        None = -100,
    }

    public enum Type
    {
        Burger  = AllType.Burger,
        Ribs    = AllType.Ribs,
        Beef    = AllType.Beef,
        Chicken = AllType.Chicken,
        Pork    = AllType.Pork,
        Lamb    = AllType.Lamb,
        Venison = AllType.Venison,
        Pasta   = AllType.Pasta,
        Grains  = AllType.Grains, 
        Rice    = AllType.Rice,
        Noodles = AllType.Noodles,
        WholeGrain = AllType.WholeGrain,
        Salmon  = AllType.Salmon,
        Crab    = AllType.Crab,
        Cake    = AllType.Cake,
        Sandwich= AllType.Sandwich,
        Leaves  = AllType.Leaves,
        Roots   = AllType.Roots,
        Flowers = AllType.Flowers,
        Seeds   = AllType.Seeds,
        Fungi   = AllType.Fungi,
        Tubers = AllType.Tubers,
        Mixed_Soup = AllType.Mixed_Soup,
        Spirit_Enhancement =AllType.Spirit_Enhancement,
        Elemental_Enhancement =AllType.Elemental_Enhancement,
        Runestone_Enhancement =AllType.Runestone_Enhancement,

        None = AllType.None,
    }


}
