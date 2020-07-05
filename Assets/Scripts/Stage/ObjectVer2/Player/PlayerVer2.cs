using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

using System;

// プレイヤーのスクリプト
namespace TeamProject
{
    public class PlayerVer2 : System.TransitionObject
    {
        public enum TRANSITION
        {
            None,       // 何もしない(使わないが一応)
            StageStart, // ステージの初めのアニメーションの処理      
            Move,       // 移動
            Choice,     // 選択
            GetChoice,  // 選択オブジェクトの取得
            RootCheck,  // ルートチェック
            Jump,       // ジャンプ
            Fall,       // 落下処理
            Goal,       // ゴール処理
            DecoyMiss,  // デコイ飛ばすルートミス
            Miss,       // 普通ルートミス
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
        private BetaPickArrow pickArrowCom;

        // Choiceできるオブジェクトのリスト
        private List<ConversPosition> choiceObjectList;

        // カメラオブジェクト
        private GameObject cameraObject;
        private Camera camereaCompoent;

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

        // プレイヤーの移動アニメーションでの坂判定用の
        // 位置情報
        float oldHight;

        private Animator anima;

        [SerializeField]
        [Header("プレイヤーの正面から下げられたレイの長さ。判定外で飛び降り処理")]
        private float downLength = 2f;

        private MinionPlatoon minionPlatoon;

        // Choiceのオブジェクト
        [SerializeField]
        private GameObject firstChoiceObject = null;

        private WalkSoundManage walkSoundManage;

        //private string[] grassSEPath;
        //private string[] walkSEPath;
        //private string[] walkRainSEPath;
        //private string[] walkSnowSEPaht;

        private float soundSpanNow = 0.0f;
        [SerializeField]
        private float soundSpan = 0.2f;

        private bool[] oldArrow = new bool[(int)InputManager.ArrowCoad.Max];

        private bool goalOnce = true;

        [SerializeField]
        private SkinnedMeshRenderer bodyMesh;

        private bool startAnimationEndFlag = false;
        public bool StartAnimationEndFlag { get { return startAnimationEndFlag; } }

        private GuideLine[] guideLine;

        [SerializeField]
        private GameObject guidBase;

        [SerializeField]
        private Material mat1;
        
        [SerializeField]
        private Material mat2;

        private Minion beforChoiceMinion = null;

        // Start is called before the first frame update
        void Start()
        {
            // 最大数セット
            SetMaxFunctionSize((uint)TRANSITION.MAX);
            SetMaxFixFunctionSize((uint)FIX_TRANSITION.MAX);

            // 各種関数セット
            CreateFunction((uint)TRANSITION.None, None);
            CreateFunction((uint)TRANSITION.StageStart, StageStart);
            CreateFunction((uint)TRANSITION.Move, Move);
            CreateFunction((uint)TRANSITION.Choice, Choice);
            CreateFunction((uint)TRANSITION.GetChoice, GetChoice);
            CreateFunction((uint)TRANSITION.RootCheck, RootCheck);
            CreateFunction((uint)TRANSITION.Jump, Jump);
            CreateFunction((uint)TRANSITION.Fall, Fall);
            CreateFunction((uint)TRANSITION.Goal, GoalFunc);

            CreateFixFunction((uint)FIX_TRANSITION.None, None);
            CreateFixFunction((uint)FIX_TRANSITION.Move, FixMove);


            // カメラを親を取得
            cameraObject = UnityEngine.Camera.main.transform.root.gameObject;
            camereaCompoent = cameraObject.GetComponent<Camera>();

            // 矢印オブジェクトを生成
            pickArrowObject = Instantiate(pickArrowPrefab, new Vector3(0f, pickArrowHight) + transform.position, Quaternion.identity);
            pickArrowCom = pickArrowObject.GetComponent<BetaPickArrow>();

            // プレイヤーコンポーネント
            pickArrowCom.PlayerComponent = this;

            // 初期はChoiceするオブジェクトがないのでtrue
            notChoice = true;

            // 初期はfalse
            rootCheckFlag = false;

            // リジットボディを取得する
            rb = GetComponent<Rigidbody>();

            anima = transform.GetComponentInChildren<Animator>();

            // ミニオンの隊列を生成
            minionPlatoon = gameObject.GetComponent<MinionPlatoon>();

            minionPlatoon.SetFunction((uint)MinionPlatoon.TRANS.Wait);

            // 初期関数セット
            SetFunction((uint)TRANSITION.StageStart);
            SetFixFunction((uint)FIX_TRANSITION.None);

            //grassSEPath = new string[4] { SEPath.SE_PLAYER_SEPARATE_DRY1, SEPath.SE_PLAYER_SEPARATE_DRY2, SEPath.SE_PLAYER_SEPARATE_DRY3, SEPath.SE_PLAYER_SEPARATE_DRY4 };
            //walkSEPath = new string[4] { SEPath.SE_PLAYER_WALK_GRASS1, SEPath.SE_PLAYER_WALK_GRASS2, SEPath.SE_PLAYER_WALK_GRASS3, SEPath.SE_PLAYER_WALK_GRASS4 };

            oldArrow[(int)InputManager.ArrowCoad.UpArrow] = false;
            oldArrow[(int)InputManager.ArrowCoad.DownArrow] = false;
            oldArrow[(int)InputManager.ArrowCoad.RightArrow] = false;
            oldArrow[(int)InputManager.ArrowCoad.LeftArrow] = false;

            walkSoundManage = GetComponent<WalkSoundManage>();

            var obj = Instantiate(guidBase, Vector3.up, Quaternion.identity);
            var obj2 = Instantiate(guidBase, Vector3.up * 1.5f, Quaternion.identity);

            obj.name = "GuidLineObject1";
            obj2.name = "GuidLineObject2";

            guideLine = new GuideLine[2];

            guideLine[0] = obj.GetComponent<GuideLine>();
            guideLine[0].Mat = mat1;

            guideLine[1] = obj2.GetComponent<GuideLine>();
            guideLine[1].Mat = mat2;
        }

