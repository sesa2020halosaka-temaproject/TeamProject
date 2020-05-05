﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーのスクリプト
namespace TeamProject
{
    public class PlayerVer2 : System.TransitionObject
    {
        enum TRANSITION
        {
            None,       // 何もしない(使わないが一応)
            Move,       // 移動
            Choice,     // 選択
            GetChoice,  // 選択オブジェクトの取得
            RootCheck,  // ルートチェック
            Jump,       // ジャンプ
            MAX,        // MAX(使用しない)
        }

        enum FIX_TRANSITION
        {
            None,       // 何もしない
            Move,       // 移動
            MAX,        // MAX(使用しない)
        }

        enum DIRECTION
        {
            RIGHT,
            LEFT,
            TOP,
            BACK,
            MAX
        }

        private class ConversPosition
        {
            public ConversPosition()
            {
                convers = new Vector3();
            }
            public GameObject befor;
            public Vector3 convers;
        }

        // 移動速度
        [SerializeField]
        [Header("移動速度調節")]
        [Range(1.0f, 20.0f)]
        private float moveSpeed = 13.0f;

        // 段差判定の精度
        [SerializeField]
        [Header("段差判定の精度(プログラマーのみ変更許可)")]
        [Range(0.01f, 10.0f)]
        private float stepJudgeAccuracy = 3f;

        // 段差判定の高さ
        [SerializeField]
        [Header("段差判定の高さ(プログラマーのみ変更許可)")]
        private float judgeHight = 3.0f;

        // 段差判定の例の開始の高さ
        [SerializeField]
        [Header("天井からの飛ばすレイの天井の高さ(このゲームの最大の高さ+10f)")]
        private float rayTopLength = 100f;

        // 選択時の矢印生成用のPrefab
        [SerializeField]
        [Header("矢印オブジェクトのPrefab設定")]
        private GameObject pickArrowPrefab;

        // 選択時の矢印生成用のPrefab
        [SerializeField]
        [Header("矢印オブジェクトの高さ")]
        private float pickArrowHight = 10f;

        // 選択時の矢印保持用のオブジェクト
        private GameObject pickArrowObject;

        // Choiceできるオブジェクトのリスト
        private List<ConversPosition> choiceObjectList;

        // カメラオブジェクト
        private GameObject cameraObject;

        // 現在Choiceされている位置情報
        private Vector3 choicePosition;
        private GameObject choiceObject;

        // キー操作がない状態はChoice
        // していないので初期はfalse
        // 再設定はGetChoiceで
        private bool notChoice;

        // ルートが確立された時にtrueになる
        private bool rootCheckFlag;

        // 矢印のMeshRenderの表示設定に使う
        public bool NotChoice { get { return notChoice; } }

        private Rigidbody rb;

        [SerializeField]
        [Header("プレイヤーの正面から下げられたレイの長さ。判定外で飛び降り処理")]
        private float downLength = 2f;

        // Start is called before the first frame update
        void Start()
        {
            // 最大数セット
            SetMaxFunctionSize((uint)TRANSITION.MAX);
            SetMaxFixFunctionSize((uint)FIX_TRANSITION.MAX);

            // 各種関数セット
            CreateFunction((uint)TRANSITION.None, None);
            CreateFunction((uint)TRANSITION.Move, Move);
            CreateFunction((uint)TRANSITION.Choice, Choice);
            CreateFunction((uint)TRANSITION.GetChoice, GetChoice);
            CreateFunction((uint)TRANSITION.RootCheck, RootCheck);
            CreateFunction((uint)TRANSITION.Jump, Jump);

            CreateFixFunction((uint)FIX_TRANSITION.None, None);
            CreateFixFunction((uint)FIX_TRANSITION.Move, FixMove);

            // 初期関数セット
            SetFunction((uint)TRANSITION.GetChoice);
            SetFixFunction((uint)FIX_TRANSITION.None);

            // カメラを親を取得
            cameraObject = UnityEngine.Camera.main.transform.root.gameObject;

            // 矢印オブジェクトを生成
            pickArrowObject = Instantiate(pickArrowPrefab);
            var pickArrowCompoent = pickArrowObject.GetComponent<BetaPickArrow>();

            // プレイヤーコンポーネント
            pickArrowCompoent.PlayerComponent = this;

            // 初期はChoiceするオブジェクトがないのでtrue
            notChoice = true;

            // 初期はfalse
            rootCheckFlag = false;

            // リジットボディを取得する
            rb = GetComponent<Rigidbody>();
        }

