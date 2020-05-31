using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{

    public class WayPoint_Box : MonoBehaviour
    {
        //ステージの正面に位置するドリールートのパスの位置を入れる用
        [Header("World01:夏ステージ用")]
        [Header("ステージの正面に位置するドリールートのWayPoint番号")]
        public float[] SummerStage_WayPoint;        //World01:夏ステージ用WayPoint番号
        [Header("World02:秋ステージ用")]
        public float[] FallStage_WayPoint;          //World02:秋ステージ用WayPoint番号
        [Header("World03:冬ステージ用")]
        public float[] WinterStage_WayPoint;        //World03:冬ステージ用WayPoint番号
        [Header("World04:春ステージ用")]
        public float[] SpringStage_WayPoint;        //World04:春ステージ用WayPoint番号
        [Header("WayPoint受け渡し用")]
        public float[] m_Stage_WayPoint;            //WayPoint番号受け渡し用配列

        //現在のワールドにおけるWayPoint格納配列のセット
        public void SetWayPoint()
        {
            switch (StageStatusManager.Instance.CurrentWorld)
            {
                case (int)WORLD_NO.W1:
                    m_Stage_WayPoint = SummerStage_WayPoint;
                    break;
                case (int)WORLD_NO.W2:
                    m_Stage_WayPoint = FallStage_WayPoint;
                    break;
                case (int)WORLD_NO.W3:
                    m_Stage_WayPoint = WinterStage_WayPoint;
                    break;
                case (int)WORLD_NO.W4:
                    m_Stage_WayPoint = SpringStage_WayPoint;
                    break;
                case (int)WORLD_NO.ALL_WORLD:
                default:
                    Debug.LogError("無効な状態です！");
                    break;
            }

        }
        // Start is called before the first frame update
        //void Start()
        //{

        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}
    }//public class WayPoint_Box : MonoBehaviour END
}//namespace END