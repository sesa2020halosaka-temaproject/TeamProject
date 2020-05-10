using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //ステージナンバープレートの親オブジェクト用処理
    public class StageNameParent : MonoBehaviour
    {
        public GameObject[] m_StageObjects = new GameObject[(int)STAGE_NO.STAGE_NUM];
        public string[] m_CanvasName = {
            "Stage1_1","Stage1_2","Stage1_3","Stage1_4","Stage1_5",
            "Stage2_1","Stage2_2","Stage2_3","Stage2_4","Stage2_5",
            "Stage3_1","Stage3_2","Stage3_3","Stage3_4","Stage3_5",
            "Stage4_1","Stage4_2","Stage4_3","Stage4_4","Stage4_5"
        };

        private StageNameCanvas m_StageNameCanvas;
        //確認用bool型配列
        //public bool[] m_bool = new bool[(int)STAGE_NO.STAGE_NUM];

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < (int)STAGE_NO.STAGE_NUM; i++)
            {
                m_StageObjects[i] = this.transform.GetChild(i).gameObject;
                m_StageNameCanvas = m_StageObjects[i].GetComponent<StageNameCanvas>();
                string CanvasName = "Canvas_" + m_CanvasName[i];
                if (m_StageObjects[i].name == CanvasName)
                {
                    //ステージナンバープレート（子オブジェクト）に
                    //ステージの通し番号（int型）を代入
                    m_StageNameCanvas.m_StageNumber = i;
                    //m_bool[i] = true;//確認用
                }
                else
                {
                    //m_bool[i] = false;//確認用

                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    } //public class StageNameParent : MonoBehaviour    END
} //namespace TeamProject    END
