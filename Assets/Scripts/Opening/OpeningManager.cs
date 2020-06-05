﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

namespace TeamProject
{
    //ワールド選択可能表示矢印用クラス
    public class OpeningManager : MonoBehaviour
    {
        [Header("SKIPアイコンが消えるまでの時間"), Range(0.0f, 37.0f)]
        public float m_InvisibleTime;
        private float m_TimeCount = 0;
        private bool m_InvisibleFlag = false;
        public VideoClip m_VideoClip;
        public VideoPlayer m_VideoPlayer;
        public GameObject m_SkipButtonObj;
        public GameObject m_PausePanelObj;
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

        // Start is called before the first frame update
        void Start()
        {
            m_MovieState = MOVIE_STATE.START;
            //Skipボタン用UIのオブジェクトの取得
            m_SkipButtonObj = GameObject.Find("SkipObj");
            m_SkipButtonObj.SetActive(true);

            //ポーズ用パネルオブジェクトの設定
            m_PausePanelObj = GameObject.Find("PAUSE_Panel");
            m_PausePanelObj.SetActive(false);


            //----------------------------------------------
            //ムービー系統の設定
            if (m_VideoPlayer == null)
            {
                Debug.Log("VideoPlayerがセットされていません！");
                m_VideoPlayer = this.GetComponent<VideoPlayer>();
                Debug.Log("だから" + m_VideoPlayer.name + "をセットしました！");
            }
            if (m_VideoClip == null)
            {
                Debug.LogError("VideoClipがセットされていません！");
            }

            // 即再生されるのを防ぐ.
            m_VideoPlayer.playOnAwake = false;

            //ビデオをセット
            m_VideoPlayer.clip = m_VideoClip;

            //VideoRenderModeをRenderTextureに設定
            m_VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
            //VideoRenderModeをCameraNearPlaneに設定
            //m_VideoPlayer.renderMode = VideoRenderMode.CameraNearPlane;

            //ループさせない。
            m_VideoPlayer.isLooping = false;

            //再生させる
            m_VideoPlayer.Play();


        }
        // Update is called once per frame
        void Update()
        {
            MoveStateUpdate();
            //Debug.LogError(m_VideoPlayer.isPlaying + "！");
        }//Update() END

        //SkipボタンUI非表示用カウント関数
        private void InvisibleTimeUpdate()
        {
            if (!m_InvisibleFlag)
            {
                m_TimeCount += Time.deltaTime;
                if (m_TimeCount > m_InvisibleTime)
                {
                    m_TimeCount = 0;
                    m_SkipButtonObj.SetActive(false);
                    m_InvisibleFlag = true;
                }

            }
        }

        //MoveState用更新関数
        private void MoveStateUpdate()
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
                if (InputManager.InputManager.Instance.GetKeyUp(InputManager.ButtunCode.Menu))
                {
                    //STARTボタンでSKIPする
                    m_MovieState = MOVIE_STATE.SCENE_CHANGE;


                    //m_MovieState = MOVIE_STATE.PAUSE;
                    //m_PausePanelObj.SetActive(true);
                }
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
                if ((InputManager.InputManager.Instance.GetKeyUp(InputManager.ButtunCode.Menu))
                    || (InputManager.InputManager.Instance.GetKeyUp(InputManager.ButtunCode.A)))
                {
                    //STARTボタンorAボタン
                    //ポーズから視聴中に戻る
                    m_VideoPlayer.Play();
                    m_MovieState = MOVIE_STATE.WATCH;
                    m_PausePanelObj.SetActive(false);
                }
                else if (InputManager.InputManager.Instance.GetKeyUp(InputManager.ButtunCode.B))
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
            SceneManager.LoadScene("Stage1_1");
        }

    } //public class OpeningManager : MonoBehaviour    END
} //namespace TeamProject    END
