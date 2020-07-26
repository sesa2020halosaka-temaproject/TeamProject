using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamProject
{
    public class ChangeUI : MonoBehaviour
    {
        [SerializeField]
        private Image[] images;

        [SerializeField]
        private Sprite[] keySprite;
        [SerializeField]
        private Sprite[] padSprite;
        
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            // 現在有効なパッドの種類を取得
            var activePad = InputManager.InputManager.ActivePad;

            if (activePad == InputManager.GamePad.Keyboad)
            {
                SetImage(keySprite);
            }
            else
            {
                SetImage(padSprite);
            }
        }

        private void SetImage(Sprite[] _sprite)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].sprite = _sprite[i];
            }
        }
    }
}