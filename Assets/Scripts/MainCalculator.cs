using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class MainCalculator
{
    // Values for the calculation of the function
    private float initialValue;
    private float growthFactor;
    private float functionMaxX;
    private string functionType;
    private float noiseLevel;

    private float functionMaxY;

    private float prevValue = 0;

    // Dictionary for storing the calculated values. Since this is also used for writing the x/y value pairs to a text file, we can't use a simple list
    private Dictionary<float, float> values = new Dictionary<float, float>();
    int dictCounter = 0;

    // Constructor
    public MainCalculator(float initial, float growth, float maxX, string type, float noise)
    {
        initialValue = initial;
        growthFactor = growth;
        functionMaxX = maxX;
        functionType = type;
        noiseLevel = noise;
        functionMaxY = getMaxY();
    }

    public float getY(float x)
    {
        float result = 0;
        if (functionType == "exp") /* Different equations for the two types of functions */
        {
            result = (initialValue * Mathf.Pow(1 + growthFactor, x)); /* Standard exponential function: y = a * (1 + b) ^ x */
        }
        if (functionType == "log")
        {
            result = ((functionMaxY / Mathf.Log(functionMaxX + 1)) * Mathf.Log(x + 1)) + (initialValue * 1 - (x/functionMaxX)); /* Logarithmic function starting and ending in the same y-value as the exponential one */
        }

        if (noiseLevel != 0) /* Only add noise if the noiseLevel is not null */
        {
            result = addNoise(result);
        }
        if (result < 0) { result = 0; } /* Limit to null */
        values.Add(x, result); /* Add the value to the dictionary */
        
        return result;
    }

    /// <summary>
    /// Method for returning y values from the dictionary of previously calculated values, to deliver the exact same values, including noise, when replaying.
    /// </summary>
    /// <returns></returns>
    public float getYAgain()
    {
        float y = values.Values.ElementAt(dictCounter); /* Get the value from the dictionary */
        dictCounter++;
        return y;
    }

    /// <summary>
    /// Calculate the 'real' maximum value of the exponential function without noise.
    /// </summary>
    /// <returns></returns>
    public float getMaxY()
    {
        return (initialValue * Mathf.Pow(1 + growthFactor, functionMaxX));
    }

    public float getMaxLogY()
    {
        return ((functionMaxY / Mathf.Log(functionMaxX + 1)) * Mathf.Log(functionMaxX + 1)) + (initialValue * 1 - (functionMaxX / functionMaxX));
    }

    // Round the provided value for the interative visualization
    public int getRoundedY(float x)
    {
        return Mathf.RoundToInt(getY(x));
    }

    public int getRoundedYAgain()
    {
        return Mathf.RoundToInt(getYAgain());
    }

    /// <summary>
    /// Function for adding noise. Adds a random value with a previously defined percentage range.
    /// </summary>
    /// <param name="input">The value the noise is added to.</param>
    /// <returns>The value with noise added.</returns>
    private float addNoise(float input)
    {
        float range = input * noiseLevel; /* Get the absolute value for the range from the percentage of noise desired and the current value of the function */
        float addedNoise = input + Random.Range(0, range);

        while (addedNoise < prevValue) /* Makes sure the function is monotonously increasing */
        {
            addedNoise = input + Random.Range(0, range);
        }

        if(addedNoise < 0) { addedNoise = 0; }

        prevValue = addedNoise;

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

    /// <summary>
    /// Returns the dictionary with the values for saving it to a text file.
    /// </summary>
    /// <returns></returns>
    public Dictionary<float,float> getValueDict()
    {
        return values;
    }
}
