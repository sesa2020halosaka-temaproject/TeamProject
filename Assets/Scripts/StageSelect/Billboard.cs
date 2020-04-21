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
        public class Billboard : MonoBehaviour
        {
            [Header("対象のカメラ")]
            public UnityEngine.Camera m_TargetCamera;

            private void LateUpdate()
            {
                // 回転をカメラと同期させる
                transform.rotation = m_TargetCamera.transform.rotation;
            }
        }
    }
}