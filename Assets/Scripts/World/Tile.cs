using System;
using Unity.VisualScripting;
using UnityEngine;
namespace WorldGeneration
{

    public abstract class Tile
    {
        public Vector2Int position;
        public Vector2 worldPosition;

        public GameObject tileObject;

        public Tile objectAbove;
        public Tile objectBelow;

        public bool walkable => isWalkable();

        public int walkSpeed;

        public float maxHealth;
        public float currentHealth;



        public Tile(Vector2Int position, Vector2 worldPosition, float maxHealth, int walkSpeed, GameObject tileObject = null)
        {
            this.position = position;
            this.worldPosition = worldPosition;

            this.tileObject = tileObject;
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
            this.walkSpeed = walkSpeed;

        }
        public bool isWalkable()
        {
            if (objectAbove != null)
            {
                return false;
            }
            if (walkSpeed == 0)
            {
                return false;
            }
            return true;
        }
        public int DamageTile(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                return 1;
            }
            return 0;
        }

    }

}