using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAlpha : MonoBehaviour
{


    public Material[] _material;
    // Start is called before the first frame update
    void Start()
    {
     
    }

    private void Update()
    {

    }

    public void OnCollisionEnter(Collision collision)
    {

     
        if (collision.collider.CompareTag("Player"))
        {
         
            for (float a = 255; a > 100; a -= Time.deltaTime)
            {
                this.transform.root.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor",new Color(0.2f,0.2f,0.2f,0.3f));
                
            }
        }
    }
    //public void OnCollisionExit(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        this.transform.root.gameObject.GetComponent<Renderer>().material=_material[1];
    //    }
    //}
}
