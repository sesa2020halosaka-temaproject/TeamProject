using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutOil : MonoBehaviour
{
    RawImage rend;
    public float time = 0;
    public float radius = 0;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<RawImage>();

        // Use the Specular shader on the material
        rend.material.shader = Shader.Find("Unlit/Oil Painting");
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        radius += Time.deltaTime + Time.deltaTime ;
        if (radius < 10)
        {

            rend.material.SetFloat("_Radius", radius);
        }
        else
        {
            //this.gameObject.SetActive(false);
        }
    }
}
