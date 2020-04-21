using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

public class StageStart : MonoBehaviour
{
    public float FadeIn_Time;
    // Start is called before the first frame update
    void Start()
    {
        if (FadeIn_Time < 0)
        {
            FadeIn_Time = 2.0f;
        }
        //フェードイン
        FadeManager.FadeIn(FadeIn_Time);

        //鳴っているSEを止める
        SEManager.Instance.Stop();
        //BGMスタート
        BGMSwitcher.FadeOutAndFadeIn(BGMPath.BGM_GAME);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
