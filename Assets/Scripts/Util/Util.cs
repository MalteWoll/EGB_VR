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
    /// Loads a text file, usually a json file, and returns the content. File must be placed in the Data folder in the Resource directory.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string LoadResourceTextFile(string path)
    {
        string filePath = "Data/" + path.Replace(".json", "");
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        return targetFile.text;
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
            //Debug.Log(sb.ToString());
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            //var file = File.CreateText(path);
            /*
            using (StreamWriter sw = File.CreateText(path))
            {
                // Create the header line in the text file
                StringBuilder sb = new StringBuilder();
                sb.Append("x0,y0"); 
                for (int i = 1; i < dict.Count; i++)
                {
                    sb.Append(",x" + i.ToString() + ",y" + i.ToString());
                }
                //sb.Append("\n");
                sw.WriteLine(sb.ToString(), Encoding.UTF8);

                // Add the values of the dictionary
                StringBuilder sb2 = new StringBuilder();
                sb2.Append(dict.Keys.ElementAt(0).ToString("F2") + "," + dict.Values.ElementAt(0).ToString("F2"));
                for (int j = 1; j < dict.Count; j++)
                {
                    sb2.Append("," + dict.Keys.ElementAt(j).ToString("F2") + "," + dict.Values.ElementAt(j).ToString("F2"));
                }
                sw.WriteLine(sb2.ToString(), Encoding.UTF8);
            } */
        } else
        {
            Debug.LogError("File " + path + " exists already!");
        }

        return dateTime + ".txt"; /* Return the name of the file */
    }
}
