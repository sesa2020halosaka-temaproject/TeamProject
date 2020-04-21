using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeamProject
{
    public class PauseManager : MonoBehaviour
    {
        //ポーズ画面の状態
        public enum PAUSE_STATE
        {
            NOT_PAUSE,      //非ポーズ状態
            RETRY,          //上段
            STAGESELECT,    //中段
            TOTITLE,        //下段
            ALL_STATE       //全状態数
        }
        public PAUSE_STATE m_state;

        //UIの選択状態兼インデックス
        public enum UI_STATE
        {
            BG,         //0:背景
            OFF,        //1:非選択状態
            ON,         //2:選択状態
            ALL_STATE   //3:全状態数
        }
        public UI_STATE m_UI;

        public static bool m_PauseFlag;//ポーズ画面フラグ

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            switch (m_state)
            {
                case PAUSE_STATE.NOT_PAUSE:
                    if (m_PauseFlag) m_state = PAUSE_STATE.RETRY;
                    break;
                case PAUSE_STATE.RETRY:
                    break;
                case PAUSE_STATE.STAGESELECT:
                    break;
                case PAUSE_STATE.TOTITLE:
                    break;
                case PAUSE_STATE.ALL_STATE:
                    break;
                default:
                    break;
            }
        }
        //ポーズ画面フラグのON
        public static void PauseFlagOn()
        {
            m_PauseFlag = true;
        }

    } //public class PauseManager : MonoBehaviour    END
} //namespace TeamProject    END
