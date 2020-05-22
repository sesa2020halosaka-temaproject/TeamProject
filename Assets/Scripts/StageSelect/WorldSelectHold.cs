using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeamProject
{
    //ワールド移動開始直前の処理用クラス
    public class WorldSelectHold : MonoBehaviour
    {
        //現在ワールド位置 
        //StageStatusManager.Instance.CurrentWorld;
        //public static GameObject m_Canvas;//一番上の親オブジェクト
        public static GameObject m_Next;//右矢印用オブジェクト
        public static GameObject m_Prev;//左矢印用オブジェクト
        public static GameObject m_Next2;//右矢印用オブジェクト
        public static GameObject m_Prev2;//左矢印用オブジェクト
        public static GameObject m_Next3;//右矢印用オブジェクト
        public static GameObject m_Prev3;//左矢印用オブジェクト

        public float m_MaxWidth_L;
        public float m_MaxWidth_R;
        public float m_MaxHeight_L;
        public float m_MaxHeight_R;
        public float m_HoldTime;
        public float m_HoldCount;
        public float m_SizeDeltaX;
        public enum INPUTSTATE
        {
            NONE,       //押されていない
            START,      //押した瞬間
            HOLD,       //押し続けている
            END,        //離した瞬間
            ALLSTATE
        }
        public INPUTSTATE m_InputState_L;
        public INPUTSTATE m_InputState_R;
        public bool m_NextMove_Flag = false;
        public bool m_PrevMove_Flag = false;

        public bool m_CurrentInput_L = false;//現在のフレーム
        public bool m_BeforeInput_L = false;//前のフレーム
        public bool m_CurrentInput_R = false;//現在のフレーム
        public bool m_BeforeInput_R = false;//前のフレーム


        private void Awake()
        {
            //m_Canvas = transform.root.gameObject;//一番上の親を取得
            m_Next = transform.GetChild(0).gameObject;//"NextWorldInformation"オブジェクト
            m_Next2 = m_Next.transform.GetChild(0).gameObject;//"NextWorldArrowObj"オブジェクト
            m_Next3 = m_Next2.transform.GetChild(1).gameObject;//"ArrowPileUp"オブジェクト

            m_Prev = transform.GetChild(1).gameObject; //"PrevWorldInformation"オブジェクト
            m_Prev2 = m_Prev.transform.GetChild(0).gameObject;//"PrevWorldArrowObj"オブジェクト
            m_Prev3 = m_Prev2.transform.GetChild(1).gameObject;//"ArrowPileUp"オブジェクト
            //m_CurrentStage = StageSelect.STAGE.STAGE1;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_InputState_L = INPUTSTATE.NONE;
            m_InputState_R = INPUTSTATE.NONE;
            m_MaxWidth_L = m_Prev3.GetComponent<RectTransform>().sizeDelta.x;
            m_MaxWidth_R = m_Next3.GetComponent<RectTransform>().sizeDelta.x;
            m_MaxHeight_L = m_Prev3.GetComponent<RectTransform>().sizeDelta.y;
            m_MaxHeight_R = m_Next3.GetComponent<RectTransform>().sizeDelta.y;
            ZeroWidth();
            m_SizeDeltaX = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //前のフレームの入力状態の保存
            m_BeforeInput_L = m_CurrentInput_L;
            m_BeforeInput_R = m_CurrentInput_R;

            //入力キーの識別
            CheckKeyInput();
            //入力状態の切り替え
            SwitchingInputState_L();
            SwitchingInputState_R();

            switch (m_InputState_L)
            {
                case INPUTSTATE.NONE:
                    break;
                case INPUTSTATE.START:
                    break;
                case INPUTSTATE.HOLD:
                    m_HoldCount += m_MaxWidth_L*Time.deltaTime / m_HoldTime;
                    m_Prev3.GetComponent<RectTransform>().sizeDelta = new Vector2(m_HoldCount, m_MaxHeight_L);
                    m_SizeDeltaX = m_Prev3.GetComponent<RectTransform>().sizeDelta.x;
                    if (m_Prev3.GetComponent<RectTransform>().sizeDelta.x >= m_MaxWidth_L)
                    {
                        PrevWorldMoveFlagOn();
                    }

                    break;
                case INPUTSTATE.END:
                    ZeroWidth();
                    PrevWorldMoveFlagOff();
                    m_HoldCount = 0.0f;
                    break;
                case INPUTSTATE.ALLSTATE:
                    break;
                default:
                    break;
            }
            switch (m_InputState_R)
            {
                case INPUTSTATE.NONE:
                    break;
                case INPUTSTATE.START:
                    break;
                case INPUTSTATE.HOLD:
                    m_HoldCount += m_MaxWidth_R*Time.deltaTime / m_HoldTime;
                    m_Next3.GetComponent<RectTransform>().sizeDelta = new Vector2(m_HoldCount, m_MaxHeight_R);
                    m_SizeDeltaX = m_Next3.GetComponent<RectTransform>().sizeDelta.x;
                    if (m_Next3.GetComponent<RectTransform>().sizeDelta.x >= m_MaxWidth_R)
                    {
                        NextWorldMoveFlagOn();
                    }
                    break;
                case INPUTSTATE.END:
                    ZeroWidth();
                    NextWorldMoveFlagOff();
                    m_HoldCount = 0.0f;
                    break;
                case INPUTSTATE.ALLSTATE:
                    break;
                default:
                    break;
            }
        }

        public bool NextWorldMoveBeginCheck()
        {
            if (m_NextMove_Flag)
            {
                return true;
            }
            return false;
        }
        public void NextWorldMoveFlagOn()
        {
            m_NextMove_Flag = true;
        }
        public void NextWorldMoveFlagOff()
        {
            m_NextMove_Flag = false;
        }

        public bool PrevWorldMoveBeginCheck()
        {
            if (m_PrevMove_Flag)
            {
                return true;
            }
            return false;
        }
        public void PrevWorldMoveFlagOn()
        {
            m_PrevMove_Flag = true;
        }
        public void PrevWorldMoveFlagOff()
        {
            m_PrevMove_Flag = false;
        }



        //m_CurrentInputをONにする
        public void CurrentInputOn_L()
        {
            m_CurrentInput_L = true;
        }

        //m_CurrentInputをONにするをOFFにする
        public void CurrentInputOff_L()
        {
            m_CurrentInput_L = false;
        }
        //m_CurrentInputをONにする
        public void CurrentInputOn_R()
        {
            m_CurrentInput_R = true;
        }

        //m_CurrentInputをONにするをOFFにする
        public void CurrentInputOff_R()
        {
            m_CurrentInput_R = false;
        }

        //入力キーの識別
        public void CheckKeyInput()
        {
            //上入力
            if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.LeftArrow))
            {
                CurrentInputOn_L();
            }
            //下入力
            else if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.RightArrow))
            {
                CurrentInputOn_R();
            }
            else
            {
                CurrentInputOff_L();
                CurrentInputOff_R();
            }
        }// END

        //入力状態の切り替え
        public void SwitchingInputState_L()
        {
            if (!m_BeforeInput_L && !m_CurrentInput_L)
            {
                m_InputState_L = INPUTSTATE.NONE;
            }
            else if (m_BeforeInput_L && !m_CurrentInput_L)
            {
                Debug.Log("END！");

                m_InputState_L = INPUTSTATE.END;
            }
            else if (!m_BeforeInput_L && m_CurrentInput_L)
            {
                Debug.Log("START！");

                m_InputState_L = INPUTSTATE.START;
            }
            else if (m_BeforeInput_L && m_CurrentInput_L)
            {
                Debug.Log("HOLD！");
                m_InputState_L = INPUTSTATE.HOLD;
            }
        }// END
        //入力状態の切り替え
        public void SwitchingInputState_R()
        {
            if (!m_BeforeInput_R && !m_CurrentInput_R)
            {
                m_InputState_R = INPUTSTATE.NONE;
            }
            else if (m_BeforeInput_R && !m_CurrentInput_R)
            {
                Debug.Log("END！");

                m_InputState_R = INPUTSTATE.END;
            }
            else if (!m_BeforeInput_R && m_CurrentInput_R)
            {
                Debug.Log("START！");

                m_InputState_R = INPUTSTATE.START;
            }
            else if (m_BeforeInput_R && m_CurrentInput_R)
            {
                Debug.Log("HOLD！");
                m_InputState_R = INPUTSTATE.HOLD;
            }
        }// END

        public void ZeroWidth()
        {
            m_Next3.GetComponent<RectTransform>().sizeDelta = new Vector2(0.0f, m_MaxHeight_L);
            m_Prev3.GetComponent<RectTransform>().sizeDelta = new Vector2(0.0f, m_MaxHeight_R);

        }
    }//public class WorldSelectArrow : MonoBehaviour END
}//namespace END