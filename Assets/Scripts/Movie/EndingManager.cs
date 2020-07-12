using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //エンディングシーン用クラス（MovieManager継承）
    public class EndingManager : MovieManager
    {
        // Start is called before the first frame update
        void Start()
        {
            //ムービー後に遷移するシーンのセット
            m_NextSceneName = "TitleScene";

            //エンディングからタイトルに遷移しますよフラグをONにする。
            StageStatusManager.Instance.m_EDtoTITLE_Flag = true;

            //デバッグ用のフラグON(基本はコメントアウト)
            //StageStatusManager.Instance.m_LastStageClearFlag = true;

        }

        // Update is called once per frame
        void Update()
        {
            base.MovieStateUpdate();
        }
    } //public class EndingManager : MonoBehaviour    END
} //namespace TeamProject    END
