using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainCalculator
{
    private float initialValue;
    private float growthFactor;
    private float functionSpeed;
    private float functionFrequency;
    private float functionMaxX;
    private string functionType;
    private float noiseLevel;

    // Dictionary for storing the calculated values
    private Dictionary<float, float> values = new Dictionary<float, float>();
    int dictCounter = 0;
    int counter = 0;

    public MainCalculator(float initial, float growth, float speed, float frequency, float maxX, string type, float noise)
    {
        initialValue = initial;
        growthFactor = growth;
        functionMaxX = maxX;
        functionType = type;

        // Speed and frequency are set in the different visualizations classes, so they are most likely not necessary here
        functionSpeed = speed;
        functionFrequency = frequency;

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
        Debug.Log("x" + counter.ToString() + ": " + x.ToString("F2") + ", y" + counter.ToString() + ": " + result.ToString("F2"));
        counter++;
        return result;
    }

    /// <summary>
    /// Method for returning y values from the dictionary of previously calculated values, to deliver the exact same values, including noise, when replaying.
    /// </summary>
    /// <returns></returns>
    public float getYAgain()
    {
        float y = values.Values.ElementAt(dictCounter);
        Debug.Log("x" + dictCounter.ToString() + ": " + values.Keys.ElementAt(dictCounter) + ", y" + dictCounter.ToString() + ": " + y.ToString("F2"));
        dictCounter++;
        return y;
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

    public int getRoundedYAgain()
    {
        return Mathf.RoundToInt(getYAgain());
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

        // If value is (above the maximum value or) below 0, set it to either one.
        //if(addedNoise > getMaxY()) { addedNoise = getMaxY(); }
        if(addedNoise < 0) { addedNoise = 0; }

        return addedNoise;
    }

    /// <summary>
    /// For debugging
    /// </summary>
    public void showValues()
    {
        foreach(KeyValuePair<float,float> value in values)
        {
            Debug.Log(value.Key.ToString("F2") + ", " + value.Value.ToString("F2"));
        }
        Debug.Log(values.Keys.ElementAt(0) + ", " + values.Values.ElementAt(0).ToString());
    }

    public void resetDictCounter() {
        dictCounter = 0;    
    }
}
