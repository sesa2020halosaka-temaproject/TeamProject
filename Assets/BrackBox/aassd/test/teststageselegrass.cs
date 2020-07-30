using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teststageselegrass : MonoBehaviour
{
    public GameObject a;
    public bool flg,flg2;
    float timeleft;
    // Start is called before the first frame update
    void Start()
    {
        flg = false;
        flg2 = false;
        timeleft = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timeleft += Time.deltaTime;
        if (timeleft >= 1.0)
        {
            if (flg2 == false)
            {
                if (flg == true)
                {
                    a.SetActive(true);
                    flg = false;
                    flg2 = true;
                }
                else
                {
                    a.SetActive(false);
                    flg = true;
                }
            }
            timeleft = 0.0f;
        }
    }
}