        // None
        private void None()
        {
            // None
        }


        // 移動
        private void Move()
        {
            // 位置固定解除
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotationX| RigidbodyConstraints.FreezeRotationZ;

            float dx = choicePosition.x - transform.position.x;
            float dy = choicePosition.z - transform.position.z;
            float rad = Mathf.Atan2(dx, dy);
            Debug.Log(rad * Mathf.Rad2Deg);

            float ySpeed = rb.velocity.y;

            rb.MoveRotation(Quaternion.AngleAxis(rad * Mathf.Rad2Deg, Vector3.up));
            var moveVector = transform.forward;
            moveVector.y = 0f;
            var vel = moveVector * moveSpeed;
            vel.y = ySpeed;
            rb.velocity = vel;

            Ray ray = new Ray(transform.position + transform.forward, new Vector3(0, -downLength, 0f));

            RaycastHit hit;
            if (!Physics.Raycast(transform.position + transform.forward, new Vector3(0, -downLength, 0f), downLength))
            {
                // SetFunction((uint)TRANSITION.Jump);
                var down = transform.forward;
                down.y = ySpeed * 2;
                rb.velocity = down;
            }
            Debug.Log((choicePosition - transform.position).magnitude);
            Vector3 a, b;
            a = choicePosition;b = transform.position;
            a.y = 0f;b.y = 0f;
            var length = (a - b).magnitude;
            // 近くなったので速度減少
            if (length < 1f)
            {
                rb.velocity *= length;
            }
            if (length < 0.8f)
            {
                rb.MovePosition(choicePosition);
                SetFunction((uint)TRANSITION.GetChoice);
            }

            Debug.DrawRay(transform.position + transform.forward, new Vector3(0, -downLength, 0f));
        }

        private void FixMove()
        {

        }

        private void Jump()
        {
            // transform.forward
        }

