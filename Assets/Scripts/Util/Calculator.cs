using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class for calculating.
/// </summary>
public static class Calculator
{
    /// <summary>
    /// Calculates the value of the exponential function and returns it.
    /// </summary>
    /// <param name="x">The x value for the calculation.</param>
    /// <param name="initialValue">The starting value of the exponential function.</param>
    /// <param name="growth">The growth value of the exponential function.</param>
    /// <returns></returns>
    public static float calculateExponentialFunctionValue(float x, float initialValue, float growth)
    {
        return (initialValue * Mathf.Pow((1 + growth), x));
    }
}
