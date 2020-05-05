﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
using UnityEngine.SceneManagement;

namespace TeamProject
{
    public class StageSelect : MonoBehaviour
    {
        //public GameObject[] Stages;//LookAtの対象となるゲームオブジェクトの格納用
        public float[] WayPoint;//ステージの正面に位置するドリーのパスの位置を入れる用
        public WayPoint_Box m_WP;//ドリールートのパス位置格納用
        public DollyTrack_Box m_DoTr;//ドリールートの格納用
        public float Volume;

        public float ToTitle_FadeOut_Time;//タイトルに遷移する時のフェードアウト時間
        public float StageIn_FadeOut_Time;//ステージに遷移する時のフェードアウト時間
        public GameObject _Dolly_Current;
        public GameObject _Dolly_Next;
        public GameObject _MixCamObj;
        private MixingCamera _Mixcam;
        private DollyCamera _Main_DollyCam;
        private DollyCamera _Sub_DollyCam;
        public StageSelectArrow _StageSelectArrow;

        public GameObject _DollyCartObj;//ドリーカート用ゲームオブジェクト
        public GameObject _TargetObj;//ドリーカートを先導するゲームオブジェクト
        public DollyCartManager _DollyCart;

        public int m_Counter = 0;
        public int m_Stopper = 3;
        public bool m_FixingFlag = false;
        private int count = 0;
        //ステージセレクトの状態
        public enum SELECT_STATE
        {
            KEY_WAIT = 0,//キー入力待ち
            BEFORE_STAGE_MOVING,//ステージ移動前の準備
            STAGE_MOVING,//ステージ移動中
            BEFORE_WORLD_MOVING,//ワールド移動前の準備
            WORLD_MOVING,//ワールド移動中
            //SWING,       //次の目的の方向へを向いている途中
            //MOVING,      //カメラ移動中
            SCENE_MOVING,//シーン遷移中
            STATE_NUM    //状態の数
        }
        public SELECT_STATE select_state;

        //ドリーの状態
        public enum DOLLY_STATE
        {
            GO = 0,  //次のステージへ
            BACK,    //前のステージへ
            STATE_NUM//状態の数
        }
        public DOLLY_STATE dolly_state;

        private int db_cnt = 0;//デバッグログ確認用カウント
        //=================================================================
        //関数ここから
        //=================================================================
        // Start is called before the first frame update
        void Start()
        {
            //フェードイン
            FadeManager.FadeIn(1.3f);
            Debug.Log("Debugカウント：" + db_cnt);
            db_cnt++;
            //BGMスタート
            //BGMSwitcher.FadeOutAndFadeIn(BGMPath.BGM_STAGE_SELECT);
            BGMSwitcher.CrossFade(BGMPath.BGM_STAGE_SELECT_SUMMER);
            //水の音追加
            BGMManager.Instance.Play(SEPath.SE_AMB_STAGE_SELECT, volumeRate: Volume, delay: 1.0f, isLoop: true, allowsDuplicate: true);


            _MixCamObj = GameObject.Find("Mixing_VCamera").gameObject;
            _Mixcam = _MixCamObj.GetComponent<MixingCamera>();

            _Dolly_Next = _MixCamObj.transform.Find("Next_VCamera").gameObject;
            _Dolly_Current = _MixCamObj.transform.Find("Current_VCamera").gameObject;

            _Main_DollyCam = _Dolly_Next.GetComponent<DollyCamera>();
            _Sub_DollyCam = _Dolly_Current.GetComponent<DollyCamera>();

            //ドリーカート用ゲームオブジェクト
            _DollyCartObj = GameObject.Find("DollyCart").gameObject;
            //ドリーカートを先導するゲームオブジェクト
            _TargetObj = _DollyCartObj.transform.GetChild(0).gameObject;
            _DollyCart = _DollyCartObj.GetComponent<DollyCartManager>();

            //DollyTrack用ゲームオブジェクト取得
            m_DoTr = GameObject.Find("DollyTrack_Obj").GetComponent<DollyTrack_Box>();
            //WayPoint用ゲームオブジェクト取得
            m_WP = GameObject.Find("WayPoint_Box").GetComponent<WayPoint_Box>();

            //現在のステージから初期設定
            //ドリールートの設定
            _Main_DollyCam.SetStartDollyPath();
            _Sub_DollyCam.SetStartDollyPath();
            //初期配置
            _Main_DollyCam.SetPathPosition(m_WP.Stage_WayPoint[StageStatusManager.Instance.StageInWorld]);
            _Sub_DollyCam.SetPathPosition(m_WP.Stage_WayPoint[StageStatusManager.Instance.StageInWorld]);
            //LookAt・注視点の設定
            _Main_DollyCam.SetLookAtTarget(TargetStages.m_Stages[(int)StageStatusManager.Instance.CurrentStage]);
            _Sub_DollyCam.SetLookAtTarget(TargetStages.m_Stages[(int)StageStatusManager.Instance.CurrentStage]);

            //Debug.Log((int)StageStatusManager.Instance.CurrentStage);

            //Debug.Log((int)StageStatusManager.Instance.CurrentStage);
            //_StageSelectArrow = this.transform.Find("Panel/StageMoveArrows").GetComponent<StageSelectArrow>();

            StageChangeManager.SelectStateChange("KEY_WAIT");
        }

