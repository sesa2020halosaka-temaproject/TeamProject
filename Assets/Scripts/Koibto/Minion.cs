using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    public class Minion : System.TransitionObject
    {
        private Vector3 targetPosition;
        public  Vector3 TargetPosition { set { targetPosition = value; } }

        private bool once = false;

        private Animator anima;
        
        enum TRANS
        {
            None,
            Wait,
            Move,
            Max,
        }

        [SerializeField]
        private float speed = 3f;

        private Rigidbody rb;

        private bool FindEndFlag;

        private GameObject player;
        
        // Start is called before the first frame update
        void Start()
        {
            SetMaxFunctionSize((int)TRANS.Max);

            CreateFunction((uint)TRANS.None, None);
            CreateFunction((uint)TRANS.Move, Move);
            CreateFunction((uint)TRANS.Wait, Wait);

            SetFunction((uint)TRANS.Wait);

            rb = GetComponent<Rigidbody>();
            anima = transform.GetComponentInChildren<Animator>();
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
            // 正規化
            invVec.Normalize();


            rb.velocity = vec- invVec;

            anima.SetBool("Move", 0.5f < rb.velocity.magnitude);




            LookPlayer();
        }

        private void Wait()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (once) return;
            var obj = other.transform.root;
            if (obj.tag != "Player") return;
            var platoon = obj.gameObject.GetComponent<MinionPlatoon>();
            if (platoon != null) { platoon.In(this);  Debug.Log("toetokeoakge"); }

            player = obj.gameObject;

            SetFunction((uint)TRANS.Move);
            once = true;

            // 発見アニメーション再生
            anima.SetTrigger("Find");
            
            tag = "Hit";
        }

        private void LookPlayer()
        {
            transform.LookAt(player.transform, Vector3.up);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }
    }
}