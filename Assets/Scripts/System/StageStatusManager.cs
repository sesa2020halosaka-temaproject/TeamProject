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
    //ワールド番号
    public enum WORLD_NO
    {
        W1 = 0, W2, W3, W4,    //ワールド番号
        ALL_WORLD              //全ワールド数
    }

    //ワールド内ステージ番号（Stage01～05）
    public enum IN_WORLD_NO
    {
        S1 = 0, S2, S3, S4, S5,//ワールド内のステージ番号
        ALLSTAGE               //ワールド内の全ステージ数
    };


    //=================================
    //現在のステージ と ステージのクリア状況の管理クラス(シングルトン)
    public class StageStatusManager : SingletonMonoBehaviour<StageStatusManager>
    {
        //現在のステージ
        public STAGE_NO CurrentStage;
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
        //現在のワールド
        public int CurrentWorld
        {
            get
            {
                var world = (int)CurrentStage / 5;
                return world;
            }
        }
        //次のワールド
        public int NextWorld
        {
            get
            {
                var world = CurrentWorld + 1;
                if (world >= 4) { world = 0; }

                return world;
            }
        }
        //前のワールド
        public int PrevWorld
        {
            get
            {
                var world = CurrentWorld - 1;
                if (world < 0) { world = 3; }
                return world;
            }
        }
        //現在のワールド内のステージ番号
        public int StageInWorld
        {
            get
            {
                var world = (int)CurrentStage % 5;
                return world;
            }
        }

        public bool m_WatchOpeningFlag;//オープニングを見たかどうかフラグ(true:見た,false:見てない)
        public bool m_LastStageClearFlag;//ラストステージをクリアしたかどうかフラグ(true:クリア済み,false:未クリア)
        public bool m_RemovalLimitFlag;//全ステージ移動可能フラグ(true:制限解除,false:制限あり)
        public bool m_AllUnlockFlag;//オールアンロックフラグ(true:オールアンロック,false:通常)
        public CLEAR_STATUS[] Stage_Status = new CLEAR_STATUS[(int)STAGE_NO.STAGE_NUM];//各ステージのクリア状況
        public int[] Minion_Count = new int[(int)STAGE_NO.STAGE_NUM];//各ステージの小人取得数

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
            //Debug.Log("Awake!");

            for (int i = 0; i < (int)STAGE_NO.STAGE_NUM; i++)
            {
                //全ステージを未クリア状態にする
                Stage_Status[i] = CLEAR_STATUS.NOT;
                //小人の取得数をゼロリセット
                Minion_Count[i] = 0;
                // Debug.Log("未クリア状態にしました。");
            }
            CurrentStage = STAGE_NO.STAGE01;
            // Debug.Log("STAGE01をセット。");

            //オープニング見たかどうかフラグのリセット
            m_WatchOpeningFlag = false;

            //破棄されないようにする
            DontDestroyOnLoad(this.gameObject);
        }//public void Awake()  END

        public void AllUnlockActivation()
        {
            for (int i = 0; i < (int)STAGE_NO.STAGE_NUM; i++)
            {
                //全ステージを星全取得状態にする
                Stage_Status[i] = CLEAR_STATUS.THREE;
                //小人の取得数をゼロリセット
                Minion_Count[i] = 0;
            }
            //オープニング見たかどうかフラグをONにする
            m_WatchOpeningFlag = true;
            //ラストステージをクリアしたかどうかフラグをONにする
            m_LastStageClearFlag = true;
            //全ステージ移動可能フラグをONにする
            m_RemovalLimitFlag = true;
            //オールアンロックフラグをONにする
            m_AllUnlockFlag = true;
            Debug.Log("オールアンロックモード起動しました。");

        }
        //  public 
    }
}