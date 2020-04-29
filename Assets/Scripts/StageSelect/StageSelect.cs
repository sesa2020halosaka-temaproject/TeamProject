﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
using UnityEngine.SceneManagement;

namespace TeamProject
{
    public class StageSelect : MonoBehaviour
    {
        public GameObject[] Stages;//LookAtの対象となるゲームオブジェクトの格納用
        public float[] WayPoint;//ステージの正面に位置するドリーのパスの位置を入れる用
        public float Volume;

        public GameObject _Dolly_Current;
        public GameObject _Dolly_Next;
        public GameObject _Mixing;
        public StageSelectArrow _StageSelectArrow;

        //ステージセレクトの状態
        public enum SELECT_STATE
        {
            KEY_WAIT = 0,//キー入力待ち
            SWING,       //次の目的の方向へを向いている途中
            MOVING,      //カメラ移動中
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


            _Mixing = GameObject.Find("Mixing_VCamera").gameObject;
            _Dolly_Current = _Mixing.transform.Find("Dolly_VCamera").gameObject;
            _Dolly_Next = _Mixing.transform.Find("Current_VCamera").gameObject;

            _Dolly_Current.GetComponent<DollyCamera>().LookAtTargetChange(Stages[(int)StageStatusManager.Instance.CurrentStage]);
            _Dolly_Next.GetComponent<DollyCamera>().LookAtTargetChange(Stages[(int)StageStatusManager.Instance.CurrentStage]);

            Debug.Log((int)StageStatusManager.Instance.CurrentStage);

            _StageSelectArrow = this.transform.Find("Panel/StageMoveArrows").GetComponent<StageSelectArrow>();
            _Dolly_Next.GetComponent<DollyCamera>().SetPathPosition(WayPoint[(int)StageStatusManager.Instance.CurrentStage]);
            Debug.Log((int)StageStatusManager.Instance.CurrentStage);
            _Dolly_Current.GetComponent<DollyCamera>().SetPathPosition(WayPoint[(int)StageStatusManager.Instance.CurrentStage]);
        }

        // Update is called once per frame
        void Update()
        {
            switch (select_state)
            {
                case SELECT_STATE.KEY_WAIT:

                    //ステージ選択（WSキー or スティック上下）
                    StageChange();
                    //ワールド選択（ADキー or スティック左右）
                    WorldChange();
                    //決定（Space (or Enter?) or Bボタン）
                    StageDecision();
                    //タイトルへ戻る(ESCキー or Startボタン)
                    BackToTitle();
                    break;
                case SELECT_STATE.SWING:
                    bool IsSwing = _Mixing.GetComponent<MixingCamera>().IsSwing();
                    Debug.Log(IsSwing);
                    if (!IsSwing)//方向転換完了したかどうか
                    {
                        _Mixing.GetComponent<MixingCamera>().MixState("ZERO");

                        //移動中にする
                        select_state = SELECT_STATE.MOVING;
                        //カメラ移動を始める
                        switch (dolly_state)
                        {
                            case DOLLY_STATE.GO:
                                DollyCameraGo(StageStatusManager.Instance.CurrentStage);
                                break;
                            case DOLLY_STATE.BACK:
                                DollyCameraBack(StageStatusManager.Instance.CurrentStage);
                                break;
                        }
                    }

                    break;

                case SELECT_STATE.MOVING:
                    bool IsMoving_2 = _Dolly_Current.GetComponent<DollyCamera>().IsMoving();
                    bool IsMoving = _Dolly_Next.GetComponent<DollyCamera>().IsMoving();
                    if (!IsMoving && !IsMoving_2)//移動完了したかどうか
                    {
                        //キー入力待ちに戻す
                        select_state = SELECT_STATE.KEY_WAIT;

                        //カメラ移動速度を0にする
                        _Dolly_Current.GetComponent<DollyCamera>().DollyState("ZERO");
                        _Dolly_Next.GetComponent<DollyCamera>().DollyState("ZERO");

                        //_Mixing.GetComponent<MixingCamera>().LookAtTargetTwoChanges(Stages[(int)stage_num], Stages[(int)StageStatusManager.Instance.CurrentStage]);

                    }
                    break;
                case SELECT_STATE.SCENE_MOVING:
                    break;
                case SELECT_STATE.STATE_NUM:
                    break;
                default:
                    break;
            }
        }//void Update()    END

