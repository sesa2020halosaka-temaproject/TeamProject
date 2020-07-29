using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
//using UnityEngine.SceneManagement;

namespace TeamProject
{
    public class StageSelect : MonoBehaviour
    {
        [Header("ステージ移動制限解除用フラグ(チェック外し忘れに注意)")]
        public bool m_UnlimitedMove_flag;
        [Header("シーン開始時のフェードイン時間"), Range(0.0f, 10.0f)]
        public float Start_FadeIn_Time;//タイトルに遷移する時のフェードアウト時間

        [Header("タイトル遷移時のフェードアウト時間"), Range(0.0f, 10.0f)]
        public float ToTitle_FadeOut_Time;//タイトルに遷移する時のフェードアウト時間

        [Header("ステージ遷移時のフェードアウト時間"), Range(0.0f, 10.0f)]
        public float StageIn_FadeOut_Time;//ステージに遷移する時のフェードアウト時間

        [Header("ステージ遷移時のズーム時間"), Range(0.0f, 20.0f)]
        public float StageIn_Zoom_Time;//時間

        [Header("開始FOV"), Range(1.0f, 90.0f)]
        public float m_StartFOV;

        [Header("ステージ遷移時の最終FOV"), Range(1.0f, 90.0f)]
        public float m_EndFOV;

        [Header("ステージ遷移時のレンズの歪ませ時間"), Range(0.0f, 20.0f)]
        public float LensDistortion_Time;//時間

        [Header("開始時のレンズの歪み強度"), Range(-1.0f, 1.0f)]
        public float m_StartIntensity;

        [Header("ステージ遷移時の最終レンズの歪み強度"), Range(-1.0f, 1.0f)]
        public float m_EndIntensity;

        //public GameObject[] Stages;//LookAtの対象となるゲームオブジェクトの格納用
        public WayPoint_Box m_WP;//ドリールートのパス位置格納用
        public float m_CurrentPathPosition;//現在のパス位置を渡す用

        public DollyTrack_Box m_DoTr;//ドリールートの格納用

        public GameObject _Dolly_Next;

        private DollyCamera _Main_DollyCam;

        public StageSelectArrow _StageSelectArrow;

        public float m_Counter = 0;//
        public float m_InputStopFrame;//ステージやワールドの移動後のキー入力を待たせるフラグ
        public bool m_KeyWait_Flag = false;

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
        public SELECT_STATE select_state;//現在の状態遷移の確認用

        private int db_cnt = 0;//デバッグログ確認用カウント

        private StageSelectSound m_SelectSound;
        private StageSelectUIManager m_StageSelectUIManager;

        private WorldSelectHold m_Hold;

        public GameObject m_TargetObj;
        public LookAtObject m_LookAt;

        private LensDistortionManager m_LensDistortionManager;
        private bool m_StageInUIMoveFlag = false;

        //カメラの角度保持用
        private Quaternion m_PrevFrameCamRot;       //前のフレームの角度
        private Quaternion m_CurrentFrameCamRot;    //現在のフレームの角度
        private bool m_CamRotCheckFlag = false;     //前のフレームと一致したかどうかの確認用フラグ

