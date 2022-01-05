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

    public MainCalculator(float initial, float growth, float speed, float frequency, float maxX, string type)
    {
        initialValue = initial;
        growthFactor = growth;
        functionSpeed = speed;
        functionFrequency = frequency;
        functionMaxX = maxX;
        functionType = type;
    }

    public float getY(float x)
    {
        float result = 0;
        if (functionType == "exp")
        {
            result = (initialValue * Mathf.Pow(1 + growthFactor, x));
        } 
        if (functionType == "log")
        {
            result = initialValue * Mathf.Log(x) + growthFactor;
            if(result < 0) { result = 0; }
        } 

        return result;
    }

    public float getMaxY()
    {
        return getY(functionMaxX);
    }

    public int getRoundedY(float x)
    {
        return Mathf.RoundToInt(getY(x));
    }
}
