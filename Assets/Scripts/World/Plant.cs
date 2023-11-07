using UnityEngine;
using System.Collections.Generic;
namespace WorldGeneration
{


    [System.Serializable]
    public class Plant : Tile
    {


        public float temperatureMin;
        public float temperatureMax;

        public float fertilityThreshold;

        public Item itemOnHarvest;
        public int amountOfItemOnDeath;

        public string plantName;


        private int tickBorn = 0;
        private int currentIndex = 0;
        public List<PlantGrowthIndex> plantGrowIndex = new List<PlantGrowthIndex>();

        private int randomValue = Random.Range(0, 10000);
        private int stopedAt;

        private bool stopGrowing = false;
        public Plant(Vector2Int position, Vector2 worldPosition, string name, float tempMin, float tempMax, float maxHealth, float fert, Item item, int amount, GameObject plant = null) : base(position, worldPosition, maxHealth, 0, plant)
        {

            plantName = name;
            temperatureMax = tempMax;
            temperatureMin = tempMin;
            fertilityThreshold = fert;
            itemOnHarvest = item;
            amountOfItemOnDeath = amount;



        }

        public Plant(Vector2Int position, Vector2 worldPosition, string name, Vector2 temp, float maxHealth, float fert, Item item, int amount, GameObject plant = null) : base(position, worldPosition, maxHealth, 0, plant)
        {

            plantName = name;
            temperatureMin = temp.x;
            temperatureMax = temp.y;
            fertilityThreshold = fert;
            itemOnHarvest = item;
            amountOfItemOnDeath = amount;



        }

        public void SetPlantGrowthIndex(List<PlantGrowthIndex> list)
        {
            plantGrowIndex = list;
            currentIndex = 0;
            stopGrowing = false;
        }
        // This method updates the plant's growth based on the current tick and world temperature
        public void UpdatePlant(int tick, WorldGeneration.World world)
        {
            if (tickBorn <= 1)
            {
                tickBorn = tick;
            }
            // Get the temperature at the plant's position
            Temperature currentTemp = world.GetTemperatureAtPosition(worldPosition);

            // If the temperature is outside the plant's tolerance range, remove the plant and exit the method
            if (currentTemp.value < temperatureMin || currentTemp.value > temperatureMax)
            {
                world.RemovePlant(position);
                return;
            }

            //Get the fertility at the plant's position
            float currentFertility = world.GetFloorAtPosition(worldPosition).fertility;
            if (currentFertility < fertilityThreshold)
            {
                world.RemovePlant(position);
                return;
            }

            if (stopGrowing)
            {
                if (stopedAt + randomValue <= tick)
                {
                    //Spawn a new plant somewhere around this plant
                    Vector2Int newPos = position + new Vector2Int(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
                    world.SpawnPlant(newPos, this);
                }
                return;
            }
            // Get the current growth stage of the plant
            PlantGrowthIndex growth = plantGrowIndex[currentIndex];

            // If enough ticks have passed since the plant was sown, update the plant's appearance and size
            if (tick - tickBorn >= growth.tickSinceSown)
            {
                // Update the plant's sprite if a new sprite is available
                if (growth.sprite != null)
                {
                    if (tileObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer))
                    {
                        renderer.sprite = growth.sprite;
                    }
                }

                // Update the plant's size if a new size is available
                if (growth.size != -1)
                {
                    tileObject.transform.localScale = new(growth.size, growth.size, 1);
                }

                // Move to the next growth stage if there is one
                if (currentIndex + 1 < plantGrowIndex.Count)
                {
                    currentIndex++;
                }
                else
                {
                    //If there is no more growth stages, stop updating the plant.
                    stopGrowing = true;
                    stopedAt = tick;
                }
            }
        }

    }

    [System.Serializable]
    public struct PlantGrowthIndex
    {
        public int tickSinceSown;
        public Sprite sprite;
        public float size;
    }
}