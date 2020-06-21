using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = new Color32(248, 168, 133, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
