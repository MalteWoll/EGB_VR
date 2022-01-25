using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

public static class Util
{
    /// <summary>
    /// Randomizes the values in a list. Uses the Cryptography package for better randomization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list that will be shuffled.</param>
    /// <returns>Returns the the list with the values in random order.</returns>
    public static List<T> shuffleList<T>(List<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    /// <summary>
    /// Returns current date and time.
    /// </summary>
    /// <returns></returns>
    public static string getCurrentDateAndTime()
    {
        CultureInfo culture = new CultureInfo("en-GB");
        DateTime localDate = DateTime.Now;
        return localDate.ToString(culture);
    }

    /// <summary>
    /// Appends the submitted string to the output file.
    /// </summary>
    /// <param name="data"></param>
    public static void WriteToOutputFile(string data)
    {
        string path = Application.persistentDataPath + "/output.txt";
        File.AppendAllText(path, data);
    }

    /// <summary>
    /// Creates an output file in CSV format for the x and y values of a function.
    /// </summary>
    /// <param name="dict">The x and y values as keys and values of a dictionary.</param>
    /// <param name="type">The type of visualization used</param>
    /// <returns>The name of the file that has been created.</returns>
    public static string SaveFunctionValues(Dictionary<float,float> dict, string type)
    {
        string dateTime = DateTime.Now.ToString(new CultureInfo("en-GB")); /* Get the date and time as unique identifier */
        dateTime = Regex.Replace(dateTime, "[\\|/|:| ]", "_"); /* Replace all undesired characters in the string with underscores */
        dateTime = dateTime + "_" + type;
        string path = Application.persistentDataPath + "/" + dateTime + ".txt"; /* Create the path from the identifier */

        // Create the header line in the text file
        StringBuilder sb = new StringBuilder();
        sb.Append("x0,y0");
        for (int i = 1; i < dict.Count; i++)
        {
            sb.Append(",x" + i.ToString() + ",y" + i.ToString());
        }
        sb.Append("\n");
        // Add the values of the dictionary
        sb.Append(dict.Keys.ElementAt(0).ToString("F2") + "," + dict.Values.ElementAt(0).ToString("F2"));
        for (int j = 1; j < dict.Count; j++)
        {
            sb.Append("," + dict.Keys.ElementAt(j).ToString("F2") + "," + dict.Values.ElementAt(j).ToString("F2"));
        }

        if (!File.Exists(path)) /* File should never exist already, but just to make sure */
        {
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        } else
        {
            Debug.LogError("File " + path + " exists already!");
        }

        return dateTime + ".txt"; /* Return the name of the file */
    }

    /// <summary>
    /// Function to store and return the strings for the instructions.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>Instruction string for the type of instruction required.</returns>
    public static string GetInstructionalText(string type)
    {
        switch(type)
        {
            case "previsualization":
                return "You will now see a series of visualisations aimed at demonstrating exponential and logarithmic growth. \n\nOnce the visualisation has concluded, you are free to repeat it again.";
            case "calculations":
                return "Now you will make calculations based on each visualization you just saw. \n\nTry to answer these as best you can based on your intuition.";
            case "decision":
                return "If you had to invest in one of the previous two functions, which option would you select?";
            case "ending":
                return "The study is now over. You can remove the headset.";
            default:
                return "";
        }
    }

    /// <summary>
    /// Loads the settings file from the device and saves the settings in the PlayerPrefs.
    /// </summary>
    public static Settings LoadSettingsJSON()
    {
        Settings settings = new Settings();
        string path = "settings.json";
        try
        {
            settings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.persistentDataPath + "/" + path));
        } catch
        {
            return null;
        }

        Debug.Log("Settings loaded: InitialMin: " + settings.initialValueMin + ", InitialMax: " + settings.initialValueMax + ", GrowthMin: " + settings.growthFactorMin + 
                  ", GrowthMax:" + settings.growthFactorMax + ", maxValueMin: " + settings.maxXValueMin + ", maxValueMax: " +
                  settings.maxXValueMax + ", Speed: " + settings.speed + ", Frequency: " + settings.frequency + ", GoldBarScaling: " + settings.goldBarScaling + ", amountOfRuns: " + 
                  settings.amountOfRuns + ", afterYearsMin: " + settings.afterYearsMin + ", " + settings.afterYearsMax + ", noise: " + settings.noise + ", sliderMultiplierMin: " + settings.sliderMultiplierMin + 
                  ", sliderMultiplierMax: " + settings.sliderMultiplierMax);

        Debug.Log("InstructionsText1: " + settings.instructionsText1 + ", size: " + settings.instructionsText1Size);
        Debug.Log("InstructionsText2: " + settings.instructionsText2 + ", size: " + settings.instructionsText2Size);
        Debug.Log("InstructionsSlider: " + settings.instructionsSliderText + ", size: " + settings.instructionsSliderTextSize);
        Debug.Log("instructionsPrevisualization: " + settings.instructionsPrevisualization + ", size: " + settings.instructionsPrevisualizationTextSize);
        Debug.Log("InstructionsCalculations: " + settings.instructionsCalculations + ", size: " + settings.instructionsCalculationsTextSize);
        Debug.Log("InstructionsDecisions: " + settings.instructionsDecisions + ", size: " + settings.instructionsDecisionsTextSize);
        Debug.Log("InstructionsEnding: " + settings.instructionsEnding + ", size: " + settings.instructionsEndingTextSize);
        Debug.Log("InvestmentPrompt: " + settings.instructionsInvestmentPrompt + ", size: " + settings.instructionsInvestmentPromptTextSize);

        /*PlayerPrefs.SetFloat("initialMin", settings.initialValueMin);
        PlayerPrefs.SetFloat("initialMax", settings.initialValueMax);
        PlayerPrefs.SetFloat("growthMin", settings.growthFactorMin);
        PlayerPrefs.SetFloat("growthMax", settings.growthFactorMax);
        PlayerPrefs.SetFloat("maxXMin", settings.maxXValueMin);
        PlayerPrefs.SetFloat("maxXMax", settings.maxXValueMax);
        PlayerPrefs.SetFloat("speed", settings.speed);
        PlayerPrefs.SetFloat("frequency", settings.frequency);
        PlayerPrefs.SetInt("goldBarScaling", settings.goldBarScaling);
        PlayerPrefs.SetInt("amountOfRuns", settings.amountOfRuns);
        PlayerPrefs.SetInt("afterYearsMin", settings.afterYearsMin);
        PlayerPrefs.SetInt("afterYearsMax", settings.afterYearsMax);
        PlayerPrefs.SetFloat("noise", settings.noise);
        PlayerPrefs.SetFloat("sliderMultiplierMin", settings.sliderMultiplierMin);
        PlayerPrefs.SetFloat("sliderMultiplierMax", settings.sliderMultiplierMax);

        PlayerPrefs.SetString("instructionsText1", settings.instructionsText1);
        PlayerPrefs.SetFloat("instructionsText1size", settings.instructionsText1Size);
        PlayerPrefs.SetString("instructionsText2", settings.instructionsText2);
        PlayerPrefs.SetFloat("instructionsText2Size", settings.instructionsText2Size);
        PlayerPrefs.SetString("instructionsPrevisualization", settings.instructionsPrevisualization);
        PlayerPrefs.SetFloat("instructionsPrevisualizationSize", settings.instructionsPrevisualizationTextSize);
        PlayerPrefs.SetString("instructionsCalculations", settings.instructionsCalculations);
        PlayerPrefs.SetFloat("instructionsCalculationsSize", settings)*/

        PlayerPrefs.Save();

        return settings;
    }
}
