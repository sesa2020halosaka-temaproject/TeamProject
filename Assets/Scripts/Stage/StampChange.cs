using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    public class StampChange : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] w1Stamps;

        [SerializeField]
        private Sprite[] w2Stamps;

        [SerializeField]
        private Sprite[] w3Stamps;

        [SerializeField]
        private Sprite[] w4Stamps;

        [SerializeField]
        private Image veryGood;
        [SerializeField]
        private Image good;
        [SerializeField]
        private Image bad;

        // Start is called before the first frame update
        void Start()
        {
            switch ((WORLD_NO)StageStatusManager.Instance.CurrentWorld)
            {
                case WORLD_NO.W1:
                    veryGood.sprite = w1Stamps[0];
                    good.sprite = w1Stamps[1];
                    bad.sprite = w1Stamps[2];
                    break;

                case WORLD_NO.W2:
                    veryGood.sprite = w2Stamps[0];
                    good.sprite = w2Stamps[1];
                    bad.sprite = w2Stamps[2];
                    break;

                case WORLD_NO.W3:
                    veryGood.sprite = w3Stamps[0];
                    good.sprite = w3Stamps[1];
                    bad.sprite = w3Stamps[2];
                    break;

                case WORLD_NO.W4:
                    veryGood.sprite = w4Stamps[0];
                    good.sprite = w4Stamps[1];
                    bad.sprite = w4Stamps[2];
                    break;
            }
        }
    }
}