        //=================================================================
        //関数ここから
        //=================================================================
        private void Awake()
        {
            //================================================
            //コンポーネントの取得
            //================================================
            //StageSelectSoundコンポーネント取得
            m_SelectSound = this.GetComponent<StageSelectSound>();

            StageChangeManager.GetComponentWorldSelectHold();

            //StageSelectUIManagerコンポーネント取得
            m_StageSelectUIManager = this.GetComponent<StageSelectUIManager>();

            //WorldSelectHoldコンポーネント取得
            m_Hold = GameObject.Find("WorldMoveArrows").GetComponent<WorldSelectHold>();

            //デバッグ用としてfalseの時だけInspector上のフラグをセットする
            if (StageStatusManager.Instance.m_RemovalLimitFlag == false)
            {
                //移動制限を解除するかどうか。
                StageStatusManager.Instance.m_RemovalLimitFlag = m_UnlimitedMove_flag;
            }
        }
        // Start is called before the first frame update
        void Start()
        {

            //画面を真っ暗にする
            FadeManager.BlackOut();
            //FadeManager.FadeIn(Start_FadeIn_Time);
            Debug.Log("Debugカウント：" + db_cnt);
            db_cnt++;


            //BGMスタート
            BGMManager.Instance.Stop();
            SEManager.Instance.Stop();
            //m_SelectSound.StageSelectStartBGM();

            //WayPoint用ゲームオブジェクト取得
            m_WP = GameObject.Find("WayPoint_Box").GetComponent<WayPoint_Box>();
            //WayPointの初期化
            WayPointInit();

            //================================================
            //ゲームオブジェクトの取得
            //================================================
            //DollyCameraのオブジェクト取得
            _Dolly_Next = GameObject.Find("Dolly_VCam");


            //Debug.Log("0：メインカメラの角度:" + UnityEngine.Camera.main.transform.rotation);

            //カメラの注視点用ゲームオブジェクト取得
            m_TargetObj = GameObject.Find("Bellwether").gameObject;
            //================================================
            //コンポーネントの取得
            //================================================
            //DollyTrackコンポーネント取得
            m_DoTr = GameObject.Find("DollyTrack_Obj").GetComponent<DollyTrack_Box>();
 
            //DollyCameraコンポーネントの取得
            _Main_DollyCam = _Dolly_Next.GetComponent<DollyCamera>();

            //LookAtObjectコンポーネントの取得
            m_LookAt = m_TargetObj.GetComponent<LookAtObject>();

            //LensDistortionManagerコンポーネントの取得
            m_LensDistortionManager = GameObject.Find("LensDistortionObj").GetComponent<LensDistortionManager>();

            //強度の初期設定
            m_LensDistortionManager.SetIntensity(m_StartIntensity);

            //シーン開始時点のステージから初期化
            //ドリールートの設定
            //LookAt・注視点の設定
            _Main_DollyCam.SetLookAtTarget(m_TargetObj);

            //Dollyカメラの座標をドリールートの座標に合わせる
            _Main_DollyCam.SetStartDollyPos();

            //固定用ドリールートのセット
            _Main_DollyCam.SetPathFixingDolly();

            //各ドリーカメラにパス位置をセット
            _Main_DollyCam.SetPathPosition(m_CurrentPathPosition);

            //開始画角のセット
            _Main_DollyCam.SetFOV(m_StartFOV);

            //現在のステージ位置でのアクティブ化処理
            m_StageSelectUIManager.GetStageSelectArrow().UIActivateFromCurrentStage();

            //左右矢印の処理
            //現在のワールド位置での移動制限とアクティブ化処理
            m_StageSelectUIManager.GetWorldSelectArrow().SettingWorldMove();


            //ステージセレクトの状態を設定する
            StageChangeManager.SelectStateChange("KEY_WAIT");
        }//void Start() END

        // Update is called once per frame
        void Update()
        {
            StageSelectUpdate();
            _Main_DollyCam.DollyUpdate();
            m_LookAt.LookAtObjectUpdate();
            m_StageSelectUIManager.StageSelectUIUpdate();
        }//void Update()    END

