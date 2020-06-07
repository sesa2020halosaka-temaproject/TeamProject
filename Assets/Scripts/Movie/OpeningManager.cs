using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

namespace TeamProject
{
    //オープニングシーン用クラス（MovieManager継承）
    public class OpeningManager : MovieManager
    {
        // Start is called before the first frame update
        void Start()
        {
            m_NextSceneName = "Stage1_1";
        }

        // Update is called once per frame
        void Update()
        {
            base.MoveStateUpdate();
        }
    } //public class OpeningManager : MonoBehaviour    END
} //namespace TeamProject    END
