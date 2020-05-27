using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{

    //UIを画面内外に動かすクラス
    public class UIMoveManager
    {
        public float m_PosRatio;//現在位置の割合
        //
        public enum UI_MOVESTATE
        {
            FIXING,         //固定
            MOVEIN,         //画面の外から内へ
            MOVEOUT,        //画面の内から外へ
            ALL_MOVESTATE  //全状態数

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
            //float distance_two = Vector3.Distance(StartPosition, EndPosition);

            // 現在の位置
            m_PosRatio += (Time.deltaTime / MoveTime);/// distance_two;

            // オブジェクトの移動(ここだけ変わった！)

            _GameObject.transform.localPosition = Vector3.Lerp(StartPosition, EndPosition, m_PosRatio);
        }

        public void PosRatioZeroReset()
        {
            m_PosRatio = 0.0f;
        }
    }//public class UIMoveManager END
}//namespace END