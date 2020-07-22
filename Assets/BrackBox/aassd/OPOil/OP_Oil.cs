using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OP_Oil : MonoBehaviour
{
    //GameObject FlagManager;
    //TeamProject.StageStatusManager StageStatusManager;
    RawImage rend;
    public bool flg;
    public float time = 0;
    public float radius = 0;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        radius = 0;
        //FlagManager = GameObject.Find("StageStatusManager");
        //StageStatusManager = FlagManager.GetComponent<TeamProject.StageStatusManager>();

        rend = GetComponent<RawImage>();
        rend.material.shader = Shader.Find("Unlit/Oil Painting");
        rend.material.SetFloat("_Radius", radius);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(time>33.3f)
        {
            radius += Time.deltaTime + Time.deltaTime + Time.deltaTime;
            rend.material.SetFloat("_Radius", radius);
        }
    }
}
