using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeamProject
{
    //================================
    //列挙体
    //ステージ番号
    public enum STAGE_NO
    {
        STAGE01 = 0, STAGE02, STAGE03, STAGE04, STAGE05,
        STAGE06, STAGE07, STAGE08, STAGE09, STAGE10,
        STAGE11, STAGE12, STAGE13, STAGE14, STAGE15,
        STAGE16, STAGE17, STAGE18, STAGE19, STAGE20,
        STAGE_NUM//ステージの総数

    }
    //クリア状況
    public enum CLEAR_STATUS
    {
        NOT,        //未クリア
        ONE,        //星1つ
        TWO,        //星2つ
        THREE,      //星3つ
        STATUS_NUM  //クリアステータスの総数
    }

    //=================================
    //現在のステージ と ステージのクリア状況の管理クラス(シングルトン)
    public class StageStatusManager : SingletonMonoBehaviour<StageStatusManager>
    {
        //次のステージ
        public STAGE_NO NextStage
        {
            get
            {
                var stage = CurrentStage + 1;
                if (stage >= STAGE_NO.STAGE_NUM) { stage = STAGE_NO.STAGE_NUM - 1; }
                return stage;
            }
        }

        //前のステージ
        public STAGE_NO PrevStage
        {
            get
            {
                var stage = CurrentStage - 1;
                if (stage < STAGE_NO.STAGE01) { stage = STAGE_NO.STAGE01; }
                return stage;
            }
        }

        //現在のステージ
        public STAGE_NO CurrentStage;

        public CLEAR_STATUS[] Stage_Status = new CLEAR_STATUS[(int)STAGE_NO.STAGE_NUM];//各ステージのクリア状況

        static private string[] stageString;
        public string[] StageString { get { return stageString; } }

        //===========================================================
        //関数ここから

        //=================================================================================
        //初期化　兼　オブジェクトを生成
        //=================================================================================
        //起動時に実行される
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            new GameObject("StageStatusManager", typeof(StageStatusManager));
            Debug.Log("StageStatusManagerオブジェクトを生成しました。");
            var s = new string[1];
            stageString = new string[(int)STAGE_NO.STAGE_NUM]
            {
                "Stage1_1","Stage1_2","Stage1_3","Stage1_4","Stage1_5",
                "Stage2_1","Stage2_2","Stage2_3","Stage2_4","Stage2_5",
                "Stage3_1","Stage3_2","Stage3_3","Stage3_4","Stage3_5",
                "Stage4_1","Stage4_2","Stage4_3","Stage4_4","Stage4_5",
            };
        }

        public void Awake()
        {
            if (this != Instance)
            {
                Destroy(this);
                return;
            }
            Debug.Log("Awake!");

            //全ステージを未クリア状態にする
            for (int i = 0; i < (int)STAGE_NO.STAGE_NUM; i++)
            {
                Stage_Status[i] = CLEAR_STATUS.NOT;
                // Debug.Log("未クリア状態にしました。");
            }
            CurrentStage = STAGE_NO.STAGE01;
            // Debug.Log("STAGE01をセット。");

            DontDestroyOnLoad(this.gameObject);
        }//public void Awake()  END

        //  public 
    }
}