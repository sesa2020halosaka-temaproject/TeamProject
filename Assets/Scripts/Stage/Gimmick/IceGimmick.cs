﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    public class IceGimmick : MonoBehaviour
    {
        private Animator anim;

        private Collider coll;

        [SerializeField] 
        private Collider collChild;

        [SerializeField]
        private uint fallNum = 1;

        private Animator iceAnime;

        private void Start()
        {
            coll = GetComponent<Collider>();

            iceAnime = GetComponentInChildren<Animator>();
        }
        
        // 壊れるかどうかのJudge
        public bool Judge(uint _minionNum)
        {
            return fallNum <= _minionNum;
        }

        // 当たり判定を消す処理
        public void StartBreak()
        {
            SEManager.Instance.Play(SEPath.SE_BROKEN_ICE);
            coll.enabled = false;
            collChild.enabled = false;
            iceAnime.SetTrigger("On");
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