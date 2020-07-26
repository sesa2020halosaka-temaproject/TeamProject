using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

namespace TeamProject
{
    //ムービーのみを再生するシーン用基底クラス
    public class MovieManager : MonoBehaviour
    {
        private float m_TimeCount = 0;
        private bool m_InvisibleBeginFlag = false;
        private bool m_InvisibleEndFlag = false;

        [Header("次に遷移するシーン名")]
        public string m_NextSceneName;
        [Header("SKIPアイコンが消え始めるまでの時間"), Range(0.0f, 37.0f)]
        public float m_InvisibleBeginTime;
        [Header("SKIPアイコンが消えきるまでの時間"), Range(0.0f, 10.0f)]
        public float m_InvisibleEndTime;
        private float m_AlphaMaxRatio;

        public GameObject m_SkipButtonObj;
        public Image m_SkipButtonUI;
        public Color m_SkipColor;
        private int m_UI_Index;//現在のオブジェクトがどちらかを判定する（０：Xbox用、１：キーボード用）

        public VideoClip m_VideoClip;
        public VideoPlayer m_VideoPlayer;
        //public GameObject m_PausePanelObj;
        // private RawImage rawImage;

        public enum MOVIE_STATE
        {
            START = 0,//ムービー開始前
            WATCH,//視聴中
            PAUSE,//ポーズ中
            SCENE_CHANGE,//シーン遷移
            ALLSTATE//
        }
        public MOVIE_STATE m_MovieState;
        private void Awake()
        {  
            //Skipボタン用UIのオブジェクトの取得
            m_SkipButtonObj = GameObject.Find("SkipObj");
            if (m_InvisibleEndTime <= 0)
            {

                m_SkipButtonObj.SetActive(false);
            }
            else
            {
                m_SkipButtonObj.SetActive(true);

            }
            SwitchingUISprite();
            m_SkipButtonUI = m_SkipButtonObj.transform.GetChild(m_UI_Index).GetComponent<Image>();
            m_SkipColor = m_SkipButtonUI.color;
            m_AlphaMaxRatio = m_SkipColor.a;
            //----------------------------------------------
            //ムービー系統の設定

            MovieSetting();
        }
        // Start is called before the first frame update
        void Start()
        {
            m_MovieState = MOVIE_STATE.START;
            //廃止されました
            //ポーズ用パネルオブジェクトの設定
            //m_PausePanelObj = GameObject.Find("PAUSE_Panel");
            //m_PausePanelObj.SetActive(false);
        }
        //ムービー系統の設定
        private void MovieSetting()
        {
            if (m_VideoPlayer == null)
            {
                Debug.Log("VideoPlayerがセットされていません！");
                m_VideoPlayer = this.GetComponent<VideoPlayer>();
                Debug.Log("だから" + m_VideoPlayer.name + "をセットしました！");
            }
            if (m_VideoPlayer.clip == null)
            {
                Debug.LogError("VideoClipがセットされていません！");
            }

            // 即再生されるのを防ぐ.
            m_VideoPlayer.playOnAwake = false;

            //ビデオをセット
            m_VideoClip = m_VideoPlayer.clip;

            //VideoRenderModeをRenderTextureに設定
            m_VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
            //VideoRenderModeをCameraNearPlaneに設定
            //m_VideoPlayer.renderMode = VideoRenderMode.CameraNearPlane;

            //ループさせない。
            m_VideoPlayer.isLooping = false;

            //再生させる
            m_VideoPlayer.Play();


        }

        //SkipボタンUI非表示用カウント関数
        private void InvisibleTimeUpdate()
        {
            if (!m_InvisibleEndFlag)
            {

                if (!m_InvisibleBeginFlag)
                {
                    m_TimeCount += Time.deltaTime;
                    if (m_TimeCount > m_InvisibleBeginTime)
                    {
                        m_TimeCount = 0;
                        // m_SkipButtonObj.SetActive(false);
                        m_InvisibleBeginFlag = true;
                    }

                }
                else
                {
                    m_SkipColor.a -= Time.deltaTime * m_AlphaMaxRatio / m_InvisibleEndTime;
                    if (m_SkipColor.a < 0.0f)
                    {
                        m_TimeCount = 0;
                        m_SkipButtonObj.SetActive(false);
                        m_InvisibleEndFlag = true;
                    }

                    m_SkipButtonUI.color = m_SkipColor;
                }
            }
        }

