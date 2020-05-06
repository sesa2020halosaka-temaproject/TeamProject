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
        
        // Start is called before the first frame update
        void Start()
        {
            SetMaxFunctionSize((int)TRANS.Max);

            CreateFunction((uint)TRANS.None, None);
            CreateFunction((uint)TRANS.Move, Move);
            CreateFunction((uint)TRANS.Wait, Wait);

            SetFunction((uint)TRANS.Wait);

            rb = GetComponent<Rigidbody>();
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

            rb.velocity = vec;
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

            SetFunction((uint)TRANS.Move);
            once = true;

            tag = "Hit";
        }
    }
}