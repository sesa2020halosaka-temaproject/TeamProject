﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    public class GoalLogoSound : MonoBehaviour
    {
        private GoalLogoBeta goalLogoBeta;

        private void Start()
        {
            goalLogoBeta = transform.GetComponentInParent<GoalLogoBeta>();
        }

        private void GoodLogoSound()
        {
            SEManager.Instance.Play(SEPath.SE_STAMP);
        }

        private void BadLogoSound()
        {
            SEManager.Instance.Play(SEPath.SE_STAMP);
        }

        private void VeryGoodSound()
        {
            SEManager.Instance.Play(SEPath.SE_STAMP);
        }

        private void StarLogoSound()
        {
            SEManager.Instance.Play(SEPath.SE_STAR);
        }

        private void KoibtoLogoSound()
        {
            SEManager.Instance.Play(SEPath.SE_MINION_COUNT);
        }

        private void SEStageClear()
        {
            SEManager.Instance.Play(SEPath.SE_CLEAR);
        }

        private void SEStageFailure()
        {
            SEManager.Instance.Play(SEPath.SE_FAILURE);
        }

        private void ResultBgm()
        {
            BGMManager.Instance.Play(BGMPath.BGM_GAME_CLEAR, 1, 0, 1, true, true);
        }

        private void NextMinion()
        {
            goalLogoBeta.NextMinion(); 
        }
    }
}
