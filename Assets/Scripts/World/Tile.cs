using UnityEngine;
using System.Collections.Generic;

namespace WorldGeneration
{


    public class Tile
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();
        private Dictionary<string, object> modifiers = new Dictionary<string, object>();

        public Vector2 worldPos
        {
            get
            {
                return (Vector2)data["worldPos"];
            }
            set
            {
                data["worldPos"] = value;
            }
        }
        public Vector2Int pos
        {
            get
            {
                return (Vector2Int)data["gridPos"];
            }
            set
            {
                data["gridPos"] = value;
            }
        }
        public Vector2Int gridPos
        {
            get
            {
                return (Vector2Int)data["gridPos"];
            }
            set
            {
                data["gridPos"] = value;
            }
        }

        public float maxHealth
        {
            get
            {
                return (float)data["maxHealth"];
            }
            set
            {
                data["maxHealth"] = value;
            }
        }
        public float currentHealth
        {
            get
            {
                return (float)data["currentHealth"];
            }
            set
            {
                data["currnetHealth"] = value;
            }
        }
        public float health => currentHealth;

        public bool HasData(string varName)
        {
            return data.ContainsKey(varName);
        }

        public bool HasModifier(string varName)
        {
            return modifiers.ContainsKey(varName);
        }

        public object GetData(string varName)
        {
            if (HasData(varName))
            {
                return data[varName];
            }
            else
            {
                Debug.LogError($"Variable '{varName}' does not exist in the data dictionary.");
                return null;
            }
        }
        public Dictionary<string, object> GetAllData()
        {
            return data;
        }

        public void SetData(string varName, object value)
        {
            data[varName] = value;
        }

        public object GetModifier(string varName)
        {
            if (HasModifier(varName))
            {
                return modifiers[varName];
            }
            else
            {
                Debug.LogError($"Variable '{varName}' does not exist in the modifiers dictionary.");
                return null;
            }
        }
        public void SetModifier(string varName, object value)
        {
            modifiers[varName] = value;
        }

        public Tile(Vector2 worldPos, Vector2Int gridPos, float health, float fertility)
        {
            data["worldPos"] = worldPos;
            data["gridPos"] = gridPos;
            data["maxHealth"] = health;
            data["currentHealth"] = health;
            data["fertility"] = fertility;
        }

        public void UpdateTileData(World world, TileData data)
        {

            maxHealth = data.health;
            currentHealth = data.health;
            SetData("fertility", data.fertilty);
            SetData("isWalkable", data.isWalkable);
            SetData("spritePath", data.spritePath);

            SpriteRenderer renderer = ((GameObject)GetData("floorObject")).GetComponent<SpriteRenderer>();
            renderer.sprite = world.LoadSprite(data);

            // Update other properties as needed...

        }


    }
}