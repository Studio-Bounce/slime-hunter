using UnityEngine;
using System;
public static class Easing
{
    public static readonly Func<float, float> Linear = t => t;

    public static readonly Func<float, float> EaseInQuad = t => t * t;
    public static readonly Func<float, float> EaseOutQuad = t => t * (2f - t);
    public static readonly Func<float, float> EaseInOutQuad = t => t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;

    public static readonly Func<float, float> EaseInCubic = t => t * t * t;
    public static readonly Func<float, float> EaseOutCubic = t => 1f - Mathf.Pow(1f - t, 3f);
    public static readonly Func<float, float> EaseInOutCubic = t => t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;

    public static readonly Func<float, float> EaseInQuart = t => t * t * t * t;
    public static readonly Func<float, float> EaseOutQuart = t => 1f - Mathf.Pow(1f - t, 4f);
    public static readonly Func<float, float> EaseInOutQuart = t => t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) / 2f;

    public static readonly Func<float, float> EaseInQuint = t => t * t * t * t * t;
    public static readonly Func<float, float> EaseOutQuint = t => 1f - Mathf.Pow(1f - t, 5f);
    public static readonly Func<float, float> EaseInOutQuint = t => t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) / 2f;

    // Additional Easing Functions
    public static readonly Func<float, float> EaseInSine = t => 1f - Mathf.Cos((t * Mathf.PI) / 2f);
    public static readonly Func<float, float> EaseOutSine = t => Mathf.Sin((t * Mathf.PI) / 2f);
    public static readonly Func<float, float> EaseInOutSine = t => -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;

    public static readonly Func<float, float> EaseInExpo = t => t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
    public static readonly Func<float, float> EaseOutExpo = t => t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
    public static readonly Func<float, float> EaseInOutExpo = t => t == 0f
        ? 0f
        : t == 1f
        ? 1f
        : t < 0.5f ? Mathf.Pow(2f, 20f * t - 10f) / 2f
        : (2f - Mathf.Pow(2f, -20f * t + 10f)) / 2f;

    public static readonly Func<float, float> EaseInCirc = t => 1f - Mathf.Sqrt(1f - Mathf.Pow(t, 2f));
    public static readonly Func<float, float> EaseOutCirc = t => Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
    public static readonly Func<float, float> EaseInOutCirc = t => t < 0.5f
        ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * t, 2f))) / 2f
        : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) / 2f;

    public static float EaseInBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;

        return c3 * x * x * x - c1 * x * x;
    }

    public static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }

    public static float EaseInOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;

        return x < 0.5f
            ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2f
            : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2f;
    }

    // Elastic Easing Functions
    public static readonly Func<float, float> EaseInElastic = t =>
    {
        float c4 = (2 * Mathf.PI) / 3;
        return t == 0 ? 0 : t == 1 ? 1 : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * c4);
    };

    public static readonly Func<float, float> EaseOutElastic = t =>
    {
        float c4 = (2 * Mathf.PI) / 3;
        return t == 0 ? 0 : t == 1 ? 1 : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
    };

    public static readonly Func<float, float> EaseInOutElastic = t =>
    {
        float c5 = (2 * Mathf.PI) / 4.5f;
        return t == 0 ? 0 : t == 1 ? 1 : t < 0.5
            ? -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2
            : (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2 + 1;
    };
}
