using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    //ステージが始まる時の処理(主にBGM関連）
    public class StageStart : MonoBehaviour
    {
        [Header("ステージのフェードイン時間"), Range(0, 5)]
        public float FadeIn_Time;
        public enum AMBIENT_SOUND
        {
            LIGHT_RAIN = 0, //小雨の音
            RAIN,//雨の音
            ALL_AMBIENT,//全環境音数
            NONE,//音無し
        }
        [Header("追加環境音の切り替え")]
        public AMBIENT_SOUND m_AmbientBGM;

        [Header("Trueにすると下記でセットされたBGMが鳴ります。")]
        [Header("チェック無し時はワールドに合わせたBGMと環境音が鳴ります")]
        public bool m_DebugBGM = false;

        [Header("BGMをセットすること(確認デバッグ用)")]
        public AudioClip m_Start_BGM;
        public AudioClip m_Start_Ambient;

        private WORLD_NO _WorldNumber;

        //==============================================================
        //関数ここから
        // Start is called before the first frame update
        void Start()
        {
            if (m_Start_BGM == null)
            {
                Debug.LogError("BGMがセットされていません！");
            }
            //フェードイン
            FadeManager.FadeIn(FadeIn_Time);

            //鳴っているBGMを止める(後で修正するかも)
            BGMManager.Instance.Stop();
            //鳴っているSEを止める
            SEManager.Instance.Stop();

            //=====ステージBGMメモ=====
            //Game画面になってから（ステージセレクト曲が鳴り終わってから）
            //ゲーム曲と環境音を同時に鳴らし始める。
            //ゲーム曲も環境音もフェードインなし。
            //=====ステージBGMメモ=====

            //m_DebugBGMをONにしていたらinspector上でセットした曲が流れる
            if (m_DebugBGM)
            {
                //BGMスタート
                BGMManager.Instance.Play(m_Start_BGM.name);
                //BGMSwitcher.CrossFade(m_Start_BGM.name);

                //草のなびく音追加
                BGMManager.Instance.Play("SE/stereo/SE_Ste_Ambient/" + m_Start_Ambient.name, /*volumeRate: Volume,*/ delay: FadeIn_Time, isLoop: true, allowsDuplicate: true);
                //BGMManager.Instance.Play(SEPath.SE_GRASS_WAVE, /*volumeRate: Volume,*/ delay: FadeIn_Time, isLoop: true, allowsDuplicate: true);

            }
            else
            {

                _WorldNumber = (WORLD_NO)StageStatusManager.Instance.CurrentWorld;
                Debug.Log("今は" + _WorldNumber + "（" + StageStatusManager.Instance.CurrentWorld + "）");

                switch (_WorldNumber)
                {
                    case WORLD_NO.W1:

                        //BGMスタート
                        BGMManager.Instance.Play(BGMPath.BGM_GAME_SUMMER, allowsDuplicate: true);
                        //ステージ環境音の追加
                        BGMManager.Instance.Play(SEPath.SE_GRASS_WAVE_SUMMER, allowsDuplicate: true, isLoop: true);

                        break;
                    case WORLD_NO.W2:
                        //BGMスタート
                        BGMManager.Instance.Play(BGMPath.BGM_GAME_FALL, allowsDuplicate: true);
                        //ステージ環境音の追加
                        BGMManager.Instance.Play(SEPath.SE_GRASS_WAVE_FALL, allowsDuplicate: true, isLoop: true);
                        break;
                    case WORLD_NO.W3:
                        //BGMスタート
                        BGMManager.Instance.Play(BGMPath.BGM_GAME_WINTER, allowsDuplicate: true);
                        //ステージ環境音の追加
                        BGMManager.Instance.Play(SEPath.SE_GRASS_WAVE_WINTER, allowsDuplicate: true, isLoop: true);
                        break;
                    case WORLD_NO.W4:
                        //BGMスタート
                        BGMManager.Instance.Play(BGMPath.BGM_GAME_SPRING, allowsDuplicate: true);
                        //ステージ環境音の追加
                        BGMManager.Instance.Play(SEPath.SE_GRASS_WAVE_SPRING, allowsDuplicate: true, isLoop: true);
                        break;
                    case WORLD_NO.ALL_WORLD:
                        break;
                    default:
                        break;
                }
            }
            //環境音の切り替え
            switch (m_AmbientBGM)
            {
                case AMBIENT_SOUND.LIGHT_RAIN:
                    BGMManager.Instance.Play(SEPath.SE_LIGHT_RAIN, allowsDuplicate: true, isLoop: true);

                    break;
                case AMBIENT_SOUND.RAIN:
                    BGMManager.Instance.Play(SEPath.SE_RAIN, allowsDuplicate: true, isLoop: true);
                    break;
                case AMBIENT_SOUND.NONE:
                    break;
                case AMBIENT_SOUND.ALL_AMBIENT:
                    break;
                default:
                    break;
            }

        }//void Start() END

        // Update is called once per frame
        //void Update()
        //{

        //}
    }//public class StageStart : MonoBehaviour END
}//namespace END