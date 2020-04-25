using System.Collections;
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

        private class ConversPosition{
            public GameObject befor;
            public Vector3 convers;
        }

        // 移動速度
        [SerializeField]
        [Header("移動速度調節")]
        [Range(1.0f,20.0f)]
        private float moveSpeed = 13.0f;

        // 段差判定の精度
        [SerializeField]
        [Header("段差判定の精度(プログラマーのみ変更許可)")]
        [Range(0.01f, 1.0f)]
        private float stepJudgeAccuracy = 0.9f;

        // 段差判定の高さ
        [SerializeField]
        [Header("段差判定の高さ(プログラマーのみ変更許可)")]
        private float judgeHight = 30.0f;

        // 選択時の矢印生成用のPrefab
        [SerializeField]
        [Header("矢印オブジェクトのPrefab設定")]
        private GameObject pickArrowPrefab;
        
        // Choiceできるオブジェクトのリスト
        private List<ConversPosition> choiceObjectList;

        // カメラオブジェクト
        private GameObject cameraObject;

        // 現在Choiceされている位置情報
        private Vector3 choicePosition;

        // キー操作がない状態はChoice
        // していないので初期はfalse
        // 再設定はGetChoiceで
        private bool notChoice;

        // 矢印のMeshRenderの表示設定に使う
        public bool NotChoice { get { return notChoice; } }
        
        // Start is called before the first frame update
        void Start()
        {
            // 最大数セット
            SetMaxFunctionSize((uint)TRANSITION.MAX);

            // 各種関数セット
            CreateFunction((uint)TRANSITION.None, None);
            CreateFunction((uint)TRANSITION.Move, Move);
            CreateFunction((uint)TRANSITION.Choice, Choice);
            CreateFunction((uint)TRANSITION.GetChoice, GetChoice);

            // 初期関数セット
            SetFunction((uint)TRANSITION.GetChoice);

            // カメラを親を取得
            cameraObject = UnityEngine.Camera.main.transform.root.gameObject;

            // 矢印オブジェクトを生成
            var pickArrowObject = Instantiate(pickArrowPrefab);
            var pickArrowCompoent = pickArrowObject.GetComponent<BetaPickArrow>();

            // プレイヤーコンポーネント
            pickArrowCompoent.PlayerComponent = this;

            // 初期はChoiceするオブジェクトがないのでtrue
            notChoice = true;
        }

        // None
        private void None()
        {
            // None
        }

        // 移動
        private void Move()
        {
            Debug.Log("Move関数");
            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtunCode.A))
            {
                SetFunction((uint)TRANSITION.Choice);
            }
        }

        // 選択
        private void Choice()
        {
            ConversPosition[] direction = new ConversPosition[(uint)DIRECTION.MAX];

            // 初期位置設定
            foreach (var itr in direction) itr.convers = choicePosition;

            // カメラとの位置情報割り出し
            foreach (var itr in choiceObjectList)
            {
                var convPos = CameraConversion(itr.befor.transform.position);

                itr.convers = convPos;
            }
            

            // 左右上下のオブジェクト割り出し
            foreach (var itr in choiceObjectList)
            {
                // 差分割り出し
                var diff = itr.convers - choicePosition;

                // 左
                if (diff.x < 0f)
                {
                    if (diff.x< direction[(uint)DIRECTION.LEFT].convers.x) {
                        direction[(uint)DIRECTION.LEFT] = itr;
                    }
                }
                else if (0f < diff.x)   // 右
                {
                    if (diff.x < direction[(uint)DIRECTION.RIGHT].convers.x)
                    {
                        direction[(uint)DIRECTION.RIGHT] = itr;
                    }
                }
                // 後
                if (diff.z < 0f)
                {
                    if (diff.x < direction[(uint)DIRECTION.BACK].convers.x)
                    {
                        direction[(uint)DIRECTION.BACK] = itr;
                    }
                }
                else if (0f < diff.z)   // 前
                {
                    if (diff.x < direction[(uint)DIRECTION.TOP].convers.x)
                    {
                        direction[(uint)DIRECTION.TOP] = itr;
                    }
                }

                //if (direcPos[(uint)DIRECTION.RIGHT].convers < itr.convers)
                //    direction[(uint)DIRECTION.RIGHT] = itr;
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
            foreach(var itr in objectArray)
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

            // 次ループから選択の関数へ
            SetFunction((uint)TRANSITION.Choice);
        }

        // カメラの位置変換
        private Vector3 CameraConversion(Vector3 _pos)
        {
            // 長さ割出
            var length = Vector3.Distance(choicePosition, _pos);

            var cameraRotY = cameraObject.transform.rotation.eulerAngles.y;

            //　ユニットの位置をターゲットの位置からの角度にベクトルをかけて計算
            return choicePosition + Quaternion.Euler(0f, cameraRotY, 0f) * new Vector3(0f, 0f, length);
        }
    }
}
