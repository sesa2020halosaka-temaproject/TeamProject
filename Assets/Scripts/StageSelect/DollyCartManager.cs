﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TeamProject
{
    //DollyCartの制御を行う
    public class DollyCartManager : MonoBehaviour
    {
        public WayPoint_Box m_WP;//ドリールートのパス位置格納用
        public DollyTrack_Box m_DoTr;//ドリールートの格納用
        public GameObject _DollyCartObj;//ドリーカート用ゲームオブジェクト
        public GameObject _TargetObj;//ドリーカートを先導するゲームオブジェクト
        public CinemachineDollyCart _DollyCart;


        public DollyCamera.DOLLY_MOVE m_DollyMove;

        public float pathPositionMax;
        public float pathPositionMin;
        public float AddTime;//移動速度の方向
        public float World_MoveRatio;//ワールド間の移動速度の倍率

        // Start is called before the first frame update
        void Start()
        {
            _DollyCartObj = GameObject.Find("DollyCart").gameObject;
            //ドリーカートを先導するゲームオブジェクト
            _TargetObj = _DollyCartObj.transform.GetChild(0).gameObject;
            //CinemachineDollyCartコンポーネント
            _DollyCart = _DollyCartObj.GetComponent<CinemachineDollyCart>();

            //
            m_DoTr = GameObject.Find("DollyTrack_Obj").GetComponent<DollyTrack_Box>();
            //
            m_WP = GameObject.Find("WayPoint_Box").GetComponent<WayPoint_Box>();


            // Positionの単位をトラック上のウェイポイント番号基準にするよう設定
            this._DollyCart.m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;

            AddTime = 1.0f;//移動速度の方向
                           //World_MoveRatio=1.0f;//ワールド間の移動速度の倍率

            m_DollyMove = DollyCamera.DOLLY_MOVE.FIXING;
        }

        // Update is called once per frame
        void Update()
        {
            m_DollyMove = StageChangeManager.DollyState();
            switch (m_DollyMove)
            {
                case DollyCamera.DOLLY_MOVE.FIXING:
                    break;
                case DollyCamera.DOLLY_MOVE.GO:
                    break;
                case DollyCamera.DOLLY_MOVE.BACK:
                    break;
                case DollyCamera.DOLLY_MOVE.WORLD:
                    //位置の更新
                    this._DollyCart.m_Position += AddTime * Time.deltaTime * World_MoveRatio;

                    if (this._DollyCart.m_Position > this._DollyCart.m_Path.MaxPos)
                    {
                        this._DollyCart.m_Position = this._DollyCart.m_Path.MaxPos;

                        Debug.Log("ドリーカート移動完了");
                        //移動完了
                        StageChangeManager.DollyCartFlagON();

                    }


                    break;
                case DollyCamera.DOLLY_MOVE.ALL_STATES:
                    break;
                default:
                    break;
            }
        }// Update()    END

        //PathPositionの両端(Min,Max)をセット
        public void SetPathPositionALL()
        {
            m_WP.SetWayPoint();
            switch (StageChangeManager.GetStageChangeKey())
            {
                case StageChangeManager.STAGE_CHANGE_KEY.UP:
                    SetPathPositionMax(m_WP.m_Stage_WayPoint[StageStatusManager.Instance.StageInWorld + 1]);
                    SetPathPositionMin(m_WP.m_Stage_WayPoint[StageStatusManager.Instance.StageInWorld]);
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.DOWN:
                    SetPathPositionMax(m_WP.m_Stage_WayPoint[StageStatusManager.Instance.StageInWorld]);
                    SetPathPositionMin(m_WP.m_Stage_WayPoint[StageStatusManager.Instance.StageInWorld - 1]);
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    SetPathPositionMax(this._DollyCart.m_Path.MaxPos);
                    SetPathPositionMin(m_WP.m_Stage_WayPoint[StageStatusManager.Instance.StageInWorld]);
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    SetPathPositionMax(this._DollyCart.m_Path.MaxPos);
                    SetPathPositionMin(m_WP.m_Stage_WayPoint[StageStatusManager.Instance.StageInWorld]);
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                    break;
                default:
                    break;
            }

        }
        // ウェイポイントの最大番号をセット
        public void SetPathPositionMax(float maxpos)
        {
            this.pathPositionMax = maxpos;
        }
        // ウェイポイントの最小番号をセット
        public void SetPathPositionMin(float minpos)
        {
            this.pathPositionMin = minpos;
        }

        //カメラのパス位置を初期化する
        public void PathPositionReset()
        {
            m_WP.SetWayPoint();
            //Debug.Log(pos);
            if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.UP)
            {
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            {
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.LEFT)
            {
                _DollyCart.m_Position = m_WP.m_Stage_WayPoint[(int)StageStatusManager.Instance.StageInWorld];
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.RIGHT)
            {
                _DollyCart.m_Position = m_WP.m_Stage_WayPoint[(int)StageStatusManager.Instance.StageInWorld];
            }

            //Debug.Log(this.dolly.m_PathPosition);
        }

        //ドリールートのセット
        public void SetDollyPath()
        {
            switch (StageChangeManager.GetStageChangeKey())
            {
                case StageChangeManager.STAGE_CHANGE_KEY.UP:
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.DOWN:
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    this._DollyCart.m_Path = m_DoTr.m_Dolly_PrevWorld[(int)StageStatusManager.Instance.CurrentStage];//前ワールド移動用ドリーパスをセット
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    this._DollyCart.m_Path = m_DoTr.m_Dolly_NextWorld[(int)StageStatusManager.Instance.CurrentStage];//次ワールド移動用ドリーパスをセット
                    //this._DollyCart.m_Path = m_DoTr.m_Dolly_W1toW2[StageStatusManager.Instance.StageInWorld];//用ドリーパスをセット
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                    break;
                default:
                    break;
            }

        }


        //固定用ドリールートをセットする
        public void SetPathFixingDolly()
        {
            this._DollyCart.m_Path = m_DoTr.m_Dolly_FIXING;
            //パス位置を0にする
            this._DollyCart.m_Position = 0;
        }

        //現在の座標を渡す
        public Vector3 GetDollyCartPosition()
        {
            return this.transform.position;
        }

    }//public class DollyCartManager : MonoBehaviour END
}//namespace END