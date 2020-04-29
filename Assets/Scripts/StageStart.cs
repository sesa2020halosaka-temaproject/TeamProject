using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

public class StageStart : MonoBehaviour
{
    [Header("ステージのフェードイン時間"), Range(0, 5)]
    public float FadeIn_Time;
    [Header("BGMをセットすること")]
    public AudioClip m_Start_BGM;
    public AudioClip m_Start_Ambient;
    // Start is called before the first frame update
    void Start()
    {
        if (m_Start_BGM == null)
        {
            Debug.LogError("BGMがセットされていません！");
        }
        //フェードイン
        FadeManager.FadeIn(FadeIn_Time);

        //鳴っているSEを止める
        SEManager.Instance.Stop();
        //BGMスタート
        BGMSwitcher.CrossFade(m_Start_BGM.name);
        //水の音追加
        //BGMManager.Instance.Play(SEPath.SE_GRASS_WAVE, /*volumeRate: Volume,*/ delay: FadeIn_Time, isLoop: true, allowsDuplicate: true);
        BGMManager.Instance.Play("SE/stereo/SE_Ste_Ambient/" + m_Start_Ambient.name, /*volumeRate: Volume,*/ delay: FadeIn_Time, isLoop: true, allowsDuplicate: true);

    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}
