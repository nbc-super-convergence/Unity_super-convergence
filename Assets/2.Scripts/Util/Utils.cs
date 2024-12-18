using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static string FilterToASCII(string input)
    {
        string result = "";

        foreach (char c in input)
        {
            if (c >= 32 && c <= 126)
            {
                result += c;
            }
        }

        return result;
    }
}