        //ステージセレクトの更新処理
        public void StageSelectUpdate()
        {

            if (!m_CamRotCheckFlag)
            {
                //カメラの角度フラグがfalseの時は
                //画面真っ暗状態で待つ。
                CameraRotCheckAtSceneStart();
            }
            else
            {
                //カメラの角度フラグがtrueになったら
                //ステージセレクト処理開始
                select_state = StageChangeManager.GetSelectState();
                switch (StageChangeManager.GetSelectState())
                {
                    case SELECT_STATE.KEY_WAIT:
                        StateKeyWait();
                        break;
                    case SELECT_STATE.BEFORE_STAGE_MOVING:
                        StateBeforeStageMoving();
                        //Debug.LogError("0：メインカメラ座標:" + UnityEngine.Camera.main.transform.position);

                        break;
                    case SELECT_STATE.STAGE_MOVING:
                        //Debug.LogError("0：メインカメラ座標:" + UnityEngine.Camera.main.transform.position);

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
            }//if (!m_CamRotCheckFlag) else END

        }

        //---------------------------------------------------
        //Update()内の関数ここから

        //キー入力待ち状態の処理
        private void StateKeyWait()
        {
            //フラグがfalseの時はカウントアップ待ち
            if (!KeyWaitFlagCheck())
            {
                //上下左右の入力がないときにカウントアップする→変更
                //上下のみの入力がないときにカウントアップする
                if (!InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCode.UpArrow)
                    && !InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCode.DownArrow)
                    //&& !InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCode.LeftArrow)
                    //&& !InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCode.RightArrow)
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
            else if (KeyWaitFlagCheck())
            {
                //フラグがONになったら入力可能にする
                //ステージ選択（WSキー or スティック上下）
                StageChangeManager.StageChange();
                //ワールド選択（ADキー or スティック左右）
                StageChangeManager.WorldChange();
                //WorldChangeManagr.WorldChange();

                //左右のワールド移動UIの更新処理
                m_Hold.WorldSelectUpdate();
                //決定処理チェック（Enterキー  or Aボタン）
                StageDecision();
                //タイトルへ戻る(ESCキー or Bボタン)
                BackToTitle();
                //フラグチェック
                FlagCheck();

                //上下矢印の処理
                //上下矢印のアクティブ化と非強調化
                m_StageSelectUIManager.GetStageSelectArrow().UINoFlashing();
                //ステージクリア状況での処理
                //現在のステージ位置でのアクティブ化処理
                m_StageSelectUIManager.GetStageSelectArrow().UIActivateFromStage();

                //左右矢印の処理
                //現在のワールド位置での移動制限とアクティブ化処理
                m_StageSelectUIManager.GetWorldSelectArrow().SettingWorldMove();

            }
            //入力デバイスに応じたUIへの表示切り替え
            m_StageSelectUIManager.SwitchingUISprite();


        }//StateKeyWait()   END

        //================================================================
        //ステージ間移動関数ここから

        //ステージ移動前の準備
        private void StateBeforeStageMoving()
        {
            count++;
            Debug.Log("count" + count);
            //Dollyカメラの座標をドリールートの座標に合わせる
            //_Main_DollyCam.SetStartDollyPos();

            //固定用ドリールートをセット
            _Main_DollyCam.SetPathFixingDolly();
            _Main_DollyCam.SetPathPosition(0.0f);

            //Dollyカメラの初期化
            ResetDollyCamera();

            //
            m_LookAt.ChangeState();
            if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            {
                m_LookAt.ChangeDollyState();
            }
            //
            //上下矢印の非アクティブ化
            //StageSelectArrow.TwoArrowsSetting();
            //左右矢印の非アクティブ化
            //WorldSelectArrow.TwoArrowsDeactivate();

            //上下矢印を強調させる
            m_StageSelectUIManager.GetStageSelectArrow().UISetFlashing();
            //左右矢印を画面外へ移動させる
            m_StageSelectUIManager.GetWorldSelectArrow().UIStateMoveOut();

            //UI移動状態を変更する
            m_StageSelectUIManager.GetUIBackGroundCurrentStageObject().UIMoveStateChange();
            m_StageSelectUIManager.GetUIBG_ArrowObject().UIMoveStateChange();

            //ステージ移動の状態へ移行
            StageChangeManager.SelectStateChange("STAGE_MOVING");

        }

        //ステージ移動中
        private void StateStageMoving()
        {

            //ステージ移動完了後
            if (StageChangeManager.DollyFlagCheck())
            {
                MoveEndSetting();
                //ステージ番号の更新
                StageNumberUpdate();
                //Dollyカメラの座標をドリールートの座標に合わせる
                _Main_DollyCam.SetStartDollyPos();


                //アイコンの差し替え
                //StageSelectArrow.ChangeStageNameIcon();
                WorldSelectArrow.ChangeWorldNameIcon();

                ////上下矢印を画面外へ移動させる
                //m_StageSelectUIManager.GetStageSelectArrow().UIStateMoveIn();

                //左右矢印を画面内へ移動させる
                m_StageSelectUIManager.GetWorldSelectArrow().UIStateMoveIn();

                Debug.Log("STAGE_MOVING終わり");

            }
            //StageChangeManager.Update();

        }
        //ステージ間移動関数ここまで
        //================================================================

        //================================================================
        //ワールド間移動関数ここから

        //ワールド移動前の準備
        private void StateBeforeWorldMoving()
        {
            //固定用ドリールートをセット
            _Main_DollyCam.SetPathFixingDolly();
            _Main_DollyCam.SetPathPosition(0.0f);

            //Dollyカメラの初期化
            ResetDollyCamera();

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

            m_LookAt.ChangeState();


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
            m_StageSelectUIManager.SkipButtonActivate();
        }
        //ワールド移動中
        private void StateWorldMoving()
        {
            //ワールド間移動のスキップ
            SkipWorldMove();

            //ワールド移動完了後
            if (StageChangeManager.IsWorldMoveEnd())
            {

                //移動処理の終了の為の初期化設定
                MoveEndSetting();

                //UIの更新ステージナンバーの更新前用
                m_StageSelectUIManager.GetWorldStatusUIObject().ChangeWorldNameIcon();

                m_StageSelectUIManager.GetWorldStatusUIObject().UIStateMoveIn();
                m_StageSelectUIManager.GetCurrentToNextWorldUIObject().UIOutMove();
                m_StageSelectUIManager.ABButtonActivate();

                //最後に処理させること（分かりやすくするために）
                //ステージ番号の更新
                StageNumberUpdate();

                //Dollyカメラの座標をドリールートの座標に合わせる
                _Main_DollyCam.SetStartDollyPos();
                //固定用ドリールートをセット
                _Main_DollyCam.SetPathFixingDolly();

                //ドリールートのパス位置の最大最小を０に設定する
                _Main_DollyCam.SetPathPosition(0.0f);

                //ドリールートのパス位置の最大最小を０に設定する
                //到着時上入力時のカメラのブレを軽減させるため
                _Main_DollyCam.SetPathPositionMax(0.0f);
                _Main_DollyCam.SetPathPositionMin(0.0f);

                //UIの更新ステージナンバーの更新後用
                m_StageSelectUIManager.GetWorldStatusUIObject().SetMinionCount();
                m_StageSelectUIManager.GetWorldStatusUIObject().SetMinionMaxCount();

                m_StageSelectUIManager.GetWorldStatusUIObject().StageStarUpdate();
                m_StageSelectUIManager.GetUIBackGroundCurrentStageObject().SetStartPosition();
                m_StageSelectUIManager.GetUIBG_ArrowObject().SetStartPosition();

                //アイコンの差し替え
                //StageSelectArrow.ChangeStageNameIcon();
                WorldSelectArrow.ChangeWorldNameIcon();

                //上下矢印を画面外へ移動させる
                m_StageSelectUIManager.GetStageSelectArrow().UIStateMoveIn();
                //左右矢印を画面内へ移動させる
                m_StageSelectUIManager.GetWorldSelectArrow().UIStateMoveIn();

                Debug.Log("WORLD_MOVING");

            }

            //WorldChangeManagr.Update();

        }
        //ワールド間移動関数ここまで
        //================================================================

        //シーン遷移中
        private void StateSceneMoving()
        {
            if (!m_StageInUIMoveFlag)
            {
                m_StageInUIMoveFlag = true;
                //上下矢印を画面外へ移動させる
                m_StageSelectUIManager.GetStageSelectArrow().UIStateMoveOut();
                //左右矢印を画面外へ移動させる
                m_StageSelectUIManager.GetWorldSelectArrow().UIStateMoveOut();
                //ステージ情報UIを画面外移動させる
                m_StageSelectUIManager.GetWorldStatusUIObject().UIStateMoveOut();
            }

            //ステージにズームする処理
            _Main_DollyCam.CameraZoom(m_StartFOV, m_EndFOV, StageIn_Zoom_Time);
            //画面をLensDistortionする（レンズの歪み）処理
            m_LensDistortionManager.IntensitySlerp(m_StartIntensity, m_EndIntensity, LensDistortion_Time);

        }
        //Update()内の関数ここまで
        //---------------------------------------------------

        //ワールド間移動のスキップ
        private void SkipWorldMove()
        {
            //if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtonCode.Menu)||

            if (Input.GetKeyDown("joystick button 7") ||        //STARTボタン個別取得
                Input.GetKeyDown(KeyCode.X))
            {//STARTボタン or Xキー入力
                _Main_DollyCam.SetEndPathPosition();

                //各種状態を待機状態に戻す
                //Dollyカメラの状態を設定する
                StageChangeManager.DollyStateChange("FIXING");

                //ステージセレクトの状態を設定する
                StageChangeManager.SelectStateChange("KEY_WAIT");

                //フラグをfalseにする
                StageChangeManager.WorldFlagChange(false);
                StageChangeManager.DollyFlagReset();

                //固定用ドリールートをセット
                _Main_DollyCam.SetPathFixingDolly();
                _Main_DollyCam.SetPathPosition(0.0f);

                //---------------------
                //ステージ番号の更新
                //---------------------
                StageNumberUpdate();

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
            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtonCode.A))
            //if (Input.GetKeyDown(KeyCode.Space))
            {
                string StageName = StageStatusManager.Instance.StageString[(int)StageStatusManager.Instance.CurrentStage];
                //FadeManager.FadeOut("StageSelectScene", StageIn_FadeOut_Time);
                FadeManager.FadeOut(StageName, StageIn_FadeOut_Time);

                //BGMのフェードアウト
                BGMManager.Instance.FadeOut(StageIn_FadeOut_Time);

                //シーン遷移中状態にする
                StageChangeManager.SelectStateChange("SCENE_MOVING");
                Debug.Log("決定です！");
                //決定音鳴らす
                SEManager.Instance.Play(SEPath.SE_OK);
            }
        }


