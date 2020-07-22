using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeOil : MonoBehaviour
{
    GameObject FlagManager;
    TeamProject.StageStatusManager StageStatusManager;
    RawImage rend;
    public bool flg;
    public float time = 10;
    public float radius = 10;

    void Start()
    {
        //フラグ管理にアクセス
        FlagManager = GameObject.Find("StageStatusManager");
        StageStatusManager = FlagManager.GetComponent<TeamProject.StageStatusManager>();
        rend = GetComponent<RawImage> ();

  
        rend.material.shader = Shader.Find("Unlit/Oil Painting");

        //if(b.m_WatchOpeningFlag)
        //{
        //    this.gameObject.SetActive(false);
        //}

        //STAGE1-1突入フラグがtrueだったら動作させない
        if(StageStatusManager.m_S1Flag)
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        radius -= Time.deltaTime + Time.deltaTime + Time.deltaTime; //仮
        if (time > 0)
        {
          
            rend.material.SetFloat("_Radius", radius);
        }
        else
        {
            StageStatusManager.m_S1Flag = true;  //STAGE1-1突入フラグをtrueにし次回から実行させなくする
            this.gameObject.SetActive(false);
        }
    }
}
