using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamProject.InputManager;

namespace TeamProject
{
    public class Camera : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        [SerializeField]
        private float min, max;
        
        // Start is called before the first frame update
        void Start()
        {
            var a = UnityEngine.Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            var stick = InputManager.InputManager.Instance.GetRStick();

            Vector3 rot = transform.rotation.eulerAngles;


            Debug.Log(stick);


            float x, y;

            x = stick.x * speed * Time.deltaTime;
            y = stick.y * speed * Time.deltaTime;

            rot.x += y;
            rot.y -= x;

            if (rot.x + x < min)
            {
                rot.x = min;
            }
            if (max < rot.x + x)
            {
                rot.x = max;
            }
            
            var qua = transform.rotation;
            rot.z = 0.0f;
            qua.eulerAngles = rot;
            transform.rotation = qua;
        }
    }
}