using KanKikuchi.AudioManager;
using UnityEngine;

namespace TeamProject
{
    //ステージセレクトのステージ選択用
    public static class StageChangeManager
    {
        private static bool m_StageChange_flag = false;
        private static bool m_WorldChange_flag = false;

        private static bool m_MainDolly_flag = false;
        // private static bool m_SubDolly_flag = false;

        private static bool m_DollyCart_flag = false;

        private static StageSelect.SELECT_STATE m_SelectState = StageSelect.SELECT_STATE.KEY_WAIT;
        private static MixingCamera.MIXING_STATE m_MixingState = MixingCamera.MIXING_STATE.FIXING;
        private static DollyCamera.DOLLY_MOVE m_DollyState = DollyCamera.DOLLY_MOVE.FIXING;
        //private static DollyCartManager.DOLLYCART_MOVE m_DollyCartState = DollyCartManager.DOLLYCART_MOVE.FIXING;

        private static int m_LeftEdge = (int)WORLD_NO.W1;//ワールド移動制限用左端
        private static int m_RightEdge = (int)WORLD_NO.W4;//ワールド移動制限用右端
        //ステージを変更するためのキー(キー入力に対応)
        public enum STAGE_CHANGE_KEY
        {
            UP, DOWN, LEFT, RIGHT, ALL
        }
        private static STAGE_CHANGE_KEY m_StageChangeKey;

        private static WorldSelectHold m_Hold = GameObject.Find("WorldMoveArrows").GetComponent<WorldSelectHold>();
        //============================================================
        //ステージセレクト処理
        //ステージ移動更新
        //public static void Update()
        //{
        //}

        public static void StageChange()
        {
            //ステージ番号を0～5に振り分ける(入力制限をかけるため)
            int StageNumber = StageStatusManager.Instance.StageInWorld;
            //ステージ番号から現在のワールドを確認する
            int WorldNumber = StageStatusManager.Instance.CurrentWorld;

            //上入力
            if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.UpArrow) && StageNumber != (int)STAGE_NO.STAGE05)
            //if (InputManager.InputManager.Instance.GetLStick().y > 0 && StageNumber != (int)STAGE_NO.STAGE05)
            {
                StageFlagChange(true);
                //カーソルの移動音
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);


