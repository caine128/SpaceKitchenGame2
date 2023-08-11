using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggerForNonMono : MonoBehaviour
{
  public static void Logger(string name, int index)
    {
        Debug.Log(name +"" +index);
    }


    public static void Logger( int index)
    {
        Debug.Log( index);
    }

    public static void Logger(float index)
    {
        Debug.Log(index);
    }

    public static void Logger(params string[] strings_IN)
    {
        foreach (var item in strings_IN)
        {
            Debug.Log(item);
        }       
    }
}