        // None
        private void None()
        {
            // None
        }

        private void StageStart()
        {
            if (startAnimationEndFlag)
            {
                SetFunction((uint)TRANSITION.GetChoice);
                if (firstChoiceObject != null)
                {
                    GetChoice();

                    choicePosition = firstChoiceObject.transform.position;
                    choiceObject = firstChoiceObject;
                    pickArrowCom.ChoicePosition = new Vector3(0f, pickArrowHight, 0f) + choicePosition;
                    pickArrowCom.transform.position = new Vector3(0f, pickArrowHight, 0f) + choicePosition;


                    var vec = choicePosition - transform.position;
                    transform.LookAt(choicePosition, Vector3.up);
                    //transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

                    notChoice = false;
                    SetFunction((uint)TRANSITION.RootCheck);
                }
            }
        }

        private bool oldFall = true;

        [SerializeField]
        private float jumpWidth = 0.6f;

        [SerializeField]
        private LayerMask goalMask;

        [SerializeField]
        [Header("シームレス開始までのゴールとの距離")]
        private float goalLenght;

        [SerializeField]
        [Header("デコイのPrefabの設定")]
        private GameObject decoyPrefab;

        private GameObject decoyObject;

        [SerializeField]
        private int beforFrame;

        [SerializeField]
        private Transform rayTopPos;

        private bool hadoukenFlag = false;

        private Vector3 hadoukenPos;

        // 移動
        private void Move()
        {
            // 位置固定解除
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotation;// X | RigidbodyConstraints.FreezeRotationZ;

            float dx = choicePosition.x - transform.position.x;
            float dy = choicePosition.z - transform.position.z;
            float rad = Mathf.Atan2(dx, dy);
            //  Debug.Log(rad * Mathf.Rad2Deg);

            float ySpeed = rb.velocity.y - 1;

            rb.MoveRotation(Quaternion.AngleAxis(rad * Mathf.Rad2Deg, Vector3.up));
            var moveVector = transform.forward;
            moveVector.y = 0f;
            var vel = moveVector * moveSpeed;
            vel.y = ySpeed;
            rb.velocity = vel;

            Ray ray = new Ray(rayTopPos.position, -Vector3.up);

            RaycastHit hit;
            var hitFlag = Physics.Raycast(ray, downLength);
            if (!hitFlag)
            {
                rb.velocity *= jumpWidth;
                SetFunction((uint)TRANSITION.Jump);
                EndFallAnima();
            }

            anima.SetBool("Fall", !hitFlag);

            if (!oldFall && hitFlag)
            {
                Debug.Log("ジャンプ中");
            }

            oldFall = hitFlag;

            // Debug.Log((choicePosition - transform.position).magnitude);
            Vector3 a, b;
            a = choicePosition; b = transform.position;
            a.y = 0f; b.y = 0f;
            var length = (a - b).magnitude;
            // 近くなったので速度減少
            if (length < 1f)
            {
                rb.velocity *= length;
            }
            if (length < 0.8f)
            {
                rb.MovePosition(choicePosition);
                anima.SetBool("Walk", false);

                SetFunction((uint)TRANSITION.GetChoice);
                minionPlatoon.SetFunction((uint)MinionPlatoon.TRANS.Wait);

                ray = new Ray(transform.position, -Vector3.up);


                if (Physics.Raycast(ray, out hit))
                {
                    GimmickChack(hit);
                }
            }

            float difY = transform.position.y - oldHight;

            anima.SetFloat("Hight", difY);

            oldHight = transform.position.y;

            soundSpanNow += Time.deltaTime;

            var randInt = UnityEngine.Random.Range(0, 4);

            if (soundSpan < soundSpanNow)
            {
                // 足音と草のサウンド
                walkSoundManage.PlayWalkSound();
                //SEManager.Instance.Play(grassSEPath[randInt], 0.1f);
                //SEManager.Instance.Play(walkSEPath[randInt]);
                soundSpanNow = 0.0f;
            }

            // シームレス移動のためのゴール判定
            Ray goalRay = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(goalRay, out hit, goalLenght, goalMask))
            {
                if (goalOnce)
                {
                    goalOnce = false;
                    hit.transform.root.GetComponent<Goal>().PlayerLook(this);

                    // Debug.Break();
                    var cam = cameraObject.GetComponent<Camera>();
                    cam.SetFunction((uint)Camera.TRANS.Goal);
                    cam.StartSeamless();
                    cam.GetPlayer(this);
                }
                Debug.Log(hit.distance);
                if (hit.distance < 6f)
                {
                 //    Debug.Break();
                    hit.transform.root.gameObject.GetComponent<Goal>().GoalIn(this);
                    SetFunction((uint)TRANSITION.Goal);
                    // gameObject.SetActive(false);
                }
            }
            // Debug.DrawRay(transform.position + transform.forward, new Vector3(0, -downLength, 0f));
            
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, transform.forward, Color.red, goalLenght);
        }