                //ステージ番号の変更キー設定
                m_StageChangeKey = STAGE_CHANGE_KEY.UP;
            }
            //下入力
            else if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.DownArrow) && StageNumber != (int)STAGE_NO.STAGE01)
            //else if (InputManager.InputManager.Instance.GetLStick().y < 0 && StageNumber != (int)STAGE_NO.STAGE01)
            {
                StageFlagChange(true);
                //カーソルの移動音
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);

                //ステージ番号の変更キー設定
                m_StageChangeKey = STAGE_CHANGE_KEY.DOWN;
                //ステージ番号の変更
                //StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.PrevStage;

            }
            //-------------------------------------------


            //-------------------------------------------
        }//    void StageChange()   END

        public static void WorldChange()
        {
            //ステージ番号を0～5に振り分ける(入力制限をかけるため)
            int StageNumber = StageStatusManager.Instance.StageInWorld;
            //ステージ番号から現在のワールドを確認する
            int WorldNumber = StageStatusManager.Instance.CurrentWorld;
            //-------------------------------------------
            //ここから下ワールド間の移動処理
            //右入力
            if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.RightArrow))// && WorldNumber != m_RightEdge)
            //else if (InputManager.InputManager.Instance.GetLStick().x > 0 && WorldNumber == 0)
            {
                Debug.Log("m_Hold = " + m_Hold);
                if (m_Hold.NextWorldMoveBeginCheck())
                {
                    WorldNumber += 1;
                    if (WorldNumber >= 4)
                    {
                        WorldNumber = 1;
                    }
                    Debug.Log("右入力後WorldNumber:" + WorldNumber);

                    WorldFlagChange(true);
                    //カーソルの移動音
                    SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);

                    //ステージ番号の変更キー設定
                    m_StageChangeKey = STAGE_CHANGE_KEY.RIGHT;
                    //ステージ番号の変更
                    //StageStatusManager.Instance.CurrentStage = (STAGE_NO)(WorldNumber * 5);

                }

            }
            //左入力
            else if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.LeftArrow))// && WorldNumber != m_LeftEdge)
            //else if (InputManager.InputManager.Instance.GetLStick().x < 0 && WorldNumber == 1)
            {
                if (m_Hold.PrevWorldMoveBeginCheck())
                {
                    WorldNumber -= 1;
                    if (WorldNumber < 0)
                    {
                        WorldNumber = 3;
                    }
                    Debug.Log("左入力後WorldNumber:" + WorldNumber);

                    WorldFlagChange(true);
                    //カーソルの移動音
                    SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
                    //ステージ番号の変更キー設定
                    m_StageChangeKey = STAGE_CHANGE_KEY.LEFT;

                }
            }

            //-------------------------------------------
        }//    void WorldChange()   END

        //フラグの切り替え(ON <－> OFF)
        public static void StageFlagChange(bool _bool)
        {
            m_StageChange_flag = _bool;
        }

        //フラグの切り替え(ON <－> OFF)
        public static void WorldFlagChange(bool _bool)
        {
            m_WorldChange_flag = _bool;
        }

        //ステージ移動中かどうか
        public static bool IsStageChange()
        {
            return m_StageChange_flag;
        }
        //ワールド移動中かどうか
        public static bool IsWorldChange()
        {
            return m_WorldChange_flag;
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
                else if (_Word == "BEFORE_STAGE_MOVING")
                {
                    m_SelectState = StageSelect.SELECT_STATE.BEFORE_STAGE_MOVING;

                }
                else if (_Word == "BEFORE_WORLD_MOVING")
                {
                    m_SelectState = StageSelect.SELECT_STATE.BEFORE_WORLD_MOVING;

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

        //ステージ変更キーの取得
        public static STAGE_CHANGE_KEY GetStageChangeKey()
        {
            return m_StageChangeKey;
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
            else if (_Word == "WORLD")
            {
                m_MixingState = MixingCamera.MIXING_STATE.WORLD;

            }
            else if (_Word == "WORLD_END")
            {
                m_MixingState = MixingCamera.MIXING_STATE.WORLD_END;

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
            else if (_Word == "WORLD")
            {
                m_DollyState = DollyCamera.DOLLY_MOVE.WORLD;

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
            if (_Word == "MAIN")
            {
                m_MainDolly_flag = true;
            }
            else if (_Word == "SUB")
            {
                //m_SubDolly_flag = true;

            }
        }
        public static void DollyFlagReset()
        {
            m_MainDolly_flag = false;
            //m_SubDolly_flag = false;
        }
        public static bool DollyFlagCheck()
        {
            if (m_MainDolly_flag)// && m_SubDolly_flag)
            {
                return true;
            }
            return false;
        }

        //============================================================
        //ドリーカート処理
        //public static DollyCartManager.DOLLYCART_MOVE DollyCartState()
        //{
        //    return m_DollyCartState;
        //}
        //public static void DollyCartStateChange(string _Word)
        //{
        //    if (_Word == "GO")
        //    {
        //        m_DollyCartState = DollyCartManager.DOLLYCART_MOVE.GO;

        //    }
        //    else if (_Word == "BACK")
        //    {
        //        m_DollyCartState = DollyCartManager.DOLLYCART_MOVE.BACK;

        //    }
        //    else if (_Word == "WORLD")
        //    {
        //        m_DollyCartState = DollyCartManager.DOLLYCART_MOVE.WORLD;

        //    }
        //    else if (_Word == "FIXING")
        //    {
        //        m_DollyCartState = DollyCartManager.DOLLYCART_MOVE.FIXING;

        //    }
        //    else
        //    {
        //        Debug.LogAssertion("DOLLYCART_MOVEの言葉が違います。");
        //        m_DollyCartState = DollyCartManager.DOLLYCART_MOVE.FIXING;

        //    }
        //}

        //public static void DollyCartFlagChange(bool _bool)
        //{
        //    m_DollyCart_flag = _bool;
        //}
        //public static bool DollyCartFlagCheck()
        //{
        //    if (m_DollyCart_flag)
        //    {
        //        return true;
        //    }
        //    return false;
        //}


        //ワールド間移動の完了確認処理
        public static bool IsWorldMoveEnd()
        {
            if (/*DollyCartFlagCheck() &&*/ DollyFlagCheck())
            {
                return true;

            }
            return false;
        }

        public static void GetComponentWorldSelectHold()
        {
            if (m_Hold == null)
            {

                m_Hold = GameObject.Find("WorldMoveArrows").GetComponent<WorldSelectHold>();
            }
        }

    }//public class StageChangeManager END
}//namespace END
