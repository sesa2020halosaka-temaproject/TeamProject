using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using KanKikuchi.AudioManager;

namespace TeamProject
{
    public class StageSceneManager : MonoBehaviour
    {
        [SerializeField]
        private KeyCode restartKey;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(restartKey))
            {
                // 現在のScene名を取得する
                Scene loadScene = SceneManager.GetActiveScene();

                FadeManager.FadeOut(loadScene.name);
            }

            // ウェーブ音
            if (!SEManager.Instance.GetCurrentAudioNames().Contains("SE_Grass_Wave")) {
                SEManager.Instance.Play(SEPath.SE_GRASS_WAVE_SUMMER);
            }
        }
    }
}