        private void FixMove()
        {

        }

        private void Jump()
        {
            // transform.forward
            RaycastHit hit;
            Ray ray = new Ray(transform.position + transform.forward*2f + Vector3.up, -Vector3.up);
            var hitFlag = Physics.Raycast(ray, downLength);
            if (hitFlag)
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            }
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
            float min = 0, max = 0;
            
            foreach (var itr in choiceObjectList)
            {
                if (itr.befor.tag != "Hit")
                {
                    var minionCom = itr.befor.GetComponent<Minion>();
                    if (minionCom != null)
                    {
                        if (minionCom.Floor != camereaCompoent.NowHight) continue;
                    }

                    if (itr.befor.tag == "Goal")
                    {
                        var goalCom = itr.befor.GetComponent<Goal>();
                        if (goalCom != null)
                        {
                            if (goalCom.Floor != camereaCompoent.NowHight) continue;
                        }
                    }
                    max = itr.convers.x;
                    min = itr.convers.x;
                    direction[(uint)DIRECTION.TOP] = itr;
                    direction[(uint)DIRECTION.BACK] = itr;
                    break;
                }
            }

            // 左右上下のオブジェクト割り出し
            foreach (var itr in choiceObjectList)
            {
                if (itr.befor.tag == "Hit") continue;

                var minionCom = itr.befor.GetComponent<Minion>();
                if (minionCom != null)
                {
                    if (minionCom.Floor != camereaCompoent.NowHight) continue;
                }

                if (itr.befor.tag == "Goal")
                {
                    var goalCom = itr.befor.GetComponent<Goal>();
                    if (goalCom != null)
                    {
                        if (goalCom.Floor != camereaCompoent.NowHight) continue;
                    }
                }
                Debug.Log("ChoiceObject" + itr.befor.name);

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

                if (max < itr.convers.x)
                {
                    max = itr.convers.x;
                    direction[(uint)DIRECTION.TOP] = itr;
                }
                if (itr.convers.x < min)
                {
                    min = itr.convers.x;
                    direction[(uint)DIRECTION.BACK] = itr;
                }
            }

            bool[] arrow = new bool[(uint)InputManager.ArrowCoad.Max];

