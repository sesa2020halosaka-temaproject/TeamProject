using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeamProject
{
    //ステージセレクトのステージ選択用
    public class StageChangeManager
    {
        private static bool m_StageChange_flag = false;

        public static void StageChange()
        {
            //ステージ番号を0～5に振り分ける(入力制限をかけるため)
            int StageNumber = (int)StageStatusManager.Instance.CurrentStage % 5;
            //上入力
            if (InputManager.InputManager.Instance.GetLStick().y > 0 && StageNumber != (int)STAGE_NO.STAGE05)
            {
                ChangeFlag();
                //MixCameraGo(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.NextStage);
                //_StageSelectArrow.SetCurrentStage(StageStatusManager.Instance.NextStage);
                //StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.NextStage;
                ////カーソルの移動音
                //SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            }
            //下入力
            else if (InputManager.InputManager.Instance.GetLStick().y < 0 && StageNumber != (int)STAGE_NO.STAGE01)
            {
                ChangeFlag();
                //MixCameraBack(StageStatusManager.Instance.CurrentStage, StageStatusManager.Instance.PrevStage);
                //_StageSelectArrow.SetCurrentStage(StageStatusManager.Instance.PrevStage);
                //StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.PrevStage;
                ////カーソルの移動音
                //SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);


            }

        }//    void StageChange()   END

        //ステージ移動更新
        public static void Update()
        {

        }
        //フラグの切り替え(ON <－> OFF)
        public static void ChangeFlag()
        {
            m_StageChange_flag = !m_StageChange_flag;
        }

        //ステージ移動中かどうか
        public static bool IsStageChange()
        {
            return m_StageChange_flag;
        }


    }//public class StageChangeManager END
}//namespace END