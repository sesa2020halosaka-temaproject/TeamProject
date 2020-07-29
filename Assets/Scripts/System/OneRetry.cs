using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamProject
{
    public class OneRetry : MonoBehaviour
    {
        [SerializeField]
        private InputManager.ButtonCode retryButtun;

        private PlayerVer2 player;

        private Pause pause;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform.root.gameObject.GetComponent<PlayerVer2>();

            pause = GameObject.FindGameObjectWithTag("StageObjectCunvas").transform.root.GetComponentInChildren<Pause>();
        }

        // Update is called once per frame
        void Update()
        {
            if (InputManager.InputManager.Instance.GetKeyDown(retryButtun))
            {
                bool playerFlag = player.NowFunctionNum == (uint)PlayerVer2.TRANSITION.Goal|| player.NowFunctionNum == (uint)PlayerVer2.TRANSITION.Move;
                bool pauseFlag = pause.NowFunctionNum != (uint)Pause.TRANS.PauseWait;

                if (!playerFlag || pauseFlag)
                {
                    var sceneName = SceneManager.GetActiveScene().name;
                    FadeManager.FadeOut(sceneName);
                }
            }
        }
    }
}