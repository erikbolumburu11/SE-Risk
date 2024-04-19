using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Stores useful functions that don't relate to any individual class
 */
public class Utils : MonoBehaviour
{
    /*
     * Removes 1 item from list that has the same value as num
     */
    public static List<int> RemoveANumberFromList(List<int> list, int num)
    {
        foreach (int i in list)
        {
            if(i == num)
            {
                list.Remove(num);
                return list;
            }
        }
        return null;
    }
}
