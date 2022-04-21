using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DmrMath
{
    // Returns sign of number
    public static float Sign(float x) { return x == 0.0f? 0.0f: x > 0.0f ? 1.0f : -1.0f; }

    // Returns 1 if true, -1 if false
    public static float Polarize(bool x) { return x? 1.0f: -1.0f; }
    public static float Polarize(float x) { return x > 0.0f ? 1.0f : -1.0f; }

    // Returns true when value is in an odd interval
    public static float BoolStep(float x, float step) {return (x % (step * 2.0f)) / step;}
    
    // Returns value closest to a factor of step
    public static float Quantize(float x, float step) { return (float)Math.Floor(x / step) * step; }
    public static float QuantizeRound(float x, float step) { return (float)Math.Round(x / step) * step; }
    public static float QuantizeCeil(float x, float step) { return (float)Math.Ceiling(x / step) * step; }


    public static float PointDistance(float x1, float y1, float x2, float y2)
    {
        return (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
    }

    public static float Approach(float x, float target, float step)
    {
        return (x < target) ?
            (x + step > target) ? target : (x + step) :
            (x - step < target) ? target : (x - step);
    }

    public static float ApproachSmooth(float x, float target, float smoothing, float merge_threshhold = 0.001f)
    {
        return Math.Abs(target-x) <= merge_threshhold ? target: x + (target - x) / smoothing;
    }
}
