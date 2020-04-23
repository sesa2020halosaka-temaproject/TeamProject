using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーのスクリプト

namespace TeamProject
{
    public class PlayerVer2 : System.TransitionObject
    {
        enum TRANSITION
        {
            None,   // 何もしない(使わないが一応)
            Move,   // 移動
            Choice, // 選択
            MAX,    // MAX(使用しない)
        }

        // 移動速度
        [SerializeField]
        [Header("移動速度調節")]
        [Range(1.0f,20.0f)]
        private float moveSpeed = 13.0f;

        // 段差判定の精度
        [SerializeField]
        [Header("段差判定の精度(プログラマーのみ変更許可)")]
        [Range(0.01f, 1.0f)]
        private float stepJudgeAccuracy;
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // 移動
        private void Move()
        {

        }

        // 選択
        private void Choice()
        {

        }
    }
}
