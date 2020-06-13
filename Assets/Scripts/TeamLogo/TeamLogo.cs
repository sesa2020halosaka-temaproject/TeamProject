using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KanKikuchi.AudioManager;


public class TeamLogo : MonoBehaviour
{
    public enum TEAMLOGO_STATE
    {
        LOGO_FADEIN = 0,  //ロゴの出現
        LOGO_KEEP,      //ロゴのキープ
        LOGO_FADEOUT,   //ロゴの消失
        SCENE_CHANGE,   //シーン遷移
        ALL_STATE       //全状態の数
    }
    public TEAMLOGO_STATE m_State;

    [Header("次のシーンに遷移するまでの時間")]
    public float NextSceneTime;//次のシーンに遷移するまでの時間
    private float m_NextSceneCount;//次のシーン遷移用カウント

    [Header("チームロゴが現れる時間"), Range(0.0f, 10.0f)]
    public float m_TeamLogoFadeIn_Time;
    [Header("チームロゴが留まる時間"), Range(0.0f, 10.0f)]
    public float m_TeamLogoKeep_Time;
    [Header("チームロゴが消える時間"), Range(0.0f, 10.0f)]
    public float m_TeamLogoFadeOut_Time;

    private float m_TeamLogoCount;//チームロゴ表示用カウント

    [Header("α値の最小値"), Range(0, 255)]
    public float m_MinAlpha;
    [Header("α値の最大値"), Range(0, 255)]
    public float m_MaxAlpha;
    [Header("α値の最大・最小の割合")]
    public float m_MaxAlphaRatio;
    public float m_MinAlphaRatio;
    //private float m_AlphaRatio;

    private Image m_TeamLogoImage;
    public Color m_TeamLogoColor;

    private void Awake()
    {
        GameObject TeamLogoObj = GameObject.Find("TeamLogo_Image");
        m_TeamLogoImage = TeamLogoObj.GetComponent<Image>();
        m_TeamLogoColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        m_TeamLogoImage.color = m_TeamLogoColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (NextSceneTime == 0)
        {
            Debug.Log("NextSceneTimeが0ですよ。");
        }
        m_State = TEAMLOGO_STATE.LOGO_FADEIN;
        float AlphaLength = 255.0f;// Mathf.Abs( m_MaxAlpha - m_MinAlpha);
        m_MinAlphaRatio = m_MinAlpha / AlphaLength;
        m_MaxAlphaRatio = m_MaxAlpha / AlphaLength;

        //BGMスタート
        // BGMManager.Instance.Play(BGMPath.BGM_TITLE);
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case TEAMLOGO_STATE.LOGO_FADEIN:
                LogoFadeInUpdate();
                break;
            case TEAMLOGO_STATE.LOGO_KEEP:
                LogoKeepUpdate();
                break;
            case TEAMLOGO_STATE.LOGO_FADEOUT:
                LogoFadeOutUpdate();
                break;
            case TEAMLOGO_STATE.SCENE_CHANGE:
                SceneChangeUpdate();
                break;
            case TEAMLOGO_STATE.ALL_STATE:
                break;
            default:
                break;
        }
    }

    //チームロゴのフェードイン用
    public void LogoFadeInUpdate()
    {
        //α値操作
        m_TeamLogoColor.a += Time.deltaTime / m_TeamLogoFadeIn_Time;
        if (m_TeamLogoColor.a >= m_MaxAlphaRatio)
        {
            m_TeamLogoColor.a = m_MaxAlphaRatio;
            m_State = TEAMLOGO_STATE.LOGO_KEEP;
        }

        m_TeamLogoImage.color = m_TeamLogoColor;

    }
     //チームロゴのキープ用
    public void LogoKeepUpdate()
    {
        //点滅表示の為のα値操作
        m_TeamLogoCount += Time.deltaTime / m_TeamLogoKeep_Time;
        if (m_TeamLogoCount >= m_TeamLogoKeep_Time)
        {
            m_TeamLogoCount = 0.0f;
            m_State = TEAMLOGO_STATE.LOGO_FADEOUT;
        }
    }
   //チームロゴのフェードアウト用
    public void LogoFadeOutUpdate()
    {
        //α値操作
        m_TeamLogoColor.a -= Time.deltaTime / m_TeamLogoFadeOut_Time;
        if (m_TeamLogoColor.a <= m_MinAlphaRatio)
        {
            m_TeamLogoColor.a = m_MinAlphaRatio;
            m_State = TEAMLOGO_STATE.SCENE_CHANGE;
        }

        m_TeamLogoImage.color = m_TeamLogoColor;

    }

    //シーン繊維用の更新処理
    public void SceneChangeUpdate()
    {
        m_NextSceneCount += Time.deltaTime / NextSceneTime;
        if (m_NextSceneCount >= NextSceneTime)
        {
            m_NextSceneCount = 0.0f;
            SceneManager.LoadScene("PrologueScene");

            //BGMフェードアウト
            //BGMManager.Instance.FadeOut();
        }

    }
}
