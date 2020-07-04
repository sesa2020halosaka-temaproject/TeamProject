using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

namespace TeamProject
{
    //DollyTrackのWayPointの座標格納用クラス
    public class DollyTrack_Position : MonoBehaviour
    {
        [Header("WayPointの座標保持用")]
        public Vector3[] m_WayPointPosition = new Vector3[(int)STAGE_NO.STAGE_NUM];

        public DollyTrack_Box m_DoTr_Box;

        private void Awake()
        {
            m_DoTr_Box = this.GetComponent<DollyTrack_Box>();
        }
        // Start is called before the first frame update
        void Start()
        {
            m_DoTr_Box.SetWayPointPosition();
        }


        public Vector3 GetCurrentPosition()
        {
            int StageNo = (int)StageStatusManager.Instance.CurrentStage;
            return m_WayPointPosition[StageNo];
        }
    }//public class DollyTrack_Position : MonoBehaviour END
}//namespace END