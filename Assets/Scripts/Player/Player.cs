using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
using TeamProject.InputManager;

namespace TeamProject
{
    public enum State
    {
        Stay = 0,
        Walk = 1,
        Kakiwake = 2,
    }

    public class Player : MonoBehaviour
    {
        // 物理ベース移動などに使用
        private Rigidbody rigidbody = null;

        // 移動ターゲットにする位置
        private Vector3 targetPos;

        // 一つ前の位置
        private Vector3 targetOldPos;

        // ターゲットまでの長さ(0~1)
        private float targetLength = 0.0f;

        // 移動速度
        [SerializeField]
        private float speed = 1.0f;

        [SerializeField]
        private Vector3 pos;

        private bool flag = false;

        // 小人の配列
        private GameObject[] KobitoList;
        private GameObject goal;

        // 上下左右のキー入力の前のキーコード
        private KeyCode beforKeyCoad;
        private int keyNum = -1;
        private GameObject targetObject;

        private int kobitoNum;
        public int KobitoNum { get { return kobitoNum; } }

        private int maxKobitoNum;

        public int MaxKobitoNum { get { return maxKobitoNum; } }

        // 当たり判定をなくすレイヤーを指定
        [SerializeField]
        public LayerMask hitMask;

        private State state;

        // childrenから引っ張てくるのが面倒なのでSerializeFieldで
        // 設定する
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private GameObject arrowObject;

        private string[] grassSEPath;
        private string[] walkSEPath;

        [SerializeField]
        private float soundSpan = 0.2f;

        private float soundSpanNow = 0.0f;

        private bool playerNotControlFlag;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float upStickPower, downStickPower;

        private Vector2 oldStickVel = new Vector2();

        // Start is called before the first frame update
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            pos = targetOldPos = targetPos = transform.position;

            // 小人のゲームオブジェクトを受けとる
            KobitoList = GameObject.FindGameObjectsWithTag("Kobito");
            goal = GameObject.FindGameObjectWithTag("Goal");

            beforKeyCoad = KeyCode.None;

            maxKobitoNum = KobitoList.Length;
            Debug.Log(maxKobitoNum);

            kobitoNum = 0;

            Debug.Assert(animator != null);

            arrowObject = GameObject.Instantiate(arrowObject);
            arrowObject.SetActive(false);

            grassSEPath = new string[4] { SEPath.SE_PLAYER_SEPARATE_DRY1, SEPath.SE_PLAYER_SEPARATE_DRY2, SEPath.SE_PLAYER_SEPARATE_DRY3, SEPath.SE_PLAYER_SEPARATE_DRY4 };
            walkSEPath = new string[4] { SEPath.SE_PLAYER_WALK_GRASS1, SEPath.SE_PLAYER_WALK_GRASS2, SEPath.SE_PLAYER_WALK_GRASS3, SEPath.SE_PLAYER_WALK_GRASS4 };

            playerNotControlFlag = false;

            Debug.Log("Playerが先");
        }

        // Update is called once per frame
        void Update()
        {
            if (playerNotControlFlag) return;

            // 移動
            if (Move())
            {
                // 移動終わっていたらターゲット変更
                Target();
            }

            AnimationState();
        }

        // 実際の動き
        private bool Move()
        {
            // アニメーションステートを設定
            state = State.Kakiwake;

            if (!flag) return true;


            targetLength += Time.deltaTime * speed / Vector3.Magnitude(targetOldPos - targetPos);
            if (1.0f < targetLength) targetLength = 1.0f;
            var ans = Vector3.Lerp(targetOldPos, targetPos, targetLength);
            //Debug.Log(Vector3.Lerp(targetPos, targetOldPos, 0));
            //Debug.Log(Vector3.Lerp(targetPos, targetOldPos, 1));
            //Debug.Log(targetLength);

            // 移動
            rigidbody.MovePosition(ans);
            //Debug.Log(targetLength);
            // 移動先が同じなら
            if (1.0f <= targetLength)
            {
                targetOldPos = targetPos;
                targetLength = 0.0f;

                kobitoNum++;

                flag = false;

                return true;
            }

            arrowObject.transform.position = targetPos + new Vector3(0.0f, 5.0f, 0.0f);

            var randInt = Random.Range(0, 4);

            soundSpanNow += Time.deltaTime;

            if (soundSpan < soundSpanNow)
            {
                SEManager.Instance.Play(grassSEPath[randInt], 0.1f);
                SEManager.Instance.Play(walkSEPath[randInt]);
                soundSpanNow = 0.0f;
            }
            return false;
        }

