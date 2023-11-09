using System.Xml;
using System.Collections.Generic;
public static class Xml
{
    public static Dictionary<string, TileData> LoadTileData(string dataPath)
    {
        Dictionary<string, TileData> tileDic = new();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(dataPath + "/TileTypes.xml"); // Path to your XML file

        XmlNodeList tileNodes = xmlDoc.SelectNodes("TileTypes/TileType");

        foreach (XmlNode tileNode in tileNodes)
        {
            string name = tileNode.SelectSingleNode("Name").InnerText;
            string sprite = tileNode.SelectSingleNode("Sprite").InnerText;
            bool isWalkable = bool.Parse(tileNode.SelectSingleNode("IsWalkable").InnerText);
            float health = float.Parse(tileNode.SelectSingleNode("Health").InnerText);
            float fertility = float.Parse(tileNode.SelectSingleNode("Fertility").InnerText);


            tileDic[name] = new(name, sprite, isWalkable, health, fertility);

        }
        return tileDic;
    }
}