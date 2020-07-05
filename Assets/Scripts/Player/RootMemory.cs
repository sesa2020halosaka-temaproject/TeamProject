using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TeamProject
{
    public struct MinionPair
    {
        public Tuple<string, int> befor;
        public Tuple<string, int> after;
    }
    // ルートを覚えておく用
    public class RootMemory : System.Singleton<RootMemory>
    {
        private List<MinionPair> pairList = new List<MinionPair>();

        //　登録
        public void Regist(Minion befor, Minion after)
        {
            // Pair用のデータに変換
            MinionPair pair;

            if (befor == null) pair.befor = Tuple.Create("None", 0);
            else pair.befor = Tuple.Create(befor.gameObject.name, befor.Floor);

            pair.after = Tuple.Create(after.gameObject.name, after.Floor);

            // あれば入れない
            if (!pairList.Contains(pair))
            {
                pairList.Add(pair);
            }
        }

        //　登録
        public void Regist(Minion befor, Goal after)
        {
            // Pair用のデータに変換
            MinionPair pair;
            if (befor == null) pair.befor = Tuple.Create("None", 0);
            else pair.befor = Tuple.Create(befor.gameObject.name, befor.Floor);

            pair.after = Tuple.Create(after.gameObject.name, after.Floor);

            // あれば入れない
            if (!pairList.Contains(pair))
            {
                pairList.Add(pair);
            }
        }

        // 検索
        public bool Search(Minion befor, Minion after)
        {
            // Pair用のデータに変換
            MinionPair pair;
            if (befor == null) pair.befor = Tuple.Create("None", 0);
            else pair.befor = Tuple.Create(befor.gameObject.name, befor.Floor);

            pair.after = Tuple.Create(after.gameObject.name, after.Floor);

            // 結果を送信
            return pairList.Contains(pair);
        }
        public bool Search(Minion befor, Goal after)
        {
            // Pair用のデータに変換
            MinionPair pair;
            if (befor == null) pair.befor = Tuple.Create("None", 0);
            else pair.befor = Tuple.Create(befor.gameObject.name, befor.Floor);

            pair.after = Tuple.Create(after.gameObject.name, after.Floor);

            // 結果を送信
            return pairList.Contains(pair);
        }

        // リセット
        public void Reset()
        {
            pairList.Clear();
        }

    }
}