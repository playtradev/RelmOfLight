using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class PlaytraGamesExtend : MonoBehaviour
{
   
}


public static class MyExtensions
{
    public static T GridFight_Where_FirstOrDefault<T>(this List<T> list, System.Func<T, bool> condition) where T : class
    {
        var arr = list;
        for (int i = 0; i < arr.Count; i++)
        {
            if (condition(arr[i]))
            {
                return arr[i];
            }
        }

        return null;
    }

    public static T GridFight_Where_FirstOrDefault<T>(this T[] array, Func<T, bool> condition) where T : class
    {
        var arr = array;
        for (int i = 0; i < arr.Length; i++)
        {
            if (condition(arr[i]))
            {
                return arr[i];
            }
        }

        return null;
    }

    public static T GridFight_Where_Struct_FirstOrDefault<T>(this T[] array, Func<T, bool> condition) where T : struct
    {
        var arr = array;
        for (int i = 0; i < arr.Length; i++)
        {
            if (condition(arr[i]))
            {
                return arr[i];
            }
        }
        T[] values = (T[])Enum.GetValues(typeof(T));
        
        return values[0];
    }

    public static T GridFight_Where_Struct_FirstOrDefault<T>(this List<T> array, Func<T, bool> condition) where T : struct
    {
        var arr = array;
        for (int i = 0; i < arr.Count; i++)
        {
            if (condition(arr[i]))
            {
                return arr[i];
            }
        }
        T[] values = (T[])Enum.GetValues(typeof(T));

        return values[0];
    }








    public static bool GridFight_Contains<T>(this List<T> list, T elementToCheck) where T : class
    {
        var arr = list;
        for (int i = 0; i < arr.Count; i++)
        {
            if (arr[i] == elementToCheck)
            {
                return true;
            }
        }
        
        return false;
    }

    public static bool GridFight_Contains<T>(this T[] array, T elementToCheck) where T : class
    {
        var arr = array;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == elementToCheck)
            {
                return true;
            }
        }

        return false;
    }


    public static bool GridFight_ContainsStruct<T>(this List<T> list, T elementToCheck) where T : struct
    {
        var arr = list;
        for (int i = 0; i < arr.Count; i++)
        {
            if (arr[i].Equals(elementToCheck))
            {
                return true;
            }
        }

        return false;
    }

    public static bool GridFight_ContainsStruct<T>(this T[] list, T elementToCheck) where T : struct
    {
        var arr = list;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].Equals(elementToCheck))
            {
                return true;
            }
        }

        return false;
    }






    

   /* public static bool GridFight_ContainsEnum<T>(this T[] array, T elementToCheck) where T : System.Enum
    {
        var arr = array;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].Equals(elementToCheck))
            {
                return true;
            }
        }

        return false;
    }*/

}

