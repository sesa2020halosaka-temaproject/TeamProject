using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace TeamProject
{
    //ワールド選択可能表示矢印用クラス
    public class OpeningManager : MonoBehaviour
    {

        public VideoClip m_VideoClip;
        public VideoPlayer m_VideoPlayer;
        public GameObject m_PausePanelObj;

        public enum MOVIE_STATE
        {
            START = 0,//ムービー開始前
            WATCH ,//視聴中
            PAUSE,//ポーズ中
            SCENE_CHANGE,//シーン遷移
            ALLSTATE//
        }
        public MOVIE_STATE m_MovieState;
        // Start is called before the first frame update
        void Start()
        {
            m_MovieState = MOVIE_STATE.START;
            if (m_VideoClip == null)
            {
                Debug.LogError("VideoClipがセットされていません！");
            }
            if (m_VideoPlayer == null)
            {
                Debug.LogError("VideoPlayerがセットされていません！");
            }
            m_PausePanelObj = GameObject.Find("PAUSE_Panel");
            m_PausePanelObj.SetActive(false);
            //            m_VideoClip.
            // m_VideoPlayer.isPlaying   

            //var videoPlayer = gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
            //var audioSource = gameObject.AddComponent<AudioSource>();
            //即時再生はさせない
            m_VideoPlayer.playOnAwake = false;

            //ビデオをセット
            m_VideoPlayer.clip = m_VideoClip;

            //VideoRenderModeをCameraNearPlaneに設定
            m_VideoPlayer.renderMode = VideoRenderMode.CameraNearPlane;

            //ループさせない。
            m_VideoPlayer.isLooping = false;

            //m_VideoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
            //m_VideoPlayer.targetMaterialProperty = "_MainTex";
            //m_VideoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.AudioSource;
            //m_VideoPlayer.SetTargetAudioSource(0, audioSource);

            //再生させる
            m_VideoPlayer.Play();
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.LogError(m_VideoPlayer.isPlaying + "！");

            switch (m_MovieState)
            {
                case MOVIE_STATE.START:
                    if (m_VideoPlayer.isPlaying)
                    {
                        m_MovieState = MOVIE_STATE.WATCH;

                    }
                    else
                    {
                        Debug.Log("まだムービーは"+m_VideoPlayer.isPlaying + "だよ！");


                    }
                    break;
                case MOVIE_STATE.WATCH:
                    if (!m_VideoPlayer.isPlaying)
                    {
                        //ムービーが終わっていれば
                        //シーン遷移モードにする
                        m_MovieState = MOVIE_STATE.SCENE_CHANGE;


                        // FadeManager.FadeIn
                    }
                    else
                    {
                        if (InputManager.InputManager.Instance.GetKeyUp(InputManager.ButtunCode.Menu))
                        {
                            m_MovieState = MOVIE_STATE.PAUSE;
                            m_PausePanelObj.SetActive(true);

                        }

                    }

                    break;
                case MOVIE_STATE.PAUSE:
                    if (m_VideoPlayer.isPlaying)
                    {
                        m_VideoPlayer.Pause();
                    }
                    else
                    {
                        if( (InputManager.InputManager.Instance.GetKeyUp(InputManager.ButtunCode.Menu))|| (InputManager.InputManager.Instance.GetKeyUp(InputManager.ButtunCode.A)))
                        {

                            m_VideoPlayer.Play();
                            m_MovieState = MOVIE_STATE.WATCH;
                            m_PausePanelObj.SetActive(false);


                        }
                        else  if (InputManager.InputManager.Instance.GetKeyUp(InputManager.ButtunCode.B))
                        {
                            //ムービースキップ予定地
                            //m_VideoPlayer.Play();
                            m_PausePanelObj.SetActive(false);
                            m_MovieState = MOVIE_STATE.SCENE_CHANGE;

                        }

                    }

                    break;
                case MOVIE_STATE.SCENE_CHANGE:
                    SceneManager.LoadScene("Stage1_1");

                    break;
                case MOVIE_STATE.ALLSTATE:
                    break;
                default:
                    break;
            }

        }

    } //public class OpeningManager : MonoBehaviour    END
} //namespace TeamProject    END
