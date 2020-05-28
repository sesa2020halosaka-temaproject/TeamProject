using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //クリア状況UI内の現在のステージの背景用の処理クラス
    public class UIBackGroundCurrentStage : MonoBehaviour
    {
        [Header("背景の移動する時間")]
        public float m_BGMoveUp_Time;
        public float m_BGMoveDown_Time;
        [Header("点滅する時間")]
        public float m_FlashTime;

        [Header("α値の最大値"),Range(1,255)]
        public float m_MaxAlpha;
        [Header("α値の最小値"),Range(0,254)]
        public float m_MinAlpha;

        [Header("ステージ名UIの座標保持用")]
        //ステージ名の座標保持用
        public float m_StagePositionX;
        public float m_StagePositionZ;
        //ステージ名のY座標保持用
        public float[] m_StagePositionY = new float[(int)IN_WORLD_NO.ALLSTAGE];
        [Header("移動開始位置")]
        public Vector3 m_StartPosition;
        [Header("移動終了位置")]
        public Vector3 m_EndPosition;
        //ステージセレクトUIの状態
        public enum UI_MOVESTATE
        {
            FIXING = 0,//待ち
            GO,//次のステージ
            BACK,//前のステージ
            STATE_NUM    //状態の数
        }
        public UI_MOVESTATE m_MoveState;
        private UIMoveManager m_UIMoveManager;

        private StageSelectUIManager m_StageSelectUIManager;

        private void Awake()
        {
            m_UIMoveManager = new UIMoveManager();
            m_StageSelectUIManager = this.transform.root.GetComponent<StageSelectUIManager>();
        }
        // Start is called before the first frame update
        void Start()
        {
            SetStartPosition();
            m_BGMoveUp_Time = m_StageSelectUIManager.m_UIMoveUP_Time;
            m_BGMoveDown_Time = m_StageSelectUIManager.m_UIMoveDown_Time;
        }

        // Update is called once per frame
        void Update()
        {
        }//void Update()    END
         //
        public void UIBackGroundCurrentStageUpdate()
        {
            switch (m_MoveState)
            {
                case UI_MOVESTATE.FIXING:
                    break;
                case UI_MOVESTATE.GO:
                    Debug.Log("背景GO未完了");

                    m_UIMoveManager.UIMove(this.gameObject, m_StartPosition, m_EndPosition, m_BGMoveUp_Time);
                    if (this.transform.localPosition.y >= m_EndPosition.y)
                    {
                        this.transform.localPosition = m_EndPosition;
                        Debug.Log("背景GO完了");
                        UIMoveStateFixing();
                        m_UIMoveManager.PosRatioZeroReset();
                    }
                    break;
                case UI_MOVESTATE.BACK:
                    Debug.Log("背景BACK未完了");
                    m_UIMoveManager.UIMove(this.gameObject, m_StartPosition, m_EndPosition, m_BGMoveDown_Time);
                    if (this.transform.localPosition.y <= m_EndPosition.y)
                    {
                        this.transform.localPosition = m_EndPosition;
                        Debug.Log("背景BACK完了");
                        UIMoveStateFixing();
                        m_UIMoveManager.PosRatioZeroReset();
                    }
                    break;
                case UI_MOVESTATE.STATE_NUM:
                    break;
                default:
                    break;
            }

        }

        //現在の座標のセット
        public void SetStartPosition()
        {
            int StageInWorld = StageStatusManager.Instance.StageInWorld;

            this.transform.localPosition = new Vector3(m_StagePositionX, m_StagePositionY[StageInWorld], m_StagePositionZ);
        }

        //UI_MOVESTATEを変更する
        public void UIMoveStateGo()
        {
            int StageInWorld = StageStatusManager.Instance.StageInWorld;
            int NextStageInWorld = StageInWorld + 1;
            //Debug.Log("S：" + StageInWorld + "E：" + NextStageInWorld);

            m_StartPosition = new Vector3(m_StagePositionX, m_StagePositionY[StageInWorld], m_StagePositionZ);
            m_EndPosition = new Vector3(m_StagePositionX, m_StagePositionY[NextStageInWorld], m_StagePositionZ);

            m_MoveState = UI_MOVESTATE.GO;
        }
        public void UIMoveStateBack()
        {
            int StageInWorld = StageStatusManager.Instance.StageInWorld;
            int PrevStageInWorld = StageInWorld - 1;
            //Debug.Log("S：" + StageInWorld + "E：" + PrevStageInWorld);
            m_StartPosition = new Vector3(m_StagePositionX, m_StagePositionY[StageInWorld], m_StagePositionZ);
            m_EndPosition = new Vector3(m_StagePositionX, m_StagePositionY[PrevStageInWorld], m_StagePositionZ);
            m_MoveState = UI_MOVESTATE.BACK;
        }
        public void UIMoveStateChange()
        {
            if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.UP)
            {
                UIMoveStateGo();
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            {
                UIMoveStateBack();
            }
        }


        public void UIMoveStateFixing()
        {
            m_MoveState = UI_MOVESTATE.FIXING;
        }


    } //public class UIBackGroundCurrentStage : MonoBehaviour    END
} //namespace TeamProject    END
