using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    //プロローグシーン用クラス（MovieManager継承）
    public class PrologueManager : MovieManager
    {
        // Start is called before the first frame update
        void Start()
        {
            m_NextSceneName = "TitleScene";
        }

        // Update is called once per frame
        void Update()
        {
            base.MoveStateUpdate();
        }
    } //public class PrologueManager : MonoBehaviour    END
} //namespace TeamProject    END
