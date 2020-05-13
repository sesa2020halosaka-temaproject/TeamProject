using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TeamProject.InputManager;

namespace TeamProject
{
    public class Camera : System.TransitionObject
    {
        public enum TRANS
        {
            None,
            Upd,
            Goal,
            Goal2,
            Max
        }

        [SerializeField]
        private float speed;

        [SerializeField]
        private float min, max;

        private Volume volume;

        private GameObject subCamera;

        private Goal goal;

        private GameObject mainCameraGameObject;

        private Vector3 startPos;

        private GameObject laneObj;

        // Start is called before the first frame update
        void Start()
        {
            SetMaxFunctionSize((uint)TRANS.Max);

            CreateFunction((uint)TRANS.None,None);
            CreateFunction((uint)TRANS.Upd, Upd);
            CreateFunction((uint)TRANS.Goal, Goal);
            CreateFunction((uint)TRANS.Goal2, Goal2);

            SetFunction((uint)TRANS.Upd);
            
            mainCameraGameObject = UnityEngine.Camera.main.gameObject;

            volume = GetComponentInChildren<Volume>();

            time = 0f;
        }

        private void None()
        {
            // None
            volume.enabled = true;
        }

        private void Upd()
        {
            // volumeをオフに
            volume.enabled = false;

            var stick = InputManager.InputManager.Instance.GetRStick();

            Vector3 rot = transform.rotation.eulerAngles;

            float x, y;

            x = stick.x * speed * Time.deltaTime;
            y = stick.y * speed * Time.deltaTime;

            rot.x += y;
            rot.y -= x;

            if (rot.x + x < min)
            {
                rot.x = min;
            }
            if (max < rot.x + x)
            {
                rot.x = max;
            }

            var qua = transform.rotation;
            rot.z = 0.0f;
            qua.eulerAngles = rot;
            transform.rotation = qua;
        }

        private float time;

        private void Goal()
        {
            var camTrans=mainCameraGameObject.transform;
            var goalTrans = goal.transform;
            var subCamTrans = subCamera.transform;
            var laneTrans = laneObj.transform;

            time += Time.deltaTime;

            camTrans.position = Vector3.Slerp(startPos, laneTrans.position, time);

            camTrans.LookAt(goalTrans.position + new Vector3(0f, 1f * goalTrans.localScale.y), Vector3.up);
        }

        private void Goal2()
        {

        }

        public  void SetGoalCom(Goal _goal) {
            goal = _goal;

            subCamera = goal.SubCameraObj;

            startPos = mainCameraGameObject.transform.position;

            laneObj = _goal.LaneObj;
        }
    }
}