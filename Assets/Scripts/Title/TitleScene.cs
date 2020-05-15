using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KanKikuchi.AudioManager;
namespace TeamProject
{
    //タイトルシーンの処理
    public class TitleScene : MonoBehaviour
    {
        //タイトル画面の状態
        public enum TITLESTATE
        {
            SCENE_FADE,       //シーン遷移のフェードイン中
            PRESS_ANYKEY,     //キー入力待ち
            TITLE_MENU_FADEIN,//メニューフェードイン
            TITLE_MENU_WAIT,  //メニュー入力待機中
            ALL_STATE         //全状態数
        }
        public TITLESTATE state;            //現在の状態
        public Color alpha;                 //α値を取るためのカラー変数
        public GameObject TitleLogoObj;     //タイトルロゴ用ゲームオブジェクト
        public GameObject TitleMenuObj;     //タイトルメニュー用ゲームオブジェクト
        public GameObject PressAnyButtonObj;//PressAnyButton用ゲームオブジェクト

        public float Title_FadeIn_Time;     //タイトル開始のフェードイン時間
        public float Menu_FadeIn_Time;      //メニューのフェードイン時間

        public float Hover_Off_Time;       //PressAnyButtonのOff状態になるまでの時間
        public float Hover_On_Time;        //PressAnyButtonのOn状態になるまでの時間
        public float Hover_TimeMax;        //PressAnyButtonの切り替え時間の比較用
        private float Hover_TimeCount;      //PressAnyButtonの切り替え時間のカウント用

        private const int OFF = 0;//OFF用インデックス値
        private const int ON = 1;//ON用インデックス値

        // Start is called before the first frame update
        void Start()
        {
            //開始時のフェードイン
            FadeManager.FadeIn(Title_FadeIn_Time);
            //鳴っているSEを止める
            SEManager.Instance.Stop();

            //タイトルBGMスタート
            BGMSwitcher.FadeIn(BGMPath.BGM_TITLE);
            state = TITLESTATE.SCENE_FADE;

            //各ゲームオブジェクト格納
            TitleLogoObj = this.transform.Find("TitleLogo").gameObject;
            TitleMenuObj = this.transform.Find("TitleMenuObj").gameObject;
            PressAnyButtonObj = this.transform.Find("PressAnyButton").gameObject;

            //カラー変数受け取り
            alpha = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
            SetAlphaTitleMenu(alpha);

            TitleMenuObj.SetActive(false);

            //PressAnyButtonの準備
            PressAnyButton_ON();

            Hover_TimeCount = 0.0f;
            if (Hover_TimeMax <= 0) Hover_TimeMax = 1.0f;
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
                    if (Hover_TimeCount > Hover_TimeMax)
                    {
                        Hover_TimeCount = 0.0f;
                        if (PressAnyButtonObj.transform.GetChild(OFF).gameObject.activeSelf)
                        {
                            //PressAnyButtonをOn状態に切り替える
                            // PressAnyButton_ON();
                            SwitchingActive.GameObject_ON(PressAnyButtonObj);
                            Hover_TimeMax = Hover_On_Time;//On切り替え時間に変更
                        }
                        else
                        {
                            //PressAnyButtonをOff状態に切り替える
                           //PressAnyButton_OFF();
                            SwitchingActive.GameObject_OFF(PressAnyButtonObj);
                            Hover_TimeMax = Hover_Off_Time;//Off切り替え時間に変更

                        }
                    }
                    Hover_TimeCount += Time.deltaTime;
                    break;
                case TITLESTATE.TITLE_MENU_FADEIN://メニュー画面のフェードイン中
                    if (alpha.a < 1.0f)
                    {
                        alpha.a += Time.deltaTime / Menu_FadeIn_Time;

                        SetAlphaTitleMenu(alpha);

                    }
                    else
                    {
                        alpha.a = 1.0f;
                        Debug.Log("alphaは1.0fになりました。");

                        CursorScript.CursorMoveFlagOn();
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
        }// void Update()   END

        //PressAnyButtonをOFF状態にする
        public void PressAnyButton_OFF()
        {
            PressAnyButtonObj.transform.GetChild(OFF).gameObject.SetActive(true);
            PressAnyButtonObj.transform.GetChild(ON).gameObject.SetActive(false);
        }
        //PressAnyButtonをON状態にする
        public void PressAnyButton_ON()
        {
            PressAnyButtonObj.transform.GetChild(OFF).gameObject.SetActive(false);
            PressAnyButtonObj.transform.GetChild(ON).gameObject.SetActive(true);
        }

        //タイトルメニューのα値をセットする
        public void SetAlphaTitleMenu(Color _alpha)
        {
            TitleMenuObj.transform.GetChild(0).GetChild(ON).GetComponent<Image>().color = _alpha;
            TitleMenuObj.transform.GetChild(1).GetChild(OFF).GetComponent<Image>().color = _alpha;
            TitleMenuObj.transform.GetChild(2).GetChild(OFF).GetComponent<Image>().color = _alpha;

        }
    } //public class TitleScene : MonoBehaviour    END
} //namespace TeamProject    END
