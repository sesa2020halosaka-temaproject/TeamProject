using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TeamProject {

    public class MinionPlatoon : System.TransitionObject
    {
        private List<Minion> minionList;

        public List<Minion> MinionList { get { return minionList; } }

        public int MinionNum { get { return minionList.ToArray().Length; } }

        [SerializeField]
        // 停止時のプレイヤーとの半径
        private float WaitaRadius = 1f;

        [SerializeField]
        private float Wight = 0.5f;

        private GameObject player;

        public enum TRANS
        {
            None,
            Move,
            Wait,
            Max
        }
        
        // Start is called before the first frame update
        void Start()
        {
            minionList = new List<Minion>();

            SetMaxFunctionSize((int)TRANS.Max);

            CreateFunction((uint)TRANS.None, None);
            CreateFunction((uint)TRANS.Move, Move);
            CreateFunction((uint)TRANS.Wait, Wait);

            SetFunction((uint)TRANS.None);

            player = gameObject;
        }
        
        private void None()
        {

        }

        private void Move()
        {
            Debug.Log("MinionMove");
            // ミニオンのペアを作る
            var pairList = new List<Tuple<Minion, Minion>>();

            // 数
            var minionNum = minionList.ToArray().Length;

            var pairNum = minionNum / 2;

            // pair作成
            for (int i = 0; i < pairNum; i++)
            {
                pairList.Add(new Tuple<Minion, Minion>(minionList[i * 2], minionList[i * 2 + 1]));
            }

            // 後ろのベース位置割り出し
            var back = player.transform.forward * -1 * WaitaRadius;

            for (int i = 0; i < pairList.ToArray().Length; i++)
            {
                var pairBack = back * (i + 1);

                // 左右
                pairList[i].Item1.TargetPosition = pairBack + player.transform.right * WaitaRadius* Wight + player.transform.position;
                pairList[i].Item2.TargetPosition = pairBack + player.transform.right * WaitaRadius * -1* Wight + player.transform.position;
            }

            if (minionNum % 2 == 1)
            {
                minionList[minionNum - 1].TargetPosition = back * (pairNum + 1) * WaitaRadius * Wight + player.transform.position;
            }
        }

        private void Wait()
        {
            Debug.Log("MinionWait");

            // 現在の隊列の量を取得
            var length = minionList.ToArray().Length;

            var oncAngle = 360f / length;

            for (int i = 0; i < length; i++)
            {
                var angle = oncAngle * i;

                float x = Mathf.Sin(angle * Mathf.Deg2Rad);
                float z = Mathf.Cos(angle * Mathf.Deg2Rad);

                // 位置情報設定
                minionList[i].TargetPosition = player.transform.position + new Vector3(x, 0f, z) * WaitaRadius;
            }

            Debug.Log("PlatoonNum" + (length + 1));
        }

        // 隊に入隊した時にthisを入れる
        public void In(Minion _minion)
        {
            minionList.Add(_minion);
        }

        // 脱隊
        // 仕様が変わったら作る
        public void Out(Minion _minion)
        {

        }
    }
}