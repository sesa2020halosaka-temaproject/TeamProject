using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace TeamProject
{
    public class Goal : MonoBehaviour
    {
        private GoalLogoBeta goalLogoAnimation;

        [SerializeField]
        private GameObject rendObject;

        [SerializeField]
        private BetaGoalAnimation animObject;

        // Start is called before the first frame update
        void Start()
        {
            var canvasObject= GameObject.Find("StageCanvasBeta");

            goalLogoAnimation = canvasObject.GetComponent<GoalLogoBeta>();

            Debug.Assert(goalLogoAnimation != null,"ゴールのアニメーションがScriptに設定されていません。GoalのInstanceを確認してください");
        }
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Goal");
            var obj = other.transform.root.gameObject;
            if (obj.tag == "Player")
            {
                var platoon = obj.GetComponent<MinionPlatoon>();
                var player = obj.GetComponent<PlayerVer2>();

                var chiceList = GameObject.FindGameObjectsWithTag("ChoiceObject");

                int num = 0;
                Debug.Log(chiceList.Length);
                foreach (var itr in chiceList) { if (itr.layer == 9) num++; Debug.Log(itr.transform.root.name); }

                rendObject.SetActive(false);
                animObject.gameObject.SetActive(true);

                // ゴールのリザルトを送る
                animObject.StartGoalAnimation(goalLogoAnimation.Goal,platoon.MinionNum, num);
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