using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}