        // 位置移動先を設定する
        private void Target()
        {
            // アニメーションステートを設定
            state = State.Stay;

            var upKobito = new List<GameObject>();
            var rightKobito = new List<GameObject>();
            var leftKobito = new List<GameObject>();
            var downKobito = new List<GameObject>();

            var myPos = transform.position;

            // 小人探知
            foreach (var itr in KobitoList)
            {
                var tarPos = itr.transform.position;

                var lookRot = Mathf.Atan2((tarPos.x - myPos.x), (tarPos.z - myPos.z)) * Mathf.Rad2Deg + 180.0f;

                var kobitoGameobject = itr.transform.root.gameObject.GetComponent<Kobito>();
                if (kobitoGameobject.SentakuFlag)
                {
                    // 左
                    if (225.0f <= lookRot && lookRot < 315.0f)
                    {
                        //Debug.Log("右");
                        rightKobito.Add(itr);
                    }
                    // 下
                    else if (135.0f <= lookRot && lookRot < 225.0f)
                    {
                        //Debug.Log("上");
                        upKobito.Add(itr);
                    }
                    // 右
                    else if (45.0f <= lookRot && lookRot < 135.0f)
                    {
                        //Debug.Log("左");
                        leftKobito.Add(itr);
                    }
                    // 上
                    else
                    {
                        //Debug.Log("下");
                        downKobito.Add(itr);
                    }
                }
            }

            // ゴール探知
            var tarGPos = goal.transform.position;

            var lookGRot = Mathf.Atan2((tarGPos.x - myPos.x), (tarGPos.z - myPos.z)) * Mathf.Rad2Deg + 180.0f;

            // 左
            if (225.0f < lookGRot && lookGRot < 315.0f)
            {
                //Debug.Log("右");
                rightKobito.Add(goal);
            }
            // 下
            else if (135.0f < lookGRot && lookGRot < 225.0f)
            {
                //Debug.Log("上");
                upKobito.Add(goal);
            }
            // 右
            else if (45.0f < lookGRot && lookGRot < 135.0f)
            {
                //Debug.Log("左");
                leftKobito.Add(goal);
            }
            // 上
            else
            {
                //Debug.Log("下");
                downKobito.Add(goal);
            }

            var StickVel = InputManager.InputManager.Instance.GetLStick();
            // オブジェクト割り出し
            // 左
            if (-upStickPower <= StickVel.x && oldStickVel.x <= -downStickPower)
            {
                KeyCoadTest(KeyCode.A, leftKobito);
            }
            // 右
            if (StickVel.x <= upStickPower && downStickPower <= oldStickVel.x)
            {
                KeyCoadTest(KeyCode.D, rightKobito);
            }
            // 下
            if (-upStickPower <= StickVel.y && oldStickVel.y <= -downStickPower)
            {
                KeyCoadTest(KeyCode.S, downKobito);
            }
            // 上
            if (StickVel.y <= upStickPower && downStickPower <= oldStickVel.y)
            {
                KeyCoadTest(KeyCode.W, upKobito);
            }

            oldStickVel = StickVel;

            // 間にモノがあるか
            var canMoveFlag = HitEny(targetObject);

            //            if (Input.GetKeyDown(KeyCode.Space) && canMoveFlag)
            if (InputManager.InputManager.Instance.GetKeyDown(ButtonCode.A) && canMoveFlag)
            {
                beforKeyCoad = KeyCode.None;
                targetPos = targetObject.transform.position;
                var kobito = targetObject.GetComponent<Kobito>();

                if (kobito) kobito.SentakuFlag = false;
                flag = true;
            }
        }

        private void KeyCoadTest(KeyCode keyCode, List<GameObject> list)
        {
            // データが0ではないか？
            if (list.ToArray().Length == 0) return;
            // キーが押されたら
            // if (Input.GetKeyDown(keyCode))
            if (true)
            // if(InputManager.InputManager.Instance.GetKeyDown(ButtunCode.A))
            {
                SEManager.Instance.Play(SEPath.SE_MINION_SELECT);
                // 前押されてたキーが同じ科
                if (beforKeyCoad == keyCode)
                {
                    Debug.Log("KeyTap");
                    keyNum++;
                    if (keyNum == list.ToArray().Length)
                    {
                        Debug.Log("In");
                        keyNum = 0;
                    }
                }
                else
                {
                    keyNum = 0;
                }

                Debug.Log(keyNum);
                targetObject = list[keyNum];
                beforKeyCoad = keyCode;
                Debug.Log(targetObject.name);
            }
        }

        private bool HitEny(GameObject _targetObject)
        {
            if (_targetObject == null) return false;

            // 何かあった時用に親を選択
            var target = _targetObject.transform.root.gameObject;
            RaycastHit hit;
            var kobitoGameobject = target.GetComponent<Kobito>();

            var vec = transform.position - target.transform.position;

            arrowObject.transform.position = target.transform.position + new Vector3(0.0f, 5.0f, 0.0f);

            vec.y = 0;

            float lenght = vec.magnitude;

            vec.Normalize();

            var isHit = Physics.SphereCast(transform.position, transform.lossyScale.x * 0.5f, -vec * lenght, out hit, 1000000.0f, hitMask);

            if (isHit)
            {
                if (hit.transform.root.gameObject.GetInstanceID() == target.GetInstanceID())
                {
                    //Debug.Log("本物");
                    if (kobitoGameobject) kobitoGameobject.Sentaku();

                    var targetPos = _targetObject.transform.position;

                    transform.LookAt(targetPos, Vector3.up);

                    arrowObject.SetActive(true);

                    return true;
                }
                else
                {
                    //Debug.Log("偽物");
                }
            }
            else
            {
                //Debug.Log("NonHit");
            }

            return false;
        }

        // アニメーション管理関数
        private void AnimationState()
        {
            animator.SetInteger("State", (int)state);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.tag == "Goal")
            {
                playerNotControlFlag = true;
            }
        }
    }
}