using UnityEngine;
using System.Collections.Generic;

namespace WorldGeneration
{


    public class Tile
    {
        public Dictionary<string, object> data = new Dictionary<string, object>();
        public Dictionary<string, object> modifiers = new Dictionary<string, object>();

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
        public Tile(Vector2 worldPos, Vector2Int gridPos, float health, float fertility)
        {
            data["worldPos"] = worldPos;
            data["gridPos"] = gridPos;
            data["maxHealth"] = health;
            data["currentHealth"] = health;
            data["fertility"] = fertility;
        }


    }
}