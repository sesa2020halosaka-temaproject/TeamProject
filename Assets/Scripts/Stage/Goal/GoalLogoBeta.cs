using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KanKikuchi.AudioManager;
using UnityEngine.SceneManagement;
using TeamProject.InputManager;

namespace TeamProject
{

    public class GoalLogoBeta : MonoBehaviour
    {
        private enum StageChoice
        {
            MIN = 0,
            Select = 1,
            Retry = 2,
            Next = 3,
            MAX,
        };

        private StageChoice stageChoice = StageChoice.Next;

        private bool once = false;

        private int kobitoNum;
        private int kobitoMaxNum;

        private bool clear;

        // ロゴアニメーションのミニオンに左右の場所
        [SerializeField]
        private Vector2 kobitoLogoLeftPos;
        [SerializeField]
        private Vector2 kobitoLogoRightPos;

        // 配列保持
        private RectTransform[] goalCharaAnimationRecTrans;
        private Animator[] goalCharaAnimationAnimator;

        // 生成するオブジェクト
        [SerializeField]
        private GameObject goalCharaAnimationObject;

        // ステージの画像データ
        [SerializeField]
        private Sprite[] numberSprite;

        [SerializeField]
        private UnityEngine.UI.Image stageNumImage;

        [SerializeField]
        private UnityEngine.UI.Image worldNumImage;

        // メインのアニメーション
        private Animator anima;

        // 星の数
        private int starNum = 0;

        private int playMinionAnimeNumber;
        private int playStarAnimeNumber;

        private bool[] oldArrow = new bool[(int)ArrowCoad.Max];

        private bool enterOnce = true;

        private MinionPlatoon platoon;

        [SerializeField]
        private Image selImg;
        [SerializeField]
        private Sprite[] select = new Sprite[2];
        [SerializeField]
        private Image retImg;
        [SerializeField]
        private Sprite[] retry = new Sprite[2];
        [SerializeField]
        private Image nexImg;
        [SerializeField]
        private Sprite[] next = new Sprite[2];
        
        // Start is called before the first frame update
        void Start()
        {
            anima = GetComponent<Animator>();

            playStarAnimeNumber = 0;
            playMinionAnimeNumber = 0;

            oldArrow[(int)ArrowCoad.UpArrow] = false;
            oldArrow[(int)ArrowCoad.DownArrow] = false;
            oldArrow[(int)ArrowCoad.RightArrow] = false;
            oldArrow[(int)ArrowCoad.LeftArrow] = false;

            var worNum = StageStatusManager.Instance.CurrentWorld;
            var staNum = StageStatusManager.Instance.StageInWorld;

            worldNumImage.sprite = numberSprite[worNum + 1];
            stageNumImage.sprite = numberSprite[staNum + 1];
        }

        // Update is called once per frame
        void Update()
        {
            if (!once) return;

            bool[] arrow = new bool[(int)ArrowCoad.Max];

            arrow[(int)ArrowCoad.UpArrow] = InputManager.InputManager.Instance.GetArrow(ArrowCoad.UpArrow);
            arrow[(int)ArrowCoad.DownArrow] = InputManager.InputManager.Instance.GetArrow(ArrowCoad.DownArrow);
            arrow[(int)ArrowCoad.RightArrow] = InputManager.InputManager.Instance.GetArrow(ArrowCoad.RightArrow);
            arrow[(int)ArrowCoad.LeftArrow] = InputManager.InputManager.Instance.GetArrow(ArrowCoad.LeftArrow);

            var triggerRight = arrow[(int)ArrowCoad.RightArrow] && !oldArrow[(int)ArrowCoad.RightArrow];
            var triggerLeft = arrow[(int)ArrowCoad.LeftArrow] && !oldArrow[(int)ArrowCoad.LeftArrow];


            if (clear)
            {
                Clear(triggerRight, triggerLeft, InputManager.InputManager.Instance.GetKeyDown(ButtunCode.B));
            }
            else
            {
                NotClear(triggerRight, triggerLeft, InputManager.InputManager.Instance.GetKeyDown(ButtunCode.B));
            }

            switch (stageChoice)
            {
                case StageChoice.Select:
                    selImg.sprite = select[1];
                    retImg.sprite = retry[0];
                    nexImg.sprite = next[0];
                    break;

                case StageChoice.Retry:
                    selImg.sprite = select[0];
                    retImg.sprite = retry[1];
                    nexImg.sprite = next[0];
                    break;

                case StageChoice.Next:
                    selImg.sprite = select[0];
                    retImg.sprite = retry[0];
                    nexImg.sprite = next[1];
                    break;
            }

            oldArrow[(int)ArrowCoad.UpArrow] = arrow[(int)ArrowCoad.UpArrow];
            oldArrow[(int)ArrowCoad.DownArrow] = arrow[(int)ArrowCoad.DownArrow];
            oldArrow[(int)ArrowCoad.RightArrow] = arrow[(int)ArrowCoad.RightArrow];
            oldArrow[(int)ArrowCoad.LeftArrow] = arrow[(int)ArrowCoad.LeftArrow];
        }


