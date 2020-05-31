using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamProject.InputManager;
using KanKikuchi.AudioManager;

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

        private uint beforPlayerTrans;
        private uint beforCamerTrans;

        // ポーズ中のフラグ
        public bool PauseFlag { get { return pauseFlag; } }

        // ロゴをまとめたゲームオブジェクト
        private GameObject logo;

        // ロゴのアニメーション
        // private Animator anima;

        private List<string> logoNames;

        private int nowLogoNumber;
        private int maxLogoNumber;

        private bool oldUpKey;
        private bool oldDownKey;

        // プレイヤー
        private PlayerVer2 player;

        private uint playerFuncNumber;

        private Camera camera;

        [SerializeField]
        private Sprite[] sprite;

        [SerializeField]
        private Image stageNumber;

        [SerializeField]
        private Image worldNumber;

        [SerializeField]
        private Image retImg;
        [SerializeField]
        private Image selImg;
        [SerializeField]
        private Image titlImg;

        [SerializeField]
        private Sprite[] ret = new Sprite[2];
        [SerializeField]
        private Sprite[] sel = new Sprite[2];
        [SerializeField]
        private Sprite[] tit = new Sprite[2];

        // Start is called before the first frame update
        void Start()
        {
            pauseFlag = false;

            logo = transform.GetChild(0).GetChild(0).gameObject;

            // anima = logo.GetComponentInChildren<Animator>();

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

            // sprite
            var worNum = StageStatusManager.Instance.CurrentWorld;
            var staNum = StageStatusManager.Instance.StageInWorld;

            stageNumber.sprite = sprite[staNum + 1];
            worldNumber.sprite = sprite[worNum + 1];
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

            // どちらか押されると反応
            if (upTrigger || downTrigger)
            {
                // anima.SetTrigger(logoNames[nowLogoNumber]);

                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            }

            switch (nowLogoNumber)
            {
                case 0:
                    retImg.sprite = ret[1];
                    selImg.sprite = sel[0];
                    titlImg.sprite = tit[0];
                    break;
                case 1:
                    retImg.sprite = ret[0];
                    selImg.sprite = sel[1];
                    titlImg.sprite = tit[0];
                    break;
                case 2:
                    retImg.sprite = ret[0];
                    selImg.sprite = sel[0];
                    titlImg.sprite = tit[1];
                    break;
            }

            // セレクト
            if (selectKey)
            {
                // フェードアウト
                StartCoroutine(ToTitle());

                // 何も反応しないようにする
                SetFunction((uint)TRANS.None);
                Time.timeScale = 1f;
                SEManager.Instance.Play(SEPath.SE_OK);
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
            if (InputManager.InputManager.Instance.GetKeyDown(ButtunCode.Menu) && player.NowFunctionNum != (int)PlayerVer2.TRANSITION.Goal)
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
            beforPlayerTrans = player.NowFunctionNum;
            beforCamerTrans = camera.NowFunctionNum;
            camera.SetFunction((uint)Camera.TRANS.None);
            player.SetFunction((uint)PlayerVer2.TRANSITION.None);


            var dec = BGMManager.FromDecibel(-7f);

            BGMManager.Instance.ChangeBaseVolume(dec);
            Time.timeScale = 0;
        }

        public void PauseEnd()
        {
            logo.transform.parent.gameObject.SetActive(false);

            pauseFlag = false;

            SetFunction((uint)TRANS.PauseWait);
            player.SetFunction(beforPlayerTrans);
            camera.SetFunction(beforCamerTrans);
            
            var dec = BGMManager.FromDecibel(0f);

            BGMManager.Instance.ChangeBaseVolume(dec);
            Time.timeScale = 1f;
        }

        public IEnumerator ToTitle()
        {
            yield return new WaitForSeconds(0.0f);
            Debug.Log("タイトルへ戻りまーす！");

            var dec = BGMManager.FromDecibel(0f);

            BGMManager.Instance.ChangeBaseVolume(dec);

            switch (nowLogoNumber)
            {
                case 2:
                    FadeManager.FadeOut("TitleScene");
                    break;
                case 0:
                    var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    FadeManager.FadeOut(sceneName);
                    break;
                case 1:
                    FadeManager.FadeOut("StageSelectScene");
                    break;
            }
        }
    }
}