using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
}
