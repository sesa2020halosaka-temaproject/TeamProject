using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOut : MonoBehaviour
{
    bool flg = false;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DelayMethod", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private  void DelayMethod()
    {
       
        if (!flg)
        {
            this.gameObject.SetActive(false);
            flg = true;
        }
    }
}
