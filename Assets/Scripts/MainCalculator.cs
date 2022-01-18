using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class MainCalculator
{
    private float initialValue;
    private float growthFactor;
    private float functionMaxX;
    private string functionType;
    private float noiseLevel;

    private float functionMaxY;

    private Vector2 mirrorLineStart;
    private Vector2 mirrorLineEnd;

    private Dictionary<float, float> logFunctionValues = new Dictionary<float, float>();

    // Dictionary for storing the calculated values
    private Dictionary<float, float> values = new Dictionary<float, float>();
    int dictCounter = 0;
    int counter = 0;

    public MainCalculator(float initial, float growth, float maxX, string type, float noise)
    {
        initialValue = initial;
        growthFactor = growth;
        functionMaxX = maxX;
        functionType = type;

        noiseLevel = noise;

        functionMaxY = getMaxY();

        mirrorLineStart = new Vector2(0, initial);
        mirrorLineEnd = new Vector2(functionMaxX, functionMaxY/2);
        Debug.Log("mirrorLineStart: " + mirrorLineStart.ToString() + ", mirrorLineEnde: " + mirrorLineEnd.ToString());

        /*if (type == "log")
        {
            logFunctionValues = calculateLogFunctionValues();
        }*/
    }

    public float getY(float x)
    {
        float result = 0;
        if (functionType == "exp") /* Different equations for the two types of functions */
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
            //result = initialValue * Mathf.Log(x) + growthFactor;

            // result = findClosestLogValue(x);

            //result = invertedExpFunc(x);

            result = calcLogFunc(x);

            if (noiseLevel != 0)
            {
                result = addNoise(result);
            }

            if (result < 0) { result = 0; }
            values.Add(x, result);
        }
        //Debug.Log("x" + counter.ToString() + ": " + x.ToString("F2") + ", y" + counter.ToString() + ": " + result.ToString("F2"));
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
        //Debug.Log("x" + dictCounter.ToString() + ": " + values.Keys.ElementAt(dictCounter) + ", y" + dictCounter.ToString() + ": " + y.ToString("F2"));
        dictCounter++;
        return y;
    }

    /// <summary>
    /// Calculate the 'real' maximum value without noise.
    /// </summary>
    /// <returns></returns>
    public float getMaxY()
    {
        /*if (functionType == "exp")
        {
            return (initialValue * Mathf.Pow(1 + growthFactor, functionMaxX));
        } else
        {
            return (initialValue * Mathf.Log(functionMaxX) + growthFactor);
        }*/
        return (initialValue * Mathf.Pow(1 + growthFactor, functionMaxX));
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
        float addedNoise = input + Random.Range(0, range);

        //float addedNoise = input + Random.Range(0, noiseLevel);

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

    public Dictionary<float,float> getValueDict()
    {
        return values;
    }

    private Vector2 findNearestPointOnLine(Vector2 origin, Vector2 end, Vector2 point)
    {
        /*
        //Get heading
        Vector2 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector2 lhs = point - origin;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
        */

        /*
        Vector2 linePnt = end;
        Vector2 lineDir = end - origin;
        lineDir.Normalize();
        var v = point - linePnt;
        var d = Vector2.Dot(v, lineDir);
        return linePnt + lineDir * d;*/

        Vector3 p = new Vector3(point.x, point.y, 0);
        Vector3 w0 = new Vector3(origin.x, origin.y, 0);
        Vector3 w1 = new Vector3(end.x, end.y, 0);
        Vector3 temp = Vector3.Project((p-w0),(w1-w0)) +w0;

        return new Vector2(temp.x, temp.y);
    }

    private Vector2 getMirroredValue(Vector2 expFunctionValue, Vector2 nearestPointOnLine)
    {
        Vector2 movedBy = (nearestPointOnLine - expFunctionValue);

        //movedBy.x = Mathf.Abs(movedBy.x);
        //movedBy.y = Mathf.Abs(movedBy.y);

        //Debug.Log(nearestPointOnLine.ToString() + " - " + expFunctionValue.ToString() + " = " + movedBy.ToString());
        //Debug.Log(movedBy.magnitude);

        //return nearestPointOnLine + movedBy;
        Vector2 result = expFunctionValue + 2*  (nearestPointOnLine - expFunctionValue);
        Debug.Log("Result: " + result.ToString());
        Debug.Log("Point on line: " + nearestPointOnLine.ToString());
        return result;
    }

    private Dictionary<float,float> calculateLogFunctionValues()
    {
        float stepX = functionMaxX / 1000;
        float x = 0;

        Dictionary<float, float> logKeyValues = new Dictionary<float, float>();

        while(x < functionMaxX)
        {
            float expResult = (initialValue * Mathf.Pow(1 + growthFactor, x));
            Vector2 expResultVec2 = new Vector2(x, expResult);

            Vector2 nearestPoint = findNearestPointOnLine(mirrorLineStart, mirrorLineEnd, expResultVec2);
            Vector2 logResultVec2 = getMirroredValue(expResultVec2, nearestPoint);

            try
            {
                logKeyValues.Add(logResultVec2.x, logResultVec2.y);
            } catch
            {
                Debug.LogError("error when adding to dict");
            }
           

            x += stepX;

            //Debug.Log(nearestPoint.ToString() + " - " + expResultVec2.ToString() + " = " + logResultVec2.ToString());
        }

        return logKeyValues;
    }

    private float findClosestLogValue(float x)
    {
        var bestMatch = logFunctionValues.OrderBy(e => Mathf.Abs(e.Key - x)).FirstOrDefault(); /* Inefficient, change if it works */

        Debug.Log(x.ToString() + ", " + bestMatch.Value.ToString());

        return bestMatch.Value;
    }

    private float invertedExpFunc(float x)
    {
        float a = initialValue;
        float b = 1 + growthFactor;

        float maxXexp = (initialValue * Mathf.Pow(1 + growthFactor, functionMaxX));
        float maxXlog = Mathf.Log((functionMaxX / a), b) + initialValue;

        float scaling = maxXexp / maxXlog;

        float result = (Mathf.Log((x / a), b) + initialValue) * scaling;

        Debug.Log("x: " + x + ", y: " + result);

        return result;
    }

    private float calcLogFunc(float x)
    {
        return ((functionMaxY / Mathf.Log(functionMaxX + 1)) * Mathf.Log(x + 1));
    }
}
