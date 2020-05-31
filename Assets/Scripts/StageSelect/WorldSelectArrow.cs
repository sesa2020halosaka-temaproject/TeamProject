using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    //ワールド選択可能表示矢印用クラス
    public class WorldSelectArrow : MonoBehaviour
    {
        [Header("UIの移動する時間")]
        public float m_MoveOutTime;
        public float m_MoveInTime;

        [Header("UIの画面内位置")]
        public Vector3 m_InPosition_Next;
        public Vector3 m_InPosition_Prev;
        [Header("UIの画面外位置")]
        public Vector3 m_OutPosition_Next;
        public Vector3 m_OutPosition_Prev;

        public float m_InPos_Next_X;
        public float m_OutPos_Next_X;
        public float m_InPos_Prev_X;
        public float m_OutPos_Prev_X;

        public bool m_EndFlag_Next;
        public bool m_EndFlag_Prev;

        //現在ワールド位置 
        //StageStatusManager.Instance.CurrentWorld;
        public static GameObject m_Canvas;//一番上の親オブジェクト
        public static GameObject m_Next;//右矢印用オブジェクト
        public static GameObject m_Prev;//左矢印用オブジェクト

        public static bool m_AdvancedWorld4_Flag = false;//ワールド４に進出したかどうかフラグ

        //スプライトのパス（固定部分）
        public const string m_ConstPath = "Sprites/StageSelect/UI_WorldName/UI_StageSelect_";

        //スプライトのパス
        public static string[] m_UI_WorldName = {
            "World01","World02","World03","World04"
        };
        public static string m_NextName;                    //次ワールド用パス
        public static string m_PrevName;                    //前ワールド用パス

        private UIMoveManager m_UIMoveManager_Next;         //次ワールドUI移動用の変数
        private UIMoveManager m_UIMoveManager_Prev;         //前ワールドUI移動用の変数
        public UIMoveManager.UI_MOVESTATE m_UIMoveState;    //UI移動の状態保持用
        private StageSelectUIManager m_StageSelectUIManager;//StageSelectUIManagerスクリプト用オブジェクト
        private StageSelect m_StageSelect;                  //StageSelectスクリプト用オブジェクト
        public float m_count;
        public float m_count2;
        private void Awake()
        {
            m_Canvas = transform.root.gameObject;//一番上の親を取得
            m_Next = transform.GetChild(0).gameObject;//右矢印用オブジェクト
            m_Prev = transform.GetChild(1).gameObject;//左矢印用オブジェクト
                                                      //m_CurrentStage = StageSelect.STAGE.STAGE1;
            m_UIMoveManager_Next = new UIMoveManager();
            m_UIMoveManager_Prev = new UIMoveManager();
            m_StageSelectUIManager = m_Canvas.GetComponent<StageSelectUIManager>();
            m_StageSelect = m_Canvas.GetComponent<StageSelect>();
        }
        // Start is called before the first frame update
        void Start()
        {
            ChangeWorldNameIcon();

            m_InPosition_Next  = new Vector3(m_InPos_Next_X, m_Next.transform.localPosition.y, m_Next.transform.localPosition.z);
            m_OutPosition_Next = new Vector3(m_OutPos_Next_X, m_Next.transform.localPosition.y, m_Next.transform.localPosition.z);
            m_InPosition_Prev  = new Vector3(m_InPos_Prev_X, m_Prev.transform.localPosition.y, m_Prev.transform.localPosition.z);
            m_OutPosition_Prev = new Vector3(m_OutPos_Prev_X, m_Prev.transform.localPosition.y, m_Prev.transform.localPosition.z);

            m_MoveOutTime      = m_StageSelectUIManager.m_UIMoveOut_Time;
            m_MoveInTime       = m_StageSelectUIManager.m_UIMoveIn_Time;

            EndFlagOff();
        }

        // Update is called once per frame
        //void Update()
        //{

        //}//void Update()    END
        //====================
        //更新処理
        public void WorldSelectArrowUpdate()
        {
           // Debug.Log("完了");

            m_count = m_UIMoveManager_Next.m_PosRatio;
            m_count2 = m_UIMoveManager_Prev.m_PosRatio;
            switch (m_UIMoveState)
            {
                case UIMoveManager.UI_MOVESTATE.FIXING:
                    break;
                case UIMoveManager.UI_MOVESTATE.MOVEIN:
                    if (m_StageSelect.KeyWaitFlagCheck())
                    {

                        m_UIMoveManager_Next.UIMove(m_Next, m_OutPosition_Next, m_InPosition_Next, m_MoveInTime);
                        if (m_Next.transform.localPosition.x <= m_InPosition_Next.x)
                        {
                            m_Next.transform.localPosition = m_InPosition_Next;
                            EndFlagOn_Next();

                        }
                        m_UIMoveManager_Prev.UIMove(m_Prev, m_OutPosition_Prev, m_InPosition_Prev, m_MoveInTime);
                        if (m_Prev.transform.localPosition.x >= m_InPosition_Prev.x)
                        {
                            m_Prev.transform.localPosition = m_InPosition_Prev;
                            EndFlagOn_Prev();
                        }
                    }
                    if (FlagCheck())
                    {
                        m_UIMoveManager_Next.PosRatioZeroReset();
                        m_UIMoveManager_Prev.PosRatioZeroReset();
                        UIStateFixing();
                        EndFlagOff();
                    }
                    break;
                case UIMoveManager.UI_MOVESTATE.MOVEOUT:

                    m_UIMoveManager_Next.UIMove(m_Next, m_InPosition_Next, m_OutPosition_Next, m_MoveOutTime);
                    if (m_Next.transform.localPosition.x >= m_OutPosition_Next.x)
                    {
                        m_Next.transform.localPosition = m_OutPosition_Next;
                        EndFlagOn_Next();
                    }

                    m_UIMoveManager_Prev.UIMove(m_Prev, m_InPosition_Prev, m_OutPosition_Prev, m_MoveOutTime);
                    if (m_Prev.transform.localPosition.x <= m_OutPosition_Prev.x)
                    {
                        m_Prev.transform.localPosition = m_OutPosition_Prev;
                        EndFlagOn_Prev();
                    }
                    if (FlagCheck())
                    {
                        m_UIMoveManager_Next.PosRatioZeroReset();
                        m_UIMoveManager_Prev.PosRatioZeroReset();
                        UIStateFixing();
                        EndFlagOff();

                    }

                    break;
                case UIMoveManager.UI_MOVESTATE.ALL_MOVESTATE:
                    break;
                default:
                    break;
            }
        }//WorldSelectArrowUpdate() END

        //====================
        //ワールド状況に応じた処理
        public static void SetCurrentWorld()
        {
            if (AdvancedW4FlagCheck())
            {
                TwoArrowsActivate();
            }
            else
            {
                switch (StageStatusManager.Instance.CurrentWorld)
                {
                    case (int)WORLD_NO.W1://World01
                        TwoSetActives(true, false);

                        break;
                    case (int)WORLD_NO.W2://World02
                        TwoSetActives(true, true);
                        break;
                    case (int)WORLD_NO.W3://World03
                        TwoSetActives(true, true);
                        break;
                    case (int)WORLD_NO.W4://World04
                        TwoSetActives(false, true);
                        break;
                    case (int)WORLD_NO.ALL_WORLD:
                    default:
                        Debug.LogAssertion("WorldSelectArrowが無効な状態！");
                        break;
                }

            }
        }

        //左右の矢印のアクティブを同時に設定
        private static void TwoSetActives(bool Next, bool Prev)
        {
            m_Next.SetActive(Next);
            m_Prev.SetActive(Prev);
        }

        //左右の矢印を非アクティブに設定
        public static void TwoArrowsActivate()
        {
            m_Next.SetActive(true);
            m_Prev.SetActive(true);
        }
        //左右の矢印を非アクティブに設定
        public static void TwoArrowsDeactivate()
        {
            m_Next.SetActive(false);
            m_Prev.SetActive(false);
        }

        //ワールド4進出フラグをONにする
        public static void AdvancedW4FlagON()
        {
            m_AdvancedWorld4_Flag = true;
        }

        //ワールド4進出フラグを確認する
        public static bool AdvancedW4FlagCheck()
        {
            return m_AdvancedWorld4_Flag;
        }

        //次と前のワールドを示すUIの差し替え
        public static void ChangeWorldNameIcon()
        {
            // Debug.Log("NamePlate");
            //スプライトのパスを切り替える
            m_NextName = m_ConstPath + m_UI_WorldName[StageStatusManager.Instance.NextWorld];
            m_PrevName = m_ConstPath + m_UI_WorldName[StageStatusManager.Instance.PrevWorld];

            //次のワールドの表示スプライトを差し替える
            m_Next.transform.GetChild(1).GetComponent<Image>().sprite = null;
            m_Next.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(m_NextName);

            //前のワールドの表示スプライトを差し替える
            m_Prev.transform.GetChild(1).GetComponent<Image>().sprite = null;
            m_Prev.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(m_PrevName);
        }//Sprites/StageSelect/UI_StageNamePlate/UI_World01_Stage01.png

        //フラグのOff
        public void EndFlagOff()
        {
            m_EndFlag_Next = false;
            m_EndFlag_Prev = false;
        }
        //フラグのON
        public void EndFlagOn_Next()
        {
            m_EndFlag_Next = true;
        }
        public void EndFlagOn_Prev()
        {
            m_EndFlag_Prev = true;
        }

        //両フラグを確認する
        public bool FlagCheck()
        {
            if (m_EndFlag_Next && m_EndFlag_Prev)
            {
                return true;
            }
            return false;
        }

        //UI_MOVESTATEを変更する
        public void UIStateMoveOut()
        {
            m_UIMoveState = UIMoveManager.UI_MOVESTATE.MOVEOUT;
        }
        public void UIStateMoveIn()
        {
            m_UIMoveState = UIMoveManager.UI_MOVESTATE.MOVEIN;
        }
        public void UIStateFixing()
        {
            m_UIMoveState = UIMoveManager.UI_MOVESTATE.FIXING;
        }

    }//public class WorldSelectArrow : MonoBehaviour END
}//namespace END