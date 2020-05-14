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
        private Quaternion startQua;

        private GameObject laneObj;

        private Quaternion laneQua;

        // Start is called before the first frame update
        void Start()
        {
            SetMaxFunctionSize((uint)TRANS.Max);

            CreateFunction((uint)TRANS.None, None);
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
            var camTrans = mainCameraGameObject.transform;
            var goalTrans = goal.transform;
            var subCamTrans = subCamera.transform;
            var laneTrans = laneObj.transform;

            time += Time.deltaTime;

            //camTrans.position = Vector3.Slerp(startPos, laneTrans.position, time);

            //camTrans.rotation = Quaternion.Slerp(startQua, laneTrans.rotation, time);
            camTrans.position = Vector3.Slerp(startPos, subCamTrans.position, time);

            camTrans.rotation = Quaternion.Slerp(startQua, subCamTrans.rotation, time);
            // camTrans.LookAt(goalTrans.position + new Vector3(0f, 1f * goalTrans.localScale.y), Vector3.up);

            if (1f <= time)
            {
                // time = 0f;
                // SetFunction((uint)TRANS.Goal2);

                // laneQua = camTrans.rotation;

                ////  Debug.Break();
                // Debug.Log(laneQua.eulerAngles + "+" + subCamera.transform.rotation.eulerAngles);
                goal.GoalStart();
            }
        }

        private void Goal2()
        {
            var camTrans = mainCameraGameObject.transform;
            var goalTrans = goal.transform;
            var subCamTrans = subCamera.transform;
            var laneTrans = laneObj.transform;

            time += Time.deltaTime;

            camTrans.position = Vector3.Slerp(laneTrans.position, subCamTrans.position, time);

            camTrans.rotation = Quaternion.Lerp(laneQua, subCamTrans.rotation, time);

            // camTrans.LookAt(goalTrans.position + new Vector3(0f, 1f * goalTrans.localScale.y), Vector3.up);

            if (1f <= time)
            {
                goal.GoalStart();
            }
        }

        public void SetGoalCom(Goal _goal)
        {
            goal = _goal;

            subCamera = goal.SubCameraObj;

            startPos = mainCameraGameObject.transform.position;
            startQua = mainCameraGameObject.transform.rotation;

            laneObj = _goal.LaneObj;
            laneObj.transform.LookAt(goal.transform.position + new Vector3(0f, 1f * goal.transform.localScale.y), Vector3.up);
        }
    }
}