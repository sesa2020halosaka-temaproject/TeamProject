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
        public uint[] m_StageMinions = new uint[(int)STAGE_NO.STAGE_NUM];
        [Header("UIの移動する時間")]
        public float m_MoveTime;

        [Header("UIの画面内位置")]
        public Vector3 m_InPosition;
        [Header("UIの画面外位置")]
        public Vector3 m_OutPosition;
        //現在ワールド位置 
        //StageStatusManager.Instance.CurrentWorld;
        public GameObject m_Canvas;//一番上の親オブジェクト
                                   // public GameObject m_Next;//次のワールド名UI用オブジェクト
        public GameObject m_Current;//現在のワールド名UI用オブジェクト
        public GameObject[] m_StageObj = new GameObject[(int)IN_WORLD_NO.ALLSTAGE];//ワールド内ステージUI用オブジェクト

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
        //UI特定用の列挙
        public enum UI_INDEX
        {
            SATGENAME,     //ステージ名
            STAR01,        //左の星
            STAR02,        //中の星
            STAR03,        //右の星
            MINION_ICON,   //ドングリアイコン
            MINION_COUNT,  //小人数テキスト
            COMPLETE,      //コンプリートUI
            ALL_INDEX      //インデックス数
        }

        private void Awake()
        {
            m_UIMoveManager = new UIMoveManager();
            //m_Next = transform.GetChild(1).gameObject;//次のワールド名UI用オブジェクト
            m_Current = transform.GetChild(1).gameObject;//現在のワールド名UI用オブジェクト
            for (int i = 0; i < (int)IN_WORLD_NO.ALLSTAGE; i++)
            {
                m_StageObj[i] = this.transform.GetChild(i + 2).gameObject;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            //m_InPosition = this.GetComponent<RectTransform>(
            m_InPosition = this.transform.localPosition;
            m_OutPosition = new Vector3(this.transform.localPosition.x, -790, this.transform.localPosition.z);
            //StartWorldNameIcon();

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
                    m_UIMoveManager.UIMove(this.gameObject, m_InPosition, m_OutPosition, m_MoveTime);
                    m_UIMoveManager.UIMove(this.gameObject, m_OutPosition, m_InPosition, m_MoveTime);

                    break;
                case UIMoveManager.UI_MOVESTATE.MOVEOUT:
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
        //public void StageStarUpdate()
        //{
        //    for (int i = 0; i < (int)IN_WORLD_NO.ALLSTAGE; i++)
        //    {
        //        m_StageObj[i] = this.transform.GetChild(i + 2).gameObject;
        //    }

        //    //親ゲームオブジェクトの取得
        //    GameObject ParentObj = this.gameObject;
        //    //星用ゲームオブジェクト取得
        //    for (int i = 0; i < (int)UI_INDEX.ALL_INDEX; i++)
        //    {
        //        m_Star_Obj[i] = ParentObj.transform.GetChild(i).gameObject;
        //    }

        //    //ステージ番号がセットされていない時はエラー
        //    if (m_StageNumber < 0)
        //    {
        //        Debug.Log(this.name + "B:m_StageNumber = " + m_StageNumber + "！");
        //        Debug.Log(this.name + "のステージ番号が登録されていません！");
        //    }
        //    m_ClearStatus = StageStatusManager.Instance.Stage_Status[m_StageNumber];

        //    switch (m_ClearStatus)
        //    {
        //        case CLEAR_STATUS.NOT:
        //            SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR01]);
        //            SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR02]);
        //            SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR03]);
        //            break;
        //        case CLEAR_STATUS.ONE:
        //            SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR01]);
        //            SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR02]);
        //            SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR03]);
        //            break;
        //        case CLEAR_STATUS.TWO:
        //            SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR01]);
        //            SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR02]);
        //            SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR03]);
        //            break;
        //        case CLEAR_STATUS.THREE:
        //            SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR01]);
        //            SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR02]);
        //            SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR03]);
        //            break;
        //        case CLEAR_STATUS.STATUS_NUM:
        //            break;
        //        default:
        //            break;
        //    }

        //}//StageStarUpdate END

        public void UIOutMove()
        {
            m_UIMoveState = UIMoveManager.UI_MOVESTATE.MOVEOUT;
        }
        public void UIInMove()
        {
            m_UIMoveState = UIMoveManager.UI_MOVESTATE.MOVEIN;
        }
    } //public class WorldStatusUIManager : MonoBehaviour    END
} //namespace TeamProject    END
