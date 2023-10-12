using UnityEngine;
using System;

[Serializable]
public class Plant
{

    public Vector2Int position;
    public Vector2 worldPosition;
    public float temperatureMin;
    public float temperatureMax;

    public float fertilityThreshold;

    public Item itemOnHarvest;
    public int amountOfItemOnDeath;

    public string plantName;

    public GameObject plantObject;

    private System.Action<int> OnTickUpdate;
    private int currentIndex = 0;
    public List<PlantGrowthIndex> plantGrowIndex = new List<PlantGrowthIndex>();
    public Plant(Vector2Int position, Vector2 worldPosition, string name, float tempMin, float tempMax, float fert, Item item, int amount, GameObject plant = null)
    {
        this.position = position;
        this.worldPosition = worldPosition;
        plantName = name;
        temperatureMax = tempMax;
        temperatureMin = tempMin;
        fertilityThreshold = fert;
        itemOnHarvest = item;
        amountOfItemOnDeath = amount;
        plantObject = plant;

        OnTickUpdate += UpdatePlant;
    }

    public Plant(Vector2Int position, Vector2 worldPosition, string name, Vector2 temp, float fert, Item item, int amount, GameObject plant = null)
    {
        this.position = position;
        this.worldPosition = worldPosition;
        plantName = name;
        temperatureMin = temp.x;
        temperatureMax = temp.y;
        fertilityThreshold = fert;
        itemOnHarvest = item;
        amountOfItemOnDeath = amount;
        plantObject = plant;

        OnTickUpdate += UpdatePlant;

    }
    public void SetOnTickUpdate(System.Action<int> OnTickUpdate)
    {
        this.OnTickUpdate = OnTickUpdate;
        this.OnTickUpdate += UpdatePlant;
    }

    public void SetPlantGrowthIndex(List<PlantGrowthIndex> list)
    {
        plantGrowIndex = list;
        currentIndex = 0;
    }
    public static void UpdatePlant(int tick)
    {
        PlantGrowthIndex growth = plantGrowthIndex[currentIndex];
        if (tick >= growth.tickSinceSown)
        {
            if (growth.sprite != null)
            {
                if (this.plantObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer))
                {
                    renderer.sprite = growth.sprite;
                }
            }
            if (growth.size != -1)
            {
                this.plantObject.transform.localScale = new(growth.size, growth.size, 1);
            }
            if (currentIndex + 1 < plantGrowthIndex.Count)
            {
                currentIndex++;
            }
        }
    }

}

[System.Serializable]
public struct PlantGrowthIndex
{
    public int tickSinceSown;
    public Sprite sprite;
    public int size;
}