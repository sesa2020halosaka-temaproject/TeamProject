﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    public class IceGimmick : MonoBehaviour
    {
        private Animator anim;

        private Collider coll;

        [SerializeField]
        private uint fallNum = 1;

        private void Start()
        {
            coll = GetComponent<Collider>();

            Debug.Log(coll+"gporekaopgkrepogokerakgpoaker ogkpaeok");
        }
        
        // 壊れるかどうかのJudge
        public bool Judge(uint _minionNum)
        {
            return fallNum <= _minionNum;
        }

        // 当たり判定を消す処理
        public void StartBreak()
        {
            coll.enabled = false;
        }

        // オブジェクト自体を消す処理
        public void EndBreak()
        {
            Destroy(this);
        }
    }

    //public class IceGimmick : System.TransitionObject
    //{
    //    public enum TRANS
    //    {
    //        None,
    //        BreakStart,
    //        BreakEnd,
    //        Max
    //    }

    //    private void Start()
    //    {
    //        SetMaxFunctionSize((uint)TRANS.Max);

    //        CreateFunction((uint)TRANS.None,);

    //        SetFunction((uint)TRANS.None);
    //    }

    //    private void None()
    //    {

    //    }

    //    private void BreakStart()
    //    {

    //    }

    //    private void BreakEnd()
    //    {

    //    }
    //}
}