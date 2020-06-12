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

        private float nowInt = 0;

        // Start is called before the first frame update
        void Start()
        {
            light = GetComponent<HDAdditionalLightData>();

            lastInt = light.intensity;
        }

        private void Awake()
        {
            light.SetIntensity(0, LightUnit.Ev100);
        }

        // Update is called once per frame
        void Update()
        {
            nowInt += lastInt * Time.deltaTime * speed;

            light.SetIntensity(nowInt, LightUnit.Ev100);

            //if (lastInt <= nowInt)
            //{
            //    nowInt = lastInt;
            //}
        }
    }
}