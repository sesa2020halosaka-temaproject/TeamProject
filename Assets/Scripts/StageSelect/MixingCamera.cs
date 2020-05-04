using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TeamProject
{
    //CinemachineMixingCameraの制御
    public class MixingCamera : MonoBehaviour
    {
        public CinemachineMixingCamera m_MixingCam;
        public CinemachineVirtualCamera vcam_before;
        public CinemachineVirtualCamera vcam_after;


        public float cam_weight;//カメラ1，2のウェイト値

        [Min(0)]
        public float m_Go_SwingTime;//前進時のカメラ方向転換時間
        [Min(0)]
        public float m_Back_SwingTime;//後進時のカメラ方向転換時間

        private float m_SwingTime;//カメラ方向転換時間

        private float AddRatio = 1.0f;//加算倍率（+1,0,-1）

        //Mixingカメラの状態
        public enum MIXING_STATE
        {
            FIXING,//固定
            GO,  //進める
            BACK,   //戻る
            WORLD,  //ワールド間移動
            ALL_STATES//全要素数
        }
        public MIXING_STATE m_MixingState;

        public bool Swing_flag;//カメラの方向が切り替え中かどうか
        private DollyCamera _Main_DollyCam;
        private DollyCamera _Sub_DollyCam;
        // Start is called before the first frame update
        void Start()
        {
            cam_weight = 0.0f;
            if (m_MixingCam == null)
            {
                Debug.LogError("CinemachineMixingCameraがセットされていません！");
                return;
            }

            _Sub_DollyCam = this.transform.GetChild(0).gameObject.GetComponent<DollyCamera>();
            _Main_DollyCam = this.transform.GetChild(1).gameObject.GetComponent<DollyCamera>();
            //this.MixState("ZERO");
        }

        //後で削除予定
        public void MixingUpdate()
        {
            //switch (StageChangeManager.MixingState())
            //{
            //    case MIXING_STATE.FIXING:
            //        break;
            //    case MIXING_STATE.GO:
            //        cam_weight += Time.deltaTime * AddRatio / m_SwingTime;
            //        if (cam_weight > 1.0f)
            //        {
            //            cam_weight = 1.0f;
            //            //方向転換完了
            //            //this.MixState("ZERO");
            //            StageChangeManager.MixingStateChange("FIXING");
            //            StageChangeManager.DollyStateChange("GO");
            //            //ドリーカメラの初期設定
            //            _Sub_DollyCam.SetDollyCamera(StageStatusManager.Instance.CurrentStage, "GO");
            //            _Main_DollyCam.SetDollyCamera(StageStatusManager.Instance.CurrentStage, "GO");
            //            LookAtTargetTwoChanges();

            //        }

            //        break;
            //    case MIXING_STATE.BACK:
            //        cam_weight += Time.deltaTime * AddRatio / m_SwingTime;
            //        if (cam_weight > 1.0f)
            //        {
            //            cam_weight = 1.0f;
            //            //方向転換完了
            //            // this.MixState("ZERO");
            //            StageChangeManager.MixingStateChange("FIXING");
            //            StageChangeManager.DollyStateChange("BACK");
            //           //ResetWeight();
            //           //ResetWeight();

            //            //ドリーカメラの初期設定
            //            _Sub_DollyCam.SetDollyCamera(StageStatusManager.Instance.CurrentStage, "BACK");
            //            _Main_DollyCam.SetDollyCamera(StageStatusManager.Instance.CurrentStage, "BACK");
            //            LookAtTargetTwoChanges();

            //        }

            //        break;
            //    case MIXING_STATE.ALL_STATES:
            //        break;
            //    default:
            //        break;
            //}
            //m_MixingCam.m_Weight0 = 1 - cam_weight;
            //m_MixingCam.m_Weight1 = cam_weight;

        }//public void MixingUpdate() END

        //Update is called once per frame
        void Update()
        {
            m_MixingState = StageChangeManager.MixingState();
            switch (StageChangeManager.MixingState())
            {
                case MIXING_STATE.FIXING:
                    break;
                case MIXING_STATE.GO:
                    //カメラウェイトの加算
                    cam_weight += Time.deltaTime * AddRatio / m_SwingTime;

                    if (cam_weight > 1.0f)
                    {
                        cam_weight = 1.0f;
                        //方向転換完了
                        StageChangeManager.MixingStateChange("FIXING");
                        StageChangeManager.DollyStateChange("GO");



                    }

                    break;
                case MIXING_STATE.BACK:
                    cam_weight += Time.deltaTime * AddRatio / m_SwingTime;
                    if (cam_weight > 1.0f)
                    {
                        cam_weight = 1.0f;
                        //方向転換完了
                        // this.MixState("ZERO");
                        StageChangeManager.MixingStateChange("FIXING");
                        StageChangeManager.DollyStateChange("BACK");
                        //ResetWeight();

                        ////ドリーカメラの初期設定
                        //_Sub_DollyCam.SetDollyCamera(StageStatusManager.Instance.PrevStage, "BACK");
                        //_Main_DollyCam.SetDollyCamera(StageStatusManager.Instance.PrevStage, "BACK");
                        //LookAtTargetTwoChanges(StageStatusManager.Instance.PrevStage, StageStatusManager.Instance.PrevStage);

                    }

                    break;
                case MIXING_STATE.WORLD:
                    //カメラウェイトの加算
                    cam_weight += Time.deltaTime * AddRatio / m_SwingTime;

                    if (cam_weight > 1.0f)
                    {
                        cam_weight = 1.0f;
                        //方向転換完了
                        StageChangeManager.MixingStateChange("FIXING");
                        StageChangeManager.DollyStateChange("WORLD");



                    }
                    break;
                case MIXING_STATE.ALL_STATES:
                    break;
                default:
                    break;
            }
            //カメラウェイトの配分
            m_MixingCam.m_Weight0 = 1 - cam_weight;
            m_MixingCam.m_Weight1 = cam_weight;

        }// void Update() END

        // Update is called once per frame
        //void Update()
        //{

        //    switch (m_MixingState)
        //    {
        //        case MIXING_STATE.FIXING:
        //            break;
        //        case MIXING_STATE.GO:
        //            cam_weight += Time.deltaTime * AddRatio / m_SwingTime;
        //            if (cam_weight > 1.0f)
        //            {
        //                cam_weight = 1.0f;
        //                //方向転換完了
        //                this.MixState("ZERO");

        //            }
        //            break;
        //        case MIXING_STATE.BACK:
        //            cam_weight += Time.deltaTime * AddRatio / m_SwingTime;
        //            if (cam_weight > 1.0f)
        //            {
        //                cam_weight = 1.0f;
        //                //方向転換完了
        //                this.MixState("ZERO");

        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //    m_MixingCam.m_Weight0 = 1 - cam_weight;
        //    m_MixingCam.m_Weight1 = cam_weight;
        //}// void Update() END

        ////ミキシングの状態を変える
        //public void MixState(string word)
        //{
        //    switch (word)
        //    {
        //        case "ZERO":
        //            //AddRatio = 0.0f;
        //            //m_MixingState = MIXING_STATE.FIXING;
        //            Swing_flag = false;//完了
        //            break;
        //        case "GO":
        //            //AddRatio = 1.0f;
        //            m_SwingTime = m_Go_SwingTime;
        //            //m_MixingState = MIXING_STATE.GO;
        //            Swing_flag = true;//開始

        //            break;
        //        case "BACK":
        //            //AddRatio = 1.0f;
        //            m_SwingTime = m_Back_SwingTime;
        //            //m_MixingState = MIXING_STATE.BACK;
        //            Swing_flag = true;//開始

        //            break;
        //        default:
        //            Debug.Log("言葉が違います。カメラを固定します。");
        //            AddRatio = 0.0f;
        //            break;
        //    }
        //}//    public void DollyState(string word)  END

        //方向転換速度をセットする
        public void SetSwingTime()
        {
            switch (StageChangeManager.MixingState())
            {
                case MIXING_STATE.FIXING:
                    m_SwingTime = 9.0f;
                    break;
                case MIXING_STATE.GO:
                    m_SwingTime = m_Go_SwingTime;
                    break;
                case MIXING_STATE.BACK:
                    m_SwingTime = m_Back_SwingTime;
                    break;
                case MIXING_STATE.ALL_STATES:
                    break;
                default:
                    break;
            }

        }
        //方向変換中かどうかboolで返す。
        public bool IsSwing()
        {
            return Swing_flag;
        }

        //フラグをtrueにする
        public void SwingFlagOn()
        {
            Swing_flag = true;
        }
        //フラグをfalseにする
        public void SwingFlagOff()
        {

            Swing_flag = false;
        }

        //
        public void ResetWeight()
        {
            cam_weight = 0.0f;
        }

        //現在のターゲットと次のターゲットのLookAtをセットする
        public void LookAtTargetTwoChanges()
        {
            switch (StageChangeManager.MixingState())
            {
                case MIXING_STATE.FIXING:
                    vcam_before.LookAt = TargetStages.m_Stages[(int)StageStatusManager.Instance.NextStage].transform;
                    vcam_after.LookAt = TargetStages.m_Stages[(int)StageStatusManager.Instance.NextStage].transform;
                    break;
                case MIXING_STATE.GO:
                    vcam_before.LookAt = TargetStages.m_Stages[(int)StageStatusManager.Instance.CurrentStage].transform;
                    vcam_after.LookAt = TargetStages.m_Stages[(int)StageStatusManager.Instance.NextStage].transform;
                    break;
                case MIXING_STATE.BACK:
                    vcam_before.LookAt = TargetStages.m_Stages[(int)StageStatusManager.Instance.CurrentStage].transform;
                    vcam_after.LookAt = TargetStages.m_Stages[(int)StageStatusManager.Instance.PrevStage].transform;
                    break;
                case MIXING_STATE.ALL_STATES:
                    break;
                default:
                    break;
            }
        }// END

 
        //現在のターゲットと次のターゲットのLookAtをセットする
       public void LookAtTargetTwoChanges(STAGE_NO _BeforeStage, STAGE_NO _AfterStage)
        {
            Debug.Log("_BeforeStage:" + _BeforeStage);
            Debug.Log("_AfterStage:" + _AfterStage);
            vcam_before.LookAt = TargetStages.m_Stages[(int)_BeforeStage].transform;
            vcam_after.LookAt = TargetStages.m_Stages[(int)_AfterStage].transform;
        }// END

        //現在のターゲットと次のターゲットのLookAtをセットする(次のターゲットがGameObject版)
        public void LookAtTargetTwoChanges(STAGE_NO _BeforeStage, GameObject _AfterObj)
        {
            Debug.Log("_BeforeStage:" + _BeforeStage);
            Debug.Log("_AfterStage:" + _AfterObj);
            vcam_before.LookAt = TargetStages.m_Stages[(int)_BeforeStage].transform;
            vcam_after.LookAt = _AfterObj.transform;
        }// END

        //ミキシングカメラのセット（カメラ注視点：現在と次、ミキシングの状態、カメラウェイトのリセット）
        //public void SetMixCamera(GameObject _MixingCameraObject, STAGE_NO _CurrentStageNo, STAGE_NO _NextStageNo, string _Word)
        //{
        //    _MixingCameraObject.GetComponent<MixingCamera>().LookAtTargetTwoChanges(TargetStages.m_Stages[(int)_CurrentStageNo], TargetStages.m_Stages[(int)_NextStageNo]);
        //    //_MixingCameraObject.GetComponent<MixingCamera>().MixState(_Word);
        //    _MixingCameraObject.GetComponent<MixingCamera>().ResetWeight();
        //}

    }//public class MixingCamera : MonoBehaviour END
}//namespace END