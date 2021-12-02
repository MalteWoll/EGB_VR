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

    public MainCalculator(float initial, float growth, float speed, float frequency, float maxX)
    {
        initialValue = initial;
        growthFactor = growth;
        functionSpeed = speed;
        functionFrequency = frequency;
        functionMaxX = maxX;
    }

    public float getY(float x)
    {
        return (initialValue * Mathf.Pow(1 + growthFactor, x));
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
