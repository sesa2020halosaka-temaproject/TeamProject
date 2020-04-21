using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    namespace Masuyama
    {
        public class Billboard : MonoBehaviour
        {

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                transform.rotation = Quaternion.LookRotation(-UnityEngine.Camera.main.transform.position, Vector3.up);
            }
        }
    }
}