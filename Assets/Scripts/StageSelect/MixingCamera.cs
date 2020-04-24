using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TeamProject
{
    public class MixingCamera : MonoBehaviour
    {
        public CinemachineVirtualCamera vcam_before;
        public CinemachineVirtualCamera vcam_after;
        public CinemachineMixingCamera mix_cam;

        public float cam_weight;//カメラ1，2のウェイト値

        [Min(0)]
        public float m_Go_SwingTime;//前進時のカメラ方向転換時間
        [Min(0)]
        public float m_Back_SwingTime;//後進時のカメラ方向転換時間

        private float m_SwingTime;//カメラ方向転換時間

        private float AddRatio;//加算倍率（+1,0,-1）

        //カメラのパスの位置の移動
        public enum MIX_MOVE
        {
            FIXING,//固定
            GO,  //進める
            BACK   //戻る
        }
        public MIX_MOVE Move_state;

        public bool Swing_flag;//カメラの方向が切り替え中かどうか

        // Start is called before the first frame update
        void Start()
        {
            cam_weight = 0.0f;

            this.MixState("ZERO");
        }

        // Update is called once per frame
        void Update()
        {

            switch (Move_state)
            {
                case MIX_MOVE.FIXING:
                    break;
                case MIX_MOVE.GO:
                    cam_weight += Time.deltaTime * AddRatio / m_SwingTime;
                    if (cam_weight > 1.0f)
                    {
                        cam_weight = 1.0f;
                        //方向転換完了
                        this.MixState("ZERO");

                    }
                    break;
                case MIX_MOVE.BACK:
                    cam_weight += Time.deltaTime * AddRatio / m_SwingTime;
                    if (cam_weight > 1.0f)
                    {
                        cam_weight = 1.0f;
                        //方向転換完了
                        this.MixState("ZERO");

                    }
                    break;
                default:
                    break;
            }
            mix_cam.m_Weight0 = 1 - cam_weight;
            mix_cam.m_Weight1 = cam_weight;
        }// void Update() END

        //ドリーの状態を変える
        public void MixState(string word)
        {
            switch (word)
            {
                case "ZERO":
                    AddRatio = 0.0f;
                    Move_state = MIX_MOVE.FIXING;
                    Swing_flag = false;//完了
                    break;
                case "GO":
                    AddRatio = 1.0f;
                    m_SwingTime = m_Go_SwingTime;
                    Move_state = MIX_MOVE.GO;
                    Swing_flag = true;//開始

                    break;
                case "BACK":
                    AddRatio = 1.0f;
                    m_SwingTime = m_Back_SwingTime;
                    Move_state = MIX_MOVE.BACK;
                    Swing_flag = true;//開始

                    break;
                default:
                    Debug.Log("言葉が違います。カメラを固定します。");
                    AddRatio = 0.0f;
                    break;
            }
        }//    public void DollyState(string word)  END

        //方向変換中かどうかboolで返す。
        public bool IsSwing()
        {
            return Swing_flag;
        }

        //フラグをfalseにする
        public void SwingFlagOff()
        {

            Swing_flag = false;
        }

        public void ResetWeight()
        {
            cam_weight = 0.0f;
        }

        //現在のターゲットと次のターゲットのLookAtをセットする
        public void LookAtTargetTwoChanges(GameObject CurrentTarget, GameObject NextTarget)
        {
            vcam_before.LookAt = CurrentTarget.transform;
            vcam_after.LookAt = NextTarget.transform;


        }// END
    }
}