        void StageChange()
        {
            switch (StageStatusManager.Instance.CurrentStage)
            {
                case STAGE_NO.STAGE01:
                    if (Input.GetKeyDown(KeyCode.W) || InputManager.InputManager.Instance.GetLStick().y > 0)
                    {//上入力
                        MixCameraGo(StageStatusManager.Instance.CurrentStage, STAGE_NO.STAGE02);
                        _StageSelectArrow.SetCurrentStage(STAGE_NO.STAGE02);
                    }
                    else if (Input.GetKeyDown(KeyCode.S) || InputManager.InputManager.Instance.GetLStick().y < 0)
                    {//下入力
                     //DollyCameraBack(STAGE_NO.STAGE02);
                    }
                    break;
                case STAGE_NO.STAGE02:
                    if (Input.GetKeyDown(KeyCode.W) || InputManager.InputManager.Instance.GetLStick().y > 0)
                    {//上入力
                     //DollyCameraGo(STAGE_NO.STAGE03);
                        MixCameraGo(StageStatusManager.Instance.CurrentStage, STAGE_NO.STAGE03);
                        _StageSelectArrow.SetCurrentStage(STAGE_NO.STAGE03);
                    }
                    else if (Input.GetKeyDown(KeyCode.S) || InputManager.InputManager.Instance.GetLStick().y < 0)
                    {//下入力
                     //DollyCameraBack(STAGE_NO.STAGE01);
                        MixCameraBack(StageStatusManager.Instance.CurrentStage, STAGE_NO.STAGE01);
                        _StageSelectArrow.SetCurrentStage(STAGE_NO.STAGE01);
                    }
                    break;
                case STAGE_NO.STAGE03:
                    if (Input.GetKeyDown(KeyCode.W) || InputManager.InputManager.Instance.GetLStick().y > 0)
                    {//上入力
                     //DollyCameraGo(STAGE_NO.STAGE04);
                        MixCameraGo(StageStatusManager.Instance.CurrentStage, STAGE_NO.STAGE04);
                        _StageSelectArrow.SetCurrentStage(STAGE_NO.STAGE04);
                    }
                    else if (Input.GetKeyDown(KeyCode.S) || InputManager.InputManager.Instance.GetLStick().y < 0)
                    {//下入力
                     //DollyCameraBack(STAGE_NO.STAGE02);
                        MixCameraBack(StageStatusManager.Instance.CurrentStage, STAGE_NO.STAGE02);
                        _StageSelectArrow.SetCurrentStage(STAGE_NO.STAGE02);
                    }
                    break;
                case STAGE_NO.STAGE04:
                    if (Input.GetKeyDown(KeyCode.W) || InputManager.InputManager.Instance.GetLStick().y > 0)
                    {//上入力
                     //DollyCameraGo(STAGE_NO.STAGE05);
                        MixCameraGo(StageStatusManager.Instance.CurrentStage, STAGE_NO.STAGE05);
                        _StageSelectArrow.SetCurrentStage(STAGE_NO.STAGE05);
                    }
                    else if (Input.GetKeyDown(KeyCode.S) || InputManager.InputManager.Instance.GetLStick().y < 0)
                    {//下入力
                     //DollyCameraBack(STAGE_NO.STAGE03);
                        MixCameraBack(StageStatusManager.Instance.CurrentStage, STAGE_NO.STAGE03);
                        _StageSelectArrow.SetCurrentStage(STAGE_NO.STAGE03);
                    }
                    break;
                case STAGE_NO.STAGE05:
                    if (Input.GetKeyDown(KeyCode.W) || InputManager.InputManager.Instance.GetLStick().y > 0)
                    {//上入力
                     //MixCameraGo(StageStatusManager.Instance.CurrentStage, STAGE_NO.STAGE01);
                     // DollyCameraGo(STAGE_NO.STAGE02);
                     //_StageSelectArrow.SetCurrentStage(STAGE_NO.STAGE01);
                    }
                    else if (Input.GetKeyDown(KeyCode.S) || InputManager.InputManager.Instance.GetLStick().y < 0)
                    {//下入力
                        MixCameraBack(StageStatusManager.Instance.CurrentStage, STAGE_NO.STAGE04);
                        //DollyCameraBack(STAGE_NO.STAGE04);
                        _StageSelectArrow.SetCurrentStage(STAGE_NO.STAGE04);
                    }
                    break;
                case STAGE_NO.STAGE06:
                    break;
                case STAGE_NO.STAGE07:
                    break;
                case STAGE_NO.STAGE08:
                    break;
                case STAGE_NO.STAGE09:
                    break;
                case STAGE_NO.STAGE10:
                    break;
                case STAGE_NO.STAGE11:
                    break;
                case STAGE_NO.STAGE12:
                    break;
                case STAGE_NO.STAGE13:
                    break;
                case STAGE_NO.STAGE14:
                    break;
                case STAGE_NO.STAGE15:
                    break;
                case STAGE_NO.STAGE16:
                    break;
                case STAGE_NO.STAGE17:
                    break;
                case STAGE_NO.STAGE18:
                    break;
                case STAGE_NO.STAGE19:
                    break;
                case STAGE_NO.STAGE20:
                    break;
                case STAGE_NO.STAGE_NUM:
                    break;
                default:
                    break;
            }

        }//    void StageChange()   END

        void WorldChange()
        {

        }

