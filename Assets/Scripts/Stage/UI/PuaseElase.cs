using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TeamProject
{
    public class PuaseElase : MonoBehaviour
    {
        private Pause pause;
        private PlayerVer2 player;

        private RawImage image;

        // Start is called before the first frame update
        void Start()
        {
            GameObject uiChild = GameObject.FindGameObjectWithTag("StageObjectCunvas");

            //var uiParentPbject = uiChild.transform.root.gameObject;

            pause = uiChild.GetComponentInChildren<Pause>();

            var playerObj = GameObject.FindGameObjectWithTag("Player");

            player = playerObj.transform.root.GetComponent<PlayerVer2>();

            image = GetComponent<RawImage>();
        }

        // Update is called once per frame
        void Update()
        {
            image.enabled = !(player.NowFunctionNum == (uint)PlayerVer2.TRANSITION.Goal || pause.NowFunctionNum != (uint)Pause.TRANS.PauseWait);
        }
    }
}