using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    //現在のワールドのクリア状況を表示するUIの処理
    public class WorldStatusUIManager : MonoBehaviour
    {
        [Header("ステージ内の小人の数を入力する→左下UIに反映される")]
        public uint[] m_StageMaxMinions = new uint[(int)STAGE_NO.STAGE_NUM];
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
                                   // public GameObject m_Next;//次のワールド名UI用オブジェクト
        public GameObject m_Current;//現在のワールド名UI用オブジェクト
        public GameObject[] m_StageObj = new GameObject[(int)IN_WORLD_NO.ALLSTAGE];//ワールド内ステージUI用オブジェクト
        public GameObject[] m_Star_Obj = new GameObject[THREE];//星用ゲームオブジェクト
        public Text[] m_MinionCount = new Text[(int)IN_WORLD_NO.ALLSTAGE]; //小人取得数テキスト
        public Text[] m_MinionMaxCount = new Text[(int)IN_WORLD_NO.ALLSTAGE];  //小人最大数テキスト
        public GameObject m_CompleteObj;//コンプリート文字オブジェクト

        private const int THREE = 3;
        //スプライトのパス（固定部分）
        public const string m_ConstPath = "Sprites/StageSelect/UI_WorldName/UI_StageSelect_Menu_";
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

        public CLEAR_STATUS m_ClearStatus;//ステージのクリア状況
        //UI特定用の列挙
        public enum UI_INDEX
        {
            STAR01,        //左の星
            STAR02,        //中の星
            STAR03,        //右の星
            MINION_COUNT,  //小人取得数テキスト
            MINION_MAXCOUNT,  //小人最大数テキスト
            COMPLETE,      //コンプリートUI
            ALL_INDEX      //インデックス数
        }

        private void Awake()
        {
            //m_Next = transform.GetChild(1).gameObject;//次のワールド名UI用オブジェクト
            m_Current = transform.GetChild(1).gameObject;//現在のワールド名UI用オブジェクト
            for (int i = 0; i < (int)IN_WORLD_NO.ALLSTAGE; i++)
            {
                m_StageObj[i] = this.transform.GetChild(i + 2).gameObject;
                m_MinionCount[i] = m_StageObj[i].transform.GetChild((int)UI_INDEX.MINION_COUNT).GetComponent<Text>();
                m_MinionMaxCount[i] = m_StageObj[i].transform.GetChild((int)UI_INDEX.MINION_MAXCOUNT).GetComponent<Text>();
            }
             m_UIMoveManager = new UIMoveManager();
           m_StageSelectUIManager = this.transform.root.GetComponent<StageSelectUIManager>();
        }
        // Start is called before the first frame update
        void Start()
        {
            //m_InPosition = this.GetComponent<RectTransform>(
            m_InPosition = new Vector3(this.transform.localPosition.x, m_InPos_Y, this.transform.localPosition.z);
            m_OutPosition = new Vector3(this.transform.localPosition.x, m_OutPos_Y, this.transform.localPosition.z);
            //StartWorldNameIcon();
            m_MoveOutTime = m_StageSelectUIManager.m_UIMoveOut_Time;
            m_MoveInTime = m_StageSelectUIManager.m_UIMoveIn_Time;
            StageStarUpdate();
            SetMinionCount();
            UIStateFixing();

        }

        // Update is called once per frame
        //void Update()
        //{

        //}//void Update()    END

        public void WorldStatusUIUpdate()
        {
            switch (m_UIMoveState)
            {
                case UIMoveManager.UI_MOVESTATE.FIXING:
                    break;
                case UIMoveManager.UI_MOVESTATE.MOVEIN:
                    Debug.Log("IN中です");

                    m_UIMoveManager.UIMove(this.gameObject, m_OutPosition, m_InPosition, m_MoveInTime);
                    if (this.transform.localPosition.y >= m_InPosition.y)
                    {
                        this.transform.localPosition = m_InPosition;
                        Debug.Log("In完了");
                        UIStateFixing();
                        m_UIMoveManager.PosRatioZeroReset();
                    }

                    break;
                case UIMoveManager.UI_MOVESTATE.MOVEOUT:
                    Debug.Log("OUT中です");

                    m_UIMoveManager.UIMove(this.gameObject, m_InPosition, m_OutPosition, m_MoveOutTime);
                    if (this.transform.localPosition.y <= m_OutPosition.y)
                    {
                        this.transform.localPosition = m_OutPosition;
                        Debug.Log("OUT完了");
                        UIStateFixing();
                        m_UIMoveManager.PosRatioZeroReset();

                    }

                    break;
                case UIMoveManager.UI_MOVESTATE.ALL_MOVESTATE:
                    break;
                default:
                    break;
            }
        }
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

        //星の数更新
        public void StageStarUpdate()
        {

            // public GameObject[] m_StageObj = new GameObject[(int)IN_WORLD_NO.ALLSTAGE];//ワールド内ステージUI用オブジェクト
            // UI内のステージ１～５のオブジェクト取得
            for (int i = 0; i < (int)IN_WORLD_NO.ALLSTAGE; i++)
            {
                m_StageObj[i] = this.transform.GetChild(i + 2).gameObject;
                m_CompleteObj = m_StageObj[i].transform.GetChild((int)UI_INDEX.COMPLETE).gameObject;
                for (int star = 0; star < THREE; star++)
                {
                    //星3つのオブジェクト取得
                    m_Star_Obj[star] = m_StageObj[i].transform.GetChild(star).gameObject;
                }
                int CurrentStage = StageStatusManager.Instance.CurrentWorld * 5 + i;
                m_ClearStatus = StageStatusManager.Instance.Stage_Status[CurrentStage];
                switch (m_ClearStatus)
                {
                    case CLEAR_STATUS.NOT:
                        SwitchingActive.GameObject_OFF(m_Star_Obj[0]);
                        SwitchingActive.GameObject_OFF(m_Star_Obj[1]);
                        SwitchingActive.GameObject_OFF(m_Star_Obj[2]);
                        m_CompleteObj.SetActive(false);
                        break;
                    case CLEAR_STATUS.ONE:
                        SwitchingActive.GameObject_ON(m_Star_Obj[0]);
                        SwitchingActive.GameObject_OFF(m_Star_Obj[1]);
                        SwitchingActive.GameObject_OFF(m_Star_Obj[2]);
                        m_CompleteObj.SetActive(false);
                        break;
                    case CLEAR_STATUS.TWO:
                        SwitchingActive.GameObject_ON(m_Star_Obj[0]);
                        SwitchingActive.GameObject_ON(m_Star_Obj[1]);
                        SwitchingActive.GameObject_OFF(m_Star_Obj[2]);
                        m_CompleteObj.SetActive(false);
                        break;
                    case CLEAR_STATUS.THREE:
                        SwitchingActive.GameObject_ON(m_Star_Obj[0]);
                        SwitchingActive.GameObject_ON(m_Star_Obj[1]);
                        SwitchingActive.GameObject_ON(m_Star_Obj[2]);
                        m_CompleteObj.SetActive(true);
                        break;
                    case CLEAR_STATUS.STATUS_NUM:
                        break;
                    default:
                        break;
                }


            }
        }//StageStarUpdate END

        //UI_MOVESTATEを変更する
        public void UIStateMoveOut()
        {
            m_UIMoveState = UIMoveManager.UI_MOVESTATE.MOVEOUT;
        }
        public void UIStateMoveIn()
        {
            m_UIMoveState = UIMoveManager.UI_MOVESTATE.MOVEIN;
        }
        public void UIStateFixing()
        {
            m_UIMoveState = UIMoveManager.UI_MOVESTATE.FIXING;
        }

        //小人の取得数を保持しているの数値からテキストに変換する
        public void SetMinionCount()
        {
            int CurrentWorld = StageStatusManager.Instance.CurrentWorld;
            int CurrentStage = CurrentWorld * 5;
            for (int i = 0; i < (int)IN_WORLD_NO.ALLSTAGE; i++)
            {
                if (m_StageMaxMinions[CurrentStage + i] < 10)
                {
                    //一桁なら十の位に０を追加する。
                    m_MinionCount[i].text = "0" + StageStatusManager.Instance.Minion_Count[CurrentStage + i].ToString();
                }
                else
                {
                    m_MinionCount[i].text = StageStatusManager.Instance.Minion_Count[CurrentStage + i].ToString();

                }

            }
        }

        //小人の最大数をinspector上の数値からテキストに変換する
        public void SetMinionMaxCount()
        {
            int CurrentWorld = StageStatusManager.Instance.CurrentWorld;
            int CurrentStage = CurrentWorld * 5;
            for (int i = 0; i < (int)IN_WORLD_NO.ALLSTAGE; i++)
            {
                if (m_StageMaxMinions[CurrentStage + i] < 10)
                {
                    //一桁なら十の位に０を追加する。
                    m_MinionMaxCount[i].text = "0" + m_StageMaxMinions[CurrentStage + i].ToString();
                }
                else
                {
                    //二桁ならそのまま表示する。
                    m_MinionMaxCount[i].text =  m_StageMaxMinions[CurrentStage + i].ToString();

                }

            }
        }
    } //public class WorldStatusUIManager : MonoBehaviour    END
} //namespace TeamProject    END
