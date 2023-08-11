using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class ConvertTime
{
    private static readonly StringBuilder Sb = new StringBuilder(capacity:10);
    public static string ToHourMinSec(float duration)
    {
        Sb.Clear();

        int hours = Mathf.FloorToInt(duration / 3600);
        int minutes = Mathf.FloorToInt(duration % 3600) / 60;
        int seconds = Mathf.FloorToInt(duration % 60);

        if (hours > 0)
            Sb.Append(hours).Append(" ").Append("H");

        if (minutes > 0)
            Sb.Append(" ").Append(minutes).Append(" ").Append("M");

        if (seconds > 0)
            Sb.Append(" ").Append(seconds).Append(" ").Append("S");

        return Sb.ToString();

        /*if (minutes > 0 && seconds > 0)
        {
            return string.Format("{0:0} M {1:0} S", minutes, seconds);
        }
        else if (minutes > 0 && seconds <= 0)
        {
            return string.Format("{0:0} M", minutes);
        }
        else
        {
            return string.Format("{0:0} S", seconds);
        }*/
    }

}
