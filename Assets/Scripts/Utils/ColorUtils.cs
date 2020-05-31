using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtils
{
    public static Color ToBlack(Color color)
    {
        color.r = 0f;
        color.g = 0f;
        color.b = 0f;

        return color;
    } 

    public static Color ToColor(Color color)
    {
        color.r = 255f;
        color.g = 255f;
        color.b = 255f;

        return color;
    } 
}
