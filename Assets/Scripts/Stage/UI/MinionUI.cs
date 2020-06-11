using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

namespace TeamProject
{
    public class MinionUI : MonoBehaviour
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private Image image2;

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

        [SerializeField]
        private Image pauseUI;

        private Animator anime;

        private int beforMinionNum = 0;

        [SerializeField]
        private VisualEffect visualEffect;

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
                maxNomOne.SetNativeSize();
            }
            else
            {
                var numV = num;

                maxNomOne.sprite = numberSprite[num % 10];
                numV /= 10;
                maxNomOne.SetNativeSize();

                maxNomTen.sprite = numberSprite[numV % 10];
                maxNomTen.SetNativeSize();
            }

            anime = GetComponent<Animator>();
        }

        private void ImageActive(bool _flag)
        {
            image.gameObject.SetActive(_flag);
            image2.gameObject.SetActive(_flag);
            nowNomOne.gameObject.SetActive(_flag);
            nowNomTen.gameObject.SetActive(_flag);
            maxNomOne.gameObject.SetActive(_flag);
            maxNomTen.gameObject.SetActive(_flag);
            pauseUI.enabled = _flag;
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
                nowNomOne.SetNativeSize();
            }
            else
            {
                var numV = num;

                nowNomOne.sprite = numberSprite[num % 10];
                numV /= 10;
                nowNomOne.SetNativeSize();

                nowNomTen.sprite = numberSprite[numV % 10];
                nowNomTen.SetNativeSize();
            }

            if (beforMinionNum != num)
            {
                MinionCountAnimation();
            }

            beforMinionNum = num;
        }

        public void MinionCountAnimation()
        {
            anime.SetTrigger("Play");

            visualEffect.gameObject.SetActive(true);

            visualEffect.Play();
        }
    }
}