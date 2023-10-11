using UnityEngine;
using System.Collections.Generic;
public class Temperature
{
    public Vector2 worldPosition;
    public Vector2Int position;


    public float value;
    public bool canChange;

    public Temperature(Vector2Int pos, Vector2 worldPos, bool canChange = true)
    {
        this.worldPosition = worldPos;
        this.position = pos;
        this.canChange = canChange;
    }

    public void UpdateTemperature(List<Temperature> neighbours)
    {
        float amount = value;
        foreach (Temperature temp in neighbours)
        {
            amount += temp.value;
        }
        value = (amount / (neighbours.Count + 1));
    }
}