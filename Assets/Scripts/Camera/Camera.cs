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

        private UnityEngine.Camera mainCamCom;
        private UnityEngine.Camera subCamCom;

        private float veiwMain;

        private Goal goal;

        private GameObject mainCameraGameObject;

        private Vector3 startPos;
        private Quaternion startQua;

        private GameObject laneObj;

        private Vector3 lanePos;

        private Quaternion laneQua;

        [SerializeField]
        private float goalSpeed=1f;

        private PlayerVer2 player;

        private bool seamlessEnd = false;
        public bool SeamlessEnd { get { return seamlessEnd; } }

        // Start is called before the first frame update
        void Start()
        {
            SetMaxFunctionSize((uint)TRANS.Max);

            CreateFunction((uint)TRANS.None, None);
            CreateFunction((uint)TRANS.Upd, Upd);
            CreateFunction((uint)TRANS.Goal, Goal);
            CreateFunction((uint)TRANS.Goal2, Goal2);

            SetFunction((uint)TRANS.Upd);

            mainCamCom = UnityEngine.Camera.main;
            mainCameraGameObject = UnityEngine.Camera.main.gameObject;

            veiwMain = mainCamCom.fieldOfView;

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

            float x=0, y=0;
            float speedTime = speed * Time.deltaTime;

            //if (Input.GetKey(KeyCode.W))
            //{
            //    y = speedTime;
            //}
            //if (Input.GetKey(KeyCode.S))
            //{
            //    y = -speedTime;
            //}
            //if (Input.GetKey(KeyCode.D))
            //{
            //    x = speedTime;
            //}
            //if (Input.GetKey(KeyCode.A))
            //{
            //    x = -speedTime;
            //}


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
            var playerTrans = player.transform;
            var playerHedPos = playerTrans.position + new Vector3(0f, 2f);

            time += Time.deltaTime * goalSpeed;

            // camTrans.position = Vector3.Slerp(startPos, playerHedPos, time);
            // camTrans.position = Vector3.Lerp(startPos, playerHedPos, time);

            // camTrans.rotation = Quaternion.Slerp(startQua, playerTrans.rotation, time);

            camTrans.position = Vector3.Lerp(startPos, subCamTrans.position, time);

            camTrans.rotation = Quaternion.Slerp(startQua, subCamTrans.rotation, time);

            player.PlayerRendNot();

            mainCamCom.fieldOfView -= (veiwMain - subCamCom.fieldOfView) * Time.deltaTime * goalSpeed;
            if (mainCamCom.fieldOfView < subCamCom.fieldOfView) mainCamCom.fieldOfView = subCamCom.fieldOfView;
            //if (0.8f <= time)
            //{
            //    player.gameObject.SetActive(false);
            //}
            if (1f * goalSpeed <= time)
            {
                //time = Time.deltaTime * goalSpeed;
                //SetFunction((uint)TRANS.Goal2);
                //laneQua = camTrans.rotation;
                //lanePos = camTrans.position;
                mainCamCom.fieldOfView = subCamCom.fieldOfView;
               //  gameObject.SetActive(false);
                seamlessEnd = true;
            }
        }

        private void Goal2()
        {
            var camTrans = mainCameraGameObject.transform;
            var goalTrans = goal.transform;
            var subCamTrans = subCamera.transform;
            var laneTrans = laneObj.transform;

            time += Time.deltaTime;
            if (1f < time) { time = 1f; }

            camTrans.position = Vector3.Lerp(lanePos, subCamTrans.position, time);

            camTrans.rotation = Quaternion.Lerp(laneQua, subCamTrans.rotation, time);

            mainCamCom.fieldOfView -= (veiwMain - subCamCom.fieldOfView) * Time.deltaTime;
            // camTrans.LookAt(goalTrans.position + new Vector3(0f, 1f * goalTrans.localScale.y), Vector3.up);
            if (camTrans.position == subCamTrans.position)
            {
                mainCamCom.fieldOfView = subCamCom.fieldOfView;
                goal.GoalStart();
                gameObject.SetActive(false);
            }
        }

        public void SetGoalCom(Goal _goal)
        {
            goal = _goal;

            subCamera = goal.SubCameraObj;
            subCamCom = subCamera.GetComponent<UnityEngine.Camera>();

            laneObj = _goal.LaneObj;
            laneObj.transform.LookAt(goal.transform.position + new Vector3(0f, 1f * goal.transform.localScale.y), Vector3.up);
        }

        public void StartSeamless()
        {
            startPos = mainCameraGameObject.transform.position;
            startQua = mainCameraGameObject.transform.rotation;
        }

        public void GetPlayer(PlayerVer2 _player)
        {
            player = _player;
        }
    }
}