using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    public class Root
    {
        private float[] lenghtArray;
        private Vector3[] outPosArray;

        static private Vector3 lerpStart;
        static private Vector3 lerpEnd;

        private Intersection intersection;

        private int arrayStartNum;  // スタート時の配列の番号
        private int arrayEndNum;    // エンド時の配列の番号

        private Root() { intersection = new Intersection(); }

        public static void SetLerp(Vector3 _start,Vector3 _end)
        {
            lerpStart = _start;
            lerpEnd = _end;

            // 長さを図る
            var lengthPosition = lerpEnd - lerpStart;

            lengthPosition.y = 0f;

        }

        public static Root Create(ref float[] rayTopLenghtArray ,int _rayNum)
        {
            // レイを作成する個数を
            // 長さと倍率で洗い出す
            var rayNum = _rayNum;
            var returnObject = new Root();

            // レイを個数分生成
            Ray[] rayArray = new Ray[rayNum];
            returnObject.lenghtArray = new float[rayNum];
            returnObject.outPosArray = new Vector3[rayNum];

            LayerMask mask;
            
            mask = LayerMask.NameToLayer("GroundCollider"); // GroundCollider 11
            
            for (int i = 0; i < rayNum; i++)
            {
                // 高さ入れる
                lerpStart.y = lerpEnd.y = rayTopLenghtArray[i];

                // 位置の補間を作成
                var inter = Vector3.Lerp(lerpStart, lerpEnd, i / rayNum);

                // Debug.Log(inter);
                // 真下に向かってRayを作成
                rayArray[i] = new Ray(inter, -Vector3.up);
            }

            bool hitNone = true;
            bool oldHit = false;

            for (int i = 0; i < rayNum; i++)
            {
                RaycastHit hit;
                var hitFlag = Physics.Raycast(rayArray[i], out hit, 10000f, mask);

                if (hitFlag)
                {
                    hitNone = false;
                }

                // 前回外れて今回当たる(BRANCH)
                if (!oldHit && hitFlag)
                {
                    returnObject.intersection.Branch((uint)i - 1);
                }

                // 前回当たって今回外れる(MERGE)
                if (oldHit && !hitFlag)
                {
                    returnObject.intersection.Merge((uint)i);
                }

                oldHit = hitFlag;

                // 長さを代入
                returnObject.lenghtArray[i] = hit.distance;
                returnObject.outPosArray[i] = hit.point;

                // レイのトップを前の当たり判定に変更
                rayTopLenghtArray[i] = returnObject.outPosArray[i].y;
            }
            
            // 当たり判定が一度もなかったので消去
            if (hitNone) return null;

            // 当たり判定が一つでもあったので作る
            return returnObject;
        }

        public static bool Check(Root _root, float _judgeHight, int _beforFrame, ref Vector3 _outPos)
        {
            var rayNum = (uint)_root.lenghtArray.Length;
            
            // 配列の低い数字から長さの差分をとる(初期は要素0を代入)
            float diff = 0;
            float oldLength = _root.lenghtArray[0];

            // 差分が設定値以上ならがけなので即座に
            // returnn false(以下は大丈夫)
            for (int i = 0; i < rayNum; i++)
            {
                diff = _root.lenghtArray[i] - oldLength;
                if (_judgeHight < -diff)
                {
                    _outPos = _root.outPosArray[i - _beforFrame]; 
                    return false;
                }
                oldLength = _root.lenghtArray[i];
            }

            _outPos = new Vector3();
            
            return true;
        }

        public static List<Root> CreateMainRoot(List<Root> _allRoot)
        {
            var roots = new List<Root>();

            // 初期化兼生成
            var intersectionParent = new IntersectionParent();

            float startLenght = 10000f;
            float endLenght = 10000f;

            // 最初のルートのナンバー
            int startArrayNum = 0;
            int endArrayNum = 0;

            // ベースルートの数
            int baseRootNum = _allRoot.ToArray().Length;

            // Intersection intersection = new Intersection();

            // スタートのルートとエンドのルート割り出し
            // 割り出し方法
            // 一番近い位置
            for (int i = 0; i < baseRootNum; i++)
            {
                var root = _allRoot[i];

                var start = lerpStart - root.outPosArray[0];
                var end = lerpEnd - root.outPosArray[root.lenghtArray.Length];

                // ベースルートの初期配列と終了配列番号の割り当て
                _allRoot[i].arrayStartNum = _allRoot[i].arrayEndNum = i;

                // 初期配列番号の割り出し
                if (start.magnitude < startLenght)
                {
                    startLenght = start.magnitude;
                    startArrayNum = i;
                }

                // 終了配列番号の割り出し
                if (end.magnitude < endLenght)
                {
                    endLenght = end.magnitude;
                    endArrayNum = i;
                }

                // ルート配列
                intersectionParent.Add(_allRoot[i].intersection);
            }

            //　全ての分岐処理の受け取り
            var intersectionParentLenght = intersectionParent.Instanc.ToArray().Length;

            for (int i = 0; i < intersectionParentLenght; i++)
            {
                var intersectionChiledLenght = intersectionParent.Instanc[i].Instans.ToArray().Length;
                var parentIns = intersectionParent.Instanc[i];

                for (int j = 0; j < intersectionChiledLenght; j++)
                {
                    var item2 = parentIns.Instans[j].Item2;

                    if (item2 == Intersection.Tree.BRANCH)
                    {
                        // ルートを増やす処理
                        // ルートがない部分は上配列のルートをコピー
                        // エンドが前回のものと新規のもので制作される変更される


                    }

                    if (item2 == Intersection.Tree.MERGE)
                    {
                        // ルートを結合させる処理
                        // エンドがどこか変更される
                    }
                }
            }

            // 全ての分木Treeを取得

            return roots;
        }

        public static Root operator+ (Root parent, Root childe)
        {
            return new Root();
        }
    }

    // 交点
    // 交点は配列要素が多いほうの配列番号が入る
    // _________[0]Array
    //    ＼____[1]Arrya
    // [0][1][2]Ray
    // 上記なら　
    // 0,1になる
    public class Intersection
    {
        // 状態
        public enum Tree
        {
            BRANCH,
            MERGE,
        }
        
        private List<Tuple<uint,  Tree>> instans;
        
        public List<Tuple<uint,  Tree>> Instans { get { return instans; } }

        public Intersection()
        {
            instans = new List<Tuple<uint,  Tree>>();
        }
        
        public void Merge(uint _inter)
        {
            instans.Add(new Tuple<uint,  Tree>(_inter,  Tree.MERGE));
        }

        public void Branch(uint _inter)
        {        
            instans.Add(new Tuple<uint,  Tree>(_inter,  Tree.BRANCH));
        }

        public void Add(Intersection _item)
        {
            Intersection returnObject = new Intersection();

            if (_item == null) returnObject.instans.AddRange(_item.instans);
        }

        public static Intersection operator +(Intersection _item1, Intersection _item2)
        {
            Intersection returnObject = new Intersection();

            if (_item1 == null) returnObject.instans.AddRange(_item1.instans);
            if (_item2 == null) returnObject.instans.AddRange(_item2.instans);

            return returnObject;
        }
    }

    public class IntersectionParent
    {
        private List<Intersection> list;
        public  List<Intersection> Instanc { get { return list; } }

        public IntersectionParent()
        {
            list = new List<Intersection>();
        }

        public void Add(Intersection elem)
        {
            list.Add(elem);
        }
        
    }
}