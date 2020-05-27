using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    //ステージセレクト内のUIを管理するクラス（クリア状況UIとワールド移動中UI）
    public class StageSelectUIManager : MonoBehaviour
    {
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

        private CurrentToNextWorldUIManager m_ToNextWUI;
        private WorldStatusUIManager m_WorldStatusUI;

        //ステージセレクトUIの状態
        public enum UI_STATE
        {
            WAIT = 0,//待ち
            BEFORE_STAGE_MOVING,//ステージ移動前の準備
            STAGE_MOVING,//ステージ移動中
            BEFORE_WORLD_MOVING,//ワールド移動前の準備
            WORLD_MOVING,//ワールド移動中
            SCENE_MOVING,//シーン遷移中
            STATE_NUM    //状態の数
        }
        public UI_STATE select_state;


        private void Awake()
        {
            //m_Next = transform.GetChild(1).gameObject;//次のワールド名UI用オブジェクト
            //m_Current = transform.GetChild(2).gameObject;//現在のワールド名UI用オブジェクト

            m_ToNextWUI = this.transform.GetChild(0).transform.GetChild(3).gameObject.GetComponent<CurrentToNextWorldUIManager>();
            m_WorldStatusUI = this.transform.GetChild(0).transform.GetChild(2).gameObject.GetComponent<WorldStatusUIManager>();

        }
        // Start is called before the first frame update
        void Start()
        {
            m_WorldStatusUI.StartWorldNameIcon();
            m_ToNextWUI.ChangeWorldNameIcon();

        }

        // Update is called once per frame
        void Update()
        {
            m_WorldStatusUI.WorldStatusUIUpdate();

        }//void Update()    END

        //ワールドを示すUIの差し替え
        public void ChangeWorldNameIcon()
        {
            m_WorldStatusUI.ChangeWorldNameIcon();
            m_ToNextWUI.ChangeWorldNameIcon();
            //次のワールドの表示スプライトを差し替える
            //現在のワールドの表示スプライトを差し替える
        }//

        //CurrentToNextWorldUIManagerのオブジェクトを渡す
        public CurrentToNextWorldUIManager GetCurrentToNextWorldUIObject()
        {
            return m_ToNextWUI;
        }

        //WorldStatusUIManagerのオブジェクトを渡す
        public WorldStatusUIManager GetWorldStatusUIObject()
        {
            return m_WorldStatusUI;
        }

    } //public class StageSelectUIManager : MonoBehaviour    END
} //namespace TeamProject    END
