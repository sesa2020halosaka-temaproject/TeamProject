using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //ワールド選択可能表示矢印用クラス
    public class WorldSelectArrow : MonoBehaviour
    {
        private enum NUMBER
        {
            W1 = 0, W2, W3, W4,//ワールド内のステージ番号
            ALLSTAGE//ワールド内のステージ数
        };
        //現在ワールド位置 
        //StageStatusManager.Instance.CurrentWorld;
        public static GameObject m_Canvas;//一番上の親オブジェクト
        public static GameObject m_Next;//上矢印用オブジェクト
        public static GameObject m_Prev;//下矢印用オブジェクト

        public static bool m_AdvancedWorld4_Flag = false;//ワールド４に進出したかどうかフラグ

        // Start is called before the first frame update
        void Start()
        {
            m_Canvas = transform.root.gameObject;//一番上の親を取得
            m_Next = transform.GetChild(0).gameObject;//右矢印用オブジェクト
            m_Prev = transform.GetChild(1).gameObject;//左矢印用オブジェクト
            //m_CurrentStage = StageSelect.STAGE.STAGE1;
        }

        // Update is called once per frame
        //void Update()
        //{

        //}//void Update()    END

        //ワールド状況に応じた処理
        public static void SetCurrentWorld()
        {
            if (AdvancedW4FlagCheck())
            {
                TwoArrowsActivate();
            }
            else
            {
                switch (StageStatusManager.Instance.CurrentWorld)
                {
                    case (int)NUMBER.W1://World01
                          TwoSetActives(true, false);

                        break;
                    case (int)NUMBER.W2://World02
                      TwoSetActives(false, true);
                        break;
                    case (int)NUMBER.W3://World03
                        TwoSetActives(true, true);
                        break;
                    case (int)NUMBER.W4://World04
                        TwoSetActives(false, true);
                        break;
                    case (int)NUMBER.ALLSTAGE:
                    default:
                        Debug.LogAssertion("WorldSelectArrowが無効な状態！");
                        break;
                }

            }
        }

        //左右の矢印のアクティブを同時に設定
        private static void TwoSetActives(bool Next, bool Prev)
        {
            m_Next.SetActive(Next);
            m_Prev.SetActive(Prev);
        }

        //左右の矢印を非アクティブに設定
        public static void TwoArrowsActivate()
        {
            m_Next.SetActive(true);
            m_Prev.SetActive(true);
        }
        //左右の矢印を非アクティブに設定
        public static void TwoArrowsDeactivate()
        {
            m_Next.SetActive(false);
            m_Prev.SetActive(false);
        }

        //ワールド4進出フラグをONにする
        public static void AdvancedW4FlagON()
        {
            m_AdvancedWorld4_Flag = true;
        }

        //ワールド4進出フラグを確認する
        public static bool AdvancedW4FlagCheck()
        {
            return m_AdvancedWorld4_Flag;
        }
    }//public class WorldSelectArrow : MonoBehaviour END
}//namespace END