        //MovieState用更新関数
        public void MovieStateUpdate()
        {
            switch (m_MovieState)
            {
                case MOVIE_STATE.START:
                    StateStartUpdate();
                    break;
                case MOVIE_STATE.WATCH:
                    StateWatchUpdate();

                    break;
                case MOVIE_STATE.PAUSE:
                    StatePauseUpdate();
                    break;
                case MOVIE_STATE.SCENE_CHANGE:
                    StateSceneChangeUpdate();
                    break;
                case MOVIE_STATE.ALLSTATE:
                    break;
                default:
                    break;
            }
        }

        //スタート時の更新処理
        private void StateStartUpdate()
        {
            if (m_VideoPlayer.isPlaying) { m_MovieState = MOVIE_STATE.WATCH; }
            else { Debug.Log("まだムービーは" + m_VideoPlayer.isPlaying + "だよ！"); }
        }

        private void StateWatchUpdate()
        {
            InvisibleTimeUpdate();

            if (!m_VideoPlayer.isPlaying)
            {
                //ムービーが終わっていれば
                //シーン遷移モードにする
                m_MovieState = MOVIE_STATE.SCENE_CHANGE;
            }
            else
            {
                //if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtonCode.Menu)||

                if (Input.GetKeyDown("joystick button 7") ||        //STARTボタン個別取得
                    Input.GetKeyDown(KeyCode.X))
                {
                    //STARTボタン or Xキー入力でSKIPする
                    m_MovieState = MOVIE_STATE.SCENE_CHANGE;


                    //m_MovieState = MOVIE_STATE.PAUSE;
                    //m_PausePanelObj.SetActive(true);
                }
            }
        }

        //入力デバイスに応じたUIへの表示切り替え
        public void SwitchingUISprite()
        {
            switch (InputManager.InputManager.ActivePad)
            {
                case InputManager.GamePad.Keyboad:
                    Debug.Log("今はKeyboardモード");
                    SwitchingActive.GameObject_ON(m_SkipButtonObj);
                    m_UI_Index = 1;
                    break;
                case InputManager.GamePad.Xbox:
                    Debug.Log("今はXboxモード");
                    SwitchingActive.GameObject_OFF(m_SkipButtonObj);
                    m_UI_Index = 0;
                    break;
            }
        }

        //廃止されました
        //ポーズ時の更新処理
        private void StatePauseUpdate()
        {
            if (m_VideoPlayer.isPlaying)
            {
                m_VideoPlayer.Pause();
            }
            else
            {
                if ((InputManager.InputManager.Instance.GetKeyUp(InputManager.ButtonCode.Menu))
                    || (InputManager.InputManager.Instance.GetKeyUp(InputManager.ButtonCode.A)))
                {
                    //STARTボタンorAボタン
                    //ポーズから視聴中に戻る
                    m_VideoPlayer.Play();
                    m_MovieState = MOVIE_STATE.WATCH;
                    //m_PausePanelObj.SetActive(false);
                }
                else if (InputManager.InputManager.Instance.GetKeyUp(InputManager.ButtonCode.B))
                {
                    //Bボタン
                    //ムービースキップ
                    m_MovieState = MOVIE_STATE.SCENE_CHANGE;
                }
            }
        }//StatePauseUpdate()   END

        //シーン遷移時の更新処理
        private void StateSceneChangeUpdate()
        {
            SceneManager.LoadScene(m_NextSceneName);
        }

    } //public class MovieManager : MonoBehaviour    END
} //namespace TeamProject    END

