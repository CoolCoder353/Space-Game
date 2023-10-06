using UnityEngine;
using System;

[Serializable]
public class Plant
{


    public float temperatureMin;
    public float temperatureMax;

    public float fertilityThreshold;

    public Item itemOnHarvest;
    public int amountOfItemOnDeath;

    public string plantName;

    public GameObject plantObject;

    public Plant(string name, float tempMin, float tempMax, float fert, Item item, int amount, GameObject plant = null)
    {
        plantName = name;
        temperatureMax = tempMax;
        temperatureMin = tempMin;
        fertilityThreshold = fert;
        itemOnHarvest = item;
        amountOfItemOnDeath = amount;
        plantObject = plant;
    }

    public Plant(string name, Vector2 temp, float fert, Item item, int amount, GameObject plant = null)
    {
        plantName = name;
        temperatureMin = temp.x;
        temperatureMax = temp.y;
        fertilityThreshold = fert;
        itemOnHarvest = item;
        amountOfItemOnDeath = amount;
        plantObject = plant;

    }

}