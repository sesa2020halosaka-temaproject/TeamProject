using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KanKikuchi.AudioManager;


public class TeamLogo : MonoBehaviour
{
    public float NextSceneTime;//次のシーンに遷移するまでの時間

    // Start is called before the first frame update
    void Start()
    {
        if (NextSceneTime==0)
        {
            NextSceneTime = 3.0f;
        }
        //BGMスタート
        BGMManager.Instance.Play(BGMPath.BGM_GAME);
    }

    // Update is called once per frame
    void Update()
    {
        NextSceneTime -= Time.deltaTime;
        if (NextSceneTime <= 0)
        {
            NextSceneTime = 0;
            SceneManager.LoadScene("TitleScene");

            //BGMフェードアウト
            BGMManager.Instance.FadeOut();
        }
    }
}
