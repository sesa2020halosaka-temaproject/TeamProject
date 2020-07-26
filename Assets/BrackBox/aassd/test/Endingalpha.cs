using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Endingalpha : MonoBehaviour
{
    GameObject obj;

    EndingBanana flg;

    RawImage end;
    // Start is called before the first frame update
    void Start()
    {
        obj= GameObject.Find("EndingBanana");
        flg = obj.GetComponent<EndingBanana>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(flg.flg)
        {
            
        }
    }
}
