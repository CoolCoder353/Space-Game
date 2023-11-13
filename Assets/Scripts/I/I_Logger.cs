using System.Collections.Generic;
using UnityEngine;


public static class Logger
{
    public static void LogDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        Debug.Log($"Logging data for {dictionary.GetType().Name}");
        foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
        {
            Debug.Log($"Key: {kvp.Key.ToString()} ({kvp.Key.GetType().Name}), Value: {kvp.Value.ToString()} ({kvp.Value.GetType().Name}) ");
        }
    }
    public static void LogTileData(WorldGeneration.Tile tile)
    {
        foreach (KeyValuePair<string, object> entry in tile.GetAllData())
        {
            Debug.Log($"Tile data: {entry.Key} = {entry.Value}");
        }
    }

}




