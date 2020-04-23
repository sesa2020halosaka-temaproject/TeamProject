using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{

    //フェード用のCanvasとImage
    private static Canvas fadeCanvas;
    private static Image fadeImage;

    //フェード用Imageの透明度
    private static float alpha = 0.0f;

    //フェードインアウトのフラグ
    public static bool isFadeIn = false;
    public static bool isFadeOut = false;

    //フェードしたい時間（単位は秒）
    private static float fadeTime = 2.0f;
    //    private static float fadeTime = 0.2f;

    //遷移先のシーン番号
    private static int nextScene = -1;

    //遷移先のシーン名
    private static string nextScene_str = null;

    //フェード用のCanvasとImage生成
    static void Init()
    {
        //フェード用のCanvas生成
        GameObject FadeCanvasObject = new GameObject("CanvasFade");
        fadeCanvas = FadeCanvasObject.AddComponent<Canvas>();
        FadeCanvasObject.AddComponent<GraphicRaycaster>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        FadeCanvasObject.AddComponent<FadeManager>();

        //最前面になるよう適当なソートオーダー設定
        fadeCanvas.sortingOrder = 100;

        //フェード用のImage生成
        fadeImage = new GameObject("ImageFade").AddComponent<Image>();
        fadeImage.transform.SetParent(fadeCanvas.transform, false);
        fadeImage.rectTransform.anchoredPosition = Vector3.zero;

        //Imageサイズは適当に大きく設定してください
        fadeImage.rectTransform.sizeDelta = new Vector2(9999, 9999);
    }

    //フェードイン開始
    public static void FadeIn()
    {
        if (fadeImage == null) Init();
        // fadeImage.color = Color.black;
        alpha = 1.0f;
        isFadeIn = true;
    }
    //フェードイン開始(時間指定)
    public static void FadeIn(float time = 2.0f)
    {
        if (fadeImage == null) Init();
        // fadeImage.color = Color.black;
        alpha = 1.0f;
        fadeTime = time;//フェードタイムの設定
        isFadeIn = true;
    }

    //フェードアウト開始(BuildSettingの番号指定)
    public static void FadeOut(int n)
    {
        if (fadeImage == null) Init();
        nextScene = n;
        fadeImage.color = Color.clear;
        fadeCanvas.enabled = true;
        isFadeOut = true;
    }
    //フェードアウト開始(シーン名の指定)
    public static void FadeOut(string Scene_name, float time = 2.0f)
    {
        if (fadeImage == null) Init();
        nextScene_str = Scene_name;
        fadeImage.color = Color.clear;
        fadeCanvas.enabled = true;
        fadeTime = time;//フェードタイムの設定

        isFadeOut = true;
    }

    //フェード中かどうか(フェード中ならfalse)
    public static bool IsFade()
    {
        if (!isFadeIn && !isFadeOut)
        {
            return true;
        }
        return false;
    }


    //更新処理
    void Update()
    {
        //フラグ有効なら毎フレームフェードイン/アウト処理
        if (isFadeIn)
        {
            //経過時間から透明度計算
            alpha -= Time.deltaTime / fadeTime;

            //フェードイン終了判定
            if (alpha <= 0.0f)
            {
                isFadeIn = false;
                alpha = 0.0f;
                fadeCanvas.enabled = false;
            }

            //フェード用Imageの色・透明度設定
            fadeImage.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
        else if (isFadeOut)
        {
            //経過時間から透明度計算
            alpha += Time.deltaTime / fadeTime;

            //フェードアウト終了判定
            if (alpha >= 1.0f)
            {
                isFadeOut = false;
                alpha = 1.0f;

                //次のシーンへ遷移
                if (nextScene < 0)
                {
                    SceneManager.LoadScene(nextScene_str);

                }
                else
                {

                    SceneManager.LoadScene(nextScene);
                }
            }

            //フェード用Imageの色・透明度設定
            fadeImage.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
    }
}