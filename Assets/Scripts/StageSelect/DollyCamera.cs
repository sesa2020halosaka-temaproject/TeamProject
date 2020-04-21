using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TeamProject
{
    //public class DollyDriver : MonoBehaviour
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
        public enum PATH_MOVE
        {
            FIXING,//固定
            GO,  //進める
            BACK   //戻る
        }
        public PATH_MOVE Move;

        public bool Move_flag;//カメラが移動しているかどうか

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
            //// バーチャルカメラがセットされていなければ中止
            //if (this.virtualCamera == null)
            //{
            //    this.enabled = false;
            //    Debug.Log("バーチャルカメラが" + this.virtualCamera);
            //    return;
            //}

            //// ドリーコンポーネントを取得できなければ中止
            //this.dolly = this.virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            //if (this.dolly == null)
            //{
            //    Debug.Log("Dollyコンポーネントが" + this.dolly);

            //    this.enabled = false;
            //    return;
            // }
            if (MoveRatio <= 0)
            {
                MoveRatio = 1.0f;
            }
            // Positionの単位をトラック上のウェイポイント番号基準にするよう設定
            this.dolly.m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;

            // ウェイポイントの最大番号・最小番号を取得
            this.pathPositionMax = this.dolly.m_Path.MaxPos;
            this.pathPositionMin = this.dolly.m_Path.MinPos;

            this.DollyState("ZERO");

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
        private void Update()
        {
            //// cycleTime秒かけてトラック上を往復させる
            //var t = 0.5f - (0.5f * Mathf.Cos((Time.time * 2.0f * Mathf.PI) / this.cycleTime));
            this.dolly.m_PathPosition += AddTime * Time.deltaTime * MoveRatio;
            switch (Move)
            {
                case PATH_MOVE.FIXING:
                    break;
                case PATH_MOVE.GO:
                    if (this.dolly.m_PathPosition >= this.pathPositionMax)
                    {
                        this.dolly.m_PathPosition = this.pathPositionMax;
                        //移動完了
                        this.DollyState("ZERO");
                    }

                    break;
                case PATH_MOVE.BACK:
                    if (this.dolly.m_PathPosition <= this.pathPositionMin)
                    {
                        this.dolly.m_PathPosition = this.pathPositionMin;
                        //移動完了
                        this.DollyState("ZERO");
                    }

                    break;
                default:
                    break;
            }
        }
        //ドリーの状態を変える
        public void DollyState(string word)
        {
            switch (word)
            {
                case "ZERO":
                    AddTime = 0.0f;
                    Move = PATH_MOVE.FIXING;
                    Move_flag = false;//移動完了

                    break;
                case "GO":
                    AddTime = 1.0f;
                    Move = PATH_MOVE.GO;
                    Move_flag = true;//移動開始

                    break;
                case "BACK":
                    AddTime = -1.0f;
                    Move = PATH_MOVE.BACK;
                    Move_flag = true;//移動開始

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
        }

        //カメラの位置を設定する
        public void SetPathPosition(float pos)
        {
            //Debug.Log(pos);
            dolly.m_PathPosition = pos;
            //Debug.Log(this.dolly.m_PathPosition);
        }
    }
}