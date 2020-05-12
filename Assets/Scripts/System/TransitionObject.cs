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

            // 設定できるFunctionの最大数
            private const uint MAX_FIX_FUNTION_NUM = 999;

            private Function[] fixFunction;

            private uint maxFixFunctionNum = 0;

            private uint nowFixFunctionNum = MAX_FUNTION_NUM;

            // 現在のFunctionのNumberを取得
            public uint NowFunctionNum { get { return nowFunctionNum; } }

            // 現在のFixFunctionのNumberを取得
            public uint NowFixFunctionNum { get { return nowFixFunctionNum; } }

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

            void FixedUpdate()
            {
                // 設定されていないので無視
                if (nowFixFunctionNum == MAX_FIX_FUNTION_NUM) return;

                // 設定されているFunctionを実行
                fixFunction[nowFixFunctionNum]();
            }

            // Functionの最大数を設定する
            protected void SetMaxFunctionSize(uint _maxFunctionNum) {
                function = new Function[_maxFunctionNum];
                maxFunctionNum = _maxFunctionNum;
            }

            // Functionを設定する
            protected bool CreateFunction(uint _functionNum, Function _setFunction)
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

            public void SetMaxFixFunctionSize(uint _maxFunctionNum)
            {
                fixFunction = new Function[_maxFunctionNum];
                maxFixFunctionNum = _maxFunctionNum;
            }

            // Functionを設定する
            protected bool CreateFixFunction(uint _functionNum, Function _setFunction)
            {
                // 配列の最大数より大きければfalseを返す
                if (maxFixFunctionNum < _functionNum) return false;

                fixFunction[_functionNum] = _setFunction;

                return true;
            }

            protected bool SetFixFunction(uint _functionNum)
            {
                // 配列の最大数より大きければfalseを返す
                if (maxFixFunctionNum < _functionNum) return false;

                nowFixFunctionNum = _functionNum;

                return true;
            }
        }
    }
}
