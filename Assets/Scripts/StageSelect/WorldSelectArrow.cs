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

        public bool m_MoveEndFlag_NextUI;
        public bool m_MoveEndFlag_PrevUI;

        //現在ワールド位置 
        //StageStatusManager.Instance.CurrentWorld;
        public static GameObject m_Canvas;//一番上の親オブジェクト
        public static GameObject m_NextUI_Obj;//右矢印用オブジェクト
        public static GameObject m_PrevUI_Obj
            ;//左矢印用オブジェクト


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
            m_NextUI_Obj = transform.GetChild(0).gameObject;//右矢印用オブジェクト
            m_PrevUI_Obj = transform.GetChild(1).gameObject;//左矢印用オブジェクト
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

            m_InPosition_Next = new Vector3(m_InPos_Next_X, m_NextUI_Obj.transform.localPosition.y, m_NextUI_Obj.transform.localPosition.z);
            m_OutPosition_Next = new Vector3(m_OutPos_Next_X, m_NextUI_Obj.transform.localPosition.y, m_NextUI_Obj.transform.localPosition.z);
            m_InPosition_Prev = new Vector3(m_InPos_Prev_X, m_PrevUI_Obj.transform.localPosition.y, m_PrevUI_Obj.transform.localPosition.z);
            m_OutPosition_Prev = new Vector3(m_OutPos_Prev_X, m_PrevUI_Obj.transform.localPosition.y, m_PrevUI_Obj.transform.localPosition.z);

            m_MoveOutTime = m_StageSelectUIManager.m_UIMoveOut_Time;
            m_MoveInTime = m_StageSelectUIManager.m_UIMoveIn_Time;

            //ワールド移動の設定
            SettingWorldMove();

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
                    //if (m_StageSelect.KeyWaitFlagCheck())
                    {

                        m_UIMoveManager_Next.UIMove(m_NextUI_Obj, m_OutPosition_Next, m_InPosition_Next, m_MoveInTime);
                        if (m_NextUI_Obj.transform.localPosition.x <= m_InPosition_Next.x)
                        {
                            m_NextUI_Obj.transform.localPosition = m_InPosition_Next;
                            EndFlagOn_Next();

                        }
                        m_UIMoveManager_Prev.UIMove(m_PrevUI_Obj, m_OutPosition_Prev, m_InPosition_Prev, m_MoveInTime);
                        if (m_PrevUI_Obj.transform.localPosition.x >= m_InPosition_Prev.x)
                        {
                            m_PrevUI_Obj.transform.localPosition = m_InPosition_Prev;
                            EndFlagOn_Prev();
                        }
                    }
                    if (FlagCheck())
                    {
                        m_UIMoveManager_Next.PosRatioZeroReset();
                        m_UIMoveManager_Prev.PosRatioZeroReset();
                        UIStateFixing();
                        //EndFlagOff();
                    }
                    break;
                case UIMoveManager.UI_MOVESTATE.MOVEOUT:

                    m_UIMoveManager_Next.UIMove(m_NextUI_Obj, m_InPosition_Next, m_OutPosition_Next, m_MoveOutTime);
                    if (m_NextUI_Obj.transform.localPosition.x >= m_OutPosition_Next.x)
                    {
                        m_NextUI_Obj.transform.localPosition = m_OutPosition_Next;
                        EndFlagOn_Next();
                    }

                    m_UIMoveManager_Prev.UIMove(m_PrevUI_Obj, m_InPosition_Prev, m_OutPosition_Prev, m_MoveOutTime);
                    if (m_PrevUI_Obj.transform.localPosition.x <= m_OutPosition_Prev.x)
                    {
                        m_PrevUI_Obj.transform.localPosition = m_OutPosition_Prev;
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
        public void SettingWorldMove()
        {
            //ステージ移動制限解除されてるかどうか
            if (StageStatusManager.Instance.m_RemovalLimitFlag)
            {
                //解除されている
                //移動フラグtrue
                StageChangeManager.NextWorldMoveFlagChange(true);
                StageChangeManager.MoveFromW1ToW4FlagChange(true);
                //左右のUIアクティブ化
                TwoSetActives(true, true);

            }
            else
            {
                //解除されていない
                int CurrentWorld;
                CurrentWorld = StageStatusManager.Instance.CurrentWorld;
                //次のワールドへの移動フラグとUIの表示の切り替え
                SettingNextWorldMove(CurrentWorld * 5 + 4);
                //前のワールドへの移動フラグとUIの表示の切り替え
                SettingPrevWorldMove(CurrentWorld);
            }

        }

        //次のワールドへの移動フラグとUIの表示の切り替え
        public void SettingNextWorldMove(int StageNo)
        {
            CLEAR_STATUS StageClearStatus;
            StageClearStatus = StageStatusManager.Instance.Stage_Status[StageNo];
            switch (StageClearStatus)
            {
                case CLEAR_STATUS.NOT:
                case CLEAR_STATUS.ONE:
                    StageChangeManager.NextWorldMoveFlagChange(false);
                    m_NextUI_Obj.SetActive(false);
                    break;
                case CLEAR_STATUS.TWO:
                case CLEAR_STATUS.THREE:
                    StageChangeManager.NextWorldMoveFlagChange(true);
                    m_NextUI_Obj.SetActive(true);
                    break;
                case CLEAR_STATUS.STATUS_NUM:
                    break;
                default:
                    Debug.LogAssertion("StageSelectArrowが無効な状態！");
                    break;
            }

        }
        //前のワールドへの移動フラグとUIの表示の切り替え
        public void SettingPrevWorldMove(int CurrentWorldNo)
        {
            switch (CurrentWorldNo)
            {
                case (int)WORLD_NO.W1://World01
                    MoveFromW1ToW4FlagChange();
                    break;
                case (int)WORLD_NO.W2://World02
                case (int)WORLD_NO.W3://World03
                case (int)WORLD_NO.W4://World04
                    StageChangeManager.MoveFromW1ToW4FlagChange(true);
                    m_PrevUI_Obj.SetActive(true);
                    break;
                case (int)WORLD_NO.ALL_WORLD:
                default:
                    Debug.LogAssertion("WorldSelectArrowが無効な状態！");
                    break;
            }

        }

        //ワールド１からワールド４への移動許可フラグとUI表示切り替え
        public void MoveFromW1ToW4FlagChange()
        {
            CLEAR_STATUS StageClearStatus;
            StageClearStatus = StageStatusManager.Instance.Stage_Status[(int)STAGE_NO.STAGE20];
            switch (StageClearStatus)
            {
                case CLEAR_STATUS.NOT:
                case CLEAR_STATUS.ONE:
                    StageChangeManager.MoveFromW1ToW4FlagChange(false);
                    m_PrevUI_Obj.SetActive(false);
                    break;
                case CLEAR_STATUS.TWO:
                case CLEAR_STATUS.THREE:
                    StageChangeManager.MoveFromW1ToW4FlagChange(true);
                    m_PrevUI_Obj.SetActive(true);
                    break;
                case CLEAR_STATUS.STATUS_NUM:
                default:
                    Debug.LogAssertion("StageSelectArrowが無効な状態！");
                    break;
            }

        }
        //左右の矢印のアクティブを同時に設定
        private static void TwoSetActives(bool Next, bool Prev)
        {
            m_NextUI_Obj.SetActive(Next);
            m_PrevUI_Obj.SetActive(Prev);
        }

        //左右の矢印を非アクティブに設定
        public static void TwoArrowsActivate()
        {
            m_NextUI_Obj.SetActive(true);
            m_PrevUI_Obj.SetActive(true);
        }
        //左右の矢印を非アクティブに設定
        public static void TwoArrowsDeactivate()
        {
            m_NextUI_Obj.SetActive(false);
            m_PrevUI_Obj.SetActive(false);
        }

        //次と前のワールドを示すUIの差し替え
        public static void ChangeWorldNameIcon()
        {
            // Debug.Log("NamePlate");
            //スプライトのパスを切り替える
            m_NextName = m_ConstPath + m_UI_WorldName[StageStatusManager.Instance.NextWorld];
            m_PrevName = m_ConstPath + m_UI_WorldName[StageStatusManager.Instance.PrevWorld];

            //次のワールドの表示スプライトを差し替える
            m_NextUI_Obj.transform.GetChild(1).GetComponent<Image>().sprite = null;
            m_NextUI_Obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(m_NextName);

            //前のワールドの表示スプライトを差し替える
            m_PrevUI_Obj.transform.GetChild(1).GetComponent<Image>().sprite = null;
            m_PrevUI_Obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(m_PrevName);
        }//Sprites/StageSelect/UI_StageNamePlate/UI_World01_Stage01.png

        //フラグのOff
        public void EndFlagOff()
        {
            m_MoveEndFlag_NextUI = false;
            m_MoveEndFlag_PrevUI = false;
        }
        //フラグのON
        public void EndFlagOn_Next()
        {
            m_MoveEndFlag_NextUI = true;
        }
        public void EndFlagOn_Prev()
        {
            m_MoveEndFlag_PrevUI = true;
        }

        //両フラグを確認する
        public bool FlagCheck()
        {
            if (m_MoveEndFlag_NextUI && m_MoveEndFlag_PrevUI)
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