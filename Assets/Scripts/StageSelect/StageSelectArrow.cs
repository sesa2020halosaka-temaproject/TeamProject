using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    //ステージ選択可能表示矢印用
    public class StageSelectArrow : MonoBehaviour
    {
        [Header("UIの移動する時間")]
        private float m_MoveOutTime;
        private float m_MoveInTime;

        [Header("UIの画面内位置")]
        public Vector3 m_InPosition_Next;
        public Vector3 m_InPosition_Prev;
        [Header("UIの画面外位置")]
        public Vector3 m_OutPosition_Next;
        public Vector3 m_OutPosition_Prev;

        public float m_InPos_Next_Y;
        public float m_OutPos_Next_Y;
        public float m_InPos_Prev_Y;
        public float m_OutPos_Prev_Y;

        private bool m_EndFlag_Next;
        private bool m_EndFlag_Prev;


        //現在ステージ位置 
        //StageStatusManager.Instance.CurrentStage;
        public static GameObject m_Canvas;//一番上の親オブジェクト
        public static GameObject m_Next;//上矢印用オブジェクト
        public static GameObject m_Prev;//下矢印用オブジェクト

        //スプライトのパス
        public static string[] m_UI_StageName = {
            "01_Stage01","01_Stage02","01_Stage03","01_Stage04","01_Stage05",
            "02_Stage01","02_Stage02","02_Stage03","02_Stage04","02_Stage05",
            "03_Stage01","03_Stage02","03_Stage03","03_Stage04","03_Stage05",
            "04_Stage01","04_Stage02","04_Stage03","04_Stage04","04_Stage05",
        };
        public static string m_NextName;//次ステージ用パス
        public static string m_PrevName;//前ステージ用パス
        //スプライトのパス（固定部分）
        public const string m_ConstPath = "Sprites/StageSelect/UI_StageNamePlate/UI_World";

        private void Awake()
        {
            m_Canvas = transform.root.gameObject;//一番上の親を取得
            m_Next = transform.GetChild(0).gameObject;
            m_Prev = transform.GetChild(1).gameObject;
            //m_CurrentStage = StageSelect.STAGE.STAGE1;
            SetCurrentStage(StageStatusManager.Instance.StageInWorld);
        }
        // Start is called before the first frame update
        void Start()
        {
            ChangeStageNameIcon();
        }

        // Update is called once per frame
        //void Update()
        //{

        //}//void Update()    END

        //更新処理
        public void StageSelectArrowUpdate()
        {

        }//StageSelectUpdate() END



        //ステージ状況に応じた処理
        public static void SetCurrentStage(int _StageInWorld)
        {
            switch (_StageInWorld)
            {
                case (int)IN_WORLD_NO.S1://Stage01
                    TwoSetActives(false, true);

                    break;
                case (int)IN_WORLD_NO.S2:
                case (int)IN_WORLD_NO.S3:
                case (int)IN_WORLD_NO.S4:
                    //Stage02～04
                    TwoSetActives(true, true);

                    break;
                case (int)IN_WORLD_NO.S5://Stage05
                    TwoSetActives(true, false);

                    break;
                case (int)IN_WORLD_NO.ALLSTAGE:
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
                case (int)IN_WORLD_NO.S1://Stage01
                    TwoSetActives(false, true);

                    break;
                case (int)IN_WORLD_NO.S2:
                case (int)IN_WORLD_NO.S3:
                case (int)IN_WORLD_NO.S4:
                    //Stage02～04
                    TwoSetActives(true, true);

                    break;
                case (int)IN_WORLD_NO.S5://Stage05
                    TwoSetActives(true, false);

                    break;
                case (int)IN_WORLD_NO.ALLSTAGE:
                default:
                    Debug.LogAssertion("StageSelectArrowが無効な状態！");
                    break;
            }
        }

        //上下の矢印のアクティブを同時に設定
        private static void TwoSetActives(bool Prev, bool Next)
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

        //前後どちらの移動かによる矢印のアクティブ設定
        public static void TwoArrowsSetting()
        {
            if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.UP)
            {
                TwoSetActives(false, true);
            }
            else if (StageChangeManager.GetStageChangeKey() == StageChangeManager.STAGE_CHANGE_KEY.DOWN)
            {
                TwoSetActives(true, false);
            }
        }

        //次と前のステージを示すUIの差し替え
        public static void ChangeStageNameIcon()
        {
            // Debug.Log("NamePlate");
            //スプライトのパスを切り替える
            m_NextName = m_ConstPath + m_UI_StageName[(int)StageStatusManager.Instance.NextStage];
            m_PrevName = m_ConstPath + m_UI_StageName[(int)StageStatusManager.Instance.PrevStage];

            //次のステージの表示スプライトを差し替える
            m_Next.transform.GetChild(1).GetComponent<Image>().sprite = null;
            m_Next.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(m_NextName);

            //前のステージの表示スプライトを差し替える
            m_Prev.transform.GetChild(1).GetComponent<Image>().sprite = null;
            m_Prev.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(m_PrevName);
        }//Sprites/StageSelect/UI_StageNamePlate/UI_World01_Stage01.png
    }//public class StageSelectArrow : MonoBehaviour END
}//namespace END