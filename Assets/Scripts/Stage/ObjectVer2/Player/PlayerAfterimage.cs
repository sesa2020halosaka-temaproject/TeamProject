using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace TeamProject {
    public class PlayerAfterimage : System.TransitionObject
    {
        // 開始位置
        private Vector3 startPos;

        // 終了位置
        private Vector3 endPos;

        private float endTime;

        // 終了位置の方向
        private Vector3 normal;
        private Vector3 normalXZ;

        private Rigidbody rb;

        [SerializeField]
        [Header("パーティクルの飛ばすスピード")]
        private float speed;
        
        private float oldLength;

        // パーティクル
        private VisualEffect[] particle;

        // ModelのActive制御のために取得しておく
        private GameObject modelObject;

        [SerializeField]
        [Header("デコイ表示後から消去までの時間")]
        private float destroyTime = 2f;

        enum TRANS
        {
            None,
            Move,
            Decoy,
            Max,
        }

        public void SetParam(Vector3 _start, Vector3 _end)
        {
            startPos = _start;
            endPos = _end;

            endTime = oldLength = (endPos - startPos).magnitude;
            
            normal = (endPos - startPos).normalized;

            normalXZ = normal;
            normalXZ.y = 0f;
            normalXZ.Normalize();
            normalXZ *= speed;

            Debug.Log(normalXZ);
            transform.position = startPos;
        }

        private void Start()
        {
            // 移動用リジットボディの追加
            rb = GetComponent<Rigidbody>();

            // マックスサイズ設定
            SetMaxFunctionSize((uint)TRANS.Max);

            // 各種関数をアップデートに設定
            CreateFunction((uint)TRANS.None, None);
            CreateFunction((uint)TRANS.Move, Move);
            CreateFunction((uint)TRANS.Decoy, Decoy);

            // 最初のアップデート関数を設定
            SetFunction((uint)TRANS.Move);

            // Modelのゲームオブジェクトを子オブジェクトから取得
            modelObject = transform.GetChild(0).gameObject;

            // パーティクル取得
            particle = transform.GetComponentsInChildren<VisualEffect>();

            // パーティクル再生
            foreach (var itr in particle) itr.Play();

            // Modelを非表示
            modelObject.SetActive(false);
        }

        private void None()
        {
            // None
        }

        private float time = 0f;

        private void Move()
        {
            var vel = rb.velocity;
            float lenght = (endPos - transform.position).magnitude;

            // normalXZ *= speed;
            
            // 重力の影響だけを受け取り
            normalXZ.y = vel.y;

            time += Time.deltaTime*speed;

            // 代入
            rb.velocity = normalXZ;

            var endFlag = endTime < time; 

             oldLength = lenght;
            Debug.Log(endTime);
            Debug.Log(time);


            // 位置が近いと終了処理を出す(Playerのデコイを出現させる)
            if (endFlag)
            {
                // 移動速度をゼロに
                rb.velocity = new Vector3();

                // 位置をendに補正
                transform.position = endPos;

                DecoyStart();

                // パーティクル再生
                foreach (var itr in particle) itr.Stop();

                // デコイを移すUpdateに変更
                SetFunction((uint)TRANS.Decoy);
            }
        }

        private void Decoy()
        {
            // 基本何もしない
        }

        private void DecoyStart()
        {
            // Modelを表示
            modelObject.SetActive(true);

            // 削除予約
            Destroy(gameObject, destroyTime);
        }
    }
}
