using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KanKikuchi.AudioManager;
namespace TeamProject
{

    public class TitleScene : MonoBehaviour
    {
        //タイトル画面の状態
        public enum TITLESTATE
        {
            SCENE_FADE,//シーン遷移のフェードイン中
            PRESS_ANYKEY,//キー入力待ち
            TITLE_MENU_FADEIN,//メニューフェードイン
            TITLE_MENU_WAIT,//メニュー入力待機中
            ALL_STATE//全状態数
        }
        public TITLESTATE state;//現在の状態
        public Color alpha;//α値を取るためのカラー変数
        public GameObject TitleLogoObj;//タイトルロゴ用ゲームオブジェクト
        public GameObject TitleMenuObj;//タイトルメニュー用ゲームオブジェクト
        public GameObject PressAnyButtonObj;//タイトルメニュー用ゲームオブジェクト
        public float FadeIn_Time;//メニューのフェードイン時間
        public float Hover_Off_Time;//PressAnyButtonの切り替え時間
        public float Hover_On_Time;//PressAnyButtonの切り替え時間
        public float Hover_Time;//PressAnyButtonの切り替え時間
        private float Hover_Count;//PressAnyButtonの切り替え時間

        // Start is called before the first frame update
        void Start()
        {
            FadeManager.FadeIn(5.0f);
            state = TITLESTATE.SCENE_FADE;
            TitleLogoObj = this.transform.Find("TitleLogo").gameObject;
            TitleMenuObj = this.transform.Find("TitleMenuObj").gameObject;
            PressAnyButtonObj = this.transform.Find("PressAnyButton").gameObject;

            //カラー変数受け取り
            alpha = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
            //alpha.a = 0.0f;
            TitleMenuObj.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = new Color(alpha.r, alpha.g, alpha.b, alpha.a);
            TitleMenuObj.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(alpha.r, alpha.g, alpha.b, alpha.a);
            TitleMenuObj.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = new Color(alpha.r, alpha.g, alpha.b, alpha.a);
            // TitleMenuObj.GetComponentInChildren<Image>().color = new Color(alpha.r, alpha.g, alpha.b,alpha.a);
            TitleMenuObj.SetActive(false);

            //鳴っているSEを止める
            SEManager.Instance.Stop();

            //タイトルBGMスタート
            BGMSwitcher.FadeIn(BGMPath.BGM_TITLE);
            PressAnyButtonObj.transform.GetChild(0).gameObject.SetActive(false);
            PressAnyButtonObj.transform.GetChild(1).gameObject.SetActive(true);
            Hover_Count = 0.0f;

            if (Hover_Time <= 0)
            {
                Hover_Time = 1.0f;
            }
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case TITLESTATE.SCENE_FADE:
                    if (FadeManager.IsFade())
                    {
                        state = TITLESTATE.PRESS_ANYKEY;
                    }
                    break;
                case TITLESTATE.PRESS_ANYKEY://キー入力待ち
                                             //何かのキー入力( or コントローラーA B X Y入力)
                    if (Input.anyKey && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
                    {
                        //PRESS ANY KEY 画像の破棄
                        Destroy(this.transform.Find("PressAnyButton").gameObject);
                        //メニュー画面のアクティブ化
                        TitleMenuObj.SetActive(true);
                        //状態遷移
                        state = TITLESTATE.TITLE_MENU_FADEIN;
                        //キー入力SE
                        SEManager.Instance.Play(SEPath.SE_OK);
                    }
                    //UIの表示切替
                    if (Hover_Count > Hover_Time)
                    {
                        Hover_Count = 0.0f;
                        if (PressAnyButtonObj.transform.GetChild(0).gameObject.activeSelf)
                        {
                            //PressAnyButtonのOff状態
                            Hover_Time = Hover_On_Time;//On切り替え時間に変更
                            PressAnyButtonObj.transform.GetChild(0).gameObject.SetActive(false);
                            PressAnyButtonObj.transform.GetChild(1).gameObject.SetActive(true);
                        }
                        else
                        {
                            //PressAnyButtonのOn状態
                            Hover_Time = Hover_Off_Time;//Off切り替え時間に変更
                            PressAnyButtonObj.transform.GetChild(0).gameObject.SetActive(true);
                            PressAnyButtonObj.transform.GetChild(1).gameObject.SetActive(false);

                        }
                    }
                    Hover_Count += Time.deltaTime;
                    break;
                case TITLESTATE.TITLE_MENU_FADEIN://メニュー画面のフェードイン中
                    if (alpha.a < 1.0f)
                    {
                        alpha.a += Time.deltaTime / FadeIn_Time;
                        // TitleMenuObj.GetComponentInChildren<Image>().color = alpha;
                        TitleMenuObj.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = alpha;
                        TitleMenuObj.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = alpha;
                        TitleMenuObj.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = alpha;

                    }
                    else
                    {
                        alpha.a = 1.0f;
                        Debug.Log("alphaは1.0fになりました。");
                        //TitleMenuObj.transform.GetChild(0).transform.Find("SelectMenu").gameObject.SetActive(true);

                        CursorScript.InputFlagOn();
                        state = TITLESTATE.TITLE_MENU_WAIT;

                    }
                    break;
                case TITLESTATE.TITLE_MENU_WAIT://メニュー選択画面、入力待ち

                    break;
                case TITLESTATE.ALL_STATE:
                    Debug.Log("無効な状態だよ。");
                    break;
                default:
                    Debug.Log("無効な状態だよ。");
                    break;
            }
        }
    }
}
