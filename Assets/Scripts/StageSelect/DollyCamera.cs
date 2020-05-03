using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TeamProject
{

    public class DollyCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        //[SerializeField] private float cycleTime = 10.0f;

        private CinemachineTrackedDolly dolly;
        public float pathPositionMax;
        public float pathPositionMin;
        public float AddTime;//移動速度の方向
        public float MoveRatio;//移動速度の倍率

        //カメラのパスの位置の移動
        public enum DOLLY_MOVE
        {
            FIXING,//固定
            GO,  //進める
            BACK,   //戻る
            ALL_STATES//全要素数

        }
        public DOLLY_MOVE m_DollyMove;

        public bool Move_flag;//カメラが移動しているかどうか
        public CinemachinePathBase m_Dolly_GO;
        public CinemachinePathBase m_Dolly_BACK;
        public WayPoint_Box m_WP;//ドリールートのパス位置格納用

        //public FixedDollyCamera //_SubDolly;
        private void Awake()
        {
            // バーチャルカメラがセットされていなければ中止
            if (this.virtualCamera == null)
            {
                this.enabled = false;
                Debug.Log("バーチャルカメラが" + this.virtualCamera);
                return;
            }

            // ドリーコンポーネントを取得できなければ中止
            this.dolly = this.virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            if (this.dolly == null)
            {
                Debug.Log("Dollyコンポーネントが" + this.dolly);

                this.enabled = false;
                return;

            }
        }
        private void Start()
        {
            m_WP = GameObject.Find("WayPoint_Box").GetComponent<WayPoint_Box>();
            //_SubDolly = GameObject.Find("Current_VCamera").GetComponent<FixedDollyCamera>();
            Debug.Log(dolly.name + "：m_Path：" + dolly.m_Path);

            if (MoveRatio <= 0)
            {
                MoveRatio = 1.0f;
            }
            // Positionの単位をトラック上のウェイポイント番号基準にするよう設定
            this.dolly.m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;


            this.dolly.m_Path = m_Dolly_GO;//前進用ドリーパスをセット

            // ウェイポイントの最大番号・最小番号を取得
            this.pathPositionMax = this.dolly.m_Path.MaxPos;
            this.pathPositionMin = this.dolly.m_Path.MinPos;

            //this.DollyState("ZERO");

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

        public void DollyUpdate()
        {
            switch (StageChangeManager.DollyState())
            {
                case DOLLY_MOVE.FIXING:
                    break;
                case DOLLY_MOVE.GO:
                    this.dolly.m_PathPosition += AddTime * Time.deltaTime * MoveRatio;
                    if (this.dolly.m_PathPosition >= this.pathPositionMax)
                    {
                        this.dolly.m_PathPosition = this.pathPositionMax;
                        //移動完了
                        //this.DollyState("ZERO");
                        Debug.Log("this.name:"+ this.name);
                        if (this.name == "Next_VCamera")
                        {
                            Debug.Log("MAIN");
                            StageChangeManager.DollyFlagON("MAIN");
                        }
                        else if (this.name == "Current_VCamera")
                        {
                            Debug.Log("SUB");

                            StageChangeManager.DollyFlagON("SUB");
                        }

                        //_SubDolly.SetCinemachineTrackedDolly(dolly);
                        ////_SubDolly.LookAtTargetChange(virtualCamera.LookAt.gameObject);
                        ////_SubDolly.SetPosition(this.gameObject);
                    }

                    break;
                case DOLLY_MOVE.BACK:
                    this.dolly.m_PathPosition += AddTime * Time.deltaTime * MoveRatio;
                    if (this.dolly.m_PathPosition <= this.pathPositionMin)
                    {
                        this.dolly.m_PathPosition = this.pathPositionMin;
                        //移動完了
                        //this.DollyState("ZERO");
                        if (this.name == "Next_VCamera")
                        {
                            StageChangeManager.DollyFlagON("MAIN");
                        }
                        else if (this.name == "Current_VCamera")
                        {
                            StageChangeManager.DollyFlagON("SUB");
                        }

                        //_SubDolly.SetCinemachineTrackedDolly(dolly);
                        ////_SubDolly.LookAtTargetChange(virtualCamera.LookAt.gameObject);
                        ////_SubDolly.SetPosition(this.gameObject);
                    }

                    break;
                case DOLLY_MOVE.ALL_STATES:
                    break;
                default:
                    break;
            }

        }
        //private void Update()
        //{
        //    //// cycleTime秒かけてトラック上を往復させる
        //    //var t = 0.5f - (0.5f * Mathf.Cos((Time.time * 2.0f * Mathf.PI) / this.cycleTime));
        //    this.dolly.m_PathPosition += AddTime * Time.deltaTime * MoveRatio;
        //    switch (m_DollyMove)
        //    {
        //        case DOLLY_MOVE.FIXING:
        //            break;
        //        case DOLLY_MOVE.GO:
        //            if (this.dolly.m_PathPosition >= this.pathPositionMax)
        //            {
        //                this.dolly.m_PathPosition = this.pathPositionMax;
        //                //移動完了
        //                this.DollyState("ZERO");
        //            }

        //            break;
        //        case DOLLY_MOVE.BACK:
        //            if (this.dolly.m_PathPosition <= this.pathPositionMin)
        //            {
        //                this.dolly.m_PathPosition = this.pathPositionMin;
        //                //移動完了
        //                this.DollyState("ZERO");
        //            }

        //            break;
        //        default:
        //            break;
        //    }
        //}
        //ドリーの状態を変える
        public void DollyState(string word)
        {
            switch (word)
            {
                case "ZERO":
                    AddTime = 0.0f;
                    m_DollyMove = DOLLY_MOVE.FIXING;
                    Move_flag = false;//移動完了

                    break;
                case "GO":
                    AddTime = 1.0f;
                    m_DollyMove = DOLLY_MOVE.GO;
                    Move_flag = true;//移動開始
                    this.dolly.m_Path = m_Dolly_GO;//前進用ドリーパスをセット
                    break;
                case "BACK":
                    AddTime = -1.0f;
                    m_DollyMove = DOLLY_MOVE.BACK;
                    Move_flag = true;//移動開始
                    this.dolly.m_Path = m_Dolly_BACK;//後進用ドリーパスをセット

                    break;
                default:
                    Debug.Log("言葉が違います。カメラを固定します。");
                    AddTime = 0.0f;
                    break;
            }
        }//    public void DollyState(string word)  END

        //移動中かどうかboolで返す
        public bool IsMoving()
        {
            return Move_flag;
        }

        public void LookAtTargetChange(GameObject NextTarget)
        {
            virtualCamera.LookAt = NextTarget.transform;
            //_SubDolly.LookAtTargetChange(NextTarget);

        }

        //カメラの位置を設定する
        public void SetPathPosition(float pos)
        {
            //Debug.Log(pos);
            dolly.m_PathPosition = pos;
            //Debug.Log(this.dolly.m_PathPosition);
        }

        //ドリーカメラのセット（カメラ注視点、パス位置、ドリーの状態）
        public void SetDollyCamera(/*GameObject _DollyCameraObject,*/ STAGE_NO _StageNo, string _Word)
        {
            //LookAtTargetChange(TargetStages.m_Stages[(int)_StageNo]);
            SetPathPositionMax(m_WP.Stage_WayPoint[(int)_StageNo]);
            SetPathPositionMin(m_WP.Stage_WayPoint[(int)_StageNo]);
            DollyState(_Word);
        }
        //private void DollyCameraGo(STAGE_NO NextStage)
        //{

        //    SetDollyCamera(_Dolly_Current, NextStage, "GO");
        //    SetDollyCamera(_Dolly_Next, NextStage, "GO");

        //    select_state = SELECT_STATE.MOVING;

        //}
        //private void DollyCameraBack(STAGE_NO PrevStage)
        //{

        //    SetDollyCamera(_Dolly_Current, PrevStage, "BACK");
        //    SetDollyCamera(_Dolly_Next, PrevStage, "BACK");

        //    select_state = SELECT_STATE.MOVING;

        //}




    }//public class DollyCamera : MonoBehaviour END
}//namespace END