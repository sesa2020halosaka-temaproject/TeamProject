using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TeamProject
{
    //DollyCameraの制御を行う
    public class DollyCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        //[SerializeField] private float cycleTime = 10.0f;

        public CinemachineTrackedDolly dolly;
        public float pathPositionMax;
        public float pathPositionMin;
        public float AddTime;//移動速度の方向
        public float Stage_MoveRatio;//ステージ間の移動速度の倍率
        public float World_MoveRatio;//ワールド間の移動速度の倍率

        //カメラのパスの位置の移動
        public enum DOLLY_MOVE
        {
            FIXING,//固定
            GO,  //進める
            BACK,   //戻る
            WORLD,  //ワールド間移動
            ALL_STATES//全要素数

        }
        public DOLLY_MOVE m_DollyMove;

        public bool Move_flag;//カメラが移動しているかどうか
        public CinemachinePathBase m_Dolly_GO;
        public CinemachinePathBase m_Dolly_BACK;
        public CinemachinePathBase m_Dolly_FIXING;
        public CinemachinePathBase[] m_Dolly_GO_4;
        public CinemachinePathBase[] m_Dolly_BACK_4;
        public CinemachinePathBase[] m_Dolly_W1toW2;
        public CinemachinePathBase[] m_Dolly_W2toW1;

        public WayPoint_Box m_WP;//ドリールートのパス位置格納用
        public DollyTrack_Box m_DoTr;//ドリールートの格納用



        public GameObject _DollyCartObj;//ドリーカート用ゲームオブジェクト
        public GameObject _TargetObj;//ドリーカートを先導するゲームオブジェクト
        public CinemachineDollyCart _DollyCart;
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
            //ドリーカート用ゲームオブジェクト
            _DollyCartObj = GameObject.Find("DollyCart").gameObject;
            //ドリーカートを先導するゲームオブジェクト
            _TargetObj = _DollyCartObj.transform.GetChild(0).gameObject;
            //CinemachineDollyCartコンポーネント
            _DollyCart = _DollyCartObj.GetComponent<CinemachineDollyCart>();

            //DollyTrack用ゲームオブジェクト取得
            m_DoTr = GameObject.Find("DollyTrack_Obj").GetComponent<DollyTrack_Box>();
            //WayPoint用ゲームオブジェクト取得
            m_WP = GameObject.Find("WayPoint_Box").GetComponent<WayPoint_Box>();

            //_SubDolly = GameObject.Find("Current_VCamera").GetComponent<FixedDollyCamera>();
            //Debug.Log(dolly.name + "：m_Path：" + dolly.m_Path);

            if (Stage_MoveRatio < 0)
            {
                Stage_MoveRatio = 1.0f;
            }
            if (World_MoveRatio < 0)
            {
                World_MoveRatio = 1.0f;
            }
            // Positionの単位をトラック上のウェイポイント番号基準にするよう設定
            this.dolly.m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;


            //this.dolly.m_Path = m_Dolly_GO;//前進用ドリーパスをセット

            // ウェイポイントの最大番号・最小番号を取得
            this.pathPositionMax = this.dolly.m_Path.MaxPos;
            this.pathPositionMin = this.dolly.m_Path.MinPos;


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

        }
        private void Update()
        {
            m_DollyMove = StageChangeManager.DollyState();
            switch (StageChangeManager.DollyState())
            {
                case DOLLY_MOVE.FIXING:
                    break;
                case DOLLY_MOVE.GO:
                    this.dolly.m_PathPosition += AddTime * Time.deltaTime * Stage_MoveRatio;
                    if (this.dolly.m_PathPosition >= this.pathPositionMax)
                    {
                        this.dolly.m_PathPosition = this.pathPositionMax;
                        //移動完了
                        //Debug.Log("this.name:" + this.name);
                        if (this.name == "Next_VCamera")
                        {
                            //Debug.Log("MAIN");
                            StageChangeManager.DollyFlagON("MAIN");

                        }
                        else if (this.name == "Current_VCamera")
                        {
                            //Debug.Log("SUB");

                            StageChangeManager.DollyFlagON("SUB");
                        }
                        //_SubDolly.SetCinemachineTrackedDolly(dolly);
                        ////_SubDolly.LookAtTargetChange(virtualCamera.LookAt.gameObject);
                        ////_SubDolly.SetPosition(this.gameObject);
                    }

                    break;
                case DOLLY_MOVE.BACK:
                    this.dolly.m_PathPosition += AddTime * Time.deltaTime * Stage_MoveRatio;
                    if (this.dolly.m_PathPosition <= this.pathPositionMin)
                    {
                        this.dolly.m_PathPosition = this.pathPositionMin;
                        //移動完了
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
                case DOLLY_MOVE.WORLD:
                    this.dolly.m_PathPosition += AddTime * Time.deltaTime * Stage_MoveRatio;
                    if (this.dolly.m_PathPosition >= this.pathPositionMax)
                    {
                        this.dolly.m_PathPosition = this.pathPositionMax;
                        //移動完了
                        if (this.name == "Next_VCamera")
                        {
                            Debug.Log("MAINのドリーカメラ移動完了");

                            StageChangeManager.DollyFlagON("MAIN");
                        }
                        else if (this.name == "Current_VCamera")
                        {
                            Debug.Log("SUBのドリーカメラ移動完了");

                            StageChangeManager.DollyFlagON("SUB");
                        }
                        //固定用ドリーパスをセット
                        //SetPathFixingDolly();
                    }


                    break;
                case DOLLY_MOVE.ALL_STATES:
                    break;
                default:
                    break;
            }


        }

        ////ドリーの状態を変える
        //public void DollyState(string word)
        //{
        //    switch (word)
        //    {
        //        case "ZERO":
        //            AddTime = 0.0f;
        //            m_DollyMove = DOLLY_MOVE.FIXING;
        //            Move_flag = false;//移動完了

        //            break;
        //        case "GO":
        //            AddTime = 1.0f;
        //            m_DollyMove = DOLLY_MOVE.GO;
        //            Move_flag = true;//移動開始
        //            //ステージ間移動の時
        //            if (StageChangeManager.IsStageChange())
        //            {
        //                //this.dolly.m_Path = m_Dolly_GO;//前進用ドリーパスをセット
        //                this.dolly.m_Path = m_Dolly_GO_4[StageStatusManager.Instance.CurrentWorld];//前進用ドリーパスをセット

        //            }
        //            //ワールド間の移動の時
        //            else if (StageChangeManager.IsWorldChange())
        //            {
        //                //論理がズレてるのでここ修正しないといけない
        //                if (StageStatusManager.Instance.CurrentWorld  == 0)
        //                {

        //                    this.dolly.m_Path = m_Dolly_W1toW2[StageStatusManager.Instance.StageInWorld];//用ドリーパスをセット
        //                        this.dolly.m_PathPosition = 0;
        //                }
        //                else if (StageStatusManager.Instance.CurrentWorld == 1)
        //                {

        //                    this.dolly.m_Path = m_Dolly_W2toW1[StageStatusManager.Instance.StageInWorld];//用ドリーパスをセット
        //                    this.dolly.m_PathPosition = 0;
        //                }

        //            }

        //            break;
        //        case "BACK":
        //            AddTime = -1.0f;
        //            m_DollyMove = DOLLY_MOVE.BACK;
        //            Move_flag = true;//移動開始
        //            //this.dolly.m_Path = m_Dolly_BACK;//後進用ドリーパスをセット
        //            this.dolly.m_Path = m_Dolly_BACK_4[StageStatusManager.Instance.CurrentWorld];//後進用ドリーパスをセット

        //            break;
        //        default:
        //            Debug.Log("言葉が違います。カメラを固定します。");
        //            AddTime = 0.0f;
        //            break;
        //    }
        //}//    public void DollyState(string word)  END

        //移動中かどうかboolで返す
        public bool IsMoving()
        {
            return Move_flag;
        }



        //カメラのパス位置を設定する
        public void SetPathPosition(float pos)
        {
            //Debug.Log(pos);
            dolly.m_PathPosition = pos;
            //Debug.Log(this.dolly.m_PathPosition);
        }

        //ウェイポイントの両端をセット
        public void SetPathPositionALL()
        {
            if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.UP)
            {
                SetPathPositionMax(m_WP.Stage_WayPoint[StageStatusManager.Instance.StageInWorld + 1]);
                SetPathPositionMin(m_WP.Stage_WayPoint[StageStatusManager.Instance.StageInWorld]);

            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            {
                SetPathPositionMax(m_WP.Stage_WayPoint[StageStatusManager.Instance.StageInWorld]);
                SetPathPositionMin(m_WP.Stage_WayPoint[StageStatusManager.Instance.StageInWorld - 1]);
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.LEFT)
            {
                SetPathPositionMax(this._DollyCart.m_Path.MaxPos);
                SetPathPositionMin(0);
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.RIGHT)
            {
                SetPathPositionMax(this._DollyCart.m_Path.MaxPos);
                SetPathPositionMin(0);
            }

        }

        //カメラのパス位置を初期化する
        public void PathPositionReset()
        {
            //Debug.Log(pos);
            if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.UP)
            {
                dolly.m_PathPosition = pathPositionMin;
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            {
                dolly.m_PathPosition = pathPositionMax;
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.LEFT)
            {
                _DollyCart.m_Position = 0;
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.RIGHT)
            {
                _DollyCart.m_Position = 0;
            }

            //Debug.Log(this.dolly.m_PathPosition);
        }
        //ドリールートの加算倍率の変更
        public void SetAddTime()
        {
            switch (StageChangeManager.DollyState())
            {
                case DOLLY_MOVE.FIXING:
                    AddTime = 0.0f;
                    break;
                case DOLLY_MOVE.GO:
                    AddTime = 1.0f;
                    break;
                case DOLLY_MOVE.BACK:
                    AddTime = -1.0f;
                    break;
                case DOLLY_MOVE.WORLD:
                    AddTime = 1.0f;
                    break;
                case DOLLY_MOVE.ALL_STATES:

                default:
                    Debug.Log("DOLLY_MOVEの状態が違います");
                    AddTime = 0.0f;
                    break;
            }
        }

        //ドリールートのセット
        public void SetDollyPath()
        {
            switch (StageChangeManager.GetStageChangeKey())
            {
                case StageChangeManager.STAGE_CHANGE_KEY.UP:
                    this.dolly.m_Path = m_Dolly_GO_4[StageStatusManager.Instance.CurrentWorld];//前進用ドリーパスをセット
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.DOWN:
                    this.dolly.m_Path = m_Dolly_BACK_4[StageStatusManager.Instance.CurrentWorld];//後進用ドリーパスをセット
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    this.dolly.m_Path = m_Dolly_W2toW1[StageStatusManager.Instance.StageInWorld];//用ドリーパスをセット
                    this._DollyCart.m_Path = m_Dolly_W2toW1[StageStatusManager.Instance.StageInWorld];//用ドリーパスをセット
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    this.dolly.m_Path = m_Dolly_W1toW2[StageStatusManager.Instance.StageInWorld];//用ドリーパスをセット
                    this._DollyCart.m_Path = m_Dolly_W1toW2[StageStatusManager.Instance.StageInWorld];//用ドリーパスをセット
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                    break;
                default:
                    break;
            }

        }
        //開始ドリールートのセット
        public void SetStartDollyPath()
        {
            this.dolly.m_Path = m_Dolly_GO_4[StageStatusManager.Instance.CurrentWorld];//前進用ドリーパスをセット

        }

        //virtualカメラのFollowをセット
        public void SetFollower(GameObject _TargetObj)
        {
            this.virtualCamera.Follow = _TargetObj.transform;
            Debug.Log("virtualCamera.Follow:" + virtualCamera.Follow.name);
        }
        //virtualカメラのFollowを解除する
        public void SetNoneFollower()
        {
            this.virtualCamera.Follow = null;
            //Debug.Log("virtualCamera.Follow:" + virtualCamera.Follow.name);
        }
        //virtualカメラのLookAtをセット
        public void SetLookAtTarget(GameObject _TargetObj)
        {
            virtualCamera.LookAt = _TargetObj.transform;
            //_SubDolly.LookAtTargetChange(NextTarget);

        }
        //AutoDollyのON <-> OFF切り替え
        public void AutoDollySwitch()
        {
            this.dolly.m_AutoDolly.m_Enabled = !this.dolly.m_AutoDolly.m_Enabled;
        }

        //ドリーカメラのセット（カメラ注視点、パス位置、ドリーの状態）
        public void SetDollyCamera(STAGE_NO _StageNo, string _Word)
        {
            Debug.Log("DollyState(_Word):" + _Word);
            Debug.Log("dolly.m_Path:" + dolly.m_Path);

            if (StageChangeManager.IsStageChange())
            {
                SetPathPositionMax(m_WP.Stage_WayPoint[(int)_StageNo % 5]);
                SetPathPositionMin(m_WP.Stage_WayPoint[(int)_StageNo % 5]);

            }
            else if (StageChangeManager.IsWorldChange())
            {
                if (StageStatusManager.Instance.CurrentWorld == 0)
                {

                    SetPathPositionMax(this.dolly.m_Path.MaxPos);
                    SetPathPositionMin(this.dolly.m_Path.MinPos);
                }
                else if (StageStatusManager.Instance.CurrentWorld == 1)
                {

                    SetPathPositionMax(this.dolly.m_Path.MaxPos);
                    SetPathPositionMin(this.dolly.m_Path.MinPos);
                }

            }

        }

        //固定用ドリールートをセットする
        public void SetPathFixingDolly()
        {
            this.dolly.m_Path = m_DoTr.m_Dolly_FIXING;
            //パス位置を0にする
            this.dolly.m_PathPosition = 0;
        }
    }//public class DollyCamera : MonoBehaviour END
}//namespace END