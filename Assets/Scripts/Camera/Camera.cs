using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TeamProject.InputManager;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    public class Camera : System.TransitionObject
    {
        public enum TRANS
        {
            None,
            StageStart,
            Upd,
            Goal,
            Goal2,
            Max
        }

        [SerializeField]
        [Header("ズームの最大、最小のパラメータ")]
        private float zoomMax, zoomMin;

        [SerializeField]
        private float speed;

        [SerializeField]
        [Header("カメラの寄り引きの速度")]
        private float inOutSpeed;

        [SerializeField]
        [Header("一階層の高さ")]
        private float hightLenge;

        [SerializeField, Range(1, 5)]
        [Header("ステージごとの階層")]
        private int hight = 1;
        public int Hight { get { return hight; } }

        [SerializeField]
        [Header("階層分けのキー割り当て")]
        private ButtonCode hightChangeButtun;

        [SerializeField]
        [Header("移動差分量")]
        private float deffSpeed = 0.6f;

        [SerializeField]
        private float min, max;

        [SerializeField]
        private float startRotSpeed;

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
        private float goalSpeed = 1f;

        private PlayerVer2 player;

        private bool seamlessEnd = false;
        public bool SeamlessEnd { get { return seamlessEnd; } }

        private float startRot = 0f;

        private Transform child;

        private int nowHight;

        //カメラの
        public int NowHight { get { return nowHight; } }

        private float targetHight;

        private GameObject lip,lipStart;

        // ベジュ曲線
        private Vector3 startPlayerPos, NextPlayerPos, EndPos;
        private Vector3 startPlayerRot, NextPlayerRot, EndRot;

        private bool[] floorMinionStayFlag = new bool[5];
        public bool[] FloorMinionStayFlag { get { return floorMinionStayFlag; } }
        public void SetFloorMinionStayFlag(uint _i) { floorMinionStayFlag[_i] = true; }

        // Start is called before the first frame update
        void Start()
        {
            SetMaxFunctionSize((uint)TRANS.Max);

            CreateFunction((uint)TRANS.None, None);
            CreateFunction((uint)TRANS.StageStart, StageStart);
            CreateFunction((uint)TRANS.Upd, Upd);
            CreateFunction((uint)TRANS.Goal, Goal);
            CreateFunction((uint)TRANS.Goal2, Goal2);

            SetFunction((uint)TRANS.StageStart);

            mainCamCom = UnityEngine.Camera.main;
            mainCameraGameObject = UnityEngine.Camera.main.gameObject;

            veiwMain = mainCamCom.fieldOfView;

            volume = GetComponentInChildren<Volume>();

            time = 0f;

            startQua = transform.rotation;

            child = transform.GetChild(0);

            nowHight = hight;

            targetHight = transform.position.y;
            player = GameObject.FindGameObjectWithTag("Player").transform.root.gameObject.GetComponent<PlayerVer2>();

            var lips = player.transform.GetChild(4);
            var camRot = transform.root.localEulerAngles;
            camRot.x = camRot.z = 0f;
            lips.eulerAngles = camRot;
            lip = lips.GetChild(0).gameObject;
            lipStart = lips.GetChild(1).gameObject;

            startPlayerPos = lipStart.transform.position;
            NextPlayerPos = lip.transform.position;
            EndPos = mainCameraGameObject.transform.position;

            // startPlayerRot = lipStart.transform.rotation.eulerAngles;
            // NextPlayerRot = lip.transform.rotation.eulerAngles;
            // EndRot = mainCameraGameObject.transform.rotation.eulerAngles;
        }

        private void None()
        {
            // None
            volume.enabled = true;
        }

        //private void StageStart()
        //{
        //    // volumeをオフに
        //    volume.enabled = false;

        //    var speedTime = Time.deltaTime * startRotSpeed;

        //    startRot += speedTime;

        //    var eulRot = transform.rotation.eulerAngles;

        //    eulRot.y += speedTime;

        //    transform.rotation = Quaternion.Euler(eulRot);
            
        //    if (360f < startRot)
        //    {
        //        transform.rotation = startQua;

        //        SetFunction((uint)TRANS.Upd);
        //    }
        //}
        private void StageStart()
        {
            // volumeをオフに
            volume.enabled = false;

            var speedTime = Time.deltaTime / startRotSpeed;

            var pos = GetPoint(startPlayerPos, NextPlayerPos, NextPlayerPos, EndPos, startRot);
            // var rot= GetPoint(startPlayerRot, NextPlayerRot, NextPlayerRot, EndRot, startRot);

            mainCameraGameObject.transform.position = pos;
            // mainCameraGameObject.transform.rotation = Quaternion.Euler(rot);

            // 終わり際に速度を半減してみる
            if(0.8f < startRot)
            {
                speedTime *= 0.8f;
            }

            //transform.rotation = Quaternion.Euler(eulRot);

            startRot += speedTime;  

            if (1f < startRot)
            {
                transform.rotation = startQua;

                mainCameraGameObject.transform.position = EndPos;
                // mainCameraGameObject.transform.rotation = Quaternion.Euler(EndRot);
                
                SetFunction((uint)TRANS.Upd);
            }
        }

        private void Upd()
        {
            // volumeをオフに
            volume.enabled = false;

            var stick = InputManager.InputManager.Instance.GetRStick();

            Vector3 rot = transform.rotation.eulerAngles;

            float x=0, y=0;
            float speedTime = speed * Time.deltaTime;
            float inOutSpeedTime = inOutSpeed * Time.deltaTime;

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
            
            x = stick.x * speedTime;
            y = stick.y * speedTime;

            rot.x += y;
            rot.y -= x;

            if (rot.x < min)
            {
                rot.x = min;
            }
            if (max < rot.x)
            {
                rot.x = max;
            }

            //if (rot.y + y < min)
            //{
            //    rot.y = min;
            //}
            //if (max < rot.y + y)
            //{
            //    rot.y = max;
            //}

            var qua = transform.rotation;
            rot.z = 0.0f;
            qua.eulerAngles = rot;
            transform.rotation = qua;

            // 高さ変更
            HightChange();

            // ズーム処理
            Zoom();

            var diff = targetHight - transform.position.y;

            diff *= deffSpeed;

            var nowPos = transform.position;

            nowPos.y += diff;

            transform.position = nowPos;
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
                camTrans.position = subCamTrans.position;
                camTrans.rotation =  subCamTrans.rotation;

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

        // 高さ変更部分
        private void HightChange()
        {
            var inputKey = InputManager.InputManager.Instance.GetKeyDown(hightChangeButtun);

            var oldHight = nowHight;
            if (inputKey)
            {
                do
                {
                    nowHight--;

                    targetHight -= hightLenge;

                    // 高さが想定以上に行ったとき
                    if (nowHight < 1)
                    {
                        nowHight = hight;

                        targetHight += hightLenge * hight;
                    }

                } while (!floorMinionStayFlag[nowHight - 1]);
            }

            if(!floorMinionStayFlag[nowHight - 1])
            {
                do
                {
                    nowHight--;

                    targetHight -= hightLenge;

                    // 高さが想定以上に行ったとき
                    if (nowHight < 1)
                    {
                        nowHight = hight;

                        targetHight += hightLenge * hight;
                    }

                } while (!floorMinionStayFlag[nowHight - 1]);
            }

            if (oldHight != nowHight)
            {
                SEManager.Instance.Play(SEPath.SE_HIERARCHY);
            }
            floorMinionStayFlag = new bool[5] { false, false, false, false, false };

        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Vector3 thisObjPos = gameObject.transform.position;

            Gizmos.DrawSphere(thisObjPos, 1f);
        }

        // ベジュ曲線ネット引用
        private Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            var oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * oneMinusT * p0 +
                   3f * oneMinusT * oneMinusT * t * p1 +
                   3f * oneMinusT * t * t * p2 +
                   t * t * t * p3;
        }

        private void Zoom()
        {
            var locPos = mainCameraGameObject.transform.localPosition;

            // 長さ割出
            var length = locPos.magnitude;

            var r1 = InputManager.InputManager.Instance.GetKey(ButtonCode.R1);
            var l1 = InputManager.InputManager.Instance.GetKey(ButtonCode.L1);

            if (r1)
            {
                length += inOutSpeed * Time.deltaTime;
            }
            if (l1)
            {
                length -= inOutSpeed * Time.deltaTime;
            }

            // 正規化して方向ベクトルに変換
            var nor = locPos.normalized;

            if(zoomMax < length) 
            {
                length = zoomMax;
            }
            if (length < zoomMin) 
            {
                length = zoomMin;
            }

            mainCameraGameObject.transform.localPosition = nor * length;
        }
    }
}


