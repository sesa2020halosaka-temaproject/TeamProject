using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //ステージを注視するためのオブジェクトを格納するクラス
    public class TargetStages : MonoBehaviour
    {        
        //ステージ注視用ゲームオブジェクト
        public static GameObject[] m_Stages = new GameObject[(int)STAGE_NO.STAGE_NUM];
        private void Awake()
        {
            // 子オブジェクトを全て取得する
            for (int i = 0; i < (int)STAGE_NO.STAGE_NUM; i++)
            {
                if (this.transform.GetChild(i).gameObject == null)
                {
                    Debug.Log("Stage" + i + "個目がありません。");
                    break;
                }
                m_Stages[i] = this.transform.GetChild(i).gameObject;

            }
        }
        // Start is called before the first frame update
        void Start()
        {
            
        }


        // Update is called once per frame
        //void Update()
        //{

        //}

    }//public class TargetStages : MonoBehaviour END
}//namespace END