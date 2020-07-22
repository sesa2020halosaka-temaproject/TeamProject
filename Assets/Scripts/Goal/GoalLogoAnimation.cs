using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
using TeamProject.InputManager;

namespace TeamProject
{
    public class GoalLogoAnimation : MonoBehaviour
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

        [SerializeField]
        private GameObject goalCharaAnimationObject;

        private int kobitoNum;
        private int kobitoMaxNum;

        private RectTransform[] goalCharaAnimationRecTrans;
        private Animator[] goalCharaAnimationAnimator;

        [SerializeField]
        private Vector2 kobitoLogoLeftPos;
        [SerializeField]
        private Vector2 kobitoLogoRightPos;

        private bool goalFlag = false;

        private Animator animator;

        private List<Animator> starAnimator = new List<Animator>();

        private bool once = false;

        private float kobitoTime = 0.0f;

        private float starTime = 0.0f;

        private int starNum = 0;

        private bool onceFadeFlag = false;

        private bool choiceFlag = false;

        [SerializeField]
        private GameObject[] image;

        private bool[] imgaeActive = null;

        private Vector2 oldStickVel = new Vector2(0.0f, 0.0f);

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float upStickPower, downStickPower;
        
        private bool[] oldArrow = new bool[(int)ArrowCode.Max];

        // Start is called before the first frame update
        void Start()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
           
            Debug.Assert(playerObject != null, "プレイヤーのタグがありません。プレイヤーが配置されていないか、一度消してプレハブから再配置してください");
            
            // 全体のAnimator
            animator = GetComponent<Animator>();

            // 星のアニメーション
            var childAnime = transform.GetComponentsInChildren<Animator>();
            foreach (var itr in childAnime) if (itr.gameObject.GetHashCode() != gameObject.GetHashCode()) starAnimator.Add(itr);
            
            oldArrow[(int)ArrowCode.UpArrow] = false;
            oldArrow[(int)ArrowCode.DownArrow] = false;
            oldArrow[(int)ArrowCode.RightArrow] = false;
            oldArrow[(int)ArrowCode.LeftArrow] = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!goalFlag) return;

            if (KobitoAnime()) return;

            if (StarAnimation()) return;

            if (!StageChoiceAnimation()) return;

            // フェードアウト
            StartCoroutine(ToTitle());
        }

        bool KobitoAnime()
        {
            kobitoTime += Time.deltaTime;

            if (kobitoTime < kobitoNum - 0.1f)
            {
                goalCharaAnimationAnimator[(int)kobitoTime].SetBool("Flag", true);
                return true;
            }
            else
            {
                return false;
            }
        }

        bool StarAnimation()
        {
            starTime += Time.deltaTime;

            if (starTime < starNum - 0.1f)
            {
                starAnimator[(int)starTime].SetBool("Flag", true);
                return true;
            }
            else
            {
                animator.SetBool("Bad", starNum == 1);
                animator.SetBool("Good", starNum == 2);
                animator.SetBool("VeryGood", starNum == 3);
                
                animator.SetBool("StageChoice", true);
                
                return false;
            }
        }

        private bool StageChoiceAnimation()
        {
            if (choiceFlag) return false;
            if (imgaeActive == null) return false;
            for (int i = 0; i < image.Length; i++)
            {
                if (image[i] != null)
                {
                    image[i].SetActive(imgaeActive[i]);
                    image[i] = null;
                }
            }

            var StickVel = InputManager.InputManager.Instance.GetLStick();

            Debug.Log(StickVel);

            // 左押された時
            // if (Input.GetKeyDown(KeyCode.A))
            if (-upStickPower <= StickVel.x && oldStickVel.x <= -downStickPower) 
            {
                stageChoice--;
                if (stageChoice == StageChoice.MIN)
                {
                    stageChoice = StageChoice.MAX - 1;

                }
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            }

            // 右押された時
            if (StickVel.x <= upStickPower && downStickPower <= oldStickVel.x) 
            {
                stageChoice++;
                if (stageChoice == StageChoice.MAX)
                {
                    stageChoice = StageChoice.MIN + 1;
                }
                SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
            }

            oldStickVel = StickVel;

            if (InputManager.InputManager.Instance.GetKeyDown(ButtonCode.A))
            {
                SEManager.Instance.Play(SEPath.SE_OK);
                return true;
            }

            animator.SetInteger("StageChoiceNum", (int)stageChoice);

            return false;
        }

        private void FeadAnimeEnd()
        {
            goalFlag = true;
        }

        public void Goal(int _maxKobitoNum,int _kobitoNum) {
            if (once) return;
            once = false;

            kobitoNum = _kobitoNum;
            kobitoMaxNum = _maxKobitoNum;

            var length = kobitoLogoLeftPos - kobitoLogoRightPos;
            var onceLengeth = length / kobitoMaxNum;

            // キャラクターロゴのAnimatior
            goalCharaAnimationRecTrans = new RectTransform[kobitoMaxNum];
            goalCharaAnimationAnimator = new Animator[kobitoMaxNum];

            for (int i = 0; i < kobitoMaxNum; i++)
            {
                var obj = Instantiate(goalCharaAnimationObject, transform);

                goalCharaAnimationRecTrans[i] = obj.GetComponent<RectTransform>();
                goalCharaAnimationAnimator[i] = obj.GetComponent<Animator>();

                goalCharaAnimationRecTrans[i].anchoredPosition = kobitoLogoLeftPos - onceLengeth * i;
            }

            animator.SetBool("Goal", true);
            foreach (var itr in goalCharaAnimationAnimator) itr.SetBool("Goal", true);
            foreach (var itr in starAnimator) itr.SetBool("Goal", true);
            Debug.Log(_kobitoNum);

            // 星数厳選
            if (_kobitoNum <= 1) starNum = 1;
            else if (_maxKobitoNum == _kobitoNum) starNum = 3;
            else starNum = 2;
        }

        private void SetStampAlpha()
        {
            imgaeActive = new bool[image.Length];

            for (int i=0;i<image.Length;i++)
            {
                if(!image[i].activeSelf)
                {
                    Destroy(image[i]);
                }
            }
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
                    var stageString=StageStatusManager.Instance.StageString[nextNum];
                    StageStatusManager.Instance.CurrentStage = StageStatusManager.Instance.NextStage;
                    FadeManager.FadeOut(stageString);
                    break;
                case StageChoice.Retry:
                    var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    FadeManager.FadeOut(sceneName);
                    break;
                case StageChoice.Select:
                    FadeManager.FadeOut("StageSelectScene");
                    break;
            }
        }
        
    }
}
