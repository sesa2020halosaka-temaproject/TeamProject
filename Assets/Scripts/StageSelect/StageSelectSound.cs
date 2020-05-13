using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
//using UnityEngine.SceneManagement;

namespace TeamProject
{
    //ステージセレクトのサウンドを担当します。
    public class StageSelectSound : MonoBehaviour
    {
        public string m_BGM_Summer;//BGM名１
        public string m_BGM_Fall;//BGM名２
        public string m_BGM_Winter;//BGM名３
        public string m_BGM_Spring;//BGM名４
        public string m_Ambient_Name;//環境音の名前
         public float m_CrossFadeTime;//クロスフェードする時間
         public float Volume;
        //=====================================================
        //関数ここから
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        //現在のワールドからBGMを設定する
        public void SetCurrentWorldBGM()
        {
            //BGMスタート
            switch (StageStatusManager.Instance.CurrentWorld)
            {
                case (int)WORLD_NO.W1://ワールド１：夏
                    m_BGM_Summer = BGMPath.BGM_STAGE_SELECT_SUMMER;
                    break;
                case (int)WORLD_NO.W2://ワールド２：秋
                    m_BGM_Summer = BGMPath.BGM_GAME_FALL;
                    break;
                case (int)WORLD_NO.W3://ワールド３：冬
                    m_BGM_Summer = BGMPath.BGM_STAGE_SELECT_WINTER;
                    break;
                case (int)WORLD_NO.W4://ワールド４：春
                    m_BGM_Summer = BGMPath.BGM_STAGE_SELECT_SPRING;
                    break;
            }

        }
        //現在のワールドから次ワールドのBGMを設定する
        public void SetNextWorldBGM()
        {
            //BGMスタート
            switch (StageStatusManager.Instance.CurrentWorld)
            {
                case (int)WORLD_NO.W1://ワールド１：夏
                     m_BGM_Summer = BGMPath.BGM_GAME_FALL;//秋
                    break;
                case (int)WORLD_NO.W2://ワールド２：秋
                     m_BGM_Summer = BGMPath.BGM_STAGE_SELECT_WINTER;//冬
                   break;
                case (int)WORLD_NO.W3://ワールド３：冬
                     m_BGM_Summer = BGMPath.BGM_STAGE_SELECT_SPRING;//春
                   break;
                case (int)WORLD_NO.W4://ワールド４：春
                   m_BGM_Summer = BGMPath.BGM_STAGE_SELECT_SUMMER;//夏
                    break;
            }

        }
        //現在のワールドから前ワールドのBGMを設定する
        public void SetPrevWorldBGM()
        {
            //BGMスタート
            switch (StageStatusManager.Instance.CurrentWorld)
            {
                case (int)WORLD_NO.W1://ワールド１：夏
                     m_BGM_Summer = BGMPath.BGM_STAGE_SELECT_SPRING;//春
                    break;
                case (int)WORLD_NO.W2://ワールド２：秋
                   m_BGM_Summer = BGMPath.BGM_STAGE_SELECT_SUMMER;//夏
                   break;
                case (int)WORLD_NO.W3://ワールド３：冬
                     m_BGM_Summer = BGMPath.BGM_GAME_FALL;//秋
                   break;
                case (int)WORLD_NO.W4://ワールド４：春
                     m_BGM_Summer = BGMPath.BGM_STAGE_SELECT_WINTER;//冬
                    break;
            }

        }
        //開始時のBGM設定
        public void StageSelectStartBGM()
        {
            SetCurrentWorldBGM();
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT;
            //BGMスタート
                    BGMManager.Instance.Play(m_BGM_Summer);
                    //水の音追加
                    BGMManager.Instance.Play(m_Ambient_Name, volumeRate: Volume, delay: 0.0f, isLoop: true, allowsDuplicate: true);
                    BGMManager.Instance.FadeIn(m_Ambient_Name, duration: 2.0f);
        }//StageSelectStartBGM() END


        public void CrossFade()
        {
            switch (StageChangeManager.GetStageChangeKey())
            {
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    SetPrevWorldBGM();
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    SetNextWorldBGM();
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                default:
                    Debug.Log("状態が違います。");
                    break;
            }
            m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT;
            string m_Ambient_Name2 = SEPath.SE_AMB_STAGE_SELECT;
            //BGMSwitcher.FadeOutAndFadeIn(m_BGM_Name, fadeInDuration: 2.5f, fadeOutDuration: 2.5f);
            BGMSwitcher.CrossFade(m_BGM_Summer, fadeDuration: m_CrossFadeTime);
            //BGMManager.Instance.FadeOut(m_Ambient_Name, duration: m_CrossFadeTime);
            BGMManager.Instance.Play(m_Ambient_Name2, volumeRate: Volume, delay: 0.0f, isLoop: true, allowsDuplicate: true);
            BGMManager.Instance.FadeIn(m_Ambient_Name2, duration: m_CrossFadeTime);
        }
    }//public class StageSelectSound : MonoBehaviour END
}//namespace END