        // Update is called once per frame
        void Update()
        {
            select_state = StageChangeManager.GetSelectState();
            switch (StageChangeManager.GetSelectState())
            {
                case SELECT_STATE.KEY_WAIT:

                    //ステージ選択（WSキー or スティック上下）
                    //StageChange();
                    StageChangeManager.StageChange();
                    //ワールド選択（ADキー or スティック左右）
                    WorldChange();
                    //WorldChangeManagr.WorldChange();
                    //決定（Space  or Bボタン）
                    StageDecision();
                    //タイトルへ戻る(ESCキー or Startボタン)
                    BackToTitle();
                    //フラグチェック
                    FlagCheck();

                    break;
                case SELECT_STATE.BEFORE_STAGE_MOVING:
                    count++;
                    Debug.Log("count" + count);
                    m_Counter++;
                    if (!m_FixingFlag)
                    {
                        m_FixingFlag = !m_FixingFlag;
                    //固定用ドリールートをセット
                    //_Main_DollyCam.SetPathFixingDolly();
                    //_Sub_DollyCam.SetPathFixingDolly();
                    //_DollyCart.SetPathFixingDolly();

                    }
                    if (m_Counter>m_Stopper)
                    {
                        m_FixingFlag = !m_FixingFlag;
                        m_Counter = 0;
                    //Mixingカメラの初期化
                    ResetMixingCamera();

                    //Dollyカメラの初期化
                    ResetDollyCamera();

                    //Dollyカメラの状態をFIXINGに戻す
                    StageChangeManager.DollyStateChange("FIXING");

                    //ステージ移動の状態へ移行
                    StageChangeManager.SelectStateChange("STAGE_MOVING");
                    }

                    break;
                case SELECT_STATE.STAGE_MOVING:
                    //_Mixcam.MixingUpdate();
                    //_Main_DollyCam.DollyUpdate();
                    //_Sub_DollyCam.DollyUpdate();

                    //移動完了後
                    if (StageChangeManager.DollyFlagCheck())
                    {
                        Debug.Log("StageChangeManager.DollyFlagCheck():" + StageChangeManager.DollyFlagCheck());
                        StageChangeManager.DollyStateChange("FIXING");

                        StageChangeManager.SelectStateChange("KEY_WAIT");

                        //フラグをfalseにする
                        StageChangeManager.StageFlagChange();

                        //フラグをfalseにする
                        StageChangeManager.DollyFlagReset();


                        if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.UP)
                        {
                            StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.NextStage;
                        }
                        else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
                        {
                            StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.PrevStage;
                        }

                        ////固定用ドリールートをセット
                        //_Main_DollyCam.SetPathFixingDolly();
                        //_Sub_DollyCam.SetPathFixingDolly();
                        //_DollyCart.SetPathFixingDolly();
                        //LookAtを一か所に固定
                        _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.CurrentStage);
                        Debug.Log("STAGE_MOVING終わり");
                    }

                    StageChangeManager.Update();
                    break;
                case SELECT_STATE.BEFORE_WORLD_MOVING:
                    Debug.Log("StageChangeManager.GetStageChangeKey()" + StageChangeManager.GetStageChangeKey());
                    count++;
                    Debug.Log("count" + count);
                    m_Counter++;
                    if (!m_FixingFlag)
                    {
                        m_FixingFlag = !m_FixingFlag;
                        ////固定用ドリールートをセット
                        //_Main_DollyCam.SetPathFixingDolly();
                        //_Sub_DollyCam.SetPathFixingDolly();
                        //_DollyCart.SetPathFixingDolly();

                    }
                    if (m_Counter > m_Stopper)
                    {
                        m_FixingFlag = !m_FixingFlag;
                        m_Counter = 0;


                        //Mixingカメラの初期化
                        ResetMixingCamera();

                        //Dollyカメラの初期化
                        ResetDollyCamera();

                        //Dollyカメラの状態をFIXINGに戻す
                        StageChangeManager.DollyStateChange("FIXING");


                        StageChangeManager.SelectStateChange("WORLD_MOVING");
                    }
                    break;
                case SELECT_STATE.WORLD_MOVING:
                    //_Mixcam.MixingUpdate();
                    //_Main_DollyCam.DollyUpdate();
                    //_Sub_DollyCam.DollyUpdate();

