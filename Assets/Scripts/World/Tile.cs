using System;
using UnityEngine;
namespace WorldGeneration
{

    [Serializable]
    public class Tile
    {
        private TileType type;
        private Color debugColour;
        private Sprite texture;
        public (int x, int y) Position { get; set; }

        public TileType GetTileType()
        {
            return type;
        }

        public Color GetColor()
        {
            return debugColour;
        }

        public Sprite GetTexture()
        {
            return texture;
        }

        public Tile(TileType type, Color debugColor, Sprite texture, int x, int y)
        {
            this.type = type;
            this.debugColour = debugColor;
            this.texture = texture;
            this.Position = (x, y);
        }

        public int WalkSpeed()
        {
            return 1;
        }

        public bool Walkable()
        {
            return WalkSpeed() == 0 ? false : true;
        }
    }

    public enum TileType
    {
        Rock,
        Space
    }
}