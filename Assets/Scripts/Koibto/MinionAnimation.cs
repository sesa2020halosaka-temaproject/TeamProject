using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    public class MinionAnimation : MonoBehaviour
    {


        private void JumpSE()
        {
            SEManager.Instance.Play(SEPath.SE_MINION_JUMP);
        }
        
    }
}