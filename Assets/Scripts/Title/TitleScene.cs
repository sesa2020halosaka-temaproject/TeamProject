﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
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
            LOGO_ANIM,       //タイトルロゴのアニメーション中
            PANEL_FADE_IN,       //DarkPanelのフェードイン中
            PRESS_ANYBUTTON,     //キー入力待ち
            TITLE_MENU_FADEIN,//メニューフェードイン
            TITLE_MENU_WAIT,  //メニュー入力待機中
            ALL_STATE         //全状態数
        }
        public TITLESTATE state;            //現在の状態
        [Header("ラストステージまでクリアしたかどうかのフラグ")]
        public bool m_LastStageClearFlag;
        [Header("タイトル開始時のフェードイン時間")]
        public float Title_FadeIn_Time;     //タイトル開始のフェードイン時間
        [Header("PressAnyButtonオブジェクトのフェードイン時間")]
        public float m_PressAB_FadeIn_Time;      //PressAnyButtonオブジェクトのフェードイン時間
        private bool m_PressAB_FadeIn_Flag = false;      //PressAnyKeyオブジェクトのフェードイン完了フラグ
        private float m_PressAB_TimeCount;
        private float m_PressAB_Alpha;
        private Color m_PressABColor;                 //α値を取るためのカラー変数

        [Header("タイトルメニューのフェードイン時間")]
        public float Menu_FadeIn_Time;      //メニューのフェードイン時間

        [Header("PressAnyButtonのON-OFFの各切り替え時間")]
        public float Hover_Off_Time;       //PressAnyButtonのOff状態になるまでの時間
        public float Hover_On_Time;        //PressAnyButtonのOn状態になるまでの時間
        public float Hover_TimeMax;        //PressAnyButtonの切り替え時間の格納・比較用
        private float Hover_TimeCount;      //PressAnyButtonの切り替え時間のカウント用

        [Header("タイトル以外隠すためのパネルのフェードイン時間")]
        public float m_DPanel_FadeIn_Time;
        private float m_DPanel_TimeCount;
        private float m_DPanel_Alpha;

        [Header("風の音のフェードイン時間")]
        public float m_GrassWave_FadeIn_Time;

        [Header("プロローグシーンへの遷移のフェードアウト時間")]
        public float m_PR_FadeOut_Time;
        [Header("プロローグシーンへ戻るまでの操作無し時間")]
        public float m_BackPrologueMaxTime;

        public float m_TimeCount;//時間のカウント用

        public Color m_TitleMenuColor;                 //α値を取るためのカラー変数
        public GameObject m_TitleLogoObj;              //タイトルロゴ用ゲームオブジェクト
        public GameObject m_LogoImageObj;              //タイトルロゴ画像用ゲームオブジェクト
        public GameObject m_LogoMovieObj;              //タイトルロゴアニメーション用ゲームオブジェクト
        public GameObject m_TitleMenuObj;              //タイトルメニュー用ゲームオブジェクト
        public GameObject m_PressAnyButtonObj;         //PressAnyButton用ゲームオブジェクト
        private GameObject m_PressABOFFObj;         //PressAnyButtonのOFF用ゲームオブジェクト
        public GameObject m_EndingBGObj;               //ラストステージクリア後背景用ゲームオブジェクト
        public GameObject m_DarkPanelObj;              //タイトルロゴ以外隠す黒背景用ゲームオブジェクト

        public VideoPlayer m_VideoPlayer;              //アニメーション再生用
        public Color m_DarkPanelColor;                 //α値を取るためのカラー変数
        public bool m_TitleLogoAnim_Flag;
        private const int OFF = 0;                     //OFF用インデックス値
        private const int ON = 1;//ON用インデックス値


        // Start is called before the first frame update
        void Start()
        {
            //鳴っているBGMやSEを止める
            SEManager.Instance.Stop();
            BGMManager.Instance.Stop();

            //タイトルBGMスタート(フェードインはなし)
            //BGMSwitcher.FadeIn(BGMPath.BGM_TITLE,fadeInDuration:0.0f);
            BGMManager.Instance.Play(BGMPath.BGM_TITLE, volumeRate: 1.0f, delay: 0.0f, isLoop: true, allowsDuplicate: true);

            //開始時のフェードイン
            if (StageStatusManager.Instance.m_EDtoTITLE_Flag)
            {
                //エンディングシーンから遷移した時
                //-----------------------------------
                //フェードインなし
                FadeManager.FadeIn(0.0f);

                //風の音は鳴らさない

                //エンディングからタイトルに遷移しますよフラグをOFFにする。
                StageStatusManager.Instance.m_EDtoTITLE_Flag = false;
            }
            else
            {
                //エンディングシーン以外から遷移した時
                //-----------------------------------
                //フェードインあり
                FadeManager.FadeIn(Title_FadeIn_Time);

                //風の音を鳴らす、ループあり、BGMの重複許可
                BGMManager.Instance.Play(SEPath.SE_TITLE_GRASS_WAVE, volumeRate: 1.0f, delay: 0.0f, isLoop: true, allowsDuplicate: true);

                //風の音のフェードインさせる（引数未設定なら1秒かける）
                BGMManager.Instance.FadeIn(SEPath.SE_TITLE_GRASS_WAVE, duration: m_GrassWave_FadeIn_Time);
            }


            //開始時の状態を設定
            state = TITLESTATE.SCENE_FADE;

            //各ゲームオブジェクト格納
            m_TitleLogoObj = this.transform.Find("TitleLogoObj").gameObject;
            m_LogoImageObj = m_TitleLogoObj.transform.GetChild(0).gameObject;
            m_LogoMovieObj = m_TitleLogoObj.transform.GetChild(1).gameObject;

            m_TitleMenuObj = this.transform.Find("TitleMenuObj").gameObject;
            m_PressAnyButtonObj = this.transform.Find("PressAnyButton").gameObject;
            m_PressABOFFObj = m_PressAnyButtonObj.transform.GetChild(OFF).gameObject;
            m_EndingBGObj = this.transform.Find("EndingBGObj").gameObject;
            m_DarkPanelObj = GameObject.Find("DarkPanel").gameObject;

            //カラー変数受け取り
            m_TitleMenuColor = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
            SetAlphaTitleMenu(m_TitleMenuColor);

            m_TitleMenuObj.SetActive(false);

            //PressAnyButtonの準備
            SwitchingActive.GameObject_OFF(m_PressAnyButtonObj);
            //カラー変数受け取り
            m_PressABColor = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
            m_PressABOFFObj.GetComponent<Image>().color = m_PressABColor;

            Hover_TimeCount = 0.0f;
            if (Hover_TimeMax <= 0) Hover_TimeMax = 1.0f;

            //カラー値をセット
            m_DarkPanelColor = m_DarkPanelObj.GetComponent<Image>().color;

            //ラストステージをクリアしたかどうかフラグをセット
            if (m_LastStageClearFlag)
            {
                //inspector上でチェックが入っているときはこっち
            }
            else
            {
                //inspector上でチェックが入っていないときはシングルトンのフラグに従う
                m_LastStageClearFlag = StageStatusManager.Instance.m_LastStageClearFlag;
            }

            if (m_LastStageClearFlag)
            {
                //フラグがtrue（4-5クリア後）なら
                //クリア後背景をアクティブにする
                m_EndingBGObj.SetActive(true);

                //タイトルロゴ以外隠す用パネルは表示しない
                m_DarkPanelColor.a = 0.0f;

            }
            else
            {
                //フラグがfalse（4-5未クリア）なら
                //クリア後背景を非アクティブにする
                m_EndingBGObj.SetActive(false);

                //タイトルロゴ以外隠す用パネルのα値を最大にする
                m_DarkPanelColor.a = 1.0f;
            }
            m_DarkPanelObj.GetComponent<Image>().color = m_DarkPanelColor;


            //追加演出用
            //ロゴのムービーのみONにする
            SwitchingActive.GameObject_ON(m_TitleLogoObj);
            MovieSetting();

            //タイトルロゴアニメーション用SE再生
            SEManager.Instance.Play(SEPath.SE_TITLE_LOGO);

        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case TITLESTATE.SCENE_FADE:
                    SceneFadeStateUpdate();
                    break;
                case TITLESTATE.LOGO_ANIM:
                    LogoAnimationStateUpdate();
                    break;
                case TITLESTATE.PANEL_FADE_IN:
                    PanelFadeInStateUpdate();
                    break;
                case TITLESTATE.PRESS_ANYBUTTON://キー入力待ち
                    PressAnyButtonStateUpdate();

                    break;
                case TITLESTATE.TITLE_MENU_FADEIN://メニュー画面のフェードイン中
                    TitleMenuFadeInStateUpdate();
                    break;
                case TITLESTATE.TITLE_MENU_WAIT://メニュー選択画面、入力待ち
                    TitleMenuWaitStateUpdate();

                    break;
                case TITLESTATE.ALL_STATE:
                    Debug.Log("無効な状態だよ。");
                    break;
                default:
                    Debug.Log("無効な状態だよ。");
                    break;
            }
        }// void Update()   END

        //====================================================
        //状態ごとの更新関数
        //====================================================
        //シーン遷移のフェードイン中
        private void SceneFadeStateUpdate()
        {
            if (FadeManager.IsFade())
            {
                //初期の設定
                //state = TITLESTATE.PRESS_ANYKEY;

                //追加演出の設定

                //状態を変更
                state = TITLESTATE.LOGO_ANIM;
                //タイトルロゴアニメーション実行
                m_VideoPlayer.Play();

            }

        }

        //タイトルロゴのアニメーション中
        private void LogoAnimationStateUpdate()
        {
            if (!m_TitleLogoAnim_Flag)
            {
                if (m_VideoPlayer.isPlaying)
                {
                    m_TitleLogoAnim_Flag = true;
                }
                else
                {
                    Debug.Log("まだムービーは" + m_VideoPlayer.isPlaying + "だよ！");
                }

            }
            else
            {
                if (!m_VideoPlayer.isPlaying)
                {
                    state = TITLESTATE.PANEL_FADE_IN;
                }
            }

        }

        //DarkPanelのフェードイン中
        private void PanelFadeInStateUpdate()
        {
            if (m_LastStageClearFlag)
            {
                //ラストステージクリアフラグがtrueなら
                //特に処理なしで状態遷移させる
                state = TITLESTATE.PRESS_ANYBUTTON;
                //SwitchingActive.GameObject_ON(m_PressAnyButtonObj);

            }
            else
            {
                //ラストステージクリアフラグがfalseなら
                //タイトルロゴ以外隠すようパネルのα値を徐々に下げる

                m_DPanel_TimeCount += Time.deltaTime / m_DPanel_FadeIn_Time;
                m_DarkPanelColor.a = 1.0f - m_DPanel_TimeCount;
                m_DPanel_Alpha = m_DarkPanelColor.a;
                m_DarkPanelObj.GetComponent<Image>().color = m_DarkPanelColor;
                if (m_DarkPanelColor.a <= 0.0f)
                {
                    m_DarkPanelColor.a = 0.0f;
                    m_DarkPanelObj.GetComponent<Image>().color = m_DarkPanelColor;
                    state = TITLESTATE.PRESS_ANYBUTTON;
                    //SwitchingActive.GameObject_ON(m_PressAnyButtonObj);

                }
            }

        }

        //キー入力待ち
        private void PressAnyButtonStateUpdate()
        {
            if (!m_PressAB_FadeIn_Flag)
            {
                //PressAnyButtonObjがフェードイン中
                PressAnyButtonObjFadeIn();
            }
            else
            {
                //フェードイン完了
                //何かのキー入力( or コントローラーA B X Y入力)
                if (CheckAnyKey())
                {
                    m_TimeCount = 0;
                    ToTitleMenu();
                }
                else
                {
                    m_TimeCount += Time.deltaTime;
                }
                //UIの発光切り替え処理
                PressAnyButtonFlash();

                //キー操作が一定時間ないときにプロローグに戻る
                ToPrologue();
            }
        }

        //メニューフェードイン
        private void TitleMenuFadeInStateUpdate()
        {
            if (m_TitleMenuColor.a < 1.0f)
            {
                m_TitleMenuColor.a += Time.deltaTime / Menu_FadeIn_Time;

                SetAlphaTitleMenu(m_TitleMenuColor);

            }
            else
            {
                m_TitleMenuColor.a = 1.0f;
                Debug.Log("alphaは1.0fになりました。");

                CursorScript.CursorMoveFlagOn();
                state = TITLESTATE.TITLE_MENU_WAIT;

            }

        }

        //メニュー入力待機中
        private void TitleMenuWaitStateUpdate()
        {

        }
        //状態ごとの関数ここまで
        //====================================================
        //その他補助関数
        //====================================================

        //タイトルメニューのα値をセットする
        public void SetAlphaTitleMenu(Color _alpha)
        {
            m_TitleMenuObj.transform.GetChild(0).GetChild(ON).GetComponent<Image>().color = _alpha;
            m_TitleMenuObj.transform.GetChild(1).GetChild(OFF).GetComponent<Image>().color = _alpha;
            m_TitleMenuObj.transform.GetChild(2).GetChild(OFF).GetComponent<Image>().color = _alpha;
        }

        //ムービー系統の設定
        private void MovieSetting()
        {
            if (m_VideoPlayer == null)
            {
                Debug.Log("VideoPlayerがセットされていません！");
                m_VideoPlayer = m_TitleLogoObj.transform.GetChild(1).GetComponent<VideoPlayer>();
                Debug.Log("だから" + m_VideoPlayer.name + "をセットしました！");
            }
            if (m_VideoPlayer.clip == null)
            {
                Debug.LogError("VideoClipがセットされていません！");
            }

            // 即再生されるのを防ぐ.
            m_VideoPlayer.playOnAwake = false;

            //VideoRenderModeをRenderTextureに設定
            m_VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
            //VideoRenderModeをCameraNearPlaneに設定
            //m_VideoPlayer.renderMode = VideoRenderMode.CameraNearPlane;

            //ループさせない。
            m_VideoPlayer.isLooping = false;

            //再生させる
            m_VideoPlayer.Play();
            //  m_VideoPlayer.Pause();
        }

        //PressAnyButtonオブジェクトのフェードイン
        private void PressAnyButtonObjFadeIn()
        {
            //α値を徐々に上げる
            m_PressAB_TimeCount += Time.deltaTime / m_PressAB_FadeIn_Time;
            m_PressABColor.a = m_PressAB_TimeCount;
            m_PressAB_Alpha = m_PressABColor.a;

            if (m_PressABColor.a >= 1.0f)
            {
                m_PressABColor.a = 1.0f;
                m_PressAB_FadeIn_Flag = true;
                SwitchingActive.GameObject_ON(m_PressAnyButtonObj);
            }
            m_PressABOFFObj.GetComponent<Image>().color = m_PressABColor;
        }

        //AnyKeyチェック(マウスのクリック以外のキー入力)
        private bool CheckAnyKey()
        {
            bool temp_InputKey;
            temp_InputKey = (Input.anyKey && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2));
            return temp_InputKey;
        }

        //タイトルメニューへ状態遷移する処理
        private void ToTitleMenu()
        {
            //PRESS ANY KEY 画像の破棄
            Destroy(this.transform.Find("PressAnyButton").gameObject);
            //メニュー画面のアクティブ化
            m_TitleMenuObj.SetActive(true);
            //状態遷移
            state = TITLESTATE.TITLE_MENU_FADEIN;
            //キー入力SE
            SEManager.Instance.Play(SEPath.SE_OK);

        }
        //UIの発光切り替え処理
        private void PressAnyButtonFlash()
        {
            //UIの表示切替
            if (Hover_TimeCount > Hover_TimeMax)
            {
                Hover_TimeCount = 0.0f;
                if (m_PressAnyButtonObj.transform.GetChild(OFF).gameObject.activeSelf)
                {
                    //PressAnyButtonをOn状態に切り替える
                    SwitchingActive.GameObject_ON(m_PressAnyButtonObj);
                    Hover_TimeMax = Hover_On_Time;//On切り替え時間に変更
                }
                else
                {
                    //PressAnyButtonをOff状態に切り替える
                    SwitchingActive.GameObject_OFF(m_PressAnyButtonObj);
                    Hover_TimeMax = Hover_Off_Time;//Off切り替え時間に変更
                }
            }
            Hover_TimeCount += Time.deltaTime;

        }//PressAnyButtonFlash()    END

        //キー操作が一定時間ないときにプロローグに戻る
        private void ToPrologue()
        {
            if (m_TimeCount > m_BackPrologueMaxTime)
            {
                m_TimeCount = 0;
                //状態遷移
                state = TITLESTATE.TITLE_MENU_WAIT;

                //シーンのフェードアウト
                FadeManager.FadeOut("PrologueScene", time: m_PR_FadeOut_Time);
                //BGMのフェードアウト
                BGMManager.Instance.FadeOut(BGMPath.BGM_TITLE, duration: m_PR_FadeOut_Time);

            }

        }//PressAnyButtonFlash()    END

    } //public class TitleScene : MonoBehaviour    END
} //namespace TeamProject    END
