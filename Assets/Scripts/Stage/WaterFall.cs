using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    //滝の音に関するクラス
    public class WaterFall : MonoBehaviour
{
        [Header("滝の音のONOFF切り替え")]
        public bool m_WaterFall_Flag;
        private bool m_PrevLoop_Flag;//前のループ時のフラグ

        [Header("滝の音（開始時に自動取得）")]
        [SerializeField]
        private AudioClip AudioClip;
        [SerializeField]
        private AudioSource AudioSource = default;
        [SerializeField]
        private bool is3D = true;

        private void Awake()
        {
            //滝のSEを取得
            AudioClip = Resources.Load<AudioClip>(SEPath.SE_WATERFALL);
            if (AudioClip == null)
            {
                Debug.LogError(AudioClip + " not found");
            }
            AudioSource.loop = true;
            AudioSource.spatialBlend = is3D ? 1f : 0f;

            AudioSource.volume = 1.0f;
            AudioSource.clip = AudioClip;

        }
        // Start is called before the first frame update
        void Start()
    {
            WaterFallSound();
        }

        // Update is called once per frame
        void Update()
    {
            if (m_WaterFall_Flag!=m_PrevLoop_Flag)
            {
                
                WaterFallSound();
                m_PrevLoop_Flag = m_WaterFall_Flag;
            }
    }

        public void WaterFallSound()
        {
            //滝の音
            if (m_WaterFall_Flag)
            {
                //フラグがONなら鳴らす
                AudioSource.Play();
            }
            else
            {
                AudioSource.Stop();
            }

        }
    }//public class WaterFall : MonoBehaviour END
}//namespace END