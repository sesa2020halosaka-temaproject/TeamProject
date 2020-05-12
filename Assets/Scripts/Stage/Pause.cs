using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamProject.InputManager;

namespace TeamProject {

    public class Pause : System.TransitionObject
    {
        enum TRANS
        {
            None,
            Pause,
            PauseWait,
            Max
        }

        // ポーズ中のフラグ
        private bool pauseFlag;

        // ポーズ中のフラグ
        public bool PauseFlag { get { return pauseFlag; } }

        // ロゴをまとめたゲームオブジェクト
        private GameObject logo;

        // ロゴのアニメーション
        private Animator anima;

        private List<string> logoNames;

        private int nowLogoNumber;
        private int maxLogoNumber;

        private bool oldUpKey;
        private bool oldDownKey;

        // プレイヤー
        private PlayerVer2 player;

        private uint playerFuncNumber;

        private Camera camera;

        // Start is called before the first frame update
        void Start()
        {
            pauseFlag = false;

            logo = transform.GetChild(0).GetChild(0).gameObject;

            anima = logo.GetComponentInChildren<Animator>();

            nowLogoNumber = 0;
            maxLogoNumber = logo.transform.childCount;

            logoNames = new List<string>();

            for (int i = 0; i < maxLogoNumber; i++)
            {
                logoNames.Add(logo.transform.GetChild(i).name);
            }

            Debug.Log(logoNames);

            oldUpKey = false;
            oldDownKey = false;

            // プレイヤーを取得する
            player = GameObject.FindWithTag("Player").transform.root.gameObject.GetComponent<PlayerVer2>();

            SetMaxFunctionSize((uint)TRANS.Max);

            CreateFunction((uint)TRANS.None, None);
            CreateFunction((uint)TRANS.Pause, PauseNow);
            CreateFunction((uint)TRANS.PauseWait, PauseWait);

            SetFunction((uint)TRANS.PauseWait);

            camera = UnityEngine.Camera.main.transform.parent.GetComponent<Camera>();
        }
        
        private void None()
        {

        }

        // ポーズ中
        private void PauseNow()
        {
            // キー取得
            var ipMane = InputManager.InputManager.Instance;

            var upKey = ipMane.GetArrow(ArrowCoad.UpArrow);
            var downKey = ipMane.GetArrow(ArrowCoad.DownArrow);
            var selectKey = ipMane.GetKeyDown(ButtunCode.B);
            var returnKey = ipMane.GetKeyDown(ButtunCode.A);

            var upTrigger = upKey && !oldUpKey;
            var downTrigger = downKey && !oldDownKey;

            // 上下のキー
            if (upTrigger)
            {
                nowLogoNumber--;

                if (nowLogoNumber < 0)
                {
                    nowLogoNumber = maxLogoNumber - 1;
                }
            }

            if (downTrigger)
            {
                nowLogoNumber++;

                if (maxLogoNumber == nowLogoNumber)
                {
                    nowLogoNumber = 0;
                }
            }
            Debug.Log(nowLogoNumber);
            Debug.Log(maxLogoNumber);
            // どちらか押されると反応
            if (upTrigger || downKey)
            {
                anima.SetTrigger(logoNames[nowLogoNumber]);
            }

            // セレクト
            if (selectKey)
            {
                // フェードアウト
                StartCoroutine(ToTitle());

                // 何も反応しないようにする
                SetFunction((uint)TRANS.None);
            }

            // oldKey取得
            oldUpKey = upKey;
            oldDownKey = downKey;
            
            // メニューが押されると戻る
            if (InputManager.InputManager.Instance.GetKeyDown(ButtunCode.Menu))
            {
                PauseEnd();
            }

            if (returnKey)
            {
                PauseEnd();
            }
        }

        // ポーズ待ち
        private void PauseWait()
        {
            if (InputManager.InputManager.Instance.GetKeyDown(ButtunCode.Menu))
            {
                PauseStart();
            }
        }

        public void PauseStart()
        {
            pauseFlag = true;
            nowLogoNumber = 0;

            logo.transform.parent.gameObject.SetActive(true);

            SetFunction((uint)TRANS.Pause);
            player.SetFunction((uint)PlayerVer2.TRANSITION.None);
            camera.SetFunction((uint)Camera.TRANS.None);
        }

        public void PauseEnd()
        {
            logo.transform.parent.gameObject.SetActive(false);

            pauseFlag = false;

            SetFunction((uint)TRANS.PauseWait);
            player.SetFunction((uint)PlayerVer2.TRANSITION.Choice); ;
            camera.SetFunction((uint)Camera.TRANS.Upd);
        }

        public IEnumerator ToTitle()
        {
            yield return new WaitForSeconds(0.0f);
            Debug.Log("タイトルへ戻りまーす！");
            
            switch (logoNames[nowLogoNumber])
            {
                case "Title":
                    FadeManager.FadeOut("TitleScene");
                    break;
                case "Retry":
                    var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    FadeManager.FadeOut(sceneName);
                    break;
                case "Select":
                    FadeManager.FadeOut("StageSelectScene");
                    break;
            }
        }
    }
}