﻿using System.Collections;
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
            KEYWAIT,//キー入力待ち
            FADE_OUT,//フェードアウト＝シーン遷移中
            ALL_STATE//全状態数
        }
        public CURSORSTATE state;//現在の状態
        [Header("ステージセレクトシーンへの遷移のフェードアウト時間")]
        public float m_FadeOut_Time;
        public float m_FadeOut_DeltaTime;
        [Header("オープニングシーンへの遷移のフェードアウト時間")]
        public float m_OP_FadeOut_Time;
        //[Header("プロローグシーンへの遷移のフェードアウト時間")]
        //public float m_PR_FadeOut_Time;
        //[Header("プロローグシーンへ戻るまでの操作無し時間")]
        //public float m_BackPrologueMaxTime;
        //public float m_TimeCount;//時間のカウント用

        public static bool m_CursorMoveFlag;//カーソル移動許可フラグ（メニューのフェードインが終わるまで待機）
        public float _BeforeTrigger;//1フレーム前の入力状態を取得する


        //入力状態確認用
        public enum INPUTSTATE
        {
            NONE,       //押されていない
            START,      //押した瞬間
            HOLD,       //押し続けている
            END,        //離した瞬間
            ALLSTATE
        }
        public INPUTSTATE m_InputState;//入力状態
        //上下入力識別用
        public enum KEYSTATE
        {
            NONE,       //押されていない
            UP,      //上入力
            DOWN,       //下入力
            DECISION,       //決定入力
            ALLSTATE
        }
        private KEYSTATE m_KeyState;//上下入力識別
        public bool m_CurrentInput = false;//現在のフレーム
        public bool m_BeforeInput = false;//前のフレーム

        public bool m_CurrentInput_L1 = false;//現在のフレーム
        public bool m_BeforeInput_L1 = false;//前のフレーム
        public bool m_CurrentInput_R1 = false;//現在のフレーム
        public bool m_BeforeInput_R1 = false;//前のフレーム
        public bool m_CurrentInput_START = false;//現在のフレーム
        public bool m_BeforeInput_START = false;//前のフレーム

        [Header("OPを一度でも見たかどうかフラグ")]
        public bool m_IsWatchOP;
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
            SwitchingActive.GameObject_ON(Menu_Obj[(int)cursor]);
            //Menu_Obj[(int)cursor].transform.GetChild(0).gameObject.SetActive(false);
            //Menu_Obj[(int)cursor].transform.GetChild(1).gameObject.SetActive(true);

            m_CursorMoveFlag = false;

            //OPを見ているかどうかフラグをセット
            m_IsWatchOP = StageStatusManager.Instance.m_WatchOpeningFlag;
        } //void Start()    END


        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case CURSORSTATE.FADE_IN:
                    if (m_CursorMoveFlag) state = CURSORSTATE.KEYWAIT;
                    break;

                case CURSORSTATE.KEYWAIT:
                    //前のフレームの入力状態の保存
                    m_BeforeInput = m_CurrentInput;
                    m_BeforeInput_L1 = m_CurrentInput_L1;
                    m_BeforeInput_R1 = m_CurrentInput_R1;
                    m_BeforeInput_START = m_CurrentInput_START;


                    //入力キーの識別
                    CheckKeyInput();
                    //入力状態の切り替え
                    SwitchingInputState();

                    //入力状態による処理
                    switch (m_InputState)
                    {
                        case INPUTSTATE.NONE:
                            break;
                        case INPUTSTATE.START:
                            //キーごとの処理
                            RunOfInputKey();
                            break;
                        case INPUTSTATE.HOLD:
                            break;
                        case INPUTSTATE.END:
                            break;
                        case INPUTSTATE.ALLSTATE:
                            break;
                        default:
                            break;
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

        //m_CurrentInputをONにする
        public void CurrentInputOn()
        {
            m_CurrentInput = true;
        }

        //m_CurrentInputをONにするをOFFにする
        public void CurrentInputOff()
        {
            m_CurrentInput = false;
        }

        //入力キーの識別
        public void CheckKeyInput()
        {
            if (!LBRBButtonInputCheck() && !STARTButtonInputCheck())
            {
                //上入力
                if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCode.UpArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    //m_TimeCount = 0;
                    CurrentInputOn();
                    m_KeyState = KEYSTATE.UP;
                }
                //下入力
                else if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCode.DownArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    //m_TimeCount = 0;
                    CurrentInputOn();
                    m_KeyState = KEYSTATE.DOWN;
                }
                //カーソルの操作（決定）
                else if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtonCode.A))
                {
                    // m_TimeCount = 0;
                    CurrentInputOn();
                    m_KeyState = KEYSTATE.DECISION;
                }
                else
                {
                    //m_TimeCount += Time.deltaTime;
                    CurrentInputOff();
                    m_KeyState = KEYSTATE.NONE;
                }

            }
            else if (LBRBButtonInputCheck())
            {
                //カーソルの操作（決定）
                if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtonCode.A))
                {
                    //ステージアンロック状態にする
                    StageStatusManager.Instance.m_RemovalLimitFlag = true;
                    //星の音鳴らす
                    SEManager.Instance.Play(SEPath.SE_STAR);

                    CurrentInputOn();
                    m_KeyState = KEYSTATE.DECISION;
                }

            }
            else if (STARTButtonInputCheck())
            {
                if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtonCode.A))
                {
                    //オールアンロック状態にする
                    StageStatusManager.Instance.AllUnlockActivation();
                    //OPを見ているかどうかフラグをセット
                    m_IsWatchOP = StageStatusManager.Instance.m_WatchOpeningFlag;

                    //星の音鳴らす
                    SEManager.Instance.Play(SEPath.SE_STAR);
                    CurrentInputOn();
                    m_KeyState = KEYSTATE.DECISION;
                }
            }
            //隠しコマンドの入力を取る関数
            InputHiddenCommand();

        }// END
        //LBボタンとRBボタン同時押し判定
        public bool LBRBButtonInputCheck()
        {
            bool Bool = false;
            if (m_CurrentInput_R1)
            {
                Debug.Log("m_CurrentInput_R1");
                if (m_CurrentInput_L1)
                {
                    Debug.Log("m_CurrentInput_L1");
                    Bool = true;
                }

            }
            return Bool;//((m_CurrentInput_R1 /*|| m_BeforeInput_R1*/) && (m_CurrentInput_L1/* || m_BeforeInput_L1*/));
        }
        //STARTボタン押してるか判定
        public bool STARTButtonInputCheck()
        {
            return (m_CurrentInput_START || m_BeforeInput_START);
        }
        //入力状態の切り替え
        public void SwitchingInputState()
        {
            if (!m_BeforeInput && !m_CurrentInput)
            {
                m_InputState = INPUTSTATE.NONE;
            }
            else if (m_BeforeInput && !m_CurrentInput)
            {
                Debug.Log("END！");

                m_InputState = INPUTSTATE.END;
            }
            else if (!m_BeforeInput && m_CurrentInput)
            {
                Debug.Log("START！");

                m_InputState = INPUTSTATE.START;
            }
            else if (m_BeforeInput && m_CurrentInput)
            {
                Debug.Log("HOLD！");
                m_InputState = INPUTSTATE.HOLD;
            }
        }// END

        //キーごとの処理
        public void RunOfInputKey()
        {
            switch (m_KeyState)
            {
                case KEYSTATE.NONE:
                    break;
                case KEYSTATE.UP:
                    //上入力
                    //カーソル位置の更新
                    UpdateCursorPosition();

                    Debug.Log("上を押しました！");
                    //カーソルの移動音
                    SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);

                    break;
                case KEYSTATE.DOWN:
                    //下入力
                    //カーソル位置の更新
                    UpdateCursorPosition();

                    Debug.Log("下を押しました！");
                    //カーソルの移動音
                    SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);

                    break;
                case KEYSTATE.DECISION:
                    switch (cursor)
                    {
                        case CURSORPOSI.GAMESTART:
                            if (!m_IsWatchOP)
                            {
                                StageStatusManager.Instance.m_WatchOpeningFlag = true;
                                StageStatusManager.Instance.CurrentStage = STAGE_NO.STAGE01;
                                FadeManager.FadeOut("OpeningScene", time: m_OP_FadeOut_Time);
                            }
                            else
                            {
                                FadeManager.FadeOut("StageSelectScene", time: m_FadeOut_Time);
                            }
                            break;
                        case CURSORPOSI.STAGESELECT:
                            FadeManager.FadeOut("StageSelectScene", time: m_FadeOut_Time);
                            break;
                        case CURSORPOSI.EXIT:
                            Quit();
                            break;
                        case CURSORPOSI.ALL:
                            Debug.Log("間違った状態です！");
                            break;
                    }

                    //カーソル状態の更新
                    state = CURSORSTATE.FADE_OUT;
                    Debug.Log("決定です！");
                    //決定音鳴らす
                    SEManager.Instance.Play(SEPath.SE_OK);
                    //BGMのフェードアウト
                    BGMManager.Instance.FadeOut(BGMPath.BGM_TITLE, duration: m_FadeOut_Time + m_FadeOut_DeltaTime);
                    break;
                case KEYSTATE.ALLSTATE:
                    break;
                default:
                    break;
            }

        }

        //カーソル位置の更新　カーソルの位置における表示UIの変更
        public void UpdateCursorPosition()
        {
            switch (cursor)
            {
                case CURSORPOSI.GAMESTART:
                    if (m_KeyState == KEYSTATE.UP) { cursor = CURSORPOSI.EXIT; }
                    else if (m_KeyState == KEYSTATE.DOWN) { cursor = CURSORPOSI.EXIT; }
                    break;

                //=============================================================================
                //廃止されました
                case CURSORPOSI.STAGESELECT:
                    //if (m_KeyState == KEYSTATE.UP) { cursor = CURSORPOSI.GAMESTART; }
                    //else if (m_KeyState == KEYSTATE.DOWN) { cursor = CURSORPOSI.EXIT; }
                    break;
                //=============================================================================

                case CURSORPOSI.EXIT:
                    if (m_KeyState == KEYSTATE.UP) { cursor = CURSORPOSI.GAMESTART; }
                    else if (m_KeyState == KEYSTATE.DOWN) { cursor = CURSORPOSI.GAMESTART; }
                    break;

                case CURSORPOSI.ALL:
                    Debug.Log("間違った状態です！");
                    break;
            }//switch (cursor)   END

            //全てをOFFにする
            for (int i = 0; i < (int)CURSORPOSI.ALL; i++)
            {
                SwitchingActive.GameObject_OFF(Menu_Obj[i]);
            }
            //現在カーソル位置をONにする
            SwitchingActive.GameObject_ON(Menu_Obj[(int)cursor]);

        }// public void UpdateCursorPosition() END

        //入力許可フラグのON
        public static void CursorMoveFlagOn()
        {
            m_CursorMoveFlag = true;
        }

        //隠しコマンド入力確認
        public void InputHiddenCommand()
        {
            //RBトリガー or Xキーを押しているかどうか
            if (Input.GetKey("joystick button 4") || Input.GetKey(KeyCode.X))
                m_CurrentInput_R1 = true;
            else
                m_CurrentInput_R1 = false;

            //LBトリガー or Zキーを押しているかどうか
            if (Input.GetKey("joystick button 5") || Input.GetKey(KeyCode.Z))
                m_CurrentInput_L1 = true;
            else
                m_CurrentInput_L1 = false;

            //STARTボタン or ESCキーを押しているかどうか
            if (InputManager.InputManager.Instance.GetKey(InputManager.ButtonCode.Menu))
                m_CurrentInput_START = true;
            else
                m_CurrentInput_START = false;

        }

        //ゲーム終了処理関数
        void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
        }

    } //public class CursorScript : MonoBehaviour    END
} //namespace TeamProject    END
