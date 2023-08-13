using UnityEngine;
namespace WorldGeneration
{


    public class Tile
    {
        private TileType type;
        private Color debugColour;
        private Sprite texture;

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

        public Tile(TileType type, Color debugColor, Sprite texture)
        {
            this.type = type;
            this.debugColour = debugColor;
            this.texture = texture;
        }
    }

    public enum TileType
    {
        Rock,
        Space
    }
}