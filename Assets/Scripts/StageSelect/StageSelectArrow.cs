using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    public class StageSelectArrow : MonoBehaviour
    {
        //現在ステージ位置 
        //StageStatusManager.Instance.CurrentStage;
        public GameObject m_Canvas;//一番上の親オブジェクト
        public GameObject m_Next;//上矢印用オブジェクト
        public GameObject m_Prev;//下矢印用オブジェクト

        // Start is called before the first frame update
        void Start()
        {
            m_Canvas = transform.root.gameObject;//一番上の親を取得
            m_Next = transform.GetChild(0).gameObject;
            m_Prev = transform.GetChild(1).gameObject;
            //m_CurrentStage = StageSelect.STAGE.STAGE1;
            SetCurrentStage(StageStatusManager.Instance.CurrentStage);
        }

        // Update is called once per frame
        void Update()
        {

        }//void Update()    END
         //ステージ状況に応じた処理
        public void SetCurrentStage(STAGE_NO CurrentStage)
        {
            //m_CurrentStage = CurrentStage;
            switch (CurrentStage)
            {
                case STAGE_NO.STAGE01:
                    TwoSetActives(true, false);
                    break;
                case STAGE_NO.STAGE02:
                    TwoSetActives(true, true);
                    break;
                case STAGE_NO.STAGE03:
                    TwoSetActives(true, true);
                    break;
                case STAGE_NO.STAGE04:
                    TwoSetActives(true, true);
                    break;
                case STAGE_NO.STAGE05:
                    TwoSetActives(false, true);
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

            //switch (CurrentStage)
            //{
            //    case StageSelect.STAGE.STAGE1:
            //        TwoSetActives(true, false);
            //        break;
            //    case StageSelect.STAGE.STAGE2:
            //        TwoSetActives(true, true);
            //        break;

            //    case StageSelect.STAGE.STAGE3:
            //        TwoSetActives(true, true);
            //        break;
            //    case StageSelect.STAGE.STAGE4:
            //        TwoSetActives(true, true);
            //        break;
            //    case StageSelect.STAGE.STAGE5:
            //        TwoSetActives(false, true);
            //        break;
            //    case StageSelect.STAGE.STAGE_NUM:
            //        break;
            //    default:
            //        break;
            //}

        }

        //上下の矢印のアクティブを同時に設定
        private void TwoSetActives(bool Next, bool Prev)
        {
            m_Next.SetActive(Next);
            m_Prev.SetActive(Prev);
        }
    }
}