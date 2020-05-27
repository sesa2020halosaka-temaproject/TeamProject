using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    //ワールド移動時に表示するUIの処理
    public class CurrentToNextWorldUIManager : MonoBehaviour
    {
        [Header("UIの移動する時間")]
        private float m_MoveOutTime;
        private float m_MoveInTime;
        [Header("UIの画面内位置")]
        public Vector3 m_InPosition;
        [Header("UIの画面外位置")]
        public Vector3 m_OutPosition;

        public float m_InPos_Y;
        public float m_OutPos_Y;

        //現在ワールド位置 
        //StageStatusManager.Instance.CurrentWorld;
        public GameObject m_Canvas;//一番上の親オブジェクト
        public GameObject m_Next;//次のワールド名UI用オブジェクト
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

        private UIMoveManager m_UIMoveManager;
        private UIMoveManager.UI_MOVESTATE m_UIMoveState;
        private StageSelectUIManager m_StageSelectUIManager;

        //==========================================================================
        private void Awake()
        {
            m_UIMoveManager = new UIMoveManager();

            m_Next = transform.GetChild(1).gameObject;//次のワールド名UI用オブジェクト
            m_Current = transform.GetChild(2).gameObject;//現在のワールド名UI用オブジェクト
            m_StageSelectUIManager = this.transform.root.GetComponent<StageSelectUIManager>();
        }
        // Start is called before the first frame update
        void Start()
        {
            m_InPosition = new Vector3(this.transform.localPosition.x, m_InPos_Y, this.transform.localPosition.z);
            m_OutPosition = new Vector3(this.transform.localPosition.x, m_OutPos_Y, this.transform.localPosition.z);
            m_MoveOutTime = m_StageSelectUIManager.m_UIMoveIn_Time;
            m_MoveInTime = m_StageSelectUIManager.m_UIMoveOut_Time;

        }

        // Update is called once per frame
        //void Update()
        //{

        //}//void Update()    END

        //更新処理
        public void ToNextWorldUIUpdate()
        {
            switch (m_UIMoveState)
            {
                case UIMoveManager.UI_MOVESTATE.FIXING:
                    break;
                case UIMoveManager.UI_MOVESTATE.MOVEIN:
                    m_UIMoveManager.UIMove(this.gameObject, m_OutPosition, m_InPosition, m_MoveInTime);
                    if (this.transform.localPosition.y >= m_InPosition.y)
                    {
                        this.transform.localPosition = m_InPosition;
                        Debug.Log("In完了");
                        m_UIMoveState = UIMoveManager.UI_MOVESTATE.FIXING;
                        m_UIMoveManager.PosRatioZeroReset();
                    }

                    break;
                case UIMoveManager.UI_MOVESTATE.MOVEOUT:
                    m_UIMoveManager.UIMove(this.gameObject, m_InPosition, m_OutPosition, m_MoveOutTime);
                    if (this.transform.localPosition.y <= m_OutPosition.y)
                    {
                        this.transform.localPosition = m_OutPosition;
                        Debug.Log("OUT完了");
                        m_UIMoveState = UIMoveManager.UI_MOVESTATE.FIXING;
                        m_UIMoveManager.PosRatioZeroReset();
                    }

                    break;
                case UIMoveManager.UI_MOVESTATE.ALL_MOVESTATE:
                    break;
                default:
                    break;
            }
        }


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
            // Debug.Log("NamePlate");
            m_CurrentName = m_ConstPath + m_UI_WorldName[StageStatusManager.Instance.CurrentWorld];

            //次のワールドの表示スプライトを差し替える
            m_Next.GetComponent<Image>().sprite = null;
            m_Next.GetComponent<Image>().sprite = Resources.Load<Sprite>(m_NextName);

            //現在のワールドの表示スプライトを差し替える
            m_Current.GetComponent<Image>().sprite = null;
            m_Current.GetComponent<Image>().sprite = Resources.Load<Sprite>(m_CurrentName);
        }//

        public void UIOutMove()
        {
            m_UIMoveState = UIMoveManager.UI_MOVESTATE.MOVEOUT;
        }
        public void UIInMove()
        {
            m_UIMoveState = UIMoveManager.UI_MOVESTATE.MOVEIN;
        }

    } //public class CurrentToNextWorldUIManager : MonoBehaviour    END
} //namespace TeamProject    END
