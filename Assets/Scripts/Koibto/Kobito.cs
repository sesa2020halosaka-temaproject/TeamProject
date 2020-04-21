using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    public class Kobito : MonoBehaviour
    {
        private bool sentakuFlag;
        public bool SentakuFlag { get { return sentakuFlag; }set { sentakuFlag = value; } }

        private Material baseMat;
        private Material mat;

        [SerializeField]
        private Material setMaterial;

        private SkinnedMeshRenderer[] renderer;

        private bool trackingFlag;

        private SphereCollider sphereCollider;

        private GameObject player;

        // 追跡の幅の間隔
        [SerializeField]
        private float trackingLenght;

        [SerializeField]
        private  float maxSpeed;

        [SerializeField]
        [Range(0.0f, 2.0f)]
        private  float delay;

        [SerializeField]
        private float animeChangeSpeed;

        private Rigidbody rigidbody;

        private Animator animator;

        private string[] kobitoMoveSe; 
        
        //[SerializeField]
        //public SphereCollider SphereCollider { get { return sphereCollider;  } }

        // Start is called before the first frame update
        void Start()
        {
            renderer = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (renderer.Length!=0) baseMat = mat = renderer[0].material;
            sentakuFlag = true;
            trackingFlag = false;
            sphereCollider = transform.GetChild(0).GetComponent<SphereCollider>();
            rigidbody = GetComponent<Rigidbody>();
            animator = transform.GetComponentInChildren<Animator>();
            kobitoMoveSe = new string[] { SEPath.SE_MINION_WALK_GROUND1, SEPath.SE_MINION_WALK_GROUND2, SEPath.SE_MINION_WALK_GROUND3, SEPath.SE_MINION_WALK_GROUND4 };
        }

        // Update is called once per frame
        void Update()
        {
            Tracking();

            foreach (var itr in renderer) itr.material = mat;

            mat = baseMat;
        }

        // 選択中にマテリアル入れ替え
        public void Sentaku()
        {
            mat = setMaterial;
        }

        private void Tracking()
        {
            if (!trackingFlag) return;


            var forw = player.transform.position - transform.position;

            transform.rotation = Quaternion.LookRotation(forw, Vector3.up);

            var length = Vector3.Distance(transform.position, player.transform.position);
            var lookVector = (player.transform.position - transform.position ).normalized;

            length -= trackingLenght;

            length *= delay;

            if (maxSpeed < length)
            {
                length = maxSpeed;
            }
            // Debug.Log(length);
            float tatat = 0.3f;
            if (length <= tatat) length *= 0;

             lookVector *= length;

             rigidbody.MovePosition(transform.position + lookVector);

            animator.SetBool("Move", lookVector.magnitude> animeChangeSpeed);

            if (lookVector.magnitude > animeChangeSpeed)
            {
                var randNum = Random.Range(1, 4);
                SEManager.Instance.Play(kobitoMoveSe[randNum]);
            }
            // Debug.Log(lookVector.magnitude);

            // rigidbody.AddForce(lookVector);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.tag == "Player")
            {
                Debug.Log("Yes");
                trackingFlag = true;
                sphereCollider.enabled = false;
                player = other.transform.root.gameObject;
            }
        }
    }
}