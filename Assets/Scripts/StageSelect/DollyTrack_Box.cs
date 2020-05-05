using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TeamProject
{
    //DollyTrackの格納用クラス

    public class DollyTrack_Box : MonoBehaviour
{
        public CinemachinePathBase m_Dolly_FIXING;
        public CinemachinePathBase[] m_Dolly_GO_4;
        public CinemachinePathBase[] m_Dolly_BACK_4;
        public CinemachinePathBase[] m_Dolly_W1toW2;
        public CinemachinePathBase[] m_Dolly_W2toW1;


        // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    }//public class DollyTrack_Box : MonoBehaviour END
}//namespace END