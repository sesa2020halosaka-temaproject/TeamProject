using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

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

    // Start is called before the first frame update
    void Start()
    {
        anima=GetComponent<Animator>();
        
        playStarAnimeNumber = 0;
        playMinionAnimeNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!once) return;
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            stageChoice--;
            if (stageChoice == StageChoice.MIN)
            {
                stageChoice = StageChoice.MAX - 1;

            }
            anima.SetTrigger("LeftKey");
            SEManager.Instance.Play(SEPath.SE_CURSOL_MOVE);
        }

        if (Input.GetKeyDown(KeyCode.D))
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
        
        if (TeamProject.InputManager.InputManager.Instance.GetKeyDown(TeamProject.InputManager.ButtunCode.B))
        {
            SEManager.Instance.Play(SEPath.SE_OK);
            // フェードアウト
            StartCoroutine(ToTitle());
        }
    }


    public void Goal(int _maxKobitoNum, int _kobitoNum)
    {
        if (once) return;
        once = true;

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
        if (starNum-1 == playStarAnimeNumber)
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
                var nextNum = (int)TeamProject. StageStatusManager.Instance.NextStage;
                var stageString = TeamProject.StageStatusManager.Instance.StageString[nextNum];
                TeamProject.StageStatusManager.Instance.CurrentStage = TeamProject.StageStatusManager.Instance.NextStage;
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
    private void SaveStar()
    {
        var curr = (int)TeamProject.StageStatusManager.Instance.CurrentStage;
        var star = TeamProject.StageStatusManager.Instance.Stage_Status[curr];

        if ((int)star < starNum)
        {
            TeamProject.StageStatusManager.Instance.Stage_Status[curr] = (TeamProject.CLEAR_STATUS)starNum;
        }
    }
}
