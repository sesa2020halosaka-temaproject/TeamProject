using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    namespace System
    {
        // デリゲート宣言
        public delegate void Function();

        // 状態遷移が頻繁に起こるオブジェクトの基底クラス
        // 基底クラスなのでScriptをゲームオブジェクトに貼りつけて
        // 使用はしない
        // 注意:継承後のクラスのUpdateを必ず消してください
        public class TransitionObject : MonoBehaviour
        {
            // 設定できるFunctionの最大数
            private const uint MAX_FUNTION_NUM = 999;

            private Function[] function;

            private uint maxFunctionNum = 0;

            private uint nowFunctionNum = MAX_FUNTION_NUM;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                // 設定されていないので無視
                if (nowFunctionNum == MAX_FUNTION_NUM) return;

                // 設定されているFunctionを実行
                function[nowFunctionNum]();
            }
            
            // Functionの最大数を設定する
            public void SetMaxFunctionSize(uint _maxFunctionNum) {
                function = new Function[_maxFunctionNum];
                maxFunctionNum = _maxFunctionNum;
            }

            // Functionを設定する
            public bool CreateFunction(uint _functionNum, Function _setFunction)
            {
                // 配列の最大数より大きければfalseを返す
                if (maxFunctionNum < _functionNum) return false;
                
                function[_functionNum] = _setFunction;

                return true;
            }

            public bool SetFunction(uint _functionNum)
            {
                // 配列の最大数より大きければfalseを返す
                if (maxFunctionNum < _functionNum) return false;

                nowFunctionNum = _functionNum;

                return true;
            }
        }
    }
}