                    //移動完了後
                    if (StageChangeManager.IsWorldMoveEnd())
                    {
                       // Debug.Log("StageChangeManager.DollyFlagCheck():" + StageChangeManager.DollyFlagCheck());

                       //各種状態を待機状態に戻す
                        StageChangeManager.DollyStateChange("FIXING");

                        StageChangeManager.SelectStateChange("KEY_WAIT");

                        //フラグをfalseにする
                        StageChangeManager.WorldFlagChange();
                        StageChangeManager.DollyCartFlagReset();
                        StageChangeManager.DollyFlagReset();


                        //AutoDollyのチェックを外す
                        _Main_DollyCam.AutoDollySwitch();
                        _Sub_DollyCam.AutoDollySwitch();

                        //virtualカメラのFollowを解除する
                        //_Main_DollyCam.SetNoneFollower();
                        //_Sub_DollyCam.SetNoneFollower();

                        //ステージ番号の更新
                        if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.LEFT)
                        {
                            StageStatusManager.Instance.CurrentStage = (STAGE_NO)(StageStatusManager.Instance.PrevWorld * 5);
                        }
                        else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.RIGHT)
                        {
                            StageStatusManager.Instance.CurrentStage = (STAGE_NO)(StageStatusManager.Instance.NextWorld * 5);
                        }

                        //固定用ドリールートをセット
                        _Main_DollyCam.SetPathFixingDolly();
                        _Sub_DollyCam.SetPathFixingDolly();
                        _DollyCart.SetPathFixingDolly();

