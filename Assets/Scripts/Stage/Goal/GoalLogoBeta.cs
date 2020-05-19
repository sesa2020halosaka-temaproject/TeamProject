using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        // メインのアニメーション
        private Animator anima;

        // 星の数
        private int starNum = 0;

        private int playMinionAnimeNumber;
        private int playStarAnimeNumber;

        private bool[] oldArrow = new bool[(int)ArrowCoad.Max];

        private bool enterOnce = true;



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

            var text = GetComponentInChildren<UnityEngine.UI.Text>();
            //text.text = EditorSceneManager.GetActiveScene().name;
            text.text = SceneManager.GetActiveScene().name;
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


            if (arrow[(int)ArrowCoad.LeftArrow] && !oldArrow[(int)ArrowCoad.LeftArrow])
            {
                stageChoice--;
                if (stageChoice == StageChoice.MIN)
                {
                    stageChoice = StageChoice.MAX - 1;

                }
                anima.SetTrigger("LeftKey");
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            }

            if (arrow[(int)ArrowCoad.RightArrow] && !oldArrow[(int)ArrowCoad.RightArrow])
            {
                stageChoice++;
                if (stageChoice == StageChoice.MAX)
                {
                    stageChoice = StageChoice.MIN + 1;
                }
                // SEMa
                anima.SetTrigger("RightKey");
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            }

            if (InputManager.InputManager.Instance.GetKeyDown(ButtunCode.B)&& enterOnce)
            {
                enterOnce = false;
                SEManager.Instance.Play(SEPath.SE_OK);
                // フェードアウト
                StartCoroutine(ToTitle());
            }
            oldArrow[(int)ArrowCoad.UpArrow] = arrow[(int)ArrowCoad.UpArrow];
            oldArrow[(int)ArrowCoad.DownArrow] = arrow[(int)ArrowCoad.DownArrow];
            oldArrow[(int)ArrowCoad.RightArrow] = arrow[(int)ArrowCoad.RightArrow];
            oldArrow[(int)ArrowCoad.LeftArrow] = arrow[(int)ArrowCoad.LeftArrow];

        }


        public void Goal(int _maxKobitoNum, int _kobitoNum)
        {
            if (once) return;

            StartCoroutine(GetKeyStart());
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
            var onceLengeth = length / kobitoMaxNum;

            for (int i = 0; i < kobitoMaxNum; i++)
            {
                var obj = Instantiate(goalCharaAnimationObject, transform);

                goalCharaAnimationRecTrans[i] = obj.GetComponent<RectTransform>();
                goalCharaAnimationAnimator[i] = obj.GetComponent<Animator>();

                goalCharaAnimationRecTrans[i].anchoredPosition = kobitoLogoLeftPos - onceLengeth * i;

                var goalLogoMinion = obj.GetComponent<GoalLogoMinion>();
                
                // goalLogoMinion.SetImage();
            }

            anima.SetTrigger("Darkness");

            foreach (var itr in goalCharaAnimationAnimator) itr.SetBool("Goal", true);

            anima.SetInteger("Stamp", starNum);

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

            Debug.Log(playMinionAnimeNumber);

            // 次のアニメーション再生
            goalCharaAnimationAnimator[playMinionAnimeNumber].SetBool("Flag", true);

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
            yield return new WaitForSeconds(2.0f);

            once = true;
        }
        private void SaveStar()
        {
            var curr = (int)StageStatusManager.Instance.CurrentStage;
            var star = StageStatusManager.Instance.Stage_Status[curr];

            if ((int)star < starNum)
            {
                StageStatusManager.Instance.Stage_Status[curr] = (CLEAR_STATUS)starNum;
            }
        }
        private void SetPlatoon(MinionPlatoon _platoon)
        {
           //  goalLogoMinion
        }

    }
}