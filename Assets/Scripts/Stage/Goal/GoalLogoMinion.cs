using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    public class GoalLogoMinion : MonoBehaviour
    {
        [SerializeField]
        Sprite[] minionLogoImage;

        [SerializeField]
        Image imageBack;

        [SerializeField]
        Image imageCom;

        private void Start()
        {
            // コンポーネント取得
            // imageCom = GetComponent<Image>();
        }

        public void SetImage(Minion.MINION_TYPE _type)
        {
            Debug.Log(imageCom);
            Debug.Log(_type);
            // 画像変更
            imageCom.sprite
                = minionLogoImage[(uint)_type];
        }

        public void On()
        {
            imageBack.gameObject.SetActive(false);
            imageCom.gameObject.SetActive(true);
        }
    }
}