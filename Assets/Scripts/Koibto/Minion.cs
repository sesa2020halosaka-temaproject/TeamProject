using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    public class Minion : System.TransitionObject
    {
        private Vector3 targetPosition;
        public  Vector3 TargetPosition { set { targetPosition = value; } }

        private bool once = false;

        private Animator anima;

        private uint minionNumber = 0;

        [SerializeField, Range(1, 5)]
        [Header("階層の入力")]
        private int floor = 1;
        
        public int Floor { get { return floor; } }

        [SerializeField]
        private GameObject particleSystem;

        // 落ちてるか判断する
        private float yOldPos;

        private bool isFall = true;
        public bool IsFall { get { return isFall; } }

        enum TRANS
        {
            None,
            Wait,
            Move,
            Max,
        }

        public enum MINION_TYPE
        {
            ANEMONE,
            STICK,
            FLAG
        }

        [SerializeField]
        private float speed = 3f;

        private Rigidbody rb;

        private bool FindEndFlag;

        private GameObject player;

        private FloorErase floorErase;

        [SerializeField]
        [Header("Modelのリスト(追加したらここにアタッチ)")]
        private GameObject[] ModelList;

        [SerializeField]
        [Header("どのモデルを適用するか")]
        private MINION_TYPE modelNumber;

        public MINION_TYPE ModelNumber { get { return modelNumber; } }

        // 通常のマテリアル
        private Material normalMaterial;

        // 福田さんのアウトラインマテリアルのアタッチ
        [SerializeField]
        [Header("福田さんのアウトラインマテリアル")]
        private Material outLineMaterial;

        // Modelのレンダラー
        // [x][y]
        // x:アネモネかどうか0body,1anemone;
        // y:メッシュのデータ
        private SkinnedMeshRenderer[] modelRendere;

        private bool choiceFlag;

        public bool ChoiceFlag { set { choiceFlag = value; } }

        [SerializeField]
        [Header("透過マテリアル")]
        private GameObject sphereMaterialObject;
    
        private bool matChangeFlag = false;
        private bool matOldChangeFlag = false;

        private Camera cameraCom;

        // Start is called before the first frame update
        void Start()
        {
            SetMaxFunctionSize((int)TRANS.Max);

            CreateFunction((uint)TRANS.None, None);
            CreateFunction((uint)TRANS.Move, Move);
            CreateFunction((uint)TRANS.Wait, Wait);

            SetFunction((uint)TRANS.Wait);

            rb = GetComponent<Rigidbody>();

            // 一度全体のモデルのactiveをfalse
            foreach(var itr in ModelList)
            {
                itr.SetActive(false);
            }
            // 現在選択されているモデルのペアレントデータ
            var activeModelObject = ModelList[(uint)modelNumber];

            // 指定のモデルのみをTrueに
            activeModelObject.SetActive(true);

            // まとめられているものからAnimatorがついている
            anima = activeModelObject.transform.GetComponent<Animator>();

            floorErase = UnityEngine.Camera.main.transform.root.gameObject.GetComponent<FloorErase>();
            
            // メッシュデータ受け取り
            modelRendere = activeModelObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            
            List<Material> mateList = new List<Material>();

            //// なんかこれで検索排出してくれるらしい
            //// マテリアルごとに分ける
            //var ane = from itr in meshRenderer where itr.material.name == "M_Aenmone" select itr;
            //var body = from itr in meshRenderer where itr.material.name == "M_Minion_02_Ver0511" select itr;

            //// マテリアルの導入
            //modelRendere[0] = ane.ToArray();
            //modelRendere[1] = body.ToArray();
            // 分けなくても1のマテリアル追加したらよかったやんけハゲ

            // modelRendere[0].materials

            cameraCom = UnityEngine.Camera.main.transform.root.GetComponent<Camera>();

            yOldPos = transform.position.y;
        }
        
        private  void None()
        {
        }

        private void Move()
        {
            float speedY = rb.velocity.y;
            var vec = targetPosition - transform.position;
            float lenght = vec.magnitude;

            vec.Normalize();

            vec *= speed;
            if (2f < lenght) vec *= 2;
            vec.y = speedY;

            // プレイヤーとの接触しすぎることを防ぐため、プレイヤーから離れる力を付ける
            var invVec = player.transform.position - transform.position;
            invVec.y = 0f;
            
            rb.velocity = vec - invVec.normalized;

            anima.SetBool("Move", 0.5f < rb.velocity.magnitude);

            if (2 < invVec.magnitude)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z) * invVec.magnitude + new Vector3(0f, rb.velocity.y, 0f);
            }

            LookPlayer();

            ChangeMaterial();
        }

        private void Wait()
        {
            GetFloor();

            ChangeMaterial();

            OldPosJuge();

            CameraFloorPush();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (once) return;
            var obj = other.transform.root;
            if (obj.tag != "Player") return;
            var platoon = obj.gameObject.GetComponent<MinionPlatoon>();
            if (platoon != null) { platoon.In(this);  Debug.Log("toetokeoakge"); }

            player = obj.gameObject;

            // SE再生
            SEManager.Instance.Play(SEPath.SE_GET_MINION);

            SetFunction((uint)TRANS.Move);
            once = true;

            // 発見アニメーション再生
            anima.SetTrigger("Find");
            
            tag = "Hit";

            Instantiate(particleSystem, transform.position + new Vector3(0f, 0.5f), transform.rotation, transform);

            sphereMaterialObject.SetActive(false);

            // パーティクルの再生
            // particleSystem.Play();
        }

        private void LookPlayer()
        {
            transform.LookAt(player.transform, Vector3.up);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }

        [SerializeField]
        private LayerMask groundMask;

        // 現在の自分がいるFloorを取得する
        private void GetFloor()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, -Vector3.up);

            LayerMask layerMask = 11;

            if (!Physics.Raycast(ray, out hit, 5, groundMask)) return;

            GameObject obj = hit.transform.root.gameObject;

            Debug.Log("Minion.cs*" + obj.name);

            var floorObject = floorErase.FloorHitObject;

            for (int i = 0; i < floorObject.Length; i++)
            {
                if (null == floorObject[i])
                {
                    
                    break;
                }
                for (int j = 0; j < floorObject[i].Length; j++)
                {
                    Debug.Log("Minion.cs * " + floorObject[i][j].transform.root.gameObject);
                    Debug.Log("Minion.cs * " + obj.GetInstanceID());
                    Debug.Log("Minion.cs * " + floorObject[i][j].transform.root.gameObject.GetInstanceID());
                    if (obj.GetInstanceID() == floorObject[i][j].transform.root.gameObject.GetInstanceID())
                    {
                        Debug.Log("Minion.cs * NowFloor" + floor);
                        Debug.Log("Minion.cs * NextFloor" + i);
                        floor = i + 1;
                    }
                }
            }
        }

        public void MatChange(bool flag)
        {
            if (modelRendere == null) return;
            if (flag)
            {
                if (modelRendere[0].materials.Length == 1) return;
                
            }
            else
            {
                if (modelRendere[0].materials.Length == 2) return;
              
            }
        }

        private void ChangeMaterial()
        {
            // 現在選択、過去未選択
            if (matChangeFlag && !matOldChangeFlag)
            {
                foreach (var itr in modelRendere)
                {
                    var mats = new Material[2];
                    mats[0] = itr.material;
                    mats[1] = outLineMaterial;
                    itr.materials = mats;
                }
            }
            // 現在未選択、過去選択
            else if (!matChangeFlag && matOldChangeFlag)
            {
                foreach (var itr in modelRendere)
                {
                    var mats = new Material[1];
                    mats[0] = itr.material;
                    itr.materials = mats;
                }
            }

            matOldChangeFlag = matChangeFlag;

            matChangeFlag = false;
        }

        private void OldPosJuge()
        {
            isFall = transform.position.y != yOldPos;

            yOldPos = transform.position.y;
        }

        public void OnFlag()
        {
            matChangeFlag = true;
        }

        private void CameraFloorPush()
        {
            cameraCom.SetFloorMinionStayFlag((uint)floor - 1);
        }
    }
}