using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prafabin : MonoBehaviour
{
    public GameObject originObject; //オリジナルのオブジェクト

    // Use this for initialization
    void Start()
    {
        GameObject cloneObject = Instantiate(originObject, new Vector3(-1.0f, 0.0f, 0.0f), Quaternion.identity);

        // 取得した戻り値の活用例
        cloneObject.name = "originObject2"; // クローンしたオブジェクトの名前を変更
        cloneObject.transform.parent = this.transform; // GameManagerを親に指定
        cloneObject.transform.position = new Vector3(-1.0f, 1.0f, 0.0f); // 座標を変更
    }
}
