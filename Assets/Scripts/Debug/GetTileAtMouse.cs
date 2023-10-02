using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using WorldGeneration;


public class GetTileAtMouse : MonoBehaviour
{

    public GameObject Text;
    public GameObject parent;


    private World world;

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

        //See if that is on a tile.
        Tile tile = world.GetHillTileAtPosition(mousePos);
        if (tile == null)
        {
            tile = world.GetFloorTileAtPosition(mousePos);
        }

        if (tile != null)
        {
            //Remove all children from the parent object.
            foreach (Transform child in parent.transform)
            {
                Destroy(child.gameObject);
            }


            foreach (FieldInfo field in typeof(Tile).GetFields(bindingFlags))
            {
                //foreach field in tile, create a text object and set the text to the field name and value.
                GameObject text = Instantiate(Text, parent.transform);
                text.GetComponent<TextMeshProUGUI>().text = $"{field.Name.FirstCharacterToUpper()}: {field.GetValue(tile)}";

            }


        }
    }
}