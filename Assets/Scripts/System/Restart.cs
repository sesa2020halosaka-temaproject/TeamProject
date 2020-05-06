using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KanKikuchi.AudioManager;

namespace TeamProject
{

    //1ボタンでリスタート(同じシーンを読み込む)する処理
    public class Restart : MonoBehaviour
    {
        public float FadeTime;//フェードアウトする時間
                              // Start is called before the first frame update
        void Start()
        {
            if (FadeTime < 0.0f)
            {
                FadeTime = 2.0f;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (InputManager.InputManager.Instance.GetKeyDown(InputManager.ButtunCode.Menu))
            //if (Input.GetKeyDown(KeyCode.Escape))
            {//ESCキー入力

                //決定音鳴らす
                SEManager.Instance.Play(SEPath.SE_OK);

                //シーン情報取得
                Scene scene = SceneManager.GetActiveScene();
                //シーン名取得
                string NowScene_Name = scene.name;
                FadeManager.FadeOut(NowScene_Name, FadeTime);
            }
        }
    }//public class Restart : MonoBehaviour END
}//namespace END