using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject {
    public class FloorUI : MonoBehaviour
    {
        [SerializeField]
        private Image[] image;

        private Camera camera;
        
        [SerializeField]
        private Sprite lightFloor;

        [SerializeField]
        private Sprite darkFloor;
        
        [SerializeField]
        private Pause pause;

        private PlayerVer2 player;
        
        // Start is called before the first frame update
        void Start()
        {
            camera = UnityEngine.Camera.main.transform.root.gameObject.GetComponent<Camera>();

            var playerObj = GameObject.FindGameObjectWithTag("Player");

            // 一番上のオブジェクトに変更
            playerObj = playerObj.transform.root.gameObject;

            // プレイヤーとミニオンン隊列のComponentを受け取る
            player = playerObj.GetComponent<PlayerVer2>();

            // 高さが1の時は全て消す
            if (camera.Hight == 1)
            {
                gameObject.SetActive(false);
            }

            foreach(var itr in image)
            {
                itr.enabled = false;
            }

            for(int i = 0; i < camera.Hight; i++)
            {
                image[i].enabled = true;
            }
        }

        void ImageActive(bool _flag)
        {
            List<GameObject> list = new List<GameObject>();
            int num = transform.childCount;
            
            for(int i = 0; i < num; i++)
            {
                list.Add(transform.GetChild(i).gameObject);
            }
            
            foreach(var itr in list)
            {
                itr.SetActive(_flag);
            }
        }

        // Update is called once per frame
        void Update()
        {
            ImageActive(!(player.NowFunctionNum == (uint)PlayerVer2.TRANSITION.Goal || pause.NowFunctionNum != (uint)Pause.TRANS.PauseWait));

            for (int i = 0; i < camera.Hight; i++)
            {
                if (i+1 == camera.NowHight)
                {
                    image[i].sprite = lightFloor;
                }
                else
                {
                    image[i].sprite = darkFloor;
                }
            }
        }
    }
}