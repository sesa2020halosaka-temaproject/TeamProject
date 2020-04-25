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
            RETRY,          //上段
            STAGESELECT,    //中段
            TOTITLE,        //下段
            NOT_PAUSE,      //非ポーズ状態
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

        private GameObject[] Menu_Obj = new GameObject[(int)PAUSE_STATE.ALL_STATE-1];

        // Start is called before the first frame update
        void Start()
        {
            //親ゲームオブジェクト
            GameObject ParentObj = GameObject.Find("PauseMenuObj");
            //メニューのゲームオブジェクト取得
            Menu_Obj[(int)PAUSE_STATE.RETRY] = ParentObj.transform.GetChild((int)PAUSE_STATE.RETRY).gameObject;
            Menu_Obj[(int)PAUSE_STATE.STAGESELECT] = ParentObj.transform.GetChild((int)PAUSE_STATE.STAGESELECT).gameObject;
            Menu_Obj[(int)PAUSE_STATE.TOTITLE] = ParentObj.transform.GetChild((int)PAUSE_STATE.TOTITLE).gameObject;

            m_state = PAUSE_STATE.RETRY;//カーソルの初期位置
            SwitchingActive.GameObject_ON(Menu_Obj[(int)m_state]);
            SwitchingActive.GameObject_OFF(Menu_Obj[(int)PAUSE_STATE.STAGESELECT]);
            SwitchingActive.GameObject_OFF(Menu_Obj[(int)PAUSE_STATE.TOTITLE]);

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
                    Debug.Log("間違った状態です。エラー！");
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