                        //_Sub_DollyCam.PathPositionReset();
                        //LookAtを一か所に固定
                        _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.CurrentStage);
                        Debug.Log("WORLD_MOVINGおしまい");

                    }

                    //WorldChangeManagr.Update();
                    break;


                //case SELECT_STATE.SWING:
                //    bool IsSwing = _MixCamObj.GetComponent<MixingCamera>().IsSwing();
                //    Debug.Log(IsSwing);
                //    if (!IsSwing)//方向転換完了したかどうか
                //    {
                //        //_MixCamObj.GetComponent<MixingCamera>().MixState("ZERO");

                //        //移動中にする
                //        select_state = SELECT_STATE.MOVING;
                //        //カメラ移動を始める
                //        switch (dolly_state)
                //        {
                //            case DOLLY_STATE.GO:
                //                DollyCameraGo(StageStatusManager.Instance.CurrentStage);
                //                break;
                //            case DOLLY_STATE.BACK:
                //                DollyCameraBack(StageStatusManager.Instance.CurrentStage);
                //                break;
                //        }
                //    }

                //    break;

                //case SELECT_STATE.MOVING:
                //    bool IsMoving_2 = _Dolly_Current.GetComponent<DollyCamera>().IsMoving();
                //    bool IsMoving = _Dolly_Next.GetComponent<DollyCamera>().IsMoving();
                //    if (!IsMoving && !IsMoving_2)//移動完了したかどうか
                //    {
                //        //キー入力待ちに戻す
                //        select_state = SELECT_STATE.KEY_WAIT;

                //        //カメラ移動速度を0にする
                //        _Dolly_Current.GetComponent<DollyCamera>().DollyState("ZERO");
                //        _Dolly_Next.GetComponent<DollyCamera>().DollyState("ZERO");

                //        //_Mixing.GetComponent<MixingCamera>().LookAtTargetTwoChanges(Stages[(int)stage_num], Stages[(int)StageStatusManager.Instance.CurrentStage]);

                //    }
                //    break;
                case SELECT_STATE.SCENE_MOVING:
                    break;
                case SELECT_STATE.STATE_NUM:
                    break;
                default:
                    break;
            }
        }//void Update()    END

        //
        void WorldChange()
        {
            //ステージ番号から現在のワールドを確認する
            int WorldNumber = (int)StageStatusManager.Instance.CurrentStage / 5;
            //右入力
            if (InputManager.InputManager.Instance.GetLStick().x > 0)
            {

            }
            //左入力
            else if (InputManager.InputManager.Instance.GetLStick().x > 0)
            {

            }

        }

        //ステージ決定
        void StageDecision()
        {
            //カーソルの操作（決定）
            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtunCode.A))
            //if (Input.GetKeyDown(KeyCode.Space))
            {
                string StageName = StageStatusManager.Instance.StageString[(int)StageStatusManager.Instance.CurrentStage];
                FadeManager.FadeOut(StageName, StageIn_FadeOut_Time);

                //シーン遷移中状態にする
                select_state = SELECT_STATE.SCENE_MOVING;
                StageChangeManager.SelectStateChange("SCENE_MOVING");
                Debug.Log("決定です！");
                //決定音鳴らす
                SEManager.Instance.Play(SEPath.SE_OK);
            }
        }


        private void BackToTitle()
        {
            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtunCode.Menu))
            //if (Input.GetKeyDown(KeyCode.Escape))
            {//ESCキー入力
                FadeManager.FadeOut("TitleScene", ToTitle_FadeOut_Time);
            }

        }

        //状態フラグのチェック
        private void FlagCheck()
        {
            if (StageChangeManager.IsStageChange())
            {
                //select_state = SELECT_STATE.STAGE_MOVING;
                StageChangeManager.SelectStateChange("BEFORE_STAGE_MOVING");
            }
            else if (StageChangeManager.IsWorldChange())
            //else if (WorldChangeManagr.IsWorldChange())
            {
                //select_state = SELECT_STATE.WORLD_MOVING;
                StageChangeManager.SelectStateChange("BEFORE_WORLD_MOVING");
            }

        }


        //Mixingカメラの初期化
        public void ResetMixingCamera()
        {
            if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.UP)
            {
                _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.NextStage);
                StageChangeManager.MixingStateChange("GO");
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            {
                _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.PrevStage);
                StageChangeManager.MixingStateChange("BACK");
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.LEFT)
            {
                _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, _TargetObj);
                StageChangeManager.MixingStateChange("WORLD");
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.RIGHT)
            {
                _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, _TargetObj);
                StageChangeManager.MixingStateChange("WORLD");
            }
            
            _Mixcam.ResetWeight();
            _Mixcam.SwingFlagOn();
            _Mixcam.SetSwingTime();

        }
        //Dollyカメラの初期化
        public void ResetDollyCamera()
        {
            if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.UP)
            {
                StageChangeManager.DollyStateChange("GO");
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            {
                StageChangeManager.DollyStateChange("BACK");
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.LEFT)
            {
                StageChangeManager.DollyStateChange("WORLD");
                _Main_DollyCam.SetFollower(_DollyCartObj);
                _Sub_DollyCam.SetFollower(_DollyCartObj);
                _Main_DollyCam.SetLookAtTarget(_TargetObj);
                _Sub_DollyCam.SetLookAtTarget(_TargetObj);
                _Main_DollyCam.AutoDollySwitch();
                _Sub_DollyCam.AutoDollySwitch();
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.RIGHT)
            {
                StageChangeManager.DollyStateChange("WORLD");
                _Main_DollyCam.SetFollower(_DollyCartObj);
                _Sub_DollyCam.SetFollower(_DollyCartObj);
                _Main_DollyCam.SetLookAtTarget(_TargetObj);
                _Sub_DollyCam.SetLookAtTarget(_TargetObj);
                _Main_DollyCam.AutoDollySwitch();
                _Sub_DollyCam.AutoDollySwitch();
            }
            _Main_DollyCam.SetDollyPath();
            _Main_DollyCam.SetAddTime();
            _Main_DollyCam.SetPathPositionALL();
            _Main_DollyCam.PathPositionReset();

            _Sub_DollyCam.SetDollyPath();
            _Sub_DollyCam.SetAddTime();
            _Sub_DollyCam.SetPathPositionALL();
            _Sub_DollyCam.PathPositionReset();

        }
        //DollyCartの初期化
        public void ResetDollyCart()
        {
            if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.UP)
            {
                Debug.Log("UP：処理が間違っています。");
                return;
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            {
                Debug.Log("DOWN：処理が間違っています。");
                return;
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.LEFT)
            {
                //StageChangeManager.DollyStateChange("WORLD");
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.RIGHT)
            {
                //StageChangeManager.DollyStateChange("WORLD");
            }
            _DollyCart.SetDollyPath();
            //_DollyCart.SetAddTime();
            //_DollyCart.SetPathPositionALL();
            _DollyCart.PathPositionReset();

           }


    }//public class StageSelect : MonoBehaviour END
}//namespace END