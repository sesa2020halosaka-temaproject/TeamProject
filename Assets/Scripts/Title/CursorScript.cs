using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    public class CursorScript : MonoBehaviour
    {
        //カーソルの現在位置用
        public enum CURSORPOSI
        {
            GAMESTART,  //上段
            STAGESELECT,//中段
            EXIT,       //下段
            ALL
        }
        private CURSORPOSI cursor;//カーソルの位置
        private GameObject[] Menu_Obj = new GameObject[(int)CURSORPOSI.ALL];

        //カーソルの状態
        public enum CURSORSTATE
        {
            FADE_IN,//メニューのフェードイン中
            PRESS_ANYKEY,//キー入力待ち
            FADE_OUT,//フェードアウト＝シーン遷移中
            ALL_STATE//全状態数
        }
        public CURSORSTATE state;//現在の状態
        public static bool m_InputFlag;//カーソル移動許可フラグ（メニューのフェードインが終わるまで待機）
        public float _BeforeTrigger;//1フレーム前の入力状態を取得する

        // Start is called before the first frame update
        void Start()
        {
            //親ゲームオブジェクト
            GameObject ParentObj = GameObject.Find("TitleMenuObj");
            //メニューのゲームオブジェクト取得
            Menu_Obj[(int)CURSORPOSI.GAMESTART] = ParentObj.transform.Find("GameStart").gameObject;
            Menu_Obj[(int)CURSORPOSI.STAGESELECT] = ParentObj.transform.Find("StageSelect").gameObject;
            Menu_Obj[(int)CURSORPOSI.EXIT] = ParentObj.transform.Find("Exit").gameObject;
            for (int i = 0; i < (int)CURSORPOSI.ALL; i++)
            {
                Menu_Obj[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            cursor = CURSORPOSI.GAMESTART;//カーソルの初期位置
            Menu_Obj[(int)cursor].transform.GetChild(0).gameObject.SetActive(false);
            Menu_Obj[(int)cursor].transform.GetChild(1).gameObject.SetActive(true);

            m_InputFlag = false;
        } //void Start()    END


        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case CURSORSTATE.FADE_IN:
                    if (m_InputFlag) state = CURSORSTATE.PRESS_ANYKEY;
                    break;

                case CURSORSTATE.PRESS_ANYKEY:
                    if ((InputManager.InputManager.Instance.GetLStick().y > 0) && _BeforeTrigger == 0)
                    {
                        //上入力
                        MenuCursorActiveChange("UP");
                        Debug.Log("上を押しました！");

                        //カーソルの移動音
                        SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
                    }
                    else if ((InputManager.InputManager.Instance.GetLStick().y < 0) && _BeforeTrigger == 0)
                    {
                        //下入力
                        MenuCursorActiveChange("DOWN");
                        Debug.Log("下を押しました！");

                        //カーソルの移動音
                        SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);

                    }
                    //現在の入力状態の更新
                    _BeforeTrigger = InputManager.InputManager.Instance.GetLStick().y;


                    //カーソルの操作（決定）
                    if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtunCode.A))
                    //if (Input.GetKeyDown(KeyCode.Space))
                    {
                        switch (cursor)
                        {
                            case CURSORPOSI.GAMESTART:
                                FadeManager.FadeOut("Stage1_1");
                                StageStatusManager.Instance.CurrentStage = STAGE_NO.STAGE01;
                                break;
                            case CURSORPOSI.STAGESELECT:
                                FadeManager.FadeOut("StageSelectScene");
                                break;
                            case CURSORPOSI.EXIT:
                                Quit();
                                break;
                            case CURSORPOSI.ALL:
                                Debug.Log("間違った状態です！");
                                break;
                        }
                        state = CURSORSTATE.FADE_OUT;
                        Debug.Log("決定です！");

                        //決定音鳴らす
                        SEManager.Instance.Play(SEPath.SE_OK);
                    }

                    break;
                case CURSORSTATE.FADE_OUT:
                    break;
                case CURSORSTATE.ALL_STATE:
                    break;
                default:
                    break;
            }// switch (state)  END

        } //void Update()    END

        //ゲーム終了処理関数
        void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
        }

        //カーソルの位置における表示UIの変更
        //stringで上下の入力の確認
        public void MenuCursorActiveChange(string _word)
        {
            switch (cursor)
            {
                case CURSORPOSI.GAMESTART:
                    Menu_Obj[(int)cursor].transform.GetChild(0).gameObject.SetActive(true);
                    Menu_Obj[(int)cursor].transform.GetChild(1).gameObject.SetActive(false);

                    if (_word == "UP") cursor = CURSORPOSI.EXIT;
                    else if (_word == "DOWN") cursor = CURSORPOSI.STAGESELECT;

                    Menu_Obj[(int)cursor].transform.GetChild(0).gameObject.SetActive(false);
                    Menu_Obj[(int)cursor].transform.GetChild(1).gameObject.SetActive(true);

                    break;
                case CURSORPOSI.STAGESELECT:
                    Menu_Obj[(int)cursor].transform.GetChild(0).gameObject.SetActive(true);
                    Menu_Obj[(int)cursor].transform.GetChild(1).gameObject.SetActive(false);

                    if (_word == "UP") cursor = CURSORPOSI.GAMESTART;
                    else if (_word == "DOWN") cursor = CURSORPOSI.EXIT;

                    Menu_Obj[(int)cursor].transform.GetChild(0).gameObject.SetActive(false);
                    Menu_Obj[(int)cursor].transform.GetChild(1).gameObject.SetActive(true);

                    break;
                case CURSORPOSI.EXIT:
                    Menu_Obj[(int)cursor].transform.GetChild(0).gameObject.SetActive(true);
                    Menu_Obj[(int)cursor].transform.GetChild(1).gameObject.SetActive(false);

                    if (_word == "UP") cursor = CURSORPOSI.STAGESELECT;
                    else if (_word == "DOWN") cursor = CURSORPOSI.GAMESTART;

                    Menu_Obj[(int)cursor].transform.GetChild(0).gameObject.SetActive(false);
                    Menu_Obj[(int)cursor].transform.GetChild(1).gameObject.SetActive(true);

                    break;
                case CURSORPOSI.ALL:
                    Debug.Log("間違った状態です！");
                    break;
            }

        }//        public void MenuCursorActiveChange(string _word) END

        //入力許可フラグのON
        public static void InputFlagOn()
        {
            m_InputFlag = true;
        }

    } //public class CursorScript : MonoBehaviour    END
} //namespace TeamProject    END
