using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{


    public class Character_Controler : MonoBehaviour
    {

        public Character_Settings settings;
        public Camera playerCamera;


        private Vector3 movement;
        private float scroll;


        // Update is called once per frame
        void Update()
        {
            //    SHIFT     \\
            float shiftMultiply = 1;
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                shiftMultiply = settings.shiftSpeedMultiplyer;
            }
            //     SCROLL     \\
            scroll = settings.scrollSpeed * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
            float percentageScroll = Mathf.Clamp(playerCamera.orthographicSize / settings.zoomScale.y, settings.zoomSpeedScale.x, settings.zoomSpeedScale.y);

            //     MOVEMENT    \\
            movement.x = Input.GetAxis("Horizontal") * settings.speed * shiftMultiply * percentageScroll * Time.deltaTime;
            movement.y = Input.GetAxis("Vertical") * settings.speed * shiftMultiply * percentageScroll * Time.deltaTime;


            //     DEBUG     \\
            if (settings.debug)
            {
                Debug.Log("Movement: " + movement);
                Debug.Log("Scroll: " + scroll);
                Debug.Log("% Scroll: " + percentageScroll);
            }

        }

        private void LateUpdate()
        {
            this.gameObject.transform.position += movement;
            playerCamera.orthographicSize = Mathf.Clamp(playerCamera.orthographicSize - scroll, settings.zoomScale.x, settings.zoomScale.y);
        }
    }
}