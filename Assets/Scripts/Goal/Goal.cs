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

        [SerializeField]
        private GameObject[] minionObject;

        private Camera camera;
        [SerializeField]
        private GameObject subCameraObj;

        public GameObject SubCameraObj { get { return subCameraObj; } }

        [SerializeField]
        private GameObject laneObj;

        public GameObject LaneObj { get { return laneObj; } }

        private int minionNum, minionMaxNum;

        // Start is called before the first frame update
        void Start()
        {
            camera = UnityEngine.Camera.main.transform.root.gameObject.GetComponent<Camera>();

            var canvasObject = GameObject.Find("StageCanvasBeta");

            goalLogoAnimation = canvasObject.GetComponentInChildren<GoalLogoBeta>();

            Debug.Assert(goalLogoAnimation != null, "ゴールのアニメーションがScriptに設定されていません。GoalのInstanceを確認してください");

            // とりあえず回転をZEROに
            transform.rotation = Quaternion.identity;
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

                for (int i = 0; i < platoon.MinionNum && i < minionObject.Length; i++)
                {
                    minionObject[i].SetActive(true);
                }

                foreach (var itr in platoon.MinionList)
                {
                    itr.gameObject.SetActive(false);
                }

                player.gameObject.SetActive(false);

                 camera.gameObject.SetActive(false);

                // ゴールの情報を渡す
                // camera.SetGoalCom(this);

                camera.SetFunction((uint)Camera.TRANS.Goal);

                minionNum = platoon.MinionNum; minionMaxNum= num;

                var pPos = player.transform.position;
                var aniObPos = animObject.transform.position;
                pPos.y = aniObPos.y;

                animObject.transform.LookAt(pPos, Vector3.up);// = Quaternion.AngleAxis(, Vector3.up);
                animObject.transform.rotation *= Quaternion.AngleAxis(-90f, Vector3.up);
                GoalStart();
            }
        }
        public IEnumerator ToTitle()
        {
            yield return new WaitForSeconds(3.0f);
            Debug.Log("タイトルへ戻りまーす！");
            FadeManager.FadeOut("TitleScene");
        }

        public void GoalStart()
        {
            animObject.gameObject.SetActive(true);
            // ゴールのリザルトを送る
            animObject.StartGoalAnimation(goalLogoAnimation.Goal, minionNum, minionMaxNum);

        }
    }
}