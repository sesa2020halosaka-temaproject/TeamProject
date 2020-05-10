using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //ステージナンバープレートの処理
    public class StageNameCanvas : MonoBehaviour
    {
        public Canvas m_Canvas;//キャンバス用オブジェクト
        public int m_SortingOrder;//ソートの優先度

        [Header("ステージの番号（１～）")]
        public int m_StageNumber;//ステージ番号
        public CLEAR_STATUS m_ClearStatus;//ステージのクリア状況

        //UI特定用の列挙
        public enum UI_INDEX
        {
            SATGENAME,  //ステージ名
            STAR01,     //左の星
            STAR02,     //中の星
            STAR03,     //右の星
            ALL_INDEX   //インデックス数
        }
        public GameObject[] m_Star_Obj = new GameObject[(int)UI_INDEX.ALL_INDEX];//星用ゲームオブジェクト

        // Start is called before the first frame update
        void Start()
        {

            //キャンバスにオブジェクトがアタッチされていない時
            if (m_Canvas == null)
            {
                m_Canvas = this.gameObject.GetComponent<Canvas>();
            }


            //優先度が0以下だったら
            if (m_SortingOrder <= 0)
            {
                m_Canvas.sortingOrder = 1;
            }
            else
            {
                m_Canvas.sortingOrder = m_SortingOrder;
            }
            //親ゲームオブジェクト（自身）の取得
            GameObject ParentObj = this.gameObject;
            //星用ゲームオブジェクト取得
            for (int i = 0; i < (int)UI_INDEX.ALL_INDEX; i++)
            {
                m_Star_Obj[i] = ParentObj.transform.GetChild(i).gameObject;
            }

            //ステージ番号がセットされていない時はエラー
            if (m_StageNumber < 0)
            {
                Debug.Log(this.name + "B:m_StageNumber = " + m_StageNumber + "！");
                Debug.Log(this.name + "のステージ番号が登録されていません！");
            }
            m_ClearStatus = StageStatusManager.Instance.Stage_Status[m_StageNumber];

            switch (m_ClearStatus)
            {
                case CLEAR_STATUS.NOT:
                    SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR01]);
                    SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR02]);
                    SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR03]);
                    break;
                case CLEAR_STATUS.ONE:
                    SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR01]);
                    SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR02]);
                    SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR03]);
                    break;
                case CLEAR_STATUS.TWO:
                    SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR01]);
                    SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR02]);
                    SwitchingActive.GameObject_OFF(m_Star_Obj[(int)UI_INDEX.STAR03]);
                    break;
                case CLEAR_STATUS.THREE:
                    SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR01]);
                    SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR02]);
                    SwitchingActive.GameObject_ON(m_Star_Obj[(int)UI_INDEX.STAR03]);
                    break;
                case CLEAR_STATUS.STATUS_NUM:
                    break;
                default:
                    break;
            }
        }// void Start()    END

        // Update is called once per frame
        //void Update()
        //{

        //}
    } //public class StageNameCanvas : MonoBehaviour    END
} //namespace TeamProject    END
