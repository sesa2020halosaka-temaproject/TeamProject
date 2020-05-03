using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    //ステージセレクトのステージ選択用
    public static class StageChangeManager
    {
        private static bool m_StageChange_flag = false;

        private static bool m_MainDolly_flag = false;
        private static bool m_SubDolly_flag = false;

        private static StageSelect.SELECT_STATE m_SelectState = StageSelect.SELECT_STATE.KEY_WAIT;
        private static MixingCamera.MIXING_STATE m_MixingState = MixingCamera.MIXING_STATE.FIXING;
        private static DollyCamera.DOLLY_MOVE m_DollyState = DollyCamera.DOLLY_MOVE.FIXING;

        //============================================================
        //ステージセレクト処理
        //ステージ移動更新
        public static void Update()
        {
        }

        public static void StageChange()
        {
            //ステージ番号を0～5に振り分ける(入力制限をかけるため)
            int StageNumber = (int)StageStatusManager.Instance.CurrentStage % 5;
            //上入力
            if (InputManager.InputManager.Instance.GetLStick().y > 0 && StageNumber != (int)STAGE_NO.STAGE05)
            {
                FlagChange();
                //カーソルの移動音
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);

                //前進させる何か
                MixingStateChange("GO");

                StageSelectArrow.SetCurrentStage(StageStatusManager.Instance.NextStage);
                StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.NextStage;
            }
            //下入力
            else if (InputManager.InputManager.Instance.GetLStick().y < 0 && StageNumber != (int)STAGE_NO.STAGE01)
            {
                FlagChange();
                //カーソルの移動音
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);

                //後進させる何か
                MixingStateChange("BACK");

                StageSelectArrow.SetCurrentStage(StageStatusManager.Instance.PrevStage);
                StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.PrevStage;

            }

        }//    void StageChange()   END

        //フラグの切り替え(ON <－> OFF)
        public static void FlagChange()
        {
            m_StageChange_flag = !m_StageChange_flag;
        }

        //ステージ移動中かどうか
        public static bool IsStageChange()
        {
            return m_StageChange_flag;
        }

        public static StageSelect.SELECT_STATE GetSelectState()
        {
            return m_SelectState;
        }

        public static void SelectStateChange(string _Word)
        {
            {
                if (_Word == "KEY_WAIT")
                {
                    m_SelectState = StageSelect.SELECT_STATE.KEY_WAIT;

                }
                else if (_Word == "STAGE_MOVING")
                {
                    m_SelectState = StageSelect.SELECT_STATE.STAGE_MOVING;

                }
                else if (_Word == "WORLD_MOVING")
                {
                    m_SelectState = StageSelect.SELECT_STATE.WORLD_MOVING;

                }
                else if (_Word == "SCENE_MOVING")
                {
                    m_SelectState = StageSelect.SELECT_STATE.SCENE_MOVING;

                }
                else
                {
                    Debug.LogAssertion("SELECT_STATEの言葉が違います。");
                    m_SelectState = StageSelect.SELECT_STATE.KEY_WAIT;

                }
            }
        }
        //============================================================
        //ミキシングカメラ処理
        public static MixingCamera.MIXING_STATE MixingState()
        {
            return m_MixingState;
        }
        public static void MixingStateChange(string _Word)
        {
            if (_Word == "GO")
            {
                m_MixingState = MixingCamera.MIXING_STATE.GO;

            }
            else if (_Word == "BACK")
            {
                m_MixingState = MixingCamera.MIXING_STATE.BACK;

            }
            else if (_Word == "FIXING")
            {
                m_MixingState = MixingCamera.MIXING_STATE.FIXING;

            }
            else
            {
                Debug.LogAssertion("MIXING_STATEの言葉が違います。");
                m_MixingState = MixingCamera.MIXING_STATE.FIXING;

            }
        }

        //============================================================
        //ドリーカメラ処理

        public static DollyCamera.DOLLY_MOVE DollyState()
        {
            return m_DollyState;
        }
        public static void DollyStateChange(string _Word)
        {
            if (_Word == "GO")
            {
                m_DollyState = DollyCamera.DOLLY_MOVE.GO;

            }
            else if (_Word == "BACK")
            {
                m_DollyState = DollyCamera.DOLLY_MOVE.BACK;

            }
            else if (_Word == "FIXING")
            {
                m_DollyState = DollyCamera.DOLLY_MOVE.FIXING;

            }
            else
            {
                Debug.LogAssertion("DOLLY_MOVEの言葉が違います。");
                m_DollyState = DollyCamera.DOLLY_MOVE.FIXING;

            }
        }

        public static void DollyFlagON(string _Word)
        {
            if (_Word=="MAIN")
            {
                m_MainDolly_flag = true;
            }
            else if (_Word=="SUB")
            {
                m_SubDolly_flag = true;

            }
        }
        public static void DollyFlagReset()
        {
            m_MainDolly_flag = false;
            m_SubDolly_flag = false;
        }
        public static bool DollyFlagCheck()
        {
            if (m_MainDolly_flag && m_SubDolly_flag)
            {
                return true;
            }
            return false;
        }
    }//public class StageChangeManager END
}//namespace END
