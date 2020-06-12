using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace TeamProject
{

    public class AddLightVol : MonoBehaviour
    {
        private HDAdditionalLightData light;

        private float lastInt;

        [SerializeField]
        private float speed;

        // Start is called before the first frame update
        void Start()
        {
            light = GetComponent<HDAdditionalLightData>();

            lastInt = light.intensity;
        }

        private void Awake()
        {
            light.intensity = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //light.intensity += lastInt * Time.deltaTime * speed;

            //if(lastInt <= light.intensity)
            //{
            //    light.intensity = lastInt;
            //}
        }
    }
}