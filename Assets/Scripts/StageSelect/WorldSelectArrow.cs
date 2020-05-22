using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    //ワールド選択可能表示矢印用クラス
    public class WorldSelectArrow : MonoBehaviour
    {
        //現在ワールド位置 
        //StageStatusManager.Instance.CurrentWorld;
        public static GameObject m_Canvas;//一番上の親オブジェクト
        public static GameObject m_Next;//右矢印用オブジェクト
        public static GameObject m_Prev;//左矢印用オブジェクト

        public static bool m_AdvancedWorld4_Flag = false;//ワールド４に進出したかどうかフラグ

        //スプライトのパス（固定部分）
        public const string m_ConstPath = "Sprites/StageSelect/UI_WorldStatus/UI_StageSelect_";

        //スプライトのパス
        public static string[] m_UI_WorldName = {
            "World01","World02","World03","World04"
        };//Assets/Resources/Sprites/StageSelect/UI_WorldStatus/UI_StageSelect_World02
        public static string m_NextName;//次ワールド用パス
        public static string m_PrevName;//前ワールド用パス

        private void Awake()
        {
            m_Canvas = transform.root.gameObject;//一番上の親を取得
            m_Next = transform.GetChild(0).gameObject;//右矢印用オブジェクト
            m_Prev = transform.GetChild(1).gameObject;//左矢印用オブジェクト
            //m_CurrentStage = StageSelect.STAGE.STAGE1;
        }
        // Start is called before the first frame update
        void Start()
        {
            ChangeWorldNameIcon();

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
                    case (int)WORLD_NO.W1://World01
                          TwoSetActives(true, false);

                        break;
                    case (int)WORLD_NO.W2://World02
                      TwoSetActives(true, true);
                        break;
                    case (int)WORLD_NO.W3://World03
                        TwoSetActives(true, true);
                        break;
                    case (int)WORLD_NO.W4://World04
                        TwoSetActives(false, true);
                        break;
                    case (int)WORLD_NO.ALL_WORLD:
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

        //次と前のワールドを示すUIの差し替え
        public static void ChangeWorldNameIcon()
        {
            // Debug.Log("NamePlate");
            //スプライトのパスを切り替える
            m_NextName = m_ConstPath + m_UI_WorldName[StageStatusManager.Instance.NextWorld];
            m_PrevName = m_ConstPath + m_UI_WorldName[StageStatusManager.Instance.PrevWorld];

            //次のワールドの表示スプライトを差し替える
            m_Next.transform.GetChild(1).GetComponent<Image>().sprite = null;
            m_Next.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(m_NextName);

            //前のワールドの表示スプライトを差し替える
            m_Prev.transform.GetChild(1).GetComponent<Image>().sprite = null;
            m_Prev.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(m_PrevName);
        }//Sprites/StageSelect/UI_StageNamePlate/UI_World01_Stage01.png

    }//public class WorldSelectArrow : MonoBehaviour END
}//namespace END