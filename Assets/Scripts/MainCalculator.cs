using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCalculator
{
    private float initialValue;
    private float growthFactor;
    private float functionSpeed;
    private float functionFrequency;
    private float functionMaxX;
    private float functionMaxY;
    private float functionCurrentX;
    private float functionCurrentY;
    private int roundedY;
    private string functionType;
    private float noiseLevel;

    // Dictionary for storing the calculated values
    private Dictionary<float, float> values = new Dictionary<float, float>();

    public MainCalculator(float initial, float growth, float speed, float frequency, float maxX, string type, float noise)
    {
        initialValue = initial;
        growthFactor = growth;
        functionSpeed = speed;
        functionFrequency = frequency;
        functionMaxX = maxX;
        functionType = type;
        noiseLevel = noise;
    }

    public float getY(float x)
    {
        float result = 0;
        if (functionType == "exp")
        {
            result = (initialValue * Mathf.Pow(1 + growthFactor, x));

            if(noiseLevel != 0)
            {
                result = addNoise(result);
            }

            values.Add(x, result);
        } 
        if (functionType == "log")
        {
            result = initialValue * Mathf.Log(x) + growthFactor;

            if (noiseLevel != 0)
            {
                result = addNoise(result);
            }

            if (result < 0) { result = 0; }
            values.Add(x, result);
        } 

        return result;
    }

    /// <summary>
    /// Calculate the 'real' maximum value without noise.
    /// </summary>
    /// <returns></returns>
    public float getMaxY()
    {
        if (functionType == "exp")
        {
            return (initialValue * Mathf.Pow(1 + growthFactor, functionMaxX));
        } else
        {
            return (initialValue * Mathf.Log(functionMaxX) + growthFactor);
        }
    }

    public int getRoundedY(float x)
    {
        return Mathf.RoundToInt(getY(x));
    }

    /// <summary>
    /// Function for adding noise. Adds or subtracts a random value with a previously defined percentage.
    /// </summary>
    /// <param name="input">The value the noise is added to.</param>
    /// <returns>The value with noise added.</returns>
    private float addNoise(float input)
    {
        float range = input * noiseLevel;
        float addedNoise = input + Random.Range(-range, range);

        // If value is above the maximum value or below 0, set it to either one.
        if(addedNoise > getMaxY()) { addedNoise = getMaxY(); }
        if(addedNoise < 0) { addedNoise = 0; }

        return addedNoise;
    }
}
