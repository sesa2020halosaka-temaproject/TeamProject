using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //エンディングシーン用クラス（MovieManager継承）
    public class EndingManager : MovieManager
    {
        [Header("(デバッグ用・チェック外し忘れに注意)")]
        [Header("ラストステージクリア済みフラグ")]
        public bool m_LastStageClear_Flag;

        // Start is called before the first frame update
        void Start()
        {
            //ムービー後に遷移するシーンのセット
            m_NextSceneName = "TitleScene";

            //エンディングからタイトルに遷移しますよフラグをONにする。
            StageStatusManager.Instance.m_EDtoTITLE_Flag = true;

            if (!StageStatusManager.Instance.m_LastStageClearFlag)
            {
                //フラグがfalseの時はデバッグ用のフラグをセットする
                StageStatusManager.Instance.m_LastStageClearFlag = m_LastStageClear_Flag;

            }
            //フラグがtrueの時はスルー
        }

        // Update is called once per frame
        void Update()
        {
            base.MovieStateUpdate();
        }
    } //public class EndingManager : MonoBehaviour    END
} //namespace TeamProject    END
