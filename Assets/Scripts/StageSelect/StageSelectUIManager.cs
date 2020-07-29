using UnityEngine;

namespace TeamProject
{
    //ステージセレクト内のUIを管理するクラス（クリア状況UIとワールド移動中UI）
    public class StageSelectUIManager : MonoBehaviour
    {
        [Header("クリア状況UIの移動する時間")]
        public float m_UIMoveOut_Time;
        public float m_UIMoveIn_Time;
        [Header("ステージUI背景の移動する時間")]
        public float m_UIMoveUP_Time;
        public float m_UIMoveDown_Time;
        //現在ワールド位置 
        //StageStatusManager.Instance.CurrentWorld;
        //public GameObject m_Canvas;//一番上の親オブジェクト
        //public GameObject m_Next;//次のワールド名UI用オブジェクト
        //public GameObject m_Current;//現在のワールド名UI用オブジェクト

        //スプライトのパス（固定部分）
        public const string m_ConstPath = "Sprites/StageSelect/UI_WorldStatus/UI_StageSelect_Menu_";
        //
        //スプライトのパス
        //public string[] m_UI_WorldName = {
        //    "World01","World02","World03","World04"
        //};
        //public string m_NextName;//次ワールド用パス
        //public string m_CurrentName;//前ワールド用パス

        private CurrentToNextWorldUIManager m_ToNextWUI;
        private WorldStatusUIManager m_WorldStatusUI;
        private UIBackGroundCurrentStage m_UIBGCurrentStage;
        private UIBackGroundCurrentStage m_UIBG_Arrow;
        private WorldSelectArrow m_WorldSelectArrow;
        private StageSelectArrow m_StageSelectArrow;
        private GameObject m_ButtonInformationObj;
        private GameObject m_AB_Button_UI_Obj;
        private GameObject m_Skip_UI_Obj;

        private void Awake()
        {
            m_StageSelectArrow = this.transform.GetChild(0).transform.GetChild(0).GetComponent<StageSelectArrow>();
            m_WorldSelectArrow = this.transform.GetChild(0).transform.GetChild(1).GetComponent<WorldSelectArrow>();
            GameObject WorldStatusObj = this.transform.GetChild(0).transform.GetChild(2).gameObject;
            m_WorldStatusUI = WorldStatusObj.GetComponent<WorldStatusUIManager>();
            m_UIBGCurrentStage = WorldStatusObj.transform.GetChild(0).transform.GetChild(0).GetComponent<UIBackGroundCurrentStage>();
            m_UIBG_Arrow = WorldStatusObj.transform.GetChild(0).transform.GetChild(1).GetComponent<UIBackGroundCurrentStage>();
            m_ToNextWUI = this.transform.GetChild(0).transform.GetChild(3).GetComponent<CurrentToNextWorldUIManager>();
        }
        // Start is called before the first frame update
        void Start()
        {
            //右下のボタン操作説明UIのゲームオブジェクト取得
            m_ButtonInformationObj = this.transform.GetChild(0).transform.GetChild(4).gameObject;
            //ABボタンUIアクティブ化
            SwitchingActive.GameObject_OFF(m_ButtonInformationObj);

            //ABボタンUIののゲームオブジェクト取得
            m_AB_Button_UI_Obj = m_ButtonInformationObj.transform.GetChild(0).gameObject;
            //SKIPボタンUIののゲームオブジェクト取得
            m_Skip_UI_Obj = m_ButtonInformationObj.transform.GetChild(1).gameObject;

            //入力デバイスに応じたUIへの表示切り替え
            SwitchingUISprite();

            m_WorldStatusUI.StartWorldNameIcon();
            m_ToNextWUI.ChangeWorldNameIcon();
        }

        // Update is called once per frame
        //void Update()
        //{
        //    //m_WorldStatusUI.WorldStatusUIUpdate();
        //    //m_ToNextWUI.ToNextWorldUIUpdate();
        //    //m_UIBGCurrentStage.UIBackGroundCurrentStageUpdate();
        //    //m_UIBG_Arrow.UIBackGroundCurrentStageUpdate();
        //    //m_StageSelectArrow.StageSelectArrowUpdate();
        //    //m_WorldSelectArrow.WorldSelectArrowUpdate();
        //}//void Update()    END

        //UI用の更新関数
        public void StageSelectUIUpdate()
        {
            m_WorldStatusUI.WorldStatusUIUpdate();
            m_ToNextWUI.ToNextWorldUIUpdate();
            m_UIBGCurrentStage.UIBackGroundCurrentStageUpdate();
            m_UIBG_Arrow.UIBackGroundCurrentStageUpdate();
            m_StageSelectArrow.StageSelectArrowUpdate();
            m_WorldSelectArrow.WorldSelectArrowUpdate();

        }

        //ワールドを示すUIの差し替え
        public void ChangeWorldNameIcon()
        {
            m_WorldStatusUI.ChangeWorldNameIcon();
            m_ToNextWUI.ChangeWorldNameIcon();
            //
            //
        }//

        public void SkipButtonSetActive(bool _bool)
        {
            m_ButtonInformationObj.SetActive(_bool);
        }
        //スキップボタンUIのアクティブ化
        public void SkipButtonActivate()
        {
            SwitchingActive.GameObject_ON(m_ButtonInformationObj);
        }
        //ABボタンUIのアクティブ化
        public void ABButtonActivate()
        {
            SwitchingActive.GameObject_OFF(m_ButtonInformationObj);
        }

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
        //UIBackGroundCurrentStageのオブジェクトを渡す
        public UIBackGroundCurrentStage GetUIBackGroundCurrentStageObject()
        {
            return m_UIBGCurrentStage;
        }
        public UIBackGroundCurrentStage GetUIBG_ArrowObject()
        {
            return m_UIBG_Arrow;
        }
        public StageSelectArrow GetStageSelectArrow()
        {
            return m_StageSelectArrow;
        }
        public WorldSelectArrow GetWorldSelectArrow()
        {
            return m_WorldSelectArrow;
        }

        //入力デバイスに応じたUIへの表示切り替え
        public void SwitchingUISprite()
        {
            switch (InputManager.InputManager.ActivePad)
            {
                case InputManager.GamePad.Keyboad:
                    Debug.Log("Keyboard入力");
                    SwitchingKeyboardUI();
                    break;
                case InputManager.GamePad.Xbox:
                    Debug.Log("Xbox入力");
                    SwitchingXboxUI();
                    break;
            }
        }

        //Xbox用UIのアクティブ化
        public void SwitchingXboxUI()
        {
            //ABButtonUI切り替え
            SwitchingActive.GameObject_OFF(m_AB_Button_UI_Obj);

            //SKIPUI切り替え
            SwitchingActive.GameObject_OFF(m_Skip_UI_Obj);

        }

        //Keyboard用UIのアクティブ化
        public void SwitchingKeyboardUI()
        {
            //ABButtonUI切り替え
            SwitchingActive.GameObject_ON(m_AB_Button_UI_Obj);

            //SKIPUI切り替え
            SwitchingActive.GameObject_ON(m_Skip_UI_Obj);

        }
    } //public class StageSelectUIManager : MonoBehaviour    END
} //namespace TeamProject    END
