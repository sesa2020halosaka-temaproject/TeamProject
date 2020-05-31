using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    public class MinionUI : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        [SerializeField]
        private Image nowNomOne;
        [SerializeField]
        private Image nowNomTen;

        [SerializeField]
        private Image maxNomOne;
        [SerializeField]
        private Image maxNomTen;

        [SerializeField]
        private Sprite[] numberSprite;

        private PlayerVer2 player;
        private MinionPlatoon minionPlatoon;

        [SerializeField]
        private Pause pause;

        // Start is called before the first frame update
        void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");

            // 一番上のオブジェクトに変更
            playerObj = playerObj.transform.root.gameObject;

            // プレイヤーとミニオンン隊列のComponentを受け取る
            player = playerObj.GetComponent<PlayerVer2>();
            minionPlatoon = playerObj.GetComponent<MinionPlatoon>();

            // 配列の取得
            var objectArray = GameObject.FindGameObjectsWithTag("ChoiceObject");
            int num = 0;


            foreach (var itr in objectArray)
            {
                if (itr.transform.root.tag == "Kobito")
                {
                    num++;
                }
            }

            if (num < 10)
            {
                maxNomOne.sprite = numberSprite[num];
                maxNomTen.enabled = false;
            }
            else
            {
                var numV = num;

                maxNomOne.sprite = numberSprite[num % 10];
                numV /= 10;

                maxNomTen.sprite = numberSprite[numV % 10];
            }
        }

        private void ImageActive(bool _flag)
        {
            image.gameObject.SetActive(_flag);
            nowNomOne.gameObject.SetActive(_flag);
            nowNomTen.gameObject.SetActive(_flag);
            maxNomOne.gameObject.SetActive(_flag);
            maxNomTen.gameObject.SetActive(_flag);
        }

        private void Update()
        {
            ImageActive(!(player.NowFunctionNum == (uint)PlayerVer2.TRANSITION.Goal || pause.NowFunctionNum != (uint)Pause.TRANS.PauseWait));

            // minionPlatoon;
            var num = minionPlatoon.MinionNum;

            if (num < 10)
            {
                nowNomOne.sprite = numberSprite[num];
                nowNomTen.enabled = false;
            }
            else
            {
                var numV = num;

                nowNomOne.sprite = numberSprite[num % 10];
                numV /= 10;

                nowNomTen.sprite = numberSprite[numV % 10];
            }
        }
    }
}