        // 選択
        private void Choice()
        {
            var direction = new ConversPosition[(uint)DIRECTION.MAX];

            // 初期位置設定
            for (int i = 0; i < direction.Length; i++)
            {
                //direction[i] = new ConversPosition();
                //direction[i].befor = gameObject;
            }
            // カメラとの位置情報割り出し
            for (int i = 0; i < choiceObjectList.ToArray().Length; i++)
            {
                var convPos = CameraConversion(choiceObjectList[i].befor.transform.position);

                choiceObjectList[i].convers = convPos;
            }

            var choicePositionConv = CameraConversion(choicePosition);
            // 左右上下のオブジェクト割り出し
            foreach (var itr in choiceObjectList)
            {
                // 差分割り出し
                var diff = itr.convers - choicePositionConv;
               //  Debug.Log(itr.convers + "+" + itr.befor.name + "dif" + diff);
                // 左
                if (diff.x < -0.1f)
                {
                    if (direction[(uint)DIRECTION.LEFT] == null)
                    {
                        direction[(uint)DIRECTION.LEFT] = itr;
                    }
                    else if (itr.convers.x > direction[(uint)DIRECTION.LEFT].convers.x)
                    {
                        direction[(uint)DIRECTION.LEFT] = itr;
                    }
                }
                else if (0.1f < diff.x)   // 右
                {
                    if (direction[(uint)DIRECTION.RIGHT] == null)
                    {
                        direction[(uint)DIRECTION.RIGHT] = itr;
                    }
                    else if (itr.convers.x < direction[(uint)DIRECTION.RIGHT].convers.x)
                    {
                        direction[(uint)DIRECTION.RIGHT] = itr;
                    }
                }
                // 後
                if (diff.z < -0.1f)
                {
                    if (direction[(uint)DIRECTION.BACK] == null)
                    {
                        direction[(uint)DIRECTION.BACK] = itr;
                    }
                    else if (itr.convers.z > direction[(uint)DIRECTION.BACK].convers.z)
                    {
                        direction[(uint)DIRECTION.BACK] = itr;
                    }
                }
                else if (0.1f < diff.z)   // 前
                {
                    if (direction[(uint)DIRECTION.TOP] == null)
                    {
                        direction[(uint)DIRECTION.TOP] = itr;
                    }
                    else if (itr.convers.z < direction[(uint)DIRECTION.TOP].convers.z)
                    {
                        direction[(uint)DIRECTION.TOP] = itr;
                    }
                }
            }
            for (int i = 0; i < (uint)DIRECTION.MAX; i++)
            {
                //if (direction[i] == null) Debug.Log((DIRECTION)i + "+" + "null");
                //else Debug.Log((DIRECTION)i + "+" + direction[i].befor.name);
            }

            // キー入力
        　   // どうにかしたい
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("InputWKey");
                Vector3 up;

                if (direction[(uint)DIRECTION.TOP] != null)
                {
                    choicePosition = up = direction[(uint)DIRECTION.TOP].befor.transform.position;
                    // オブジェクト情報を設定
                    choiceObject = direction[(uint)DIRECTION.TOP].befor;
                }
                else
                    up = choicePosition;


                pickArrowObject.transform.position = up + new Vector3(0f, pickArrowHight, 0f);
                notChoice = false;

                // 検査に移動
                SetFunction((int)TRANSITION.RootCheck);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("InputWKey");
                Vector3 up;

                if (direction[(uint)DIRECTION.BACK] != null)
                {
                    choicePosition = up = direction[(uint)DIRECTION.BACK].befor.transform.position;
                    // オブジェクト情報を設定
                    choiceObject = direction[(uint)DIRECTION.BACK].befor;
                }
                else
                    up = choicePosition;

                pickArrowObject.transform.position = up + new Vector3(0f, pickArrowHight, 0f);
                notChoice = false;
                // 検査に移動
                SetFunction((int)TRANSITION.RootCheck);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("InputWKey");
                Vector3 up;

                if (direction[(uint)DIRECTION.RIGHT] != null)
                {
                    choicePosition = up = direction[(uint)DIRECTION.RIGHT].befor.transform.position;
                    // オブジェクト情報を設定
                    choiceObject = direction[(uint)DIRECTION.RIGHT].befor;
                }
                else
                    up = choicePosition;

                pickArrowObject.transform.position = up + new Vector3(0f, pickArrowHight, 0f);
                notChoice = false;
                // 検査に移動
                SetFunction((int)TRANSITION.RootCheck);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("InputWKey");
                Vector3 up;

                if (direction[(uint)DIRECTION.LEFT] != null)
                {
                    choicePosition = up = direction[(uint)DIRECTION.LEFT].befor.transform.position;
                    // オブジェクト情報を設定
                    choiceObject = direction[(uint)DIRECTION.LEFT].befor;
                }
                else
                    up = choicePosition;

                pickArrowObject.transform.position = up + new Vector3(0f, pickArrowHight, 0f);
                notChoice = false;
                // 検査に移動
                SetFunction((int)TRANSITION.RootCheck);
            }

            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtunCode.A))
            {
                Debug.Log(rootCheckFlag);
                if (rootCheckFlag)
                {
                    SetFunction((uint)TRANSITION.Move);
                    SetFixFunction((uint)FIX_TRANSITION.Move);
                }
            }
        }

        // 選択の配列取得
        private void GetChoice()
        {
            // 配列の生成
            choiceObjectList = new List<ConversPosition>();

            // 配列の取得
            var objectArray = GameObject.FindGameObjectsWithTag("ChoiceObject");

            // 配列のオブジェクトの親をListに追加
            foreach (var itr in objectArray)
            {
                ConversPosition parentObject = new ConversPosition();
                // 親を取得
                parentObject.befor = itr.transform.root.gameObject;


                // リストに追加
                choiceObjectList.Add(parentObject);
            }

            // 配列が更新されたためChoiceを変更
            notChoice = true;

            // 洗い出しが終わった時時点では自分が選択されている
            choicePosition = transform.position;

            // 指定できないようにする
            rootCheckFlag = false;

            // 位置を固定
            rb.constraints = 
                RigidbodyConstraints.FreezePosition| 
                RigidbodyConstraints.FreezeRotationX| 
                RigidbodyConstraints.FreezeRotationZ;

            // 次ループから選択の関数へ
            SetFunction((uint)TRANSITION.Choice);
        }

        private void RootCheck()
        {
            // Centerのレイを確認
            var centerRayCheck = CenterRayCheck();

            if (!centerRayCheck)
            {
                SetFunction((uint)TRANSITION.Choice);
                rootCheckFlag = false;
                Debug.Log("直線でアウト");
                return;
            }

            // 天井のレイを確認
            var topRayCheck = TopRayCheck();

            if (!topRayCheck)
            {
                SetFunction((uint)TRANSITION.Choice);
                rootCheckFlag = false;
                return;
            }

            rootCheckFlag = true;
            SetFunction((uint)TRANSITION.Choice);
        }
        
        // カメラの位置変換
        private Vector3 CameraConversion(Vector3 _pos)
        {
            // カメラの位置変更の補正を後出かけます　_pos

            // Quaternion.AngleAxisがなんかがばいので回転行列のみ作成

            var test = _pos - cameraObject.transform.position;
            var pos = new Vector2(test.x, test.z);

            Vector2 ans;
            float angle = -Mathf.Deg2Rad * cameraObject.transform.root.eulerAngles.y;


            ans.x = pos.x * Mathf.Cos(angle) + pos.y * Mathf.Sin(angle);
            ans.y = -pos.x * Mathf.Sin(angle) + pos.y * Mathf.Cos(angle);

            return new Vector3(ans.x, _pos.y, ans.y);
        }

        public LayerMask kobitoLayer;
        private bool CenterRayCheck()
        {

            Ray ray;
            RaycastHit hit;

            float rayLenght = 10000;

            var vec = choicePosition - transform.position;

            vec.Normalize();

            //レイの生成(レイの原点、レイの飛ぶ方向)
            ray = new Ray(transform.position, vec);

            Debug.Log(vec);
            // 直レイの判定
            if (!Physics.Raycast(ray, out hit, rayLenght, kobitoLayer))
            {
                // 当たり判定がなかったのでretunr 
                return false;
            }
            else
            {
                var hitHash = hit.collider.gameObject.transform.root.gameObject.GetHashCode();

                var minionHash = choiceObject.GetHashCode();

                Debug.Log(hit.collider.gameObject.transform.root.gameObject.name+"+"+choiceObject.name);

                // 当たったものが選択していないものなのでreturn 
                if (hitHash != minionHash)
                {
                    return false;
                }
            }

            return true;

        }

        public LayerMask test;

        private bool TopRayCheck()
        {
            // 長さを図る
            var lengthPosition = choicePosition - transform.position;

            lengthPosition.y = 0f;

            var length = lengthPosition.magnitude;

            // レイを作成する個数を
            // 長さと倍率で洗い出す

            // レイの個数(倍率 * 長さ)
            var rayNum = length * stepJudgeAccuracy;
            
            if (rayNum == 0) return true;

            // レイを個数分生成
            var rayArray = new Ray[(uint)rayNum];
            var lerpStart = transform.position;
            var lerpEnd = choicePosition;
            LayerMask mask;

            mask = 11;// GroundCollider

            // 高さ入れる
            lerpStart.y = lerpEnd.y = rayTopLength;

            for (int i = 0; i < (uint)rayNum; i++)
            {
                // 位置の補間を作成
                var inter = Vector3.Lerp(lerpStart, lerpEnd, (float)i / rayNum);

                Debug.Log(inter);
                // 真下に向かってRayを作成
                rayArray[i] = new Ray(inter, -Vector3.up);
            }

            // 地面判定のオブジェクトのみのレイを取って
            // 配列に入れる
            var lengthArray = new float[(uint)rayNum];
            for (int i = 0; i < (uint)rayNum; i++)
            {
                RaycastHit hit;
                Physics.Raycast(rayArray[i], out hit, 10000f, test);

                // 長さを代入
                lengthArray[i] = hit.distance;
            }

            // 配列の低い数字から長さの差分をとる(初期は要素0を代入)
            float diff = 0;
            float oldLength = lengthArray[0];

            // 差分が設定値以上ならがけなので即座に
            // returnn false(以下は大丈夫)
            for (int i = 0; i < (uint)rayNum; i++)
            {
                diff = lengthArray[i] - oldLength;
                Debug.Log(diff);
                if (judgeHight < -diff)
                {
                    Debug.Log("Inしたよ");
                    return false;
                }
                oldLength = lengthArray[i];
            }

            // 最後まで来たら成功
            return true;
        }
    }
}
