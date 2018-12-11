using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Quality { Common, Special, Rare, Artifact }

public static class QualityColor
{
    private static Dictionary<Quality, string> colors = new Dictionary<Quality, string>()
    {
        {Quality.Common, "#ffffff"},
        {Quality.Special, "#00e600"},
        {Quality.Rare, "#4d88ff"},
        {Quality.Artifact, "#ff471a"},
    };

    public static Dictionary<Quality, string> MyColors
    {
        get
        {
            return colors;
        }
    }
}
