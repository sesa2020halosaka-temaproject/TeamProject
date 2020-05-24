using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    //現在のワールドのクリア状況を表示するUIの処理
    public class WorldStatusUIManager : MonoBehaviour
    {
        //現在ワールド位置 
        //StageStatusManager.Instance.CurrentWorld;
        public GameObject m_Canvas;//一番上の親オブジェクト
       // public GameObject m_Next;//次のワールド名UI用オブジェクト
        public GameObject m_Current;//現在のワールド名UI用オブジェクト

        //スプライトのパス（固定部分）
        public const string m_ConstPath = "Sprites/StageSelect/UI_WorldStatus/UI_StageSelect_Detail_";
        //
        //スプライトのパス
        public string[] m_UI_WorldName = {
            "World01","World02","World03","World04"
        };
        public string m_NextName;//次ワールド用パス
        public string m_CurrentName;//前ワールド用パス

        private void Awake()
        {
            //m_Next = transform.GetChild(1).gameObject;//次のワールド名UI用オブジェクト
            m_Current = transform.GetChild(1).gameObject;//現在のワールド名UI用オブジェクト
        }
        // Start is called before the first frame update
        void Start()
        {
            //StartWorldNameIcon();

        }

        // Update is called once per frame
        //void Update()
        //{

        //}//void Update()    END

        //開始時のワールドを示すUIの差し替え
        public void StartWorldNameIcon()
        {
            //スプライトのパスを切り替える
            m_CurrentName = m_ConstPath + m_UI_WorldName[StageStatusManager.Instance.CurrentWorld];

            //現在のワールド名の表示スプライトを差し替える
            m_Current.GetComponent<Image>().sprite = null;
            m_Current.GetComponent<Image>().sprite = Resources.Load<Sprite>(m_CurrentName);
        }//

        //次と前のワールドを示すUIの差し替え
        public void ChangeWorldNameIcon()
        {
            switch (StageChangeManager.GetStageChangeKey())
            {
                case StageChangeManager.STAGE_CHANGE_KEY.UP:
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.DOWN:
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    //スプライトのパスを切り替える
                    m_NextName = m_ConstPath + m_UI_WorldName[StageStatusManager.Instance.PrevWorld];
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    //スプライトのパスを切り替える
                    m_NextName = m_ConstPath + m_UI_WorldName[StageStatusManager.Instance.NextWorld];
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                    break;
                default:
                    break;
            }


            //現在のワールド名の表示スプライトを現在のワールド名に差し替える
            m_Current.GetComponent<Image>().sprite = null;
            m_Current.GetComponent<Image>().sprite = Resources.Load<Sprite>(m_NextName);
        }//
    } //public class WorldStatusUIManager : MonoBehaviour    END
} //namespace TeamProject    END
