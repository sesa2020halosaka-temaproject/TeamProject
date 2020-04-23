using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    //インスタンス
    private static T _instance;

    //インスタンスを外部から参照する用(getter)
    public static T Instance
    {
        get
        {
            //インスタンスがまだ作られていない
            if (_instance == null)
            {
                //シーン内からインスタンスを取得
                _instance = (T)FindObjectOfType(typeof(T));

                //シーン内に存在しない場合はエラー
                if (_instance == null)
                {
                    Debug.LogError(typeof(T) + " is nothing");
                }
            }

            //インスタンスが存在
            return _instance;
        }
    }

}
