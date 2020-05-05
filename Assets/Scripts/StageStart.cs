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
        [Header("BGMをセットすること")]
        public AudioClip m_Start_BGM;
        public AudioClip m_Start_Ambient;

        private enum WORLD
        {
            W1 = 0, W2, W3, W4, ALL_WORLD
        }
        private WORLD _WorldNumber;

        [Header("Trueにすると上記でセットされたBGMが鳴ります。")]
        public bool m_DebugBGM = false;
        // Start is called before the first frame update
        void Start()
        {
            if (m_Start_BGM == null)
            {
                Debug.LogError("BGMがセットされていません！");
            }
            //フェードイン
            FadeManager.FadeIn(FadeIn_Time);

            //鳴っているSEを止める
            SEManager.Instance.Stop();

            //=====ステージBGMメモ=====
            //Game画面になってから（ステージセレクト曲が鳴り終わってから）
            //ゲーム曲と環境音を同時に鳴らし始める。
            //ゲーム曲も環境音もフェードインなし。
            if (m_DebugBGM)
            {
                //BGMスタート
                BGMManager.Instance.Play(m_Start_BGM.name);
                //BGMSwitcher.CrossFade(m_Start_BGM.name);

                //水の音追加
                BGMManager.Instance.Play("SE/stereo/SE_Ste_Ambient/" + m_Start_Ambient.name, /*volumeRate: Volume,*/ delay: FadeIn_Time, isLoop: true, allowsDuplicate: true);
                //BGMManager.Instance.Play(SEPath.SE_GRASS_WAVE, /*volumeRate: Volume,*/ delay: FadeIn_Time, isLoop: true, allowsDuplicate: true);

            }
            else
            {

                _WorldNumber = (WORLD)StageStatusManager.Instance.CurrentWorld;
                Debug.Log("今は" + _WorldNumber + "（" + StageStatusManager.Instance.CurrentWorld + "）");

                switch (_WorldNumber)
                {
                    case WORLD.W1:

                        //BGMスタート
                        BGMManager.Instance.Play(BGMPath.BGM_GAME_SUMMER, allowsDuplicate: true);
                        //水の音追加
                        BGMManager.Instance.Play(SEPath.SE_GRASS_WAVE_SUMMER, allowsDuplicate: true);

                        break;
                    case WORLD.W2:
                        //BGMスタート
                        BGMManager.Instance.Play(BGMPath.BGM_GAME_FALL, allowsDuplicate: true);
                        //水の音追加   
                        BGMManager.Instance.Play(SEPath.SE_GRASS_WAVE_FALL, allowsDuplicate: true);
                        break;
                    case WORLD.W3:
                        Debug.Log("まだ" + _WorldNumber + "の曲は未実装だよ！");
                        //BGMスタート
                        BGMManager.Instance.Play(BGMPath.BGM_GAME_SUMMER, allowsDuplicate: true);
                        //水の音追加
                        BGMManager.Instance.Play(SEPath.SE_GRASS_WAVE_SUMMER, allowsDuplicate: true);
                        break;
                    case WORLD.W4:
                        Debug.Log("まだ" + _WorldNumber + "の曲は未実装だよ！");
                        //BGMスタート
                        BGMManager.Instance.Play(BGMPath.BGM_GAME_SUMMER, allowsDuplicate: true);
                        //水の音追加
                        BGMManager.Instance.Play(SEPath.SE_GRASS_WAVE_SUMMER, allowsDuplicate: true);
                        break;
                    case WORLD.ALL_WORLD:
                        break;
                    default:
                        break;
                }
            }


        }

        // Update is called once per frame
        //void Update()
        //{

        //}
    }//public class StageStart : MonoBehaviour END
}//namespace END