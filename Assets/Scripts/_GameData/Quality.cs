using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Quality
{
    private static QualityRatios[] qualityRatios = new QualityRatios[]
    {
        new QualityRatios(){level =Level.Superior,minValue = 1.5f , maxValue =50f }, // this is modified to be able to test quality items !! later to dimisish the ratio
        new QualityRatios(){level =Level.Flawless,minValue = .5f , maxValue=1.5f},
        new QualityRatios(){level =Level.Epic, minValue = 0.25f , maxValue=.5f},
        new QualityRatios(){level =Level.Legendary,minValue = 0.01f,maxValue=0.25f},
        new QualityRatios(){level =Level.Mythic,minValue=0f , maxValue =0.01f},
    };



    public static System.Random random = new System.Random(); // can be marked as static in the gamemanager ! to acces from everywhere 

    public struct QualityRatios
    {
        public Level level;
        public float minValue;
        public float maxValue;
    }
    public enum Level
    {
        Normal,
        Superior,
        Flawless,
        Epic,
        Legendary,
        Mythic,
    }


    public static Level CalculateQualityLevel(IEnumerable<int> qualitychanceModifiers_IN)
    {
        float rnd = (float)random.NextDouble();
        rnd *= 100;

        //for (int i = 0; i < qualitychanceModifiers_IN.Count; i++)
        //{
        foreach (var qualityChanceModifier in qualitychanceModifiers_IN)
        {
            foreach (QualityRatios qualityRatio in qualityRatios)
            {
                if (rnd >= qualityRatio.minValue * qualityChanceModifier && rnd < qualityRatio.maxValue * qualityChanceModifier)
                {
                    return qualityRatio.level;
                }
            }
        }
        //}
        return Level.Normal;
    }

    public static float ValueModifierPerQuality(Level qualityLevel_IN)
        => qualityLevel_IN switch
        {
            Level.Normal => 1f,
            Level.Superior => 1.25f,
            Level.Flawless => 2f,
            Level.Epic => 3f,
            Level.Legendary => 5f,
            Level.Mythic => 10f,
            _ => 1f,
        };

    public static float StatModifierPerQuality(Level qualityLevel_IN)
    => qualityLevel_IN switch
    {
        Level.Normal => 1f,
        Level.Superior => 1.25f,
        Level.Flawless => 1.5f,
        Level.Epic => 2f,
        Level.Legendary => 3f,
        Level.Mythic => 5f,
        _ => 1f,
    };

    public static bool TryGetNextQualityLevel(Quality.Level currentQualityLevel, out Quality.Level? nextQualityLevel)
    {
        nextQualityLevel = null;
        if (Enum.IsDefined(typeof(Quality.Level), (int)currentQualityLevel + 1))
        {
            nextQualityLevel = (Quality.Level)((int)currentQualityLevel + 1);
            return true;
        }
        else
        {
            return false;
        }
    }
}
