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

        private bool playerGoal=false;

        private PlayerVer2 player;

        private bool goalOnce = true;

        private bool goalInOnce = true;

        [SerializeField, Range(1, 5)]
        [Header("ゴールも階層分けいるらしいので追加")]
        private int floor;

        public int Floor { get { return floor; } }
        // Start is called before the first frame update
        void Start()
        {
            camera = UnityEngine.Camera.main.transform.root.gameObject.GetComponent<Camera>();

            var canvasObject = GameObject.Find("StageCanvasBeta");

            goalLogoAnimation = canvasObject.GetComponentInChildren<GoalLogoBeta>();

            Debug.Assert(goalLogoAnimation != null, "ゴールのアニメーションがScriptに設定されていません。GoalのInstanceを確認してください");

            // とりあえず回転をZEROに
            transform.rotation = Quaternion.identity;
            
            camera.SetGoalCom(this);
        }

        private void Update()
        {
            if (!playerGoal) return;

            if (camera.SeamlessEnd && goalOnce)
            {
                goalOnce = false;
                Debug.Log("個々が悪いよおおおおおおおおおおおおおおお");
                GoalStart();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!goalInOnce)
            {
                return;
            }
            Debug.Log("Goal");
            var obj = other.transform.root.gameObject;
            if (obj.tag == "Player")
            {
                playerGoal = true;
                goalInOnce = false;

                var platoon = obj.GetComponent<MinionPlatoon>();

                player = obj.GetComponent<PlayerVer2>();

                goalLogoAnimation.SetPlatoon(platoon);
                
                var chiceList = GameObject.FindGameObjectsWithTag("ChoiceObject");

                int num = 0;

                Debug.Log(chiceList.Length);

                foreach (var itr in chiceList) { if (itr.layer == 9) num++; Debug.Log(itr.transform.root.name); }

                Debug.Log("ミニオンの数" + platoon.MinionNum + "ミニオンの最大数" + num);

                for (int i = 0; i < platoon.MinionNum && i < minionObject.Length; i++)
                {
                    minionObject[i].SetActive(true);
                }

                foreach (var itr in platoon.MinionList)
                {
                    itr.gameObject.SetActive(false);
                }

                //player.gameObject.SetActive(false);

                // camera.gameObject.SetActive(false);

                // ゴールの情報を渡す
                // camera.SetGoalCom(this);

                // camera.SetFunction((uint)Camera.TRANS.Goal);

                minionNum = platoon.MinionNum; minionMaxNum= num;

                player.SetFunction((uint)PlayerVer2.TRANSITION.Goal);

                if (camera.NowFunctionNum == (uint)Camera.TRANS.Upd) GoalStart();
                //GoalStart();
            }
        }
        public void GoalIn(PlayerVer2 _player)
        {
            if (!goalInOnce)
            {
                return;
            }
            goalInOnce = false;
            playerGoal = true;
            var platoon = _player.GetComponent<MinionPlatoon>();
            player = _player;

            goalLogoAnimation.SetPlatoon(platoon);

            var chiceList = GameObject.FindGameObjectsWithTag("ChoiceObject");

            int num = 0;
            Debug.Log(chiceList.Length);
            foreach (var itr in chiceList) { if (itr.layer == 9) num++; Debug.Log(itr.transform.root.name); }
            Debug.Log("ミニオンの数" + platoon.MinionNum + "ミニオンの最大数" + num);
            for (int i = 0; i < platoon.MinionNum && i < minionObject.Length; i++)
            {
                minionObject[i].SetActive(true);
            }

            foreach (var itr in platoon.MinionList)
            {
                itr.gameObject.SetActive(false);
            }
            
            minionNum = platoon.MinionNum; minionMaxNum = num;
            player.SetFunction((uint)PlayerVer2.TRANSITION.Goal);
        }

        public IEnumerator ToTitle()
        {
            yield return new WaitForSeconds(3.0f);
            Debug.Log("タイトルへ戻りまーす！");
            FadeManager.FadeOut("TitleScene");
        }

        public void GoalStart()
        {
            rendObject.SetActive(false);

            player.gameObject.SetActive(false);
            //player.PlayerRendNot();
            animObject.gameObject.SetActive(true);
            
            // ゴールのリザルトを送る
            animObject.StartGoalAnimation(goalLogoAnimation.Goal, minionNum, minionMaxNum);

           // player.gameObject.GetComponentInChildren<Animator>().SetTrigger("Goal");
        }

        public void PlayerLook(PlayerVer2 _player)
        {
            var pPos = _player.transform.position;
            var aniObPos = animObject.transform.position;
            pPos.y = aniObPos.y;
            animObject.transform.LookAt(pPos, Vector3.up);// = Quaternion.AngleAxis(, Vector3.up);
            animObject.transform.rotation *= Quaternion.AngleAxis(-90f, Vector3.up);
        }
}
}