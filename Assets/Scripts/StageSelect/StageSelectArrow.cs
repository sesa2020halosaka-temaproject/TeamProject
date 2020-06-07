using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    //ステージ選択可能表示矢印用
    public class StageSelectArrow : MonoBehaviour
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

        public float m_InPos_Next_Y;
        public float m_OutPos_Next_Y;

        public float m_InPos_Prev_Y;
        public float m_OutPos_Prev_Y;

        private bool m_MoveEndFlag_NextUI;
        private bool m_MoveEndFlag_PrevUI;


        //現在ステージ位置 
        //StageStatusManager.Instance.CurrentStage;
        public static GameObject m_Canvas;//一番上の親オブジェクト
        public static GameObject m_NextUI_Obj;//上矢印用オブジェクト
        public static GameObject m_PrevUI_Obj;//下矢印用オブジェクト

        //スプライトのパス（固定部分）
        public const string m_ConstPath = "Sprites/StageSelect/UI_StageNamePlate/UI_World";

        //スプライトのパス
        public static string[] m_UI_StageName = {
            "01_Stage01","01_Stage02","01_Stage03","01_Stage04","01_Stage05",
            "02_Stage01","02_Stage02","02_Stage03","02_Stage04","02_Stage05",
            "03_Stage01","03_Stage02","03_Stage03","03_Stage04","03_Stage05",
            "04_Stage01","04_Stage02","04_Stage03","04_Stage04","04_Stage05",
        };
        public static string m_NextName;//次ステージ用パス
        public static string m_PrevName;//前ステージ用パス

        private UIMoveManager m_UIMoveManager_Next;         //次ステージUI移動用の変数
        private UIMoveManager m_UIMoveManager_Prev;         //前ステージUI移動用の変数
        public UIMoveManager.UI_MOVESTATE m_UIMoveState;    //UI移動の状態保持用
        private StageSelectUIManager m_StageSelectUIManager;//StageSelectUIManagerスクリプト用オブジェクト
        private StageSelect m_StageSelect;                  //StageSelectスクリプト用オブジェクト


        private void Awake()
        {
            m_Canvas = transform.root.gameObject;//一番上の親を取得
            m_NextUI_Obj = transform.GetChild(0).gameObject;//上矢印用オブジェクト
            m_PrevUI_Obj = transform.GetChild(1).gameObject;//下矢印用オブジェクト

            m_UIMoveManager_Next = new UIMoveManager();
            m_UIMoveManager_Prev = new UIMoveManager();
            m_StageSelectUIManager = m_Canvas.GetComponent<StageSelectUIManager>();
            m_StageSelect = m_Canvas.GetComponent<StageSelect>();

            UIActivateFromStage();
        }
        // Start is called before the first frame update
        void Start()
        {
            //ChangeStageNameIcon();

            m_InPosition_Next = new Vector3(m_NextUI_Obj.transform.localPosition.x, m_InPos_Next_Y, m_NextUI_Obj.transform.localPosition.z);
            m_OutPosition_Next = new Vector3(m_NextUI_Obj.transform.localPosition.x, m_OutPos_Next_Y, m_NextUI_Obj.transform.localPosition.z);

            m_InPosition_Prev = new Vector3(m_PrevUI_Obj.transform.localPosition.x, m_InPos_Prev_Y, m_PrevUI_Obj.transform.localPosition.z);
            m_OutPosition_Prev = new Vector3(m_PrevUI_Obj.transform.localPosition.x, m_OutPos_Prev_Y, m_PrevUI_Obj.transform.localPosition.z);

            m_MoveOutTime = m_StageSelectUIManager.m_UIMoveOut_Time;
            m_MoveInTime = m_StageSelectUIManager.m_UIMoveIn_Time;

            //ステージ移動制限解除されてるかどうか
            if (StageStatusManager.Instance.m_RemovalLimitFlag)
            {
                //解除されている
                //移動フラグtrue
                StageChangeManager.NextStageMoveFlagChange(true);
            }
            else
            {
                //解除されていない
                //移動フラグfalse
                StageChangeManager.NextStageMoveFlagChange(false);
            }


            EndFlagOff();
        }

        // Update is called once per frame
        //void Update()
        //{

        //}//void Update()    END
        //====================
        //更新処理
        public void StageSelectArrowUpdate()
        {
            switch (m_UIMoveState)
            {
                case UIMoveManager.UI_MOVESTATE.FIXING:
                    break;
                case UIMoveManager.UI_MOVESTATE.MOVEIN:
                    Debug.Log("IN中です");
                    if (m_StageSelect.KeyWaitFlagCheck())
                    {

                        m_UIMoveManager_Next.UIMove(m_NextUI_Obj, m_OutPosition_Next, m_InPosition_Next, m_MoveInTime);
                        if (m_NextUI_Obj.transform.localPosition.y <= m_InPosition_Next.y)
                        {
                            m_NextUI_Obj.transform.localPosition = m_InPosition_Next;
                            EndFlagOn_Next();

                        }
                        m_UIMoveManager_Prev.UIMove(m_PrevUI_Obj, m_OutPosition_Prev, m_InPosition_Prev, m_MoveInTime);
                        if (m_PrevUI_Obj.transform.localPosition.y >= m_InPosition_Prev.y)
                        {
                            m_PrevUI_Obj.transform.localPosition = m_InPosition_Prev;
                            EndFlagOn_Prev();
                        }
                    }
                    if (FlagCheck())
                    {
                        Debug.Log("In完了");
                        m_UIMoveManager_Next.PosRatioZeroReset();
                        m_UIMoveManager_Prev.PosRatioZeroReset();
                        UIStateFixing();
                        EndFlagOff();
                    }
                    break;
                case UIMoveManager.UI_MOVESTATE.MOVEOUT:
                    Debug.Log("OUT中です");

                    m_UIMoveManager_Next.UIMove(m_NextUI_Obj, m_InPosition_Next, m_OutPosition_Next, m_MoveOutTime);
                    if (m_NextUI_Obj.transform.localPosition.y >= m_OutPosition_Next.y)
                    {
                        m_NextUI_Obj.transform.localPosition = m_OutPosition_Next;
                        EndFlagOn_Next();
                    }
                    m_UIMoveManager_Prev.UIMove(m_PrevUI_Obj, m_InPosition_Prev, m_OutPosition_Prev, m_MoveOutTime);
                    if (m_PrevUI_Obj.transform.localPosition.y <= m_OutPosition_Prev.y)
                    {
                        m_PrevUI_Obj.transform.localPosition = m_OutPosition_Prev;
                        EndFlagOn_Prev();
                    }
                    if (FlagCheck())
                    {
                        Debug.Log("OUT完了");
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

        }//StageSelectUpdate() END

        //ステージの状況に応じた処理
        public void UIActivateFromStage()
        {
            //ステージ移動制限解除されてるかどうか
            if (StageStatusManager.Instance.m_RemovalLimitFlag)
            {
                //解除されている
                //現在のステージ位置に応じた処理
                UIActivateFromCurrentStage();
            }
            else
            {
                //解除されていない
                //ステージクリア状況に応じた処理
                UIActivateFromClearStatus();
            }
        }

        //ステージクリア状況に応じた処理
        public void UIActivateFromClearStatus()
        {
            CLEAR_STATUS StageClearStatus;
            int CurrentStageNo;
            CurrentStageNo = (int)StageStatusManager.Instance.CurrentStage;
            StageClearStatus = StageStatusManager.Instance.Stage_Status[CurrentStageNo];
            switch (StageClearStatus)
            {
                case CLEAR_STATUS.NOT:
                case CLEAR_STATUS.ONE:
                    StageChangeManager.NextStageMoveFlagChange(false);
                    m_NextUI_Obj.SetActive(false);
                    break;
                case CLEAR_STATUS.TWO:
                case CLEAR_STATUS.THREE:
                    StageChangeManager.NextStageMoveFlagChange(true);
                    m_NextUI_Obj.SetActive(true);
                    break;
                case CLEAR_STATUS.STATUS_NUM:
                    break;
                default:
                    Debug.LogAssertion("StageSelectArrowが無効な状態！");
                    break;
            }
            //ステージ位置における制限部分
            switch (StageStatusManager.Instance.StageInWorld)
            {
                case (int)IN_WORLD_NO.S1://Stage01
                    m_PrevUI_Obj.SetActive(false);

                    break;
                case (int)IN_WORLD_NO.S2:
                case (int)IN_WORLD_NO.S3:
                case (int)IN_WORLD_NO.S4:
                    //Stage02～04
                    m_PrevUI_Obj.SetActive(true);

                    break;
                case (int)IN_WORLD_NO.S5://Stage05
                    m_PrevUI_Obj.SetActive(true);
                    m_NextUI_Obj.SetActive(false);

                    break;
                case (int)IN_WORLD_NO.ALLSTAGE:
                default:
                    Debug.LogAssertion("StageSelectArrowが無効な状態！");
                    break;
            }

        }

        //現在のステージ位置に応じた処理
        public void UIActivateFromCurrentStage()
        {
            switch (StageStatusManager.Instance.StageInWorld)
            {
                case (int)IN_WORLD_NO.S1://Stage01
                    TwoSetActives(false, true);

                    break;
                case (int)IN_WORLD_NO.S2:
                case (int)IN_WORLD_NO.S3:
                case (int)IN_WORLD_NO.S4:
                    //Stage02～04
                    TwoSetActives(true, true);

                    break;
                case (int)IN_WORLD_NO.S5://Stage05
                    TwoSetActives(true, false);

                    break;
                case (int)IN_WORLD_NO.ALLSTAGE:
                default:
                    Debug.LogAssertion("StageSelectArrowが無効な状態！");
                    break;
            }
        }


        //上下の矢印のアクティブを同時に設定
        private static void TwoSetActives(bool Prev, bool Next)
        {
            m_NextUI_Obj.SetActive(Next);
            m_PrevUI_Obj.SetActive(Prev);
        }

        //上下の矢印を非アクティブに設定
        public static void TwoArrowsDeactivate()
        {
            m_NextUI_Obj.SetActive(false);
            m_PrevUI_Obj.SetActive(false);
        }

        //前後どちらの移動かによる矢印のアクティブと強調化の設定
        public void UISetFlashing()
        {
            if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.UP)
            {
                NextUIFlashing();
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            {
                PrevUIFlashing();
            }
        }
        //次のステージ矢印を強調版に変更する
        public void NextUIFlashing()
        {
            SwitchingActive.GameObject_ON(m_NextUI_Obj);
            m_PrevUI_Obj.SetActive(false);
        }
        //前のステージ矢印を強調版に変更する
        public void PrevUIFlashing()
        {
            SwitchingActive.GameObject_ON(m_PrevUI_Obj);
            m_NextUI_Obj.SetActive(false);
        }
        //両UIを非強調版に戻しつつアクティブ化させる
        public void UINoFlashing()
        {
            SwitchingActive.GameObject_OFF(m_NextUI_Obj);
            SwitchingActive.GameObject_OFF(m_PrevUI_Obj);
        }


        //次と前のステージを示すUIの差し替え
        public static void ChangeStageNameIcon()
        {
            // Debug.Log("NamePlate");
            //スプライトのパスを切り替える
            m_NextName = m_ConstPath + m_UI_StageName[(int)StageStatusManager.Instance.NextStage];
            m_PrevName = m_ConstPath + m_UI_StageName[(int)StageStatusManager.Instance.PrevStage];

            //次のステージの表示スプライトを差し替える
            m_NextUI_Obj.transform.GetChild(1).GetComponent<Image>().sprite = null;
            m_NextUI_Obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(m_NextName);

            //前のステージの表示スプライトを差し替える
            m_PrevUI_Obj.transform.GetChild(1).GetComponent<Image>().sprite = null;
            m_PrevUI_Obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(m_PrevName);
        }//

        //両移動フラグの同時Off
        public void EndFlagOff()
        {
            m_MoveEndFlag_NextUI = false;
            m_MoveEndFlag_PrevUI = false;
        }
        //次ステージ用移動フラグのON
        public void EndFlagOn_Next()
        {
            m_MoveEndFlag_NextUI = true;
        }
        //前ステージ用移動フラグのON
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

    }//public class StageSelectArrow : MonoBehaviour END
}//namespace END