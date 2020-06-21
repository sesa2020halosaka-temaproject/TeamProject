using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class pao : MonoBehaviour
{
    private Material material;
    Image fadeImage;

    public float fadeSpeed = 0.02f;
    float progress = 0.0f;
    float edge = 0.0f;

    public bool isFadeOut = false;  //フェードアウト処理の開始、完了を管理するフラグ
    public bool isFadeIn = false;   //フェードイン処理の開始、完了を管理するフラグ
    // Start is called before the first frame update
    void Start()
    {
        //        Debug.Log(this.name);

        fadeImage = GetComponent<Image>();

        if (isFadeIn)
            progress = 0.0f;

        if (isFadeOut)
            progress = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFadeIn)
        {
            FadeIn();
        }

        if (isFadeOut)
        {
            FadeOut();
        }
    }

    void FadeIn()
    {
        fadeImage.enabled = true;
        progress += fadeSpeed;
        edge = 0.1f;
        SetProgress();
        if (progress >= 0.5f)
            edge = 0.0f;
        if (progress >= 1.0f)
        {
            isFadeIn = false;
            fadeImage.enabled = false;
        }
    }

    void FadeOut()
    {
        fadeImage.enabled = true;
        progress -= fadeSpeed;
        SetProgress();
        if (progress <= 0.5f)
            edge = 0.1f;
        if (progress <= 0.0f)
        {
            //Debug.Log("aaaaaaaaaa");

            isFadeOut = false;
            //fadeImage.enabled = false;
        }
    }

    void SetProgress()
    {
        material = this.GetComponent<Image>().material;
        material.SetFloat("_Progress", progress);
        material.SetFloat("_Edge", edge);
    }

    public void SetFadeInFlg()
    {
        isFadeIn = true;
        progress = 0.0f;
    }
    public void SetFadeOutFlg()
    {
        isFadeOut = true;
        progress = 1.0f;
    }
}
