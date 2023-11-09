using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using WorldGeneration;
using System.Collections.Generic;


public class GetTileAtMouse : MonoBehaviour
{

    public GameObject Text;
    public GameObject parent;


    private World world;


    private List<GameObject> uiElements = new List<GameObject>();
    private BindingFlags bindingFlags = BindingFlags.Public |
                                BindingFlags.NonPublic |
                                BindingFlags.Instance |
                                BindingFlags.Static;
    private void Start()
    {
        world = FindObjectOfType<World>();

    }

    private void Update()
    {
        //Get mouse pos at this point.
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Plant plant = world.GetPlantAtPosition(mousePos);
        if (plant == null)
        {
            //See if that is on a tile.
            Floor tile = world.GetHillAtPosition(mousePos);
            if (tile == null)
            {
                tile = world.GetFloorAtPosition(mousePos);
            }

            showTileData(tile);
        }
        else
        {
            showPlantData(plant);
        }

    }


    private void showTileData(Tile tile)
    {
        if (tile != null)
        {
            int i = 0;
            foreach (FieldInfo field in typeof(Tile).GetFields(bindingFlags))
            {

                if (uiElements.Count <= i)
                {

                    //foreach field in tile, create a text object and set the text to the field name and value.
                    GameObject text = Instantiate(Text, parent.transform);
                    uiElements.Add(text);
                    text.GetComponent<TextMeshProUGUI>().text = $"{field.Name.FirstCharacterToUpper()}: {field.GetValue(tile)}";
                }
                else
                {
                    GameObject text = uiElements[i];
                    text.GetComponent<TextMeshProUGUI>().text = $"{field.Name.FirstCharacterToUpper()}: {field.GetValue(tile)}";
                }

                i++;
            }

            for (int x = i; x < uiElements.Count; x++)
            {
                GameObject element = uiElements[x];
                uiElements.Remove(element);
                Destroy(element);
            }
        }
    }
}