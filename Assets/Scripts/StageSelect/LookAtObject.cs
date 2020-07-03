using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //
    public class LookAtObject : MonoBehaviour
    {
        public Vector3[] m_StageTargetPos = new Vector3[(int)STAGE_NO.STAGE_NUM];
        [Header("前進時の方向転換時間"), Min(0)]
        public float m_Go_SwingTime;//前進時のカメラ方向転換時間

        [Header("後進時の方向転換時間"), Min(0)]
        public float m_Back_SwingTime;//後進時のカメラ方向転換時間

        [Header("ワールド間移動時の方向転換時間"), Min(0)]
        public float m_World_SwingTime;//ワールド間移動時のカメラ方向転換時間

        private float m_MoveTime;//カメラ方向転換時間

        public enum TARGET_MOVESTATE
        {
            FIXING = 0, //カメラ固定
            NEXTSTAGE,  //次のステージへ
            PREVSTAGE,  //前のステージへ
            NEXTWORLD,  //次のワールドへ
            PREVWORLD,  //前のワールドへ
            ALL_STATE   //全状態数
        }
        public TARGET_MOVESTATE m_TargetMoveState;
        private float m_PosRatio;

        //private void Awake()
        //{
        //    //Debug.Log(":");
        //    for (int i = 0; i < (int)STAGE_NO.STAGE_NUM; i++)
        //    {
        //        //ステージごとの注視点座標を格納する
        //        m_StageTargetPos[i] = TargetStages.m_Stages[i].transform.position;
        //    }
        //    m_TargetMoveState = TARGET_MOVESTATE.FIXING;
        //    StartPosition();
        //}

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < (int)STAGE_NO.STAGE_NUM; i++)
            {
                //ステージごとの注視点座標を格納する
                m_StageTargetPos[i] = TargetStages.m_Stages[i].transform.position;
            }
            m_TargetMoveState = TARGET_MOVESTATE.FIXING;
            StartPosition();
        }


        // Update is called once per frame
        void Update()
        {
            Vector3 StartPosition = m_StageTargetPos[(int)StageStatusManager.Instance.CurrentStage];
            Vector3 EndPosition = StartPosition; ;

            switch (m_TargetMoveState)
            {
                case TARGET_MOVESTATE.FIXING:
                    EndPosition = StartPosition;
                    m_MoveTime = -1.0f;
                    break;
                case TARGET_MOVESTATE.NEXTSTAGE:
                    EndPosition = m_StageTargetPos[(int)StageStatusManager.Instance.NextStage];
                    m_MoveTime = m_Go_SwingTime;
                    break;
                case TARGET_MOVESTATE.PREVSTAGE:
                    EndPosition = m_StageTargetPos[(int)StageStatusManager.Instance.PrevStage];
                    m_MoveTime = m_Back_SwingTime;
                    break;
                case TARGET_MOVESTATE.NEXTWORLD:
                    EndPosition = m_StageTargetPos[(int)StageStatusManager.Instance.NextWorld * 5];
                    m_MoveTime = m_World_SwingTime;
                    break;
                case TARGET_MOVESTATE.PREVWORLD:
                    EndPosition = m_StageTargetPos[(int)StageStatusManager.Instance.PrevWorld * 5];
                    m_MoveTime = m_World_SwingTime;
                    break;
                case TARGET_MOVESTATE.ALL_STATE:
                    break;
                default:
                    break;
            }
            TargetMove(StartPosition, EndPosition, m_MoveTime);
            if (m_PosRatio >= 1.0f)
            {
                this.transform.position = EndPosition;
                m_TargetMoveState = TARGET_MOVESTATE.FIXING;
                PosRatioZeroReset();

                ChangeDollyState();

            }
        }
        public void TargetMove(Vector3 StartPosition, Vector3 EndPosition, float MoveTime)
        {
            if (MoveTime < 0.0f)
            {
                return;
            }
            // 現在の位置(2点間の距離からの割合)
            m_PosRatio += (Time.deltaTime / MoveTime);

            // オブジェクトの移動
            this.transform.position = Vector3.Lerp(StartPosition, EndPosition, m_PosRatio);
        }

        public void PosRatioZeroReset()
        {
            m_PosRatio = 0.0f;
        }

        public void ChangeState()
        {
            switch (StageChangeManager.GetStageChangeKey())
            {
                case StageChangeManager.STAGE_CHANGE_KEY.UP:
                    m_TargetMoveState = TARGET_MOVESTATE.NEXTSTAGE;
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.DOWN:
                    m_TargetMoveState = TARGET_MOVESTATE.PREVSTAGE;
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    m_TargetMoveState = TARGET_MOVESTATE.PREVWORLD;
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    m_TargetMoveState = TARGET_MOVESTATE.NEXTWORLD;
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                    break;
                default:
                    break;
            }

        }
        public void ChangeDollyState()
        {
            switch (StageChangeManager.GetStageChangeKey())
            {
                case StageChangeManager.STAGE_CHANGE_KEY.UP:
                    StageChangeManager.DollyStateChange("GO");
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.DOWN:
                    StageChangeManager.DollyStateChange("BACK");
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    StageChangeManager.DollyStateChange("WORLD");
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    StageChangeManager.DollyStateChange("WORLD");
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                    break;
                default:
                    break;
            }

        }

        public void StartPosition()
        {
            int CurrentStageNo = (int)StageStatusManager.Instance.CurrentStage;
            this.transform.position = m_StageTargetPos[CurrentStageNo];
        }
    }//public class LookAtObject : MonoBehaviour END
}//namespace END