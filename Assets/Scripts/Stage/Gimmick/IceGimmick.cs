﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
using UnityEngine.VFX;

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

        [SerializeField]
        private MeshRenderer[] renderer;

        [SerializeField]
        private SpriteRenderer[] Srenderer;


        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private Minion[] fallMinion;

        [SerializeField]
        private VisualEffect effect;

        private void Start()
        {
            coll = GetComponent<Collider>();

            iceAnime = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (coll.enabled) return;

            var col = Srenderer[0].material.color;
            col.a -= speed;//  * Time.deltaTime;

            foreach (var itr in Srenderer) itr.gameObject.SetActive(false);

            foreach (var itr in renderer) itr.enabled = false;
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

            effect.gameObject.SetActive(true);

            coll.enabled = false;
            collChild.enabled = false;
            iceAnime.SetTrigger("On");
        }

        // オブジェクト自体を消す処理
        public void EndBreak()
        {
           //  foreach (var itr in fallMinion) itr.DonwFloor();
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