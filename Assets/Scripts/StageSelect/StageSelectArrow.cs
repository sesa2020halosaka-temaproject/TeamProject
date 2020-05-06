using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    public class StageSelectArrow : MonoBehaviour
    {
        private enum NUMBER
        {
            S1 = 0, S2, S3, S4, S5,//ワールド内のステージ番号
            ALLSTAGE//ワールド内のステージ数
        };
        //現在ステージ位置 
        //StageStatusManager.Instance.CurrentStage;
        public static GameObject m_Canvas;//一番上の親オブジェクト
        public static GameObject m_Next;//上矢印用オブジェクト
        public static GameObject m_Prev;//下矢印用オブジェクト

        // Start is called before the first frame update
        void Start()
        {
            m_Canvas = transform.root.gameObject;//一番上の親を取得
            m_Next = transform.GetChild(0).gameObject;
            m_Prev = transform.GetChild(1).gameObject;
            //m_CurrentStage = StageSelect.STAGE.STAGE1;
            SetCurrentStage(StageStatusManager.Instance.StageInWorld);
        }

        // Update is called once per frame
        //void Update()
        //{

        //}//void Update()    END
        //ステージ状況に応じた処理
        public static void SetCurrentStage(int _StageInWorld)
        {
            switch (_StageInWorld)
            {
                case (int)NUMBER.S1://Stage01
                    TwoSetActives(true, false);

                    break;
                case (int)NUMBER.S2:
                case (int)NUMBER.S3:
                case (int)NUMBER.S4:
                    //Stage02～04
                    TwoSetActives(true, true);

                    break;
                case (int)NUMBER.S5://Stage05
                    TwoSetActives(false, true);

                    break;
                case (int)NUMBER.ALLSTAGE:
                default:
                    Debug.LogAssertion("StageSelectArrowが無効な状態！");
                    break;
            }
        }
        public static void SetCurrentStage(STAGE_NO CurrentStage)
        {
            int StageNumber = (int)CurrentStage % 5;//0～5に振り分け
            switch (StageNumber)
            {
                case (int)NUMBER.S1://Stage01
                    TwoSetActives(true, false);

                    break;
                case (int)NUMBER.S2:
                case (int)NUMBER.S3:
                case (int)NUMBER.S4:
                    //Stage02～04
                    TwoSetActives(true, true);

                    break;
                case (int)NUMBER.S5://Stage05
                    TwoSetActives(false, true);

                    break;
                case (int)NUMBER.ALLSTAGE:
                default:
                    Debug.LogAssertion("StageSelectArrowが無効な状態！");
                    break;
            }
        }

        //上下の矢印のアクティブを同時に設定
        private static void TwoSetActives(bool Next, bool Prev)
        {
            m_Next.SetActive(Next);
            m_Prev.SetActive(Prev);
        }

        //上下の矢印を非アクティブに設定
        public static void TwoArrowsDeactivate()
        {
            m_Next.SetActive(false);
            m_Prev.SetActive(false);
        }
    }//public class StageSelectArrow : MonoBehaviour END
}//namespace END