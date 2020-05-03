using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //CinemachineMixingCameraのセット
    public class SetMixingCamera : MonoBehaviour
    {
        public GameObject[] Stages;//LookAtの対象となるゲームオブジェクトの格納用

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        ////ミキシングカメラのセット（カメラ注視点：現在と次、ミキシングの状態、カメラウェイトのリセット）
        //public void SetMixCamera(GameObject _MixingCameraObject, STAGE_NO _CurrentStageNo, STAGE_NO _NextStageNo, string _Word)
        //{
        //    _MixingCameraObject.GetComponent<MixingCamera>().LookAtTargetTwoChanges(Stages[(int)_CurrentStageNo], Stages[(int)_NextStageNo]);
        //    _MixingCameraObject.GetComponent<MixingCamera>().MixState(_Word);
        //    _MixingCameraObject.GetComponent<MixingCamera>().ResetWeight();

        //}
        //private void MixCameraGo(STAGE_NO CurrentStage, STAGE_NO NextStage)
        //{

        //    SetMixCamera(_Mixing, CurrentStage, NextStage, "GO");


        //    dolly_state = DOLLY_STATE.GO;
        //    select_state = SELECT_STATE.SWING;
        //    //カーソルの移動音
        //    SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);

        //}
        //private void MixCameraBack(STAGE_NO CurrentStage, STAGE_NO NextStage)
        //{

        //    SetMixCamera(_Mixing, CurrentStage, NextStage, "BACK");

        //    dolly_state = DOLLY_STATE.BACK;
        //    select_state = SELECT_STATE.SWING;

        //}


    }//public class SetMixingCamera : MonoBehaviour END
}//namespace END