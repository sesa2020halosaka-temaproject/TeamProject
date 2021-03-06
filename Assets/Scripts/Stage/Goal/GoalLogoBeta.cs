﻿using System.Collections;
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
        private Image stageNumImage;

        [SerializeField]
        private Image worldNumImage;

        // メインのアニメーション
        private Animator anima;

        // 星の数
        private int starNum = 0;

        private int playMinionAnimeNumber;
        private int playStarAnimeNumber;

        private bool[] oldArrow = new bool[(int)ArrowCode.Max];

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

        [SerializeField]
        private ParticleSystem[] starParticle = new ParticleSystem[3];

        private bool lastOnce = false;
        private bool lastStage = false;

        // Start is called before the first frame update
        void Start()
        {
            anima = GetComponent<Animator>();

            playStarAnimeNumber = 0;
            playMinionAnimeNumber = 0;

            oldArrow[(int)ArrowCode.UpArrow] = false;
            oldArrow[(int)ArrowCode.DownArrow] = false;
            oldArrow[(int)ArrowCode.RightArrow] = false;
            oldArrow[(int)ArrowCode.LeftArrow] = false;

            var worNum = StageStatusManager.Instance.CurrentWorld;
            var staNum = StageStatusManager.Instance.StageInWorld;

            worldNumImage.sprite = numberSprite[worNum + 1];
            stageNumImage.sprite = numberSprite[staNum + 1];

            // ラストステージかつ最終ゴールが終わっているか
            lastOnce = StageStatusManager.Instance.m_LastStageClearFlag;
            lastStage = StageStatusManager.Instance.CurrentStage == STAGE_NO.STAGE20;
            // ステージの番号が5番目のものなら
            // NextWorldに変更
            if (staNum == 4)
            {
                next[0] = next[2];
                next[1] = next[3];

                // 最終ステージですね
                if (StageStatusManager.Instance.CurrentStage == STAGE_NO.STAGE20)
                {

                    next[0] = next[4];
                    next[1] = next[5];
                }

                nexImg.sprite = next[0];
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!once) return;

            bool[] arrow = new bool[(int)ArrowCode.Max];

            arrow[(int)ArrowCode.UpArrow] = InputManager.InputManager.Instance.GetArrow(ArrowCode.UpArrow);
            arrow[(int)ArrowCode.DownArrow] = InputManager.InputManager.Instance.GetArrow(ArrowCode.DownArrow);
            arrow[(int)ArrowCode.RightArrow] = InputManager.InputManager.Instance.GetArrow(ArrowCode.RightArrow);
            arrow[(int)ArrowCode.LeftArrow] = InputManager.InputManager.Instance.GetArrow(ArrowCode.LeftArrow);

            var triggerRight = arrow[(int)ArrowCode.RightArrow] && !oldArrow[(int)ArrowCode.RightArrow];
            var triggerLeft = arrow[(int)ArrowCode.LeftArrow] && !oldArrow[(int)ArrowCode.LeftArrow];


            if (clear && !lastStage)
            {
                Debug.Log("通っとるよ");
                Clear(triggerRight, triggerLeft, InputManager.InputManager.Instance.GetKeyDown(ButtonCode.A));
            }
            else if (!clear && !lastStage)
            {
                Debug.Log("通っとるよ");
                NotClear(triggerRight, triggerLeft, InputManager.InputManager.Instance.GetKeyDown(ButtonCode.A));
            }
            else if (clear && lastStage)
            {
                if (!lastOnce)
                {
                    Debug.Log("通っとるよ");
                    LastClear(triggerRight, triggerLeft, InputManager.InputManager.Instance.GetKeyDown(ButtonCode.A));
                }
                else
                {
                    Debug.Log("kotti通っとるよ");
                    Clear(triggerRight, triggerLeft, InputManager.InputManager.Instance.GetKeyDown(ButtonCode.A));
                }
            }
            else
            {
                Debug.Log(clear);
                Debug.Log(lastStage);
                Debug.Log("通っとるよ");
                NotClear(triggerRight, triggerLeft, InputManager.InputManager.Instance.GetKeyDown(ButtonCode.A));
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

            oldArrow[(int)ArrowCode.UpArrow] = arrow[(int)ArrowCode.UpArrow];
            oldArrow[(int)ArrowCode.DownArrow] = arrow[(int)ArrowCode.DownArrow];
            oldArrow[(int)ArrowCode.RightArrow] = arrow[(int)ArrowCode.RightArrow];
            oldArrow[(int)ArrowCode.LeftArrow] = arrow[(int)ArrowCode.LeftArrow];
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

            // 小人の位置調節を行う部分(前準備)
            var length = kobitoLogoLeftPos - kobitoLogoRightPos;
            var onceLengeth = length / (kobitoMaxNum + 1);

            var goalLogoMinionList = new GoalLogoMinion[kobitoMaxNum];
            
            // 小人の位置調節を行う部分(背景)
            for (int i = kobitoNum; i < kobitoMaxNum; i++)
            {
                var obj = Instantiate(goalCharaAnimationObject, transform);

                goalCharaAnimationRecTrans[i] = obj.GetComponent<RectTransform>();
                goalCharaAnimationAnimator[i] = obj.GetComponent<Animator>();

                goalCharaAnimationRecTrans[i].anchoredPosition = kobitoLogoLeftPos - onceLengeth * (i + 1);

                goalLogoMinionList[i] = obj.GetComponent<GoalLogoMinion>();
            }

            // 小人の位置調節を行う部分(小人)
            for (int i = 0; i < kobitoNum; i++)
            {
                var obj = Instantiate(goalCharaAnimationObject, transform);

                goalCharaAnimationRecTrans[i] = obj.GetComponent<RectTransform>();
                goalCharaAnimationAnimator[i] = obj.GetComponent<Animator>();

                goalCharaAnimationRecTrans[i].anchoredPosition = kobitoLogoLeftPos - onceLengeth * (i + 1);

                goalLogoMinionList[i] = obj.GetComponent<GoalLogoMinion>();
            }

            for (int i = 0; i < kobitoNum; i++)
            {
                goalLogoMinionList[i].On();
            }

            for (int i = 0; i < platoon.MinionNum; i++)
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
            Debug.Log(StageStatusManager.Instance.CurrentStage);
            if (StageStatusManager.Instance.CurrentStage == STAGE_NO.STAGE20&&!lastOnce && clear)
            {
                nexImg.rectTransform.anchoredPosition = new Vector2(0, -384);

                selImg.enabled = false;
                retImg.enabled = false;
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

        public void OnStarParticle()
        {
            Debug.Log("パーティクルが出るゾ！気を付けろぉ！！" + playStarAnimeNumber);
            starParticle[playStarAnimeNumber].Play();
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
                    var nowStage = StageStatusManager.Instance.CurrentStage;
                    var nextStage = StageStatusManager.Instance.NextStage;

                    if (STAGE_NO.STAGE20 != nowStage)
                    {
                        var stageString = StageStatusManager.Instance.StageString[(int)nextStage];
                        StageStatusManager.Instance.CurrentStage = nextStage;
                        FadeManager.FadeOut(stageString);
                    }
                    else
                    {
                        //福田追記>
                        GameObject parent = GameObject.Find("Canvas");  //親検索
                        Transform oil = parent.transform.Find("Oil");   //子にアクセス
                        oil.gameObject.SetActive(true);                 //アクティブ化
                        //<ここまで
                        StageStatusManager.Instance.m_LastStageClearFlag = true;
                        //FadeManager.FadeOut("EndingScene",5.0f);
                    }
                    RootMemory.Instance.Reset();
                    break;

                case StageChoice.Retry:
                    var sceneName = SceneManager.GetActiveScene().name;
                    FadeManager.FadeOut(sceneName);
                    break;

                case StageChoice.Select:
                    FadeManager.FadeOut("StageSelectScene");
                    RootMemory.Instance.Reset();
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

        private void LastClear(bool _rightFlag, bool _leftFlag, bool _enter)
        {
            // ステージをネクストのみで
            stageChoice = StageChoice.Next;
            
            if (!enterOnce) return;

            if (_leftFlag)
            {
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            }

            if (_rightFlag)
            {
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