        //ステージ決定
        void StageDecision()
        {
            //カーソルの操作（決定）
            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtunCode.A))
            //if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (StageStatusManager.Instance.CurrentStage)
                {
                    case STAGE_NO.STAGE01:
                        FadeManager.FadeOut("Stage1_1");
                        break;
                    case STAGE_NO.STAGE02:
                        FadeManager.FadeOut("Stage1_2");
                        break;
                    case STAGE_NO.STAGE03:
                        FadeManager.FadeOut("Stage1_3");
                        break;
                    case STAGE_NO.STAGE04:
                        FadeManager.FadeOut("Stage1_4");
                        break;
                    case STAGE_NO.STAGE05:
                        FadeManager.FadeOut("Stage1_5");
                        break;
                    case STAGE_NO.STAGE06:
                        break;
                    case STAGE_NO.STAGE07:
                        break;
                    case STAGE_NO.STAGE08:
                        break;
                    case STAGE_NO.STAGE09:
                        break;
                    case STAGE_NO.STAGE10:
                        break;
                    case STAGE_NO.STAGE11:
                        break;
                    case STAGE_NO.STAGE12:
                        break;
                    case STAGE_NO.STAGE13:
                        break;
                    case STAGE_NO.STAGE14:
                        break;
                    case STAGE_NO.STAGE15:
                        break;
                    case STAGE_NO.STAGE16:
                        break;
                    case STAGE_NO.STAGE17:
                        break;
                    case STAGE_NO.STAGE18:
                        break;
                    case STAGE_NO.STAGE19:
                        break;
                    case STAGE_NO.STAGE20:
                        break;
                    case STAGE_NO.STAGE_NUM:
                        break;
                    default:
                        break;
                }
                //シーン遷移中状態にする
                select_state = SELECT_STATE.SCENE_MOVING;
                Debug.Log("決定です！");
                //決定音鳴らす
                SEManager.Instance.Play(SEPath.SE_OK);
            }
        }

        private void MixCameraGo(STAGE_NO CurrentStage, STAGE_NO NextStage)
        {
            StageStatusManager.Instance.CurrentStage = NextStage;

            SetMixCamera(_Mixing, CurrentStage, NextStage, "GO");


            dolly_state = DOLLY_STATE.GO;
            select_state = SELECT_STATE.SWING;
            //カーソルの移動音
            SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);

        }
        private void MixCameraBack(STAGE_NO CurrentStage, STAGE_NO NextStage)
        {
            StageStatusManager.Instance.CurrentStage = NextStage;

            SetMixCamera(_Mixing, CurrentStage, NextStage, "BACK");

            dolly_state = DOLLY_STATE.BACK;
            select_state = SELECT_STATE.SWING;
            //カーソルの移動音
            SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);

        }
        private void DollyCameraGo(STAGE_NO NextStage)
        {
            //stage_num = NextStage;

            SetDollyCamera(_Dolly_Current, NextStage, "GO");
            SetDollyCamera(_Dolly_Next, NextStage, "GO");

            select_state = SELECT_STATE.MOVING;

        }
        private void DollyCameraBack(STAGE_NO PrevStage)
        {
            //stage_num = PrevStage;

            SetDollyCamera(_Dolly_Current, PrevStage, "BACK");
            SetDollyCamera(_Dolly_Next, PrevStage, "BACK");

            select_state = SELECT_STATE.MOVING;
            //カーソルの移動音
            //   SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);

        }

        private void BackToTitle()
        {
            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtunCode.Menu))
            //if (Input.GetKeyDown(KeyCode.Escape))
            {//ESCキー入力
                FadeManager.FadeOut("TitleScene", 1.5f);
            }

        }


        //ドリーカメラのセット（カメラ注視点、パス位置、ドリーの状態）
        public void SetDollyCamera(GameObject _DollyCameraObject, STAGE_NO _StageNo, string _Word)
        {
            _DollyCameraObject.GetComponent<DollyCamera>().LookAtTargetChange(Stages[(int)_StageNo]);
            _DollyCameraObject.GetComponent<DollyCamera>().SetPathPositionMax(WayPoint[(int)_StageNo]);
            _DollyCameraObject.GetComponent<DollyCamera>().SetPathPositionMin(WayPoint[(int)_StageNo]);
            _DollyCameraObject.GetComponent<DollyCamera>().DollyState(_Word);
        }

        //ミキシングカメラのセット（カメラ注視点：現在と次、ミキシングの状態、カメラウェイトのリセット）
        public void SetMixCamera(GameObject _MixingCameraObject, STAGE_NO _CurrentStageNo, STAGE_NO _NextStageNo, string _Word)
        {
            _MixingCameraObject.GetComponent<MixingCamera>().LookAtTargetTwoChanges(Stages[(int)_CurrentStageNo], Stages[(int)_NextStageNo]);
            _MixingCameraObject.GetComponent<MixingCamera>().MixState(_Word);
            _MixingCameraObject.GetComponent<MixingCamera>().ResetWeight();

        }

    }//public class StageSelect : MonoBehaviour END
}//namespace END