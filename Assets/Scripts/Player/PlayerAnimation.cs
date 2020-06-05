using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    public class PlayerAnimation : MonoBehaviour
    {
        private PlayerVer2 player;
        
        // Start is called before the first frame update
        void Start()
        {
            player = GetComponentInParent<PlayerVer2>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void EndFindAnima()
        {
            player.EndFindAnima();
        }

        private void EndFallAnima()
        {
            player.EndFallAnima();
        }
        private void StartJumpStart()
        {
            player.JumpStart();
        }

        private void SEPlayerLR()
        {
            SEManager.Instance.Play(SEPath.SE_PLAYER_LR);
        }

        private void SEPlayerStop()
        {
            SEManager.Instance.Play(SEPath.SE_PLAYER_STOP);
        }

        private void StageStartEnd()
        {
            player.StartAnimationEnd();
        }

        private void StartStageStartAnimation()
        {
            SEManager.Instance.Play(SEPath.SE_FLYING_PC);
        }
    }
}