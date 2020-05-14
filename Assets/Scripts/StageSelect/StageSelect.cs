using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
//using UnityEngine.SceneManagement;

namespace TeamProject
{
    public class StageSelect : MonoBehaviour
    {
        //public GameObject[] Stages;//LookAtの対象となるゲームオブジェクトの格納用
        public WayPoint_Box m_WP;//ドリールートのパス位置格納用
        public float m_CurrentPathPosition;//現在のパス位置を渡す用
        private float[] m_WayPoint;//受け渡し用

        public DollyTrack_Box m_DoTr;//ドリールートの格納用

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

        public int m_Counter = 0;//
        public int m_InputStopFrame;//ステージやワールドの移動後のキー入力を待たせるフラグ
        public bool m_KeyWait_Flag = false;
        public bool m_WorldEndMixing_Flag = false;
        private int count = 0;
        //ステージセレクトの状態
        public enum SELECT_STATE
        {
            KEY_WAIT = 0,//キー入力待ち
            BEFORE_STAGE_MOVING,//ステージ移動前の準備
            STAGE_MOVING,//ステージ移動中
            BEFORE_WORLD_MOVING,//ワールド移動前の準備
            WORLD_MOVING,//ワールド移動中
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

        private StageSelectSound m_SelectSound;
        //=================================================================
        //関数ここから
        //=================================================================
        private void Awake()
        {
            m_SelectSound = this.GetComponent<StageSelectSound>();
        }
        // Start is called before the first frame update
        void Start()
        {
            //フェードイン
            FadeManager.FadeIn(1.3f);
            Debug.Log("Debugカウント：" + db_cnt);
            db_cnt++;


            //BGMスタート
            m_SelectSound.StageSelectStartBGM();

            //switch (StageStatusManager.Instance.CurrentWorld)
            //{
            //    case (int)WORLD_NO.W1://ワールド１：夏
            //        //BGMSwitcher.FadeOutAndFadeIn(BGMPath.BGM_STAGE_SELECT);
            //        //BGMSwitcher.CrossFade(BGMPath.BGM_STAGE_SELECT_SUMMER);
            //        BGMManager.Instance.Play(BGMPath.BGM_STAGE_SELECT_SUMMER);

            //        //水の音追加
            //        BGMManager.Instance.Play(SEPath.SE_AMB_STAGE_SELECT, volumeRate: Volume, delay: 0.0f, isLoop: true, allowsDuplicate: true);
            //        BGMManager.Instance.FadeIn(SEPath.SE_AMB_STAGE_SELECT, duration: 2.0f);
            //        break;
            //    case (int)WORLD_NO.W2://ワールド２：秋
            //        //BGMSwitcher.FadeOutAndFadeIn(BGMPath.BGM_STAGE_SELECT);
            //        //BGMSwitcher.CrossFade(BGMPath.BGM_STAGE_SELECT_SUMMER);
            //        BGMManager.Instance.Play(BGMPath.BGM_GAME_FALL);

            //        //水の音追加
            //        BGMManager.Instance.Play(SEPath.SE_AMB_STAGE_SELECT, volumeRate: Volume, delay: 0.0f, isLoop: true, allowsDuplicate: true);
            //        BGMManager.Instance.FadeIn(SEPath.SE_AMB_STAGE_SELECT, duration: 2.0f);
            //        break;
            //    case (int)WORLD_NO.W3://ワールド３：冬
            //        BGMManager.Instance.Play(BGMPath.BGM_STAGE_SELECT_WINTER);

            //        //水の音追加
            //        BGMManager.Instance.Play(SEPath.SE_AMB_STAGE_SELECT, volumeRate: Volume, delay: 0.0f, isLoop: true, allowsDuplicate: true);
            //        BGMManager.Instance.FadeIn(SEPath.SE_AMB_STAGE_SELECT, duration: 2.0f);

            //        break;
            //    case (int)WORLD_NO.W4://ワールド４：春

            //        BGMManager.Instance.Play(BGMPath.BGM_STAGE_SELECT_WINTER);

            //        //水の音追加
            //        BGMManager.Instance.Play(SEPath.SE_AMB_STAGE_SELECT, volumeRate: Volume, delay: 0.0f, isLoop: true, allowsDuplicate: true);
            //        BGMManager.Instance.FadeIn(SEPath.SE_AMB_STAGE_SELECT, duration: 2.0f);
            //        break;
            //}

            _MixCamObj = GameObject.Find("Mixing_VCamera").gameObject;
            _Mixcam = _MixCamObj.GetComponent<MixingCamera>();

            _Dolly_Current = _MixCamObj.transform.GetChild(0).gameObject;
            _Dolly_Next = _MixCamObj.transform.GetChild(1).gameObject;

            _Sub_DollyCam = _Dolly_Current.GetComponent<DollyCamera>();
            _Main_DollyCam = _Dolly_Next.GetComponent<DollyCamera>();

            //ドリーカート用ゲームオブジェクト
            _DollyCartObj = GameObject.Find("DollyCart").gameObject;
            //ドリーカートを先導するゲームオブジェクト
            _TargetObj = _DollyCartObj.transform.GetChild(0).gameObject;//"Bellwhether"オブジェクトを取得
            _DollyCart = _DollyCartObj.GetComponent<DollyCartManager>();

            //DollyTrack用ゲームオブジェクト取得
            m_DoTr = GameObject.Find("DollyTrack_Obj").GetComponent<DollyTrack_Box>();
            //WayPoint用ゲームオブジェクト取得
            m_WP = GameObject.Find("WayPoint_Box").GetComponent<WayPoint_Box>();

            //シーン開始時点のステージから初期化
            //ドリールートの設定
            _Main_DollyCam.SetStartDollyPath();
            _Sub_DollyCam.SetStartDollyPath();

            //ドリーカメラの初期化
            //WayPoint格納配列のセット
            m_WP.SetWayPoint();
            switch (StageStatusManager.Instance.CurrentWorld)
            {
                case (int)WORLD_NO.W1:
                    m_WayPoint = m_WP.SummerStage_WayPoint;
                    break;
                case (int)WORLD_NO.W2:
                    m_WayPoint = m_WP.FallStage_WayPoint;
                    break;
                case (int)WORLD_NO.W3:
                    m_WayPoint = m_WP.WinterStage_WayPoint;
                    break;
                case (int)WORLD_NO.W4:
                    m_WayPoint = m_WP.SpringStage_WayPoint;
                    break;
                case (int)WORLD_NO.ALL_WORLD:
                default:
                    Debug.LogError("無効な状態です！");
                    break;
            }
            //開始時点のパス位置をセット
            m_CurrentPathPosition = m_WP.m_Stage_WayPoint[StageStatusManager.Instance.StageInWorld];

            //各ドリーカメラにパス位置をセット
            _Main_DollyCam.SetPathPosition(m_CurrentPathPosition);
            _Sub_DollyCam.SetPathPosition(m_CurrentPathPosition);

            //LookAt・注視点の設定
            _Main_DollyCam.SetLookAtTarget(TargetStages.m_Stages[(int)StageStatusManager.Instance.CurrentStage]);
            _Sub_DollyCam.SetLookAtTarget(TargetStages.m_Stages[(int)StageStatusManager.Instance.CurrentStage]);

            //Debug.Log((int)StageStatusManager.Instance.CurrentStage);

            StageChangeManager.SelectStateChange("KEY_WAIT");
        }//void Start() END

