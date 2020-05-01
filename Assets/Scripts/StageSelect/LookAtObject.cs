using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{

    public class LookAtObject : MonoBehaviour
    {
        public UnityEngine.Camera Main_Camera;
        Vector3 Camera_Position;

        // Start is called before the first frame update
        void Start()
        {
            if (Main_Camera == null)
            {

            }
            // UnityEngine.Camera Main_Camera = UnityEngine.Camera.main;
            Debug.Log("camera:" + Main_Camera);
            Debug.Log("camera:" + Main_Camera.name);
             Camera_Position = Main_Camera.transform.position;
            Debug.Log("camera:" + Camera_Position);

            this.transform.position = Camera_Position + Main_Camera.transform.forward * 1;
            Debug.Log("camera:" + Main_Camera.transform.forward);
        }

        // Update is called once per frame
        void Update()
        {
            Camera_Position = Main_Camera.transform.position;
            this.transform.position = Camera_Position + Main_Camera.transform.forward * 1;

        }

    }//public class LookAtObject : MonoBehaviour END
}//namespace END