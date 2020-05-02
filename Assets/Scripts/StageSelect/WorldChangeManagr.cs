using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //ワールド間移動用
    public class WorldChangeManagr
    {
        private static bool m_WorldChange_flag = false;//ワールド間移動中フラグ

        private enum WORLDCHANGE_STATE
        {
            WAIT,       //
            SWING,      //
            MOVING,     //
            ALL_STATES  //
        }

        private static WORLDCHANGE_STATE m_State=WORLDCHANGE_STATE.WAIT;
        public static void WorldChange()
        {
            //ステージ番号から現在のワールドを確認する
            int WorldNumber = (int)StageStatusManager.Instance.CurrentStage / 5;
            //右入力
            if (InputManager.InputManager.Instance.GetLStick().x > 0)
            {
                ChangeFlag();
                m_State = WORLDCHANGE_STATE.SWING;
            }
            //左入力
            else if (InputManager.InputManager.Instance.GetLStick().x > 0)
            {
                ChangeFlag();
                m_State = WORLDCHANGE_STATE.SWING;

            }

            ////上入力
            //if (InputManager.InputManager.Instance.GetLStick().y > 0 && StageNumber != (int)STAGE_NO.STAGE05)
            //{
            //    ChangeFlag();
            //    //MixCameraGo(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.NextStage);
            //    //_StageSelectArrow.SetCurrentStage(StageStatusManager.Instance.NextStage);
            //    //StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.NextStage;
            //    ////カーソルの移動音
            //    //SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            //}
            ////下入力
            //else if (InputManager.InputManager.Instance.GetLStick().y < 0 && StageNumber != (int)STAGE_NO.STAGE01)
            //{
            //    ChangeFlag();
            //    //MixCameraBack(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.PrevStage);
            //    //_StageSelectArrow.SetCurrentStage(StageStatusManager.Instance.PrevStage);
            //    //StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.PrevStage;
            //    ////カーソルの移動音
            //    //SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);


            //}

        }//    void StageChange()   END

         //ワールド間移動更新
        public static void Update()
        {
            switch (m_State)
            {
                case WORLDCHANGE_STATE.WAIT:
                    break;
                case WORLDCHANGE_STATE.SWING:
                    break;
                case WORLDCHANGE_STATE.MOVING:
                    break;
                case WORLDCHANGE_STATE.ALL_STATES:
                    break;
                default:
                    break;
            }
        }

        //フラグの切り替え(ON <－> OFF)
        public static void ChangeFlag()
        {
            m_WorldChange_flag = !m_WorldChange_flag;
        }

        //ステージ移動中かどうか
        public static bool IsWorldChange()
        {
            return m_WorldChange_flag;
        }




    }//public class WorldChangeManagr END
}//namespace END