            arrow[(int)InputManager.ArrowCoad.RightArrow] = InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.RightArrow);
            arrow[(int)InputManager.ArrowCoad.LeftArrow] = InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.LeftArrow);

            // キー入力
            MinionChoiceVer2(arrow[(uint)InputManager.ArrowCoad.RightArrow] && !oldArrow[(uint)InputManager.ArrowCoad.RightArrow],
                ref direction[(uint)DIRECTION.RIGHT],
                direction[(uint)DIRECTION.BACK]);
            MinionChoiceVer2(arrow[(uint)InputManager.ArrowCoad.LeftArrow] && !oldArrow[(uint)InputManager.ArrowCoad.LeftArrow],
                ref direction[(uint)DIRECTION.LEFT],
                direction[(uint)DIRECTION.TOP]);

            Debug.Log("NowChoice" + choiceObject.name);

            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtunCode.A))
            {
                Debug.Log(rootCheckFlag);
                if (rootCheckFlag)
                {
                    if (choiceObject.tag == "Kobito")
                    {
                        var minionCom = choiceObject.GetComponent<Minion>();
                        if (minionCom != null)
                        {
                            RootMemory.Instance.Regist(beforChoiceMinion, minionCom);
                            beforChoiceMinion = minionCom;
                        }
                    }
                    if (choiceObject.tag == "Goal")
                    {
                        var goalCom = choiceObject.GetComponent<Goal>();
                        if (goalCom != null)
                        {
                            RootMemory.Instance.Regist(beforChoiceMinion, goalCom);
                        }
                    }

                    SetFunction((uint)TRANSITION.None);
                    SetFixFunction((uint)FIX_TRANSITION.Move);
                    minionPlatoon.SetFunction((uint)MinionPlatoon.TRANS.Move);
                    oldHight = transform.position.y;
                    anima.SetTrigger("Find");

                    SEManager.Instance.Play(SEPath.SE_PLAY_START);
                }
                else
                {
                    //if (decoyObject == null && !hadoukenFlag)
                    //{
                    //    var decoy = Instantiate(decoyPrefab);

                    //    decoy.GetComponent<PlayerAfterimage>().SetParam(transform.position, hadoukenPos);

                    //    decoyObject = decoy;
                    //}
                    anima.SetTrigger("UnFind");
                }
            }

            oldArrow[(int)InputManager.ArrowCoad.UpArrow] = arrow[(int)InputManager.ArrowCoad.UpArrow];
            oldArrow[(int)InputManager.ArrowCoad.DownArrow] = arrow[(int)InputManager.ArrowCoad.DownArrow];
            oldArrow[(int)InputManager.ArrowCoad.RightArrow] = arrow[(int)InputManager.ArrowCoad.RightArrow];
            oldArrow[(int)InputManager.ArrowCoad.LeftArrow] = arrow[(int)InputManager.ArrowCoad.LeftArrow];

            if (choiceObject != null)
            {
                var minion = choiceObject.GetComponent<Minion>();

                if (minion != null)
                {
                    minion.OnFlag();
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

            float minLength = 10000f;

            // 配列のオブジェクトの親をListに追加
            foreach (var itr in objectArray)
            {
                ConversPosition parentObject = new ConversPosition();
                // 親を取得
                parentObject.befor = itr.transform.root.gameObject;

                // リストに追加
                choiceObjectList.Add(parentObject);

                // 長さ割出
                var length = (parentObject.befor.transform.position - transform.position).magnitude;

                if (itr.transform.root.tag != "Hit")
                {
                    if (length < minLength)
                    {
                        minLength = length;
                        choicePosition = parentObject.befor.transform.position;
                        choiceObject = parentObject.befor;
                    }
                }
            }
            
            // 落ちている途中の小人がいれば、やりなおし
            foreach (var itr in choiceObjectList)
            {
                if (itr.befor.tag != "Hit")
                {
                    var minionCom = itr.befor.transform.root.GetComponent<Minion>();
                    if (minionCom != null)
                    {
                        if (minionCom.IsFall)
                        {
                            return;
                        }
                    }
                }
            }
            
            // 指定できないようにする
            rootCheckFlag = false;

            if (minLength != 10000f)
            {
                RootCheck();
                pickArrowCom.ChoicePosition = new Vector3(0f, pickArrowHight, 0f) + choicePosition;

                var vec = choicePosition - transform.position;
                transform.LookAt(choicePosition, Vector3.up);
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

                Debug.Log("次の場所" + choiceObject.name);
            }

            // 配列が更新されたためChoiceを変更
            // notChoice = true;

            // 洗い出しが終わった時時点では自分が選択されている
            // choicePosition = transform.position;


            // 位置を固定
            rb.constraints =
                RigidbodyConstraints.FreezePosition |
                RigidbodyConstraints.FreezeRotation;
            // RigidbodyConstraints.FreezeRotationZ;

            // 次ループから選択の関数へ
            SetFunction((uint)TRANSITION.Choice);
        }

        private void RootCheck()
        {
            // Centerのレイを確認
            var centerRayCheck = CenterRayCheck();

            guideLine[0].SetPoint(Vector3.zero, null);
            guideLine[0].Hoge();
            guideLine[1].SetPoint(Vector3.zero, null);
            guideLine[1].Hoge();

            if (!centerRayCheck)
            {
                SetFunction((uint)TRANSITION.Choice);
                rootCheckFlag = false;
                // anima.SetTrigger("UnFind");
                Debug.Log("直線でアウト");
                return;
            }

            var outPos = new Vector3();
            // 線引くよ
            Vector3[] goodLane = null;
            Vector3[] outLane = null;

            // 天井のレイを確認
            // var topRayCheck = TopRayCheck(out outPos);
            var topRayCheck = TopRayChecVer3(ref outPos, ref goodLane, ref outLane);

            hadoukenFlag = topRayCheck;
            hadoukenPos = outPos;

            if (!topRayCheck)
            {
                // ここでFALSEが出ると妖精を飛ばす
                SetFunction((uint)TRANSITION.Choice);
                rootCheckFlag = false;

                // 成功してたらここ入ってこないから気兼ねなくここで赤線引くよ
                // マジなんで提出二日前に没仕様をあたかも俺の案のように言うて
                // 追加仕様として入れるかなマジでキレるで('ω')

                if (goodLane == null) return;

                guideLine[0].SetPoint(transform.position, goodLane);
                guideLine[1].SetPoint(goodLane[goodLane.Length-1], outLane);

                guideLine[0].Hoge();
                guideLine[1].Hoge();

                return;
            }

            rootCheckFlag = true;
            SetFunction((uint)TRANSITION.Choice);
            // anima.SetTrigger("Find");
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
            // 追加処理発生
            // 違う小人に当たっても追加でレイをとばす　
            var eraseObject = new List<GameObject>();
            var objList = choiceObjectList;

            Ray ray;
            RaycastHit hit;

            float rayLenght = 10000;


            var minionTop = choicePosition;
            var playerEye = transform.position;
            minionTop.y += 1.5f;
            playerEye.y += 2f;

            var vec = minionTop - playerEye;

            vec.Normalize();

            //レイの生成(レイの原点、レイの飛ぶ方向)
            ray = new Ray(playerEye, vec);

            Debug.Log(vec);

            bool returnFlag = false;

            // 追加処理
            // レイを指定の小人に当たるまでか、地面に当たるまで飛ばす
            while (true)
            {
                // 直レイの判定
                if (!Physics.Raycast(ray, out hit, rayLenght, kobitoLayer))
                {
                    // 当たり判定がなかったのでretunr 
                    returnFlag = false;
                    break;
                }
                else
                {
                    var hitHash = hit.collider.gameObject.transform.root.gameObject.GetHashCode();

                    var minionHash = choiceObject.GetHashCode();

                    Debug.Log(hit.collider.gameObject.transform.root.gameObject.name + "+" + choiceObject.name);

                    if (hitHash == minionHash)
                    {
                        returnFlag = true;
                        // Debug.Log("出た");
                        break;
                    }

                    if (objList.ToArray().Length == 0)
                    {
                        returnFlag = true;
                        Debug.Log("出た");
                        break;
                    }

                    bool groundHit = true;
                    // 追記処理
                    foreach (var itr in objList)
                    {
                        var otherMinionHash = itr.befor.GetHashCode();

                        // 当たったものが選択していないものなのでreturn 
                        if (hitHash == otherMinionHash)
                        {
                            eraseObject.Add(itr.befor);
                            itr.befor.SetActive(false);
                            // objList.Remove(itr);
                            Debug.Log("入った");
                            groundHit = false;
                            break;
                        }
                    }
                    Debug.Log(hit.collider.gameObject.name);
                    if (groundHit)
                    {
                        Debug.Log("出た");
                        returnFlag = false;
                        break;
                    }
                }
            }

            foreach (var itr in eraseObject)
            {
                itr.SetActive(true);
                Debug.Log("くりーん ");
            }

            return returnFlag;

        }

        // public LayerMask test;

        [SerializeField]
        private  LayerMask rayCheck;

        [SerializeField]
        private LayerMask rayCheckCeiling;

        private bool TopRayCheck(out Vector3 _outPos)
        {
            // Vectorの受け取りの初期化
            // falseが出たらoutに代入
            _outPos = new Vector3();
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

                // Debug.Log(inter);
                // 真下に向かってRayを作成
                rayArray[i] = new Ray(inter, -Vector3.up);
            }

            // 地面判定のオブジェクトのみのレイを取って
            // 配列に入れる
            var lengthArray = new float[(uint)rayNum];
            var outPos = new Vector3[(uint)rayNum];
            for (int i = 0; i < (uint)rayNum; i++)
            {
                RaycastHit hit;
                Physics.Raycast(rayArray[i], out hit, 10000f, rayCheck);

                // 長さを代入
                lengthArray[i] = hit.distance;
                outPos[i] = hit.point;
               //  Debug.Log(i + "aa" + outPos[i]);
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
                    _outPos = outPos[i- beforFrame];Debug.Log(_outPos);
                    return false;
                }
                oldLength = lengthArray[i];
            }

            // 最後まで来たら成功
            return true;
        }

        private bool TopRayCheckVer2(ref Vector3 _outPos)
        {
            var lerpStart = transform.position;
            var lerpEnd = choicePosition;

            var lengthPosition = lerpEnd - lerpStart;

            lengthPosition.y = 0f;

            var lerpLenght = lengthPosition.magnitude;

            // レイの個数(倍率 * 長さ)
            var rayNum = (int)(lerpLenght * stepJudgeAccuracy);

            if (rayNum == 0)
            {
                return false;
            }

            // ここから追加のArrayが存在するか取得
            Root.SetLerp(lerpStart, lerpEnd);

            float[] rayTopArraya = new float[rayNum];

            // 初期取りのRayTopは指定のものに
            for (int i = 0; i < rayNum; i++)
            {
                rayTopArraya[i] = rayTopLength;
            }

            List<Root> rootList = new List<Root>();

            // 基本ルートの洗い出し
            while (true)
            {
                Root root = Root.Create(ref rayTopArraya, rayNum);

                if (root == null) break;

                rootList.Add(root);
            }

            // ルートで実際に使うルートを再検索
           　var mainRootList = Root.CreateMainRoot(rootList);

            foreach(var itr in rootList)
            {
                Root.Check(itr, judgeHight, beforFrame, ref _outPos);
            }

            return true; 
        }

        [SerializeField]
        private LayerMask topElaseMask;

        private bool TopRayChecVer3(ref Vector3 _outPos,ref Vector3[] _goodLane,ref Vector3[] _outLane)
        {
            // Vectorの受け取りの初期化
            // falseが出たらoutに代入
            _outPos = new Vector3();

            // 長さを図る
            var lengthPosition = choicePosition - transform.position;

            lengthPosition.y = 0f;

            var length = lengthPosition.magnitude;

            // レイを作成する個数を
            // 長さと倍率で洗い出す

            // レイの個数(倍率 * 長さ)
            var rayNum = (int)(length * stepJudgeAccuracy);

            if (rayNum <= 0 + beforFrame) return true;

           // rayNum++;
            
            // レイを個数分生成
            var rayArray = new Ray[rayNum];
            var lerpStart = transform.position;
            var lerpEnd = choicePosition;
            LayerMask mask;

            mask = 11;// GroundCollider

            // 高さ入れる
            lerpStart.y = lerpEnd.y = rayTopLength;

            for (int i = 0; i < rayNum; i++)
            {
                // 位置の補間を作成
                var inter = Vector3.Lerp(lerpStart, lerpEnd, (float)i / rayNum);

                // Debug.Log(inter);
                // 真下に向かってRayを作成
                rayArray[i] = new Ray(inter, -Vector3.up);
            }

            // ------------------------------------ここから追記
            var elaseObject = new List<GameObject>();
            //　バグが出たので、変更
            // var elaseObjectPlayer = new List<GameObject>(); // プレイヤー
            // var elaseObjectTarget = new List<GameObject>(); // ターゲット
            
            // プレイヤーの位置にレイを飛ばし、プレイヤーが来るまで回す
            while (true)
            {
                RaycastHit hit;
                var hitFlag = Physics.SphereCast(rayArray[0], 1f, out hit, 10000f, topElaseMask);
                
                if (!hitFlag)
                {
                    foreach (var itr in elaseObject) itr.SetActive(false);
                    // foreach (var itr in elaseObjectPlayer) itr.SetActive(false);
                    // プレイヤーにすら当たらなかったのでなんかおかしいからfalseを返す
                    // Debug.Break();
                        return false;
                }

                if (hit.collider.tag == "Player")
                {
                    break;
                }

                // 当たり判定を消す、消したものを記憶しておく
                var obj = hit.collider.gameObject;
                obj.SetActive(false);
                elaseObject.Add(obj);
                // elaseObjectPlayer.Add(obj);
            }

            // 一旦存在Onにする。ターゲットの分
            // foreach (var itr in elaseObjectPlayer) itr.SetActive(true);

            // 同じく小人もする(重なってたらどうする？、位置が近くなった時にしようか)
            while (true)
            {
                RaycastHit hit;
                var hitFlag = Physics.SphereCast(rayArray[rayNum - 1], 1f, out hit, 10000f, topElaseMask);
                if (!hitFlag)
                {
                    // Instantiate(new GameObject(), rayArray[rayNum - 1].origin, Quaternion.identity);
                    // 当たらなかったのでなんかおかしいからfalseを返す
                    foreach (var itr in elaseObject) itr.SetActive(false);
                    // foreach (var itr in elaseObjectTarget) itr.SetActive(false);
                    Debug.Log("当たってねえ所");
                    //Debug.Break();
                    return false;
                }

                if (hit.collider.tag == "Kobito" || hit.collider.transform.root.tag == "Goal")
                {
                    break;
                }

                // 当たり判定を消す、消したものを記憶しておく
                var obj = hit.collider.gameObject;
                obj.SetActive(false);

                elaseObject.Add(obj);
                // elaseObjectTarget.Add(obj);
            }

            // 一旦存在Onにする。
            // foreach (var itr in elaseObjectTarget) itr.SetActive(true);

            // プレイヤーとターゲットの足元のオブジェクトを取得し、消したオブジェクトの中にそれがあるか確認
            // あった場合は絶対たどり着かないのでreturn false;

            // ------------------------------------ここまで追記

            GameObject minionGroundObject= null;
            
            // 小人の乗っている床を判定する必要がるので取得する
            // ----------------------------------------地面取得
            {
                RaycastHit hit;
                Ray minionGroundRay = new Ray(choicePosition, -Vector3.up);
                var flag = Physics.Raycast(minionGroundRay, out hit, 10000f, rayCheck);
                if (flag)
                {
                    minionGroundObject = hit.collider.gameObject;
                    Debug.Log(minionGroundObject.name);
                    Debug.Log(minionGroundObject.transform.root.name);
                }
            }
            // ------------------------------------地面取得終了


            // 地面判定のオブジェクトのみのレイを取って
            // 配列に入れる
            var lengthArray = new float[rayNum];
            var outPos = new Vector3[rayNum];
            var hitOjbect = new GameObject[rayNum];

            var elaseHitData = new List<GameObject>();

            for (int i = 0; i < rayNum; i++)
            {
                RaycastHit hit;
                var flag = Physics.Raycast(rayArray[i], out hit, 10000f, rayCheck);

                // 長さを代入
                lengthArray[i] = hit.distance;
                outPos[i] = hit.point;
                if (flag)
                {
                    hitOjbect[i] = hit.collider.gameObject;
                }
                else
                {
                    hitOjbect[i] = null;
                }
            }

            // 配列の低い数字から長さの差分をとる(初期は要素0を代入)
            float diff = 0;
            float oldLength = lengthArray[0];
            var outPosList = new List<Vector3>();
            var outPosIndx = new List<int>();
            int num = 0;

            // 差分が設定値以上ならがけなので即座に
            // returnn false(以下は大丈夫)
            for (int i = 0; i < rayNum; i++)
            {
                diff = lengthArray[i] - oldLength;
                Debug.Log(diff);
                if (judgeHight < -diff)
                {
                    RaycastHit hit;
                    var flag = Physics.Raycast(rayArray[i], out hit, 10000f, rayCheck);

                    if (hitOjbect[i] && minionGroundObject)
                    {
                        if (flag)
                        {
                            if (minionGroundObject.GetHashCode() != hit.collider.gameObject.GetHashCode())
                            {
                                elaseHitData.Add(hitOjbect[i]);
                                hitOjbect[i].SetActive(false);
                                hitOjbect[i] = hit.collider.gameObject;
                                // 長さを代入
                                lengthArray[i] = hit.distance;
                                outPos[i] = hit.point;
                                Debug.Log(hit.collider.gameObject.name);
                                // もう一度判定したいので戻す
                                i--;
                                continue;

                            }
                            else
                            {
                                hitOjbect[i] = hit.collider.gameObject;
                                // 長さを代入
                                lengthArray[i] = hit.distance;
                                outPos[i] = hit.point;
                                Debug.Log(hit.collider.gameObject.name);
                            }
                        }
                    }



                    var elem = i - beforFrame;
                    if (elem < 0) continue;
                    outPosList.Add(outPos[i - beforFrame]);
                    outPosIndx.Add(i);
                    num++;
                   //  return false;
                }
                oldLength = lengthArray[i];
            }

            foreach (var itr in elaseObject) itr.SetActive(true);
            foreach (var itr in elaseHitData) itr.SetActive(true);
            for (int i = 0; i < num; i++)
            {
                var hight = 0.5f;
                var ind = outPosIndx[i];
                // 小人の位置情報が明らか高ければ崖
                if (outPosList[i].y + hight < choicePosition.y)
                {
                    // 参照物を返す
                    // _outPos = outPosList[i];

                    Vector3[] goolLine = new Vector3[ind];
                    Vector3[] badLine = new Vector3[outPos.Length - ind + 1];

                    Array.Copy(outPos, 0, goolLine, 0, ind);
                    Array.Copy(outPos, ind - 1, badLine, 0, outPos.Length - ind + 1);

                    _goodLane = goolLine;
                    _outLane = badLine;
                    
                    return false;
                }
                else
                {
                    //Debug.Break();
                    //Debug.Log(ind);
                    //outPos[ind].y = outPos[ind - 1].y;
                    //lengthArray[ind] = lengthArray[ind - 1];
                    //diff = lengthArray[ind+1] - lengthArray[ind];
                    //if (judgeHight < -diff)
                    //{
                    //    var elem = ind - beforFrame;
                    //    if (elem < 0) continue;
                    //    outPosList.Add(outPos[ind - beforFrame]);
                    //    outPosIndx.Add(outPosIndx[i] + 1);
                    //    num++;
                    //}
                }
            }

            guideLine[0].SetPoint(transform.position, outPos);
            guideLine[0].Hoge();

            // 最後まで来たら成功
            return true;
        }

        private void GoalFunc()
        {
            rb.velocity = new Vector3();
        }

        public void EndFindAnima()
        {
            StartCoroutine(ToMove());
        }
        public void EndFallAnima()
        {
            StartCoroutine(JumpEnd());
        }

        [SerializeField]
        private float fallMiss = 0.5f;

        private void Fall()
        {
            Ray ray = new Ray(transform.position, -Vector3.up);
            RaycastHit hit;

            var ySpd = rb.velocity.y;

            rb.velocity = new Vector3(0f, ySpd, 0f);

            // 直レイの判定
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.distance);
                if (hit.distance < fallMiss)
                {
                    Debug.Log("Playerが");
                    // とりあえず地面あるから
                    SetFunction((uint)TRANSITION.GetChoice);
                    minionPlatoon.SetFunction((uint)MinionPlatoon.TRANS.Wait);
                }
            }
        }

        private IEnumerator ToMove()
        {
            yield return new WaitForSeconds(0f);
            SetFunction((uint)TRANSITION.Move);
            anima.SetBool("Walk", true);
        }

        private IEnumerator JumpEnd()
        {
            yield return new WaitForSeconds(2f);
            SetFunction((uint)TRANSITION.Move);
            anima.SetBool("Walk", true);
        }
        private IEnumerator ToJumpStart()
        {
            yield return new WaitForSeconds(0.7f);
            if (NowFunctionNum == (uint)TRANSITION.Jump)
            {
               //  SetFunction((uint)TRANSITION.Move);
            }
            var down = transform.forward * moveSpeed;
            down.y = rb.velocity.y;
            rb.velocity = down;
            anima.SetBool("Walk", true);
        }

        public void JumpStart()
        {
            rb.velocity = new Vector3();
            StartCoroutine(ToJumpStart());
        }

        public void GimmickChack(RaycastHit _hit)
        {
            Debug.Log("a");
            Debug.Log(_hit.collider.tag);
            switch (_hit.collider.tag)
            {
                case "IceGimmick":
                    var ice = _hit.transform.root.gameObject.GetComponent<IceGimmick>();

                    var minionNum = minionPlatoon.MinionNum;

                    if (ice.Judge((uint)minionNum))
                    {
                        Debug.Log("c");
                        ice.StartBreak();
                        SetFunction((uint)TRANSITION.Fall);
                        minionPlatoon.SetFunction((uint)MinionPlatoon.TRANS.Wait);
                    }
                    break;
            }
        }

        private void MinionChoice(bool _keyFlag, ref ConversPosition _direc)
        {
            if (_keyFlag)
            {
                Debug.Log("InputWKey");
                Vector3 up;

                if (_direc != null)
                {
                    choicePosition = up = _direc.befor.transform.position;
                    // オブジェクト情報を設定
                    choiceObject = _direc.befor;
                }
                else
                {
                    up = choicePosition;
                    // オブジェクト情報を設定
                    // choiceObject = _direc.befor;
                }

                pickArrowCom.ChoicePosition = up + new Vector3(0f, pickArrowHight, 0f);
                notChoice = false;

                transform.LookAt(up, Vector3.up);
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

                SEManager.Instance.Play(SEPath.SE_MINION_SELECT);

                // 検査に移動
                SetFunction((int)TRANSITION.RootCheck);
            }
        }

        private void MinionChoiceVer2(bool _keyFlag, ref ConversPosition _direc, ConversPosition _direc2)
        {
            if (_keyFlag)
            {
                Debug.Log("InputWKey");
                Vector3 up;

                if (_direc != null)
                {
                    choicePosition = up = _direc.befor.transform.position;
                    // オブジェクト情報を設定
                    choiceObject = _direc.befor;
                }
                else
                {
                    if (_direc2 != null)
                    {
                        up = choicePosition = _direc2.befor.transform.position;
                        // オブジェクト情報を設定
                        choiceObject = _direc2.befor;
                    }
                    else
                    {
                        up = choicePosition;
                    }
                }

                pickArrowCom.ChoicePosition = up + new Vector3(0f, pickArrowHight, 0f);
                notChoice = false;

                transform.LookAt(up, Vector3.up);
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

                SEManager.Instance.Play(SEPath.SE_MINION_SELECT);

                // 検査に移動
                SetFunction((int)TRANSITION.RootCheck);
            }
        }

        public void PlayerRendNot()
        {
            if (!bodyMesh.isVisible)
            {
                for (int i = 0; i < transform.GetChild(0).childCount; i++)
                {
                    transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public void StartAnimationEnd()
        {
            startAnimationEndFlag = true;
        }

        private void ChoiceMinion()
        {
            var minion = choiceObject.transform.root.GetComponent<Minion>();

            minion.OnFlag();
        }
    }
}