        // Update is called once per frame
        void Update()
        {
            select_state = StageChangeManager.GetSelectState();
            switch (StageChangeManager.GetSelectState())
            {
                case SELECT_STATE.KEY_WAIT:
                    m_Counter++;

                    //m_InputStopFrameの分だけ待たせる
                    if (m_Counter > m_InputStopFrame && !m_KeyWait_Flag)
                    {
                        //フラグをONにする
                        m_KeyWait_Flag = !m_KeyWait_Flag;
                        m_Counter = 0;
                    }
                    //フラグがONになったら入力可能にする
                    if (m_KeyWait_Flag)
                    {

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

                        //上下矢印の処理
                        StageSelectArrow.SetCurrentStage(StageStatusManager.Instance.StageInWorld);
                        //左右矢印の処理
                        WorldSelectArrow.SetCurrentWorld();

                    }

                    break;
                case SELECT_STATE.BEFORE_STAGE_MOVING:
                    count++;
                    Debug.Log("count" + count);

                    //固定用ドリールートをセット
                    _Main_DollyCam.SetPathFixingDolly();
                    _Sub_DollyCam.SetPathFixingDolly();
                    //   _DollyCart.SetPathFixingDolly();

                    //Mixingカメラの初期化
                    ResetMixingCamera();

                    //Dollyカメラの初期化
                    ResetDollyCamera();

                    ////Dollyカメラの状態をFIXINGに戻す
                    //StageChangeManager.DollyStateChange("FIXING");

                    //上下矢印の非アクティブ化
                    StageSelectArrow.TwoArrowsDeactivate();
                    //左右矢印の非アクティブ化
                    WorldSelectArrow.TwoArrowsDeactivate();

                    //ステージ移動の状態へ移行
                    StageChangeManager.SelectStateChange("STAGE_MOVING");

                    break;
                case SELECT_STATE.STAGE_MOVING:
                    //_Mixcam.MixingUpdate();
                    //_Main_DollyCam.DollyUpdate();
                    //_Sub_DollyCam.DollyUpdate();

                    //移動完了後
                    if (StageChangeManager.DollyFlagCheck())
                    {
                        Debug.Log("StageChangeManager.DollyFlagCheck():" + StageChangeManager.DollyFlagCheck());
                        //Dollyカメラの状態を設定する
                        StageChangeManager.DollyStateChange("FIXING");

                        //ステージセレクトの状態を設定する
                        StageChangeManager.SelectStateChange("KEY_WAIT");

                        //フラグをfalseにする
                        StageChangeManager.StageFlagChange();

                        //フラグをfalseにする
                        StageChangeManager.DollyFlagReset();

                        //ステージ番号の更新
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

                    //StageChangeManager.Update();
                    break;
                case SELECT_STATE.BEFORE_WORLD_MOVING:
                    Debug.Log("StageChangeManager.GetStageChangeKey()" + StageChangeManager.GetStageChangeKey());
                    count++;
                    Debug.Log("count" + count);
                    m_Counter++;
                    if (!m_KeyWait_Flag)
                    {
                        m_KeyWait_Flag = !m_KeyWait_Flag;

                    }
                    if (m_Counter > m_InputStopFrame)
                    {
                        m_KeyWait_Flag = !m_KeyWait_Flag;
                        m_Counter = 0;
                    }

                    //Mixingカメラの初期化
                    ResetMixingCamera();

                    //Dollyカメラの初期化
                    ResetDollyCamera();

                    //DollyCartの初期化
                    ResetDollyCart();

                    //上下矢印の非アクティブ化
                    StageSelectArrow.TwoArrowsDeactivate();
                    //左右矢印の非アクティブ化
                    WorldSelectArrow.TwoArrowsDeactivate();


                    ////Dollyカメラの状態をFIXINGに戻す
                    //StageChangeManager.DollyStateChange("FIXING");

                    //ステージセレクトの状態を設定する
                    StageChangeManager.SelectStateChange("WORLD_MOVING");

                    //BGMのクロスフェード（仮実装）
                    m_SelectSound.CrossFade();
                    break;
                case SELECT_STATE.WORLD_MOVING:
                    //_Mixcam.MixingUpdate();
                    //_Main_DollyCam.DollyUpdate();
                    //_Sub_DollyCam.DollyUpdate();

                    //
                    //if (!m_WorldEndMixing_Flag && StageChangeManager.MixingState() == MixingCamera.MIXING_STATE.WORLD_END)
                    //{
                    //    m_WorldEndMixing_Flag = !m_WorldEndMixing_Flag;
                    //    ResetMixingCamera_WorldEnd();
                    //}

                    //移動完了後
                    if (StageChangeManager.IsWorldMoveEnd())
                    {
                        //m_WorldEndMixing_Flag = !m_WorldEndMixing_Flag;
                        // Debug.Log("StageChangeManager.DollyFlagCheck():" + StageChangeManager.DollyFlagCheck());

                        //各種状態を待機状態に戻す
                        //Dollyカメラの状態を設定する
                        StageChangeManager.DollyStateChange("FIXING");

                        //ステージセレクトの状態を設定する
                        StageChangeManager.SelectStateChange("KEY_WAIT");

                        //フラグをfalseにする
                        StageChangeManager.WorldFlagChange();
                        StageChangeManager.DollyCartFlagReset();
                        StageChangeManager.DollyFlagReset();


                        ////AutoDollyのチェックを外す
                        //_Main_DollyCam.AutoDollySwitch();
                        //_Sub_DollyCam.AutoDollySwitch();

                        //virtualカメラのFollowを解除する
                        //_Main_DollyCam.SetNoneFollower();
                        //_Sub_DollyCam.SetNoneFollower();

                        //Dollyカメラの座標をドリーカートの座標に合わせる
                        _Dolly_Next.transform.position = _DollyCart.GetDollyCartPosition();
                        _Dolly_Current.transform.position = _DollyCart.GetDollyCartPosition();

                        //固定用ドリールートをセット
                        _Main_DollyCam.SetPathFixingDolly();
                        _Sub_DollyCam.SetPathFixingDolly();
                        //_DollyCart.SetPathFixingDolly();
                        _Main_DollyCam.sss();
                        //_Sub_DollyCam.PathPositionReset();
                        //ステージ番号の更新
                        if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.LEFT)
                        {
                            StageStatusManager.Instance.CurrentStage = (STAGE_NO)(StageStatusManager.Instance.PrevWorld * 5);
                        }
                        else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.RIGHT)
                        {
                            StageStatusManager.Instance.CurrentStage = (STAGE_NO)(StageStatusManager.Instance.NextWorld * 5);
                        }
                        //LookAtを一か所に固定
                        _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.CurrentStage);
                        Debug.Log("WORLD_MOVINGおしまい");

                    }

