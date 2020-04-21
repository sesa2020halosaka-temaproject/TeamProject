using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace TeamProject
{
    public class Goal : MonoBehaviour
    {
        private GoalLogoAnimation goalLogoAnimation;

        // Start is called before the first frame update
        void Start()
        {
            var canvasObject= GameObject.Find("StageCanvas");

            goalLogoAnimation = canvasObject.GetComponent<GoalLogoAnimation>();

            Debug.Assert(goalLogoAnimation != null,"ゴールのアニメーションがScriptに設定されていません。GoalのInstanceを確認してください");
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Goal");
            var obj = other.transform.root.gameObject;
            if (obj.tag == "Player")
            {
                var player = obj.GetComponent<Player>();

                // ゴールのリザルトを送る
                goalLogoAnimation.Goal(player.MaxKobitoNum, player.KobitoNum);
            }
        }
        public IEnumerator ToTitle()
        {
            yield return new WaitForSeconds(3.0f);
            Debug.Log("タイトルへ戻りまーす！");
            FadeManager.FadeOut("TitleScene");
        }
    }
}