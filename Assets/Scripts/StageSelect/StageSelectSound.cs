using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
//using UnityEngine.SceneManagement;

namespace TeamProject
{
    //ステージセレクトのサウンドを担当します。
    public class StageSelectSound : MonoBehaviour
    {
        public float m_CrossFadeTime;//クロスフェードする時間
        public float m_SEVolume;
        public string m_Ambient_Name;//環境音の名前
        public bool m_ChangeBGM_Flag = false;//
        public float[] m_Volume = new float[(int)WORLD_NO.ALL_WORLD];//
        public float[] m_Switching = new float[(int)WORLD_NO.ALL_WORLD];//

        [SerializeField]
        private AudioClip[] AudioClip = new AudioClip[(int)WORLD_NO.ALL_WORLD];
        [SerializeField]
        private AudioSource[] AudioSource = new AudioSource[(int)WORLD_NO.ALL_WORLD];


        //=====================================================
        //関数ここから
        private void Awake()
        {
            string[] AudioName = {
                BGMPath.BGM_STAGE_SELECT_SUMMER,
                BGMPath.BGM_STAGE_SELECT_FALL,
                BGMPath.BGM_STAGE_SELECT_WINTER,
                BGMPath.BGM_STAGE_SELECT_SPRING
            };
            //BGMを取得
            for (int i = 0; i < (int)WORLD_NO.ALL_WORLD; i++)
            {
                AudioClip[i] = Resources.Load<AudioClip>(AudioName[i]);
                if (AudioClip[i] == null)
                {
                    Debug.LogError(AudioClip[i] + " not found");
                }
                AudioSource[i] = default;
                AudioSource[i] = gameObject.AddComponent<AudioSource>();
                AudioSource[i].loop = true;
                AudioSource[i].spatialBlend = 0f;

                m_Volume[i] = 0.0f;
                AudioSource[i].volume = m_Volume[i];
                AudioSource[i].clip = AudioClip[i];
            }



        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            ChangeBGMVolume();
        }

        public void SetSwitching()
        {
            //まずは全てのボリューム増減値をゼロに
            for (int i = 0; i < (int)WORLD_NO.ALL_WORLD; i++)
            {
                m_Switching[i] = 0.0f;

            }
            //指定されたワールド前後のBGMのボリュームの増減をセットする
            switch (StageChangeManager.GetStageChangeKey())
            {
                //前のワールドへ
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    m_Switching[StageStatusManager.Instance.CurrentWorld] = -1.0f;
                    m_Switching[StageStatusManager.Instance.PrevWorld] = 1.0f;
                    break;
                //次のワールドへ
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    m_Switching[StageStatusManager.Instance.CurrentWorld] = -1.0f;
                    m_Switching[StageStatusManager.Instance.NextWorld] = 1.0f;
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                default:
                    Debug.Log("状態が違います。");
                    break;
            }

        }

        //音量を変更する
        public void ChangeBGMVolume()
        {
            if (m_ChangeBGM_Flag)
            {
                float DeltaTime = Time.deltaTime;

                //まずは全てのボリューム増減値をゼロに
                for (int i = 0; i < (int)WORLD_NO.ALL_WORLD; i++)
                {
                    m_Volume[i] += m_Switching[i] * DeltaTime / m_CrossFadeTime;

                    if (m_Volume[i] >= 1.0f)
                    {
                        m_Volume[i] = 1.0f;
                        int prev = Rooping(i - 1, 0, 3);
                        int next = Rooping(i + 1, 0, 3);

                        if (m_Volume[prev] <= 0.0f || m_Volume[next] <= 0.0f)
                        {
                            m_Volume[prev] = 0.0f;
                            m_Volume[next] = 0.0f;
                            ChangeBGMFlagOFF();
                        }
                    }
                    AudioSource[i].volume = m_Volume[i];
                }



            }

        }
        //最大最小を超えた時の処理
        public int Rooping(int value, int min, int max)
        {
            if (value < min)
            {
                value = max;
            }
            else if (value > max)
            {
                value = min;
            }
            return value;
        }


