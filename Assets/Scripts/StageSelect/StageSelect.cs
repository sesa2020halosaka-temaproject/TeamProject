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

        public float m_Counter = 0;//
        public float m_InputStopFrame;//ステージやワールドの移動後のキー入力を待たせるフラグ
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
        //private CurrentToNextWorldUIManager m_ToNextWName;
        //private WorldStatusUIManager m_WorldStatus;
        private StageSelectUIManager m_StageSelectUIManager;

        private WorldSelectHold m_Hold;


        //=================================================================
        //関数ここから
        //=================================================================
        private void Awake()
        {
            m_SelectSound = this.GetComponent<StageSelectSound>();
            StageChangeManager.GetComponentWorldSelectHold();

            m_StageSelectUIManager = this.GetComponent<StageSelectUIManager>();
            //m_ToNextWName = GameObject.Find("CurrentToNextWorld").GetComponent<CurrentToNextWorldUIManager>();
            //m_WorldStatus = GameObject.Find("WorldStatus").GetComponent<WorldStatusUIManager>();
           m_Hold = GameObject.Find("WorldMoveArrows").GetComponent<WorldSelectHold>();
    }
    // Start is called before the first frame update
    void Start()
        {
            //フェードイン
            FadeManager.FadeIn(1.3f);
            Debug.Log("Debugカウント：" + db_cnt);
            db_cnt++;


            //BGMスタート
            BGMManager.Instance.Stop();
            SEManager.Instance.Stop();
            m_SelectSound.StageSelectStartBGM();


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


            //上下矢印の処理
            StageSelectArrow.SetCurrentStage();
            //左右矢印の処理
            WorldSelectArrow.SetCurrentWorld();


            StageChangeManager.SelectStateChange("KEY_WAIT");
        }//void Start() END

        // Update is called once per frame
        void Update()
        {
            select_state = StageChangeManager.GetSelectState();
            switch (StageChangeManager.GetSelectState())
            {
                case SELECT_STATE.KEY_WAIT:
                    StateKeyWait();
                    break;
                case SELECT_STATE.BEFORE_STAGE_MOVING:
                    StateBeforeStageMoving();
                    break;
                case SELECT_STATE.STAGE_MOVING:
                    StateStageMoving();
                    break;
                case SELECT_STATE.BEFORE_WORLD_MOVING:
                    StateBeforeWorldMoving();
                    break;
                case SELECT_STATE.WORLD_MOVING:
                    StateWorldMoving();
                    break;


                case SELECT_STATE.SCENE_MOVING:
                    StateSceneMoving();
                    break;
                case SELECT_STATE.STATE_NUM:
                    break;
                default:
                    break;
            }//switch (StageChangeManager.GetSelectState()) END
        }//void Update()    END

        //---------------------------------------------------
        //Update()内の関数ここから

        //キー入力待ち
        private void StateKeyWait()
        {
            if (!KeyWaitFlagCheck())
            {
                //上下左右の入力がないときにカウントアップする
                if (   !InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.UpArrow)
                    && !InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.DownArrow)
                    && !InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.LeftArrow)
                    && !InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.RightArrow)
                    )
                {
                    m_Counter += Time.deltaTime;

                }

                //m_InputStopFrameの分だけ待たせる
                if (m_Counter > m_InputStopFrame)
                {
                    //フラグをONにする
                    KeyWaitFlagChange(true);
                    m_Counter = 0;

                }
            }
            //フラグがONになったら入力可能にする
            else if (KeyWaitFlagCheck())
            {

                //ステージ選択（WSキー or スティック上下）
                //StageChange();
                StageChangeManager.StageChange();
                //ワールド選択（ADキー or スティック左右）
                StageChangeManager.WorldChange();
                //WorldChangeManagr.WorldChange();

                //
                m_Hold.WorldSelectUpdate();
                //決定（Space  or Bボタン）
                StageDecision();
                //タイトルへ戻る(ESCキー or Startボタン)
                BackToTitle();
                //フラグチェック
                FlagCheck();

                //上下矢印の処理
                StageSelectArrow.SetCurrentStage();
                //左右矢印の処理
                WorldSelectArrow.SetCurrentWorld();

            }

        }//StateKeyWait()   END

        //================================================================
        //ステージ間移動関数ここから

        //ステージ移動前の準備
        private void StateBeforeStageMoving()
        {
            count++;
            Debug.Log("count" + count);

            //固定用ドリールートをセット
            _Main_DollyCam.SetPathFixingDolly();
            _Sub_DollyCam.SetPathFixingDolly();
            //_DollyCart.SetPathFixingDolly();

            //Mixingカメラの初期化
            ResetMixingCamera();

            //Dollyカメラの初期化
            ResetDollyCamera();

            //DollyCartの初期化
            ResetDollyCart();

            ////Dollyカメラの状態をFIXINGに戻す
            //StageChangeManager.DollyStateChange("FIXING");


            //
            //上下矢印の非アクティブ化
            //StageSelectArrow.TwoArrowsSetting();
            //左右矢印の非アクティブ化
            //WorldSelectArrow.TwoArrowsDeactivate();
            //上下矢印を画面外へ移動させる
            m_StageSelectUIManager.GetStageSelectArrow().UIStateMoveOut();
            //左右矢印を画面外へ移動させる
            m_StageSelectUIManager.GetWorldSelectArrow().UIStateMoveOut();

            //UI移動状態を変更する
            m_StageSelectUIManager.GetUIBackGroundCurrentStageObject().UIMoveStateChange();

            //ステージ移動の状態へ移行
            StageChangeManager.SelectStateChange("STAGE_MOVING");
        }

        //ステージ移動中
        private void StateStageMoving()
        {
            //_Mixcam.MixingUpdate();
            //_Main_DollyCam.DollyUpdate();
            //_Sub_DollyCam.DollyUpdate();

            //ステージ移動完了後
            if (StageChangeManager.DollyFlagCheck())
            {
                MoveEndSetting();
                //ステージ番号の更新
                StageNumberUpdate();

                ////固定用ドリールートをセット
                //_Main_DollyCam.SetPathFixingDolly();
                //_Sub_DollyCam.SetPathFixingDolly();
                //_DollyCart.SetPathFixingDolly();

                //アイコンの差し替え
                StageSelectArrow.ChangeStageNameIcon();
                WorldSelectArrow.ChangeWorldNameIcon();

                //上下矢印を画面外へ移動させる
                m_StageSelectUIManager.GetStageSelectArrow().UIStateMoveIn();

                //左右矢印を画面内へ移動させる
                m_StageSelectUIManager.GetWorldSelectArrow().UIStateMoveIn();

                //LookAtを一か所に固定
                _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.CurrentStage);
                Debug.Log("STAGE_MOVING終わり");

            }

            //Dollyカメラの座標をドリーカートの座標に合わせる
            _Dolly_Next.transform.position = _DollyCart.GetDollyCartPosition();
            _Dolly_Current.transform.position = _DollyCart.GetDollyCartPosition();

            //StageChangeManager.Update();

        }
        //ステージ間移動関数ここまで
        //================================================================

        //================================================================
        //ワールド間移動関数ここから

        //ワールド移動前の準備
        private void StateBeforeWorldMoving()
        {
            Debug.Log("StageChangeManager.GetStageChangeKey()" + StageChangeManager.GetStageChangeKey());
            count++;
            Debug.Log("count" + count);

            //Mixingカメラの初期化
            ResetMixingCamera();

            //Dollyカメラの初期化
            ResetDollyCamera();

            //DollyCartの初期化
            ResetDollyCart();

            //上下矢印の非アクティブ化
            //StageSelectArrow.TwoArrowsDeactivate();
            //左右矢印の非アクティブ化
            //WorldSelectArrow.TwoArrowsDeactivate();

            //上下矢印を画面外へ移動させる
            m_StageSelectUIManager.GetStageSelectArrow().UIStateMoveOut();
            //左右矢印を画面外へ移動させる
            m_StageSelectUIManager.GetWorldSelectArrow().UIStateMoveOut();


            ////Dollyカメラの状態をFIXINGに戻す
            //StageChangeManager.DollyStateChange("FIXING");

            //ステージセレクトの状態を設定する
            StageChangeManager.SelectStateChange("WORLD_MOVING");

            //BGMのクロスフェード
            m_SelectSound.CrossFade();

            //ワールド移動中のUIの画像変更
            //m_StageSelectUIManager.ChangeWorldNameIcon();
            m_StageSelectUIManager.GetCurrentToNextWorldUIObject().ChangeWorldNameIcon();
            //m_WorldStatus.ChangeWorldNameIcon();

            //
            m_StageSelectUIManager.GetWorldStatusUIObject().UIStateMoveOut();
            m_StageSelectUIManager.GetCurrentToNextWorldUIObject().UIInMove();
            m_StageSelectUIManager.SkipButtonSetActive(true);
        }
        //ワールド移動中
        private void StateWorldMoving()
        {
            //ワールド間移動のスキップ
            SkipWorldMove();

            //_Mixcam.MixingUpdate();
            //_Main_DollyCam.DollyUpdate();
            //_Sub_DollyCam.DollyUpdate();

            //
            //if (!m_WorldEndMixing_Flag && StageChangeManager.MixingState() == MixingCamera.MIXING_STATE.WORLD_END)
            //{
            //    m_WorldEndMixing_Flag = !m_WorldEndMixing_Flag;
            //    ResetMixingCamera_WorldEnd();
            //}

            //ワールド移動完了後
            if (StageChangeManager.IsWorldMoveEnd())
            {
                MoveEndSetting();

                //Dollyカメラの座標をドリーカートの座標に合わせる
                _Dolly_Next.transform.position = _DollyCart.GetDollyCartPosition();
                _Dolly_Current.transform.position = _DollyCart.GetDollyCartPosition();

                //固定用ドリールートをセット
                _Main_DollyCam.SetPathFixingDolly();
                _Sub_DollyCam.SetPathFixingDolly();
                //_DollyCart.SetPathFixingDolly();

                //_Sub_DollyCam.PathPositionReset();

                //UIの更新ステージナンバーの更新前用
                m_StageSelectUIManager.GetWorldStatusUIObject().ChangeWorldNameIcon();

                m_StageSelectUIManager.GetWorldStatusUIObject().UIStateMoveIn();
                m_StageSelectUIManager.GetCurrentToNextWorldUIObject().UIOutMove();
                m_StageSelectUIManager.SkipButtonSetActive(false);

                //最後に処理させること（分かりやすくするために）
                //ステージ番号の更新
                StageNumberUpdate();

                //UIの更新ステージナンバーの更新後用
                m_StageSelectUIManager.GetWorldStatusUIObject().SetMinionCount();
                m_StageSelectUIManager.GetWorldStatusUIObject().SetMinionMaxCount();
                
                m_StageSelectUIManager.GetWorldStatusUIObject().StageStarUpdate();
                m_StageSelectUIManager.GetUIBackGroundCurrentStageObject().SetStartPosition();

                //アイコンの差し替え
                StageSelectArrow.ChangeStageNameIcon();
                WorldSelectArrow.ChangeWorldNameIcon();

                //上下矢印を画面外へ移動させる
                m_StageSelectUIManager.GetStageSelectArrow().UIStateMoveIn();
                //左右矢印を画面内へ移動させる
                m_StageSelectUIManager.GetWorldSelectArrow().UIStateMoveIn();


                //LookAtを一か所に固定
                _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.CurrentStage);
                Debug.Log("WORLD_MOVING");
            }

            //Dollyカメラの座標をドリーカートの座標に合わせる
            _Dolly_Next.transform.position = _DollyCart.GetDollyCartPosition();
            _Dolly_Current.transform.position = _DollyCart.GetDollyCartPosition();


            //WorldChangeManagr.Update();

        }
        //ワールド間移動関数ここまで
        //================================================================

        //シーン遷移中
        private void StateSceneMoving()
        {

        }
        //Update()内の関数ここまで
        //---------------------------------------------------

        //ワールド間移動のスキップ
        private void SkipWorldMove()
        {
            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtunCode.Menu))
            {//STARTボタン or Pキー入力
                _Main_DollyCam.SetEndPathPosition();
                _Sub_DollyCam.SetEndPathPosition();

                //各種状態を待機状態に戻す
                //Dollyカメラの状態を設定する
                StageChangeManager.DollyStateChange("FIXING");
                StageChangeManager.DollyCartStateChange("FIXING");

                //ステージセレクトの状態を設定する
                StageChangeManager.SelectStateChange("KEY_WAIT");

                //フラグをfalseにする
                StageChangeManager.WorldFlagChange(false);
                StageChangeManager.DollyCartFlagChange(false);
                StageChangeManager.DollyFlagReset();

                //Dollyカメラの座標をドリーカートの座標に合わせる
                _Dolly_Next.transform.position = _DollyCart.GetDollyCartPosition();
                _Dolly_Current.transform.position = _DollyCart.GetDollyCartPosition();

                //固定用ドリールートをセット
                _Main_DollyCam.SetPathFixingDolly();
                _Sub_DollyCam.SetPathFixingDolly();
                //_DollyCart.SetPathFixingDolly();

                //_Sub_DollyCam.PathPositionReset();

                //---------------------
                //ステージ番号の更新
                //---------------------
                StageNumberUpdate();

                //LookAtを一か所に固定
                _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.CurrentStage);
                Debug.Log("WORLD_MOVINGスキップ");

                FadeManager.FadeOut("StageSelectScene", ToTitle_FadeOut_Time);
            }

        }

        //ステージ番号の更新
        void StageNumberUpdate()
        {
            switch (StageChangeManager.GetStageChangeKey())
            {
                case StageChangeManager.STAGE_CHANGE_KEY.UP:
                    StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.NextStage;
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.DOWN:
                    StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.PrevStage;
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    StageStatusManager.Instance.CurrentStage = (STAGE_NO)(StageStatusManager.Instance.PrevWorld * 5);
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    StageStatusManager.Instance.CurrentStage = (STAGE_NO)(StageStatusManager.Instance.NextWorld * 5);
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                    break;
                default:
                    break;
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
            {//STARTボタン or Pキー入力
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
                KeyWaitFlagChange(false);
            }
            else if (StageChangeManager.IsWorldChange())
            //else if (WorldChangeManagr.IsWorldChange())
            {
                //select_state = SELECT_STATE.WORLD_MOVING;
                StageChangeManager.SelectStateChange("BEFORE_WORLD_MOVING");

                //フラグをOFFにする
                KeyWaitFlagChange(false);
            }

        }
        void KeyWaitFlagChange(bool _bool)
        {
            m_KeyWait_Flag = _bool;
        }
       public bool KeyWaitFlagCheck()
        {
            return m_KeyWait_Flag;
        }
        //================================================
        void MoveEndSetting()
        {
            //m_WorldEndMixing_Flag = !m_WorldEndMixing_Flag;
            //Debug.Log("StageChangeManager.DollyFlagCheck():" + StageChangeManager.DollyFlagCheck());
            // Debug.Log("StageChangeManager.DollyFlagCheck():" + StageChangeManager.DollyFlagCheck());

            //各種状態を待機状態に戻す
            //Dollyカメラの状態を設定する
            StageChangeManager.DollyStateChange("FIXING");
            StageChangeManager.DollyCartStateChange("FIXING");

            //ステージセレクトの状態を設定する
            StageChangeManager.SelectStateChange("KEY_WAIT");

            //フラグをfalseにする
            StageChangeManager.StageFlagChange(false);
            StageChangeManager.WorldFlagChange(false);
            StageChangeManager.DollyCartFlagChange(false);
            StageChangeManager.DollyFlagReset();



        }

        //================================================
        //Mixingカメラの初期化
        public void ResetMixingCamera()
        {
            switch (StageChangeManager.GetStageChangeKey())
            {
                case StageChangeManager.STAGE_CHANGE_KEY.UP:
                    _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.NextStage);
                    StageChangeManager.MixingStateChange("GO");
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.DOWN:
                    _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.PrevStage);
                    StageChangeManager.MixingStateChange("BACK");
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, (STAGE_NO)(StageStatusManager.Instance.PrevWorld * 5));
                    //_Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, _TargetObj);
                    StageChangeManager.MixingStateChange("WORLD");
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    _Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, (STAGE_NO)(StageStatusManager.Instance.NextWorld * 5));
                    //_Mixcam.LookAtTargetTwoChanges(StageStatusManager.Instance.CurrentStage, _TargetObj);
                    StageChangeManager.MixingStateChange("WORLD");
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                    break;
                default:
                    break;
            }

            _Mixcam.ResetWeight();
            _Mixcam.SwingFlagOn();
            _Mixcam.SetSwingTime();

        }//ResetMixingCamera() END

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
            //if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.UP)
            //{
            //    Debug.Log("UP：処理が間違っています。");
            //    return;
            //}
            //else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            //{
            //    Debug.Log("DOWN：処理が間違っています。");
            //    return;
            //}
            //else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.LEFT)
            //{
            //    //StageChangeManager.DollyStateChange("WORLD");
            //}
            //else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.RIGHT)
            //{
            //    //StageChangeManager.DollyStateChange("WORLD");
            //}

            _DollyCart.SetDollyPath();
            //_DollyCart.SetAddTime();
            _DollyCart.SetPathPositionALL();
            _DollyCart.PathPositionReset();

        }


    }//public class StageSelect : MonoBehaviour END
}//namespace END