using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{

    //UIを画面内外に動かすクラス
    public class UIMoveManager
    {

        //
        public enum UI_MOVESTATE
        {
            FIXING,         //
            MOVEIN,         //
            MOVEOUT,        //
            ALL_MOVESTATE  //

        }
        public UI_MOVESTATE m_UI_MoveState = UI_MOVESTATE.FIXING;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        public void UIMove(GameObject _GameObject, Vector3 StartPosition, Vector3 EndPosition, float MoveTime)
        {        //二点間の距離を代入(スピード調整に使う)
            float distance_two = Vector3.Distance(StartPosition, EndPosition);

            // 現在の位置
            float present_Location = (Time.deltaTime / MoveTime) / distance_two;

            // オブジェクトの移動(ここだけ変わった！)

            _GameObject.transform.localPosition = Vector3.Slerp(StartPosition, EndPosition, present_Location);
        }
    }//public class UIMoveManager END
}//namespace END