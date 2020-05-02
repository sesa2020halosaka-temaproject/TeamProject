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

        private enum STAGECHANGE_STATE
        {
            WAIT = 0,//待ち
            GO,       //前進
            BACK,      //後退
            ALL_STATES    //状態の数

        }
        private static STAGECHANGE_STATE m_State = STAGECHANGE_STATE.WAIT;
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
                m_State = STAGECHANGE_STATE.GO;
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
                m_State = STAGECHANGE_STATE.BACK;
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

        //ステージ移動の状態を返す
        public static string MovingState()
        {
            if (m_State == STAGECHANGE_STATE.GO)
            {
                return "GO";

            }
            else if (m_State == STAGECHANGE_STATE.BACK)
            {
                return "BACK";

            }
            return "WAIT";
        }


    }//public class StageChangeManager END
}//namespace END