                    //WorldChangeManagr.Update();
                    break;


                case SELECT_STATE.SCENE_MOVING:
                    break;
                case SELECT_STATE.STATE_NUM:
                    break;
                default:
                    break;
            }//switch (StageChangeManager.GetSelectState()) END
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
            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtunCode.B))
            //if (Input.GetKeyDown(KeyCode.Space))
            {
                string StageName = StageStatusManager.Instance.StageString[(int)StageStatusManager.Instance.CurrentStage];
                FadeManager.FadeOut(StageName, StageIn_FadeOut_Time);

                //BGMのフェードアウト
                BGMManager.Instance.FadeOut(StageIn_FadeOut_Time);

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

                //フラグをOFFにする
                m_KeyWait_Flag = !m_KeyWait_Flag;
            }
            else if (StageChangeManager.IsWorldChange())
            //else if (WorldChangeManagr.IsWorldChange())
            {
                //select_state = SELECT_STATE.WORLD_MOVING;
                StageChangeManager.SelectStateChange("BEFORE_WORLD_MOVING");

                //フラグをOFFにする
                m_KeyWait_Flag = !m_KeyWait_Flag;
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
                _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, (STAGE_NO)(StageStatusManager.Instance.PrevWorld * 5));
                //_Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, _TargetObj);
                StageChangeManager.MixingStateChange("WORLD");
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.RIGHT)
            {
                _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, (STAGE_NO)(StageStatusManager.Instance.NextWorld * 5));
                //_Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, _TargetObj);
                StageChangeManager.MixingStateChange("WORLD");
            }

            _Mixcam.ResetWeight();
            _Mixcam.SwingFlagOn();
            _Mixcam.SetSwingTime();

        }
        //Mixingカメラの初期化(MixingカメラがWORLD_ENDの時)
        public void ResetMixingCamera_WorldEnd()
        {
            STAGE_NO _StageNo;
            switch (StageChangeManager.GetStageChangeKey())
            {
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    _StageNo = (STAGE_NO)(StageStatusManager.Instance.PrevWorld * 5);
                    //  _Mixcam.LookAtTargetTwoChanges(_TargetObj, _StageNo);
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    _StageNo = (STAGE_NO)(StageStatusManager.Instance.NextWorld * 5);
                    //  _Mixcam.LookAtTargetTwoChanges(_TargetObj, _StageNo);
                    break;

                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                default:
                    Debug.Log("STAGE_CHANGE_KEYの状態が違います");
                    break;
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
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            {
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.LEFT)
            {
                //Followの設定（ドリーカートをセット）
                //_Main_DollyCam.SetFollower(_DollyCartObj);
                //_Sub_DollyCam.SetFollower(_DollyCartObj);

                //LookAtの設定（先導用オブジェクトをセット）
                //_Main_DollyCam.SetLookAtTarget(_TargetObj);
                //_Sub_DollyCam.SetLookAtTarget(_TargetObj);

                //AudoDollyをONにする
                //_Main_DollyCam.AutoDollySwitch();
                //_Sub_DollyCam.AutoDollySwitch();
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.RIGHT)
            {
                //Followの設定（ドリーカートをセット）
                //_Main_DollyCam.SetFollower(_DollyCartObj);
                //_Sub_DollyCam.SetFollower(_DollyCartObj);
                //LookAtの設定（先導用オブジェクトをセット）
                //_Main_DollyCam.SetLookAtTarget(_TargetObj);
                //_Sub_DollyCam.SetLookAtTarget(_TargetObj);

                //AudoDollyをONにする
                //_Main_DollyCam.AutoDollySwitch();
                //_Sub_DollyCam.AutoDollySwitch();
            }
            //ドリールートのセット
            _Main_DollyCam.SetDollyPath();
            _Sub_DollyCam.SetDollyPath();
            //ドリールートの加算倍率の変更
            _Main_DollyCam.SetAddTime();
            _Sub_DollyCam.SetAddTime();
            //PathPositionの両端(Min,Max)をセット
            _Main_DollyCam.SetPathPositionALL();
            _Sub_DollyCam.SetPathPositionALL();
            //カメラのパス位置(m_Path)を初期化する
            _Sub_DollyCam.PathPositionReset();
            _Main_DollyCam.PathPositionReset();


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
            _DollyCart.SetPathPositionALL();
            _DollyCart.PathPositionReset();

        }


    }//public class StageSelect : MonoBehaviour END
}//namespace END