        //現在のワールドから環境音を設定する
        public void SetCurrentWorldAmbient()
        {
            //環境音セット
            switch (StageStatusManager.Instance.CurrentWorld)
            {
                case (int)WORLD_NO.W1://ワールド１：夏
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_SUMMER;
                    break;
                case (int)WORLD_NO.W2://ワールド２：秋
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_FALL;
                    break;
                case (int)WORLD_NO.W3://ワールド３：冬
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_WINTER;
                    break;
                case (int)WORLD_NO.W4://ワールド４：春
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_SPRING;
                    break;
            }

        }
        //現在のワールドから次ワールドの環境音を設定する
        public void SetNextWorldAmbient()
        {
            //環境音セット
            switch (StageStatusManager.Instance.CurrentWorld)
            {
                case (int)WORLD_NO.W1://ワールド１：夏
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_FALL;//秋
                    break;
                case (int)WORLD_NO.W2://ワールド２：秋
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_WINTER;//冬
                    break;
                case (int)WORLD_NO.W3://ワールド３：冬
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_SPRING;//春
                    break;
                case (int)WORLD_NO.W4://ワールド４：春
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_SUMMER;//夏
                    break;
            }

        }
        //現在のワールドから前ワールドの環境音を設定する
        public void SetPrevWorldAmbient()
        {
            //環境音セット
            switch (StageStatusManager.Instance.CurrentWorld)
            {
                case (int)WORLD_NO.W1://ワールド１：夏
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_SPRING;//春
                    break;
                case (int)WORLD_NO.W2://ワールド２：秋
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_SUMMER;//夏
                    break;
                case (int)WORLD_NO.W3://ワールド３：冬
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_FALL;//秋
                    break;
                case (int)WORLD_NO.W4://ワールド４：春
                    m_Ambient_Name = SEPath.SE_AMB_STAGE_SELECT_WINTER;//冬
                    break;
            }

        }
        //開始時のBGM設定
        public void StageSelectStartBGM()
        {
            //現在のワールドのボリュームを1にする
            m_Volume[StageStatusManager.Instance.CurrentWorld] = 1.0f;

            for (int i = 0; i < (int)WORLD_NO.ALL_WORLD; i++)
            {
                AudioSource[i].volume = m_Volume[i];
                //BGMスタート
                AudioSource[i].Play();
            }
            //SetCurrentWorldBGM();

            //環境音追加
            //SEセット
            SetCurrentWorldAmbient();
            BGMManager.Instance.Play(m_Ambient_Name, volumeRate: 1.0f, delay: 0.0f, isLoop: true, allowsDuplicate: true);
            BGMManager.Instance.FadeIn(m_Ambient_Name, duration: 2.0f);
        }//StageSelectStartBGM() END


        //フラグON
        public void ChangeBGMFlagON()
        {
            m_ChangeBGM_Flag = true;
        }
        //フラグOFF
        public void ChangeBGMFlagOFF()
        {
            m_ChangeBGM_Flag = false;
        }

        public void CrossFade()
        {
            //
            SetSwitching();
            //
            ChangeBGMFlagON();

            //環境音のセット
            switch (StageChangeManager.GetStageChangeKey())
            {
                case StageChangeManager.STAGE_CHANGE_KEY.LEFT:
                    SetPrevWorldAmbient();
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.RIGHT:
                    SetNextWorldAmbient();
                    break;
                case StageChangeManager.STAGE_CHANGE_KEY.ALL:
                default:
                    Debug.Log("状態が違います。");
                    break;
            }
            //環境音のクロスフェード
             BGMSwitcher.CrossFade(m_Ambient_Name, fadeDuration: m_CrossFadeTime);


           //BGMSwitcher.FadeOutAndFadeIn(m_BGM_Name, fadeInDuration: 2.5f, fadeOutDuration: 2.5f);
            //BGMManager.Instance.FadeOut(m_Ambient_Name, duration: m_CrossFadeTime);
            //BGMManager.Instance.Play(m_Ambient_Name2, volumeRate: Volume, delay: 0.0f, isLoop: true, allowsDuplicate: true);
            //BGMManager.Instance.FadeIn(m_Ambient_Name2, duration: m_CrossFadeTime);
        }
    }//public class StageSelectSound : MonoBehaviour END
}//namespace END