        public void Goal(int _maxKobitoNum, int _kobitoNum)
        {
            if (once) return;

            kobitoNum = _kobitoNum;
            kobitoMaxNum = _maxKobitoNum;

            // キャラクターロゴのAnimatior
            goalCharaAnimationRecTrans = new RectTransform[kobitoMaxNum];
            goalCharaAnimationAnimator = new Animator[kobitoMaxNum];

            // 星数厳選
            if (_kobitoNum <= 1) starNum = 1;
            else if (_maxKobitoNum == _kobitoNum) starNum = 3;
            else starNum = 2;

            var length = kobitoLogoLeftPos - kobitoLogoRightPos;
            var onceLengeth = length / (kobitoMaxNum - 1);
            Debug.Log(platoon.MinionList.ToArray().Length);
            Debug.Log(_maxKobitoNum);

            var goalLogoMinionList = new List<GoalLogoMinion>();

            for (int i = 0; i < kobitoMaxNum; i++)
            {
                var obj = Instantiate(goalCharaAnimationObject, transform);

                goalCharaAnimationRecTrans[i] = obj.GetComponent<RectTransform>();
                goalCharaAnimationAnimator[i] = obj.GetComponent<Animator>();

                goalCharaAnimationRecTrans[i].anchoredPosition = kobitoLogoLeftPos - onceLengeth * i;

                goalLogoMinionList.Add(obj.GetComponent<GoalLogoMinion>());
            }

            for(int i = 0; i < kobitoNum; i++)
            {
                goalLogoMinionList[i].On();
            }

            for(int i = 0; i < platoon.MinionNum; i++)
            {
                goalLogoMinionList[i].SetImage(platoon.MinionList[i].ModelNumber);
            }

            anima.SetTrigger("Darkness");

            foreach (var itr in goalCharaAnimationAnimator)
            {
                itr.SetBool("Goal", true);
            }

            anima.SetInteger("Stamp", starNum);

            // 小人の数がクリア未満です
            if (_kobitoNum <= 1)
            {
                clear = false;
                nexImg.enabled = false;

                selImg.rectTransform.anchoredPosition = new Vector2(-270, -384);
                retImg.rectTransform.anchoredPosition = new Vector2(270, -384);

                stageChoice = StageChoice.Retry;
            }
            else
            {
                clear = true;
            }

            NextMinion();
        }

        public void NextMinion()
        {
            // とりあえずリターン
            if (kobitoNum == playMinionAnimeNumber)
            {
                // 終わっていたら
                anima.SetTrigger("Minion");
                return;
            }

            Debug.Log("GoalLogoBata"+playMinionAnimeNumber);

            // 次のアニメーション再生
            goalCharaAnimationAnimator[playMinionAnimeNumber].SetTrigger("Flag");

            playMinionAnimeNumber++;
        }

        public void NextStar()
        {
            if (starNum - 1 == playStarAnimeNumber)
            {
                // アニメーション再生
                anima.SetTrigger("StarEnd");
                return;
            }

            // アニメーション再生
            anima.SetTrigger("NextStar");


            playStarAnimeNumber++;
        }

        public IEnumerator ToTitle()
        {
            yield return new WaitForSeconds(0.0f);
            Debug.Log("タイトルへ戻りまーす！");

            // スターの設定
            SaveStar();

            switch (stageChoice)
            {
                case StageChoice.Next:
                    var nextNum = (int)StageStatusManager.Instance.NextStage;
                    var stageString = StageStatusManager.Instance.StageString[nextNum];
                    StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.NextStage;
                    FadeManager.FadeOut(stageString);
                    break;
                case StageChoice.Retry:
                    var sceneName = SceneManager.GetActiveScene().name;
                    FadeManager.FadeOut(sceneName);
                    break;
                case StageChoice.Select:
                    FadeManager.FadeOut("StageSelectScene");
                    break;
            }
        }
        public IEnumerator GetKeyStart()
        {
            yield return new WaitForSeconds(0.5f);

            once = true;
        }
        private void SaveStar()
        {
            var curr = (int)StageStatusManager.Instance.CurrentStage;
            var star = StageStatusManager.Instance.Stage_Status[curr];
            var nowSaveMinionNum = StageStatusManager.Instance.Minion_Count[curr];

            if ((int)star < starNum)
            {
                StageStatusManager.Instance.Stage_Status[curr] = (CLEAR_STATUS)starNum;
            }
            if (nowSaveMinionNum < kobitoNum)
            {
                StageStatusManager.Instance.Minion_Count[curr] = kobitoNum;
            }
        }
        public void SetPlatoon(MinionPlatoon _platoon)
        {
            platoon = _platoon;
        }

        private void KeyStart()
        {
            StartCoroutine(GetKeyStart());
        }

        private void Clear(bool _rightFlag, bool _leftFlag, bool _enter)
        {
            if (!enterOnce) return;
            if (_leftFlag)
            {
                stageChoice--;
                if (stageChoice == StageChoice.MIN)
                {
                    stageChoice = StageChoice.MAX - 1;

                }
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            }

            if (_rightFlag)
            {
                stageChoice++;
                if (stageChoice == StageChoice.MAX)
                {
                    stageChoice = StageChoice.MIN + 1;
                }
                // SEMa
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            }

            if (_enter)
            {
                enterOnce = false;
                SEManager.Instance.Play(SEPath.SE_OK);
                // フェードアウト
                StartCoroutine(ToTitle());
            }
        }

        private void NotClear(bool _rightFlag, bool _leftFlag, bool _enter)
        {
            if (!enterOnce) return;
            if (_leftFlag)
            {
                stageChoice--;
                if (stageChoice == StageChoice.MIN)
                {
                    stageChoice = StageChoice.MAX - 2;

                }
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            }

            if (_rightFlag)
            {
                stageChoice++;
                if (stageChoice == StageChoice.MAX - 1)
                {
                    stageChoice = StageChoice.MIN + 1;
                }
                // SEMa
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            }

            if (_enter)
            {
                enterOnce = false;
                SEManager.Instance.Play(SEPath.SE_OK);
                // フェードアウト
                StartCoroutine(ToTitle());
            }
        }
    }
}