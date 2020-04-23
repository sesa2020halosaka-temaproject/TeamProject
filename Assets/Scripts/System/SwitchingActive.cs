using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //UIのONOFF切り替え用クラス
    public class SwitchingActive
    {
        private const int OFF = 0;//OFF用インデックス値
        private const int ON = 1;//ON用インデックス値
        //オブジェクトをOFF状態にする
        public static void GameObject_OFF(GameObject _Object)
        {
            _Object.transform.GetChild(OFF).gameObject.SetActive(true);
            _Object.transform.GetChild(ON).gameObject.SetActive(false);
        }

        //オブジェクトをON状態にする
        public static void GameObject_ON(GameObject _Object)
        {
            _Object.transform.GetChild(OFF).gameObject.SetActive(false);
            _Object.transform.GetChild(ON).gameObject.SetActive(true);
        }

    } //public class SwitchingActive　END
} //namespace TeamProject    END
