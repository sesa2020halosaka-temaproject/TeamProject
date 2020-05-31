using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TeamProject
{
    //DollyTrackの格納用クラス

    public class DollyTrack_Box : MonoBehaviour
    {
        public CinemachineSmoothPath m_Dolly_FIXING;
        [Header("次のステージへのDollyルート(上入力時)")]
        public CinemachineSmoothPath[] m_Dolly_NextStage;
        [Header("前のステージへのDollyルート(下入力時)")]
        public CinemachineSmoothPath[] m_Dolly_PrevStage;

        [Header("次のワールドへのDollyルート(右入力時)")]
        public CinemachineSmoothPath[] m_Dolly_NextWorld;

        [Header("前のワールドへのDollyルート(左入力時)")]
        public CinemachineSmoothPath[] m_Dolly_PrevWorld;
        // private 



        private void Awake()
        {
            //配列の要素数確保
            m_Dolly_NextStage = new CinemachineSmoothPath[(int)WORLD_NO.ALL_WORLD];
            m_Dolly_PrevStage = new CinemachineSmoothPath[(int)WORLD_NO.ALL_WORLD];
            m_Dolly_NextWorld = new CinemachineSmoothPath[(int)STAGE_NO.STAGE_NUM];
            m_Dolly_PrevWorld = new CinemachineSmoothPath[(int)STAGE_NO.STAGE_NUM];

            //親オブジェクトの取得
            Transform tmp_NextStageObj = this.transform.Find("Dolly_NextStage");
            Transform tmp_PrevStageObj = this.transform.Find("Dolly_PrevStage");
            Transform tmp_NextWorldObj = this.transform.Find("Dolly_NextWorld");
            Transform tmp_PrevWorldObj = this.transform.Find("Dolly_PrevWorld");
            //this.m_Dolly_FIXING.

            //ワールド間移動用ルートの取得
            for (int i = 0; i < (int)STAGE_NO.STAGE_NUM; i++)
            {
                m_Dolly_NextWorld[i] = tmp_NextWorldObj.GetChild(i).GetComponent<CinemachineSmoothPath>();
                m_Dolly_PrevWorld[i] = tmp_PrevWorldObj.GetChild(i).GetComponent<CinemachineSmoothPath>();
                //Debug.Log("m_Dolly_NextWorld[i]:"+ m_Dolly_NextWorld[i].name);
            }
            for (int i = 0; i < (int)WORLD_NO.ALL_WORLD; i++)
            {
                m_Dolly_NextStage[i] = tmp_NextStageObj.GetChild(i).GetComponent<CinemachineSmoothPath>();
                m_Dolly_PrevStage[i] = tmp_PrevStageObj.GetChild(i).GetComponent<CinemachineSmoothPath>();
            }
            if (m_Dolly_FIXING == null)
            {

                m_Dolly_FIXING = GameObject.Find("Fixing_DollyTrack").GetComponent<CinemachineSmoothPath>();
            }
            else
            {
                Debug.Log("m_Dolly_FIXINGはnullじゃなかったよ！:" + m_Dolly_FIXING.name);

            }
            //Debug.Log(""+);

        }
        // Start is called before the first frame update
        //void Start()
        //{
        //}

        // Update is called once per frame
        //void Update()
        //{

        //}
    }//public class DollyTrack_Box : MonoBehaviour END
}//namespace END