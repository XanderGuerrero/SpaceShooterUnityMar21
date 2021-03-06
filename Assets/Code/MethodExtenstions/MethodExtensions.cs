using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MethodExtensions
{
   public static float TwoDecimals(this float value)
    {
        return Mathf.Round(value * 1000.0f) / 1000.0f;
    }
}
