using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace TeamProject
{
    //LensDistortionコンポーネントを制御するクラス
    public class LensDistortionManager : MonoBehaviour
    {
        public VolumeProfile m_VolumeProfile;
        private LensDistortion m_LensDistortion;
        public float m_CurrentSlerpRatio;
        private void Awake()
        {
            if (m_VolumeProfile==null)
            {
                Debug.LogError("m_VolumeProfileがNULLです！");
                return;
            }
            else
            {
                //LensDistortionコンポーネントの取得
                m_LensDistortion = m_VolumeProfile.Add<LensDistortion>();
                //intensityを変更できるようにする
                m_LensDistortion.intensity.overrideState = true;

            }
        }
        // Start is called before the first frame update
        void Start()
        {
            ResetSlerpRatio();
        }
        private void OnDisable()
        {
            Debug.Log("LensDistortionコンポーネントを取り除きました！すごいでしょ！");
            m_VolumeProfile.Remove<LensDistortion>();
        }
        //Intensityのスラープ処理
        public void IntensitySlerp(float StartIntensity, float EndIntensity, float SlerpTime)
        {
            float TempIntensity;
            // 現在の画角の割合
            m_CurrentSlerpRatio += (Time.deltaTime / SlerpTime);

            // 現在の強度
            TempIntensity = Mathf.Lerp(StartIntensity, EndIntensity, m_CurrentSlerpRatio);
            //強度のセットのセット
            SetIntensity(TempIntensity);
        }
        //Intensityをセットする
        public void SetIntensity(float _intensity)
        {
            m_LensDistortion.intensity.value = _intensity;


        }
        //画角の割合のリセット
        public void ResetSlerpRatio()
        {
            m_CurrentSlerpRatio = 0.0f;
        }


    }//public class UIMoveManager END
}//namespace END