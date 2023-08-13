using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{


    public class Character_Controler : MonoBehaviour
    {

        public Character_Settings settings;


        private Vector2 movement;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            movement.x = Input.GetAxis("Horizontal") * settings.speed * Time.deltaTime;
            movement.y = Input.GetAxis("Vertical") * settings.speed * Time.deltaTime;
        }

        private void LateUpdate()
        {

        }
    }
}