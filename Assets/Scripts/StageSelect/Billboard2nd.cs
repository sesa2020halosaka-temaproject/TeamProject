using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeamProject
{
    namespace Koshima
    {

        /// <summary>
        /// 常にカメラの方を向くオブジェクト回転をカメラに固定
        /// </summary>
        public class Billboard2nd : MonoBehaviour
        {
            [Header("対象のカメラ")]
            public UnityEngine.Camera m_TargetCamera;
            private void Start()
            {
                if (UnityEngine.Camera.main == null)
                {
                    Debug.Log("MainCameraタグがありません。");
                    if (m_TargetCamera == null)
                    {
                        Debug.Log("m_TargetCameraにカメラがセットされていません。");
                        Debug.LogError("MainCameraタグかm_TargetCameraにカメラのセットどちらかをしてください！");

                    }
                    else
                    {
                        Debug.Log("m_TargetCamera:" + m_TargetCamera.name);

                    }
                }
            }
            private void LateUpdate()
            {
                // MainCameraタグがあるかどうか
                if (UnityEngine.Camera.main)
                {
                    // 回転をMainCameraタグのついたカメラと同期させる

                    transform.rotation = UnityEngine.Camera.main.transform.rotation;

                }
                else
                {
                    // 回転をアタッチしたカメラと同期させる
                    transform.rotation = m_TargetCamera.transform.rotation;

                }
            }
        }
    }
}