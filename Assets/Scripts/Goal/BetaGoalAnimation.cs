using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    public class BetaGoalAnimation : MonoBehaviour
    {
        public delegate void goalFunc(int _a, int _b);

        private enum Anima
        {
            Player,
            Minion,
            Flowar,
            Camera,
            Max,
        }
        
        private bool startFlag;
        private Animation[] anim = new Animation[(uint)Anima.Max];

        private int goalNum;
        private int goalNumMax;

        private goalFunc func;

        // Start is called before the first frame update
        void Start()
        {
        }

        private void Awake()
        {
            for (int i = 0; i < (int)Anima.Max; i++)
            {
                anim[i] = transform.GetChild(i).GetComponent<Animation>();
            }
            startFlag = false;
        }

        private void Update()
        {
            if (!startFlag) return;

            bool allFalse = true; 

            foreach (var itr in anim)
            {
                if (!itr.isPlaying)
                {
                    allFalse = false;
                }
            }

            if (!allFalse)
            {
                EndGoalAnimation();
            }
        }


        public void StartGoalAnimation(goalFunc _func , int _goalNum,int _goalMaxNum)
        {
            // 全てのアニメーションを再生
            foreach(var itr in anim)
            {
                itr.Play();
            }

            startFlag = true;

            goalNum = _goalNum;
            goalNumMax = _goalMaxNum;

            func = _func;
        }

        void EndGoalAnimation()
        {
            func(goalNumMax, goalNum);
        }
    }
}