        private void BackToTitle()
        {
            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtonCode.B))
            //if (Input.GetKeyDown(KeyCode.Escape))
            {//Bボタン or ESCキー入力
                //FadeManager.FadeOut("StageSelectScene", ToTitle_FadeOut_Time);
                FadeManager.FadeOut("TitleScene", ToTitle_FadeOut_Time);
            }

        }

        //状態フラグのチェック
        private void FlagCheck()
        {
            if (StageChangeManager.IsStageChange())
            {
                StageChangeManager.SelectStateChange("BEFORE_STAGE_MOVING");

                //フラグをOFFにする
                KeyWaitFlagChange(false);
            }
            else if (StageChangeManager.IsWorldChange())
            //else if (WorldChangeManagr.IsWorldChange())
            {
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
            //各種状態を待機状態に戻す
            //Dollyカメラの状態を設定する
            StageChangeManager.DollyStateChange("FIXING");

            //ステージセレクトの状態を設定する
            StageChangeManager.SelectStateChange("KEY_WAIT");

            //フラグをfalseにする
            StageChangeManager.StageFlagChange(false);
            StageChangeManager.WorldFlagChange(false);

            StageChangeManager.DollyFlagReset();



        }

        //================================================
        //Dollyカメラの初期化
        public void ResetDollyCamera()
        {
            //ドリールートのセット
            _Main_DollyCam.SetDollyPath();

            //ドリールートの加算倍率の変更
            _Main_DollyCam.SetAddTime();

            //PathPositionの両端(Min,Max)をセット
            _Main_DollyCam.SetPathPositionALL();

            //カメラのパス位置(m_Path)を初期化する
            _Main_DollyCam.PathPositionReset();
        }
        //================================================


        //WayPointの初期化
        public void WayPointInit()
        {
            //現在のワールドにおけるWayPoint格納配列のセット
            m_WP.SetWayPoint();

            //開始時点のパス位置をセット
            m_CurrentPathPosition = m_WP.m_Stage_WayPoint[StageStatusManager.Instance.StageInWorld];

        }

        //=============================================
        //ステージセレクトの開始直後のカメラの処理
        private void CameraRotCheckAtSceneStart()
        {
            //カメラの角度フラグがfalseの時は
            //画面真っ暗状態で待つ。
            m_CurrentFrameCamRot = UnityEngine.Camera.main.transform.rotation;
            if (m_CurrentFrameCamRot == m_PrevFrameCamRot)
            {
                //カメラの角度が前フレームと一致している時
                m_CamRotCheckFlag = true;
                //フェードイン
                FadeManager.FadeIn(Start_FadeIn_Time);
                //BGMスタート
                m_SelectSound.StageSelectStartBGM();

            }
            else
            {
                //カメラの角度が前フレームと一致していない時
                //前フレームのカメラ角度に代入
                m_PrevFrameCamRot = UnityEngine.Camera.main.transform.rotation;
            }

        }
    }//public class StageSelect : MonoBehaviour END
}//namespace END