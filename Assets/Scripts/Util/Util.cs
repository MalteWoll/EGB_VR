using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;
using UnityEngine;

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

    public static void WriteOutputFile(string data)
    {
        string path = Application.persistentDataPath + "/output.txt"; /* TODO: Check path and create random identifier */
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(data);
        writer.Close();
    }
}
