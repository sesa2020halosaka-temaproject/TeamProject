using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
namespace TeamProject {
    public class GoalEffect : MonoBehaviour
    {
        [SerializeField]
        private GameObject effect;

        [SerializeField]
        private BetaGoalAnimation betaGoalAnimation;

        private void OnEffectActive()
        {
            effect.SetActive(true);
        }

        private void StartBGM()
        {
            var worNum = (WORLD_NO)StageStatusManager.Instance.CurrentWorld;
            var feadDir = 1.5f;
            Debug.Log("worNum:" + worNum);
            switch (worNum)
            {
                case WORLD_NO.W1:
                    BGMManager.Instance.FadeOut(BGMPath.BGM_GAME_SUMMER, feadDir);
                    break;

                case WORLD_NO.W2:
                    BGMManager.Instance.FadeOut(BGMPath.BGM_GAME_FALL, feadDir);
                    break;

                case WORLD_NO.W3:
                    BGMManager.Instance.FadeOut(BGMPath.BGM_GAME_WINTER, feadDir);
                    break;

                case WORLD_NO.W4:
                    BGMManager.Instance.FadeOut(BGMPath.BGM_GAME_SPRING, feadDir);
                    break;
            }
            BGMManager.Instance.Play(BGMPath.BGM_GOAL, 1, 0, 1, false, true);
        }

        private void StartLogoAnimation()
        {
            betaGoalAnimation.EndGoalAnimation();
        }
        private void GoalSE()
        {
            SEManager.Instance.Play(SEPath.SE_GOAL);
        }

        private void StopAnimation()
        {
            foreach(var itr in betaGoalAnimation.Anims)
            {
                itr.Stop();
            }
        }
    }
}