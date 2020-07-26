// InputManger
// UnityInputのラッピング
// 一括管理して変更を楽にする
// UnityのMonoBehaviourを使うべきだった(Trigger処理が作れない)

using UnityEngine;
using System;
using TeamProject.System;

namespace TeamProject
{
    namespace InputManager
    {
        public enum ButtonCode:int
        {
            A,
            B,
            X,
            Y,
            R1,
            R2,
            L1,
            L2,
            Menu,
            View,
            Max,
        }

        public enum ArrowCode : int
        {
            UpArrow,
            DownArrow,
            RightArrow,
            LeftArrow,
            Max,
        }

        public enum GamePad:int
        {
            Keyboad,
            Xbox,
            PS4,
            Max,
        }
        
        class InputManager : Singleton<InputManager>
        {
            static private Tuple<string,KeyCode>[] keyString;
            static private Tuple<string, KeyCode>[] ps4KeyString;

            static private Tuple<string, KeyCode>[] activeKeyString;

            static private Tuple<string, KeyCode>[] arrowString;
            static private Tuple<string, KeyCode>[] ps4ArrowString;

            static private Tuple<string, KeyCode>[] activeArrowString;

            static private GamePad activePad;
            static public GamePad ActivePad { get { return activePad; } } 

            public InputManager()
            {
                keyString = new Tuple<string, KeyCode>[(int)ButtonCode.Max];
                ps4KeyString = new Tuple<string, KeyCode>[(int)ButtonCode.Max];

                keyString[(int)ButtonCode.A] = Tuple.Create("joystick button 0", KeyCode.Return);        // Xboxコン = Aボタン
                keyString[(int)ButtonCode.B] = Tuple.Create("joystick button 1", KeyCode.Escape);        // Xboxコン = Bボタン
                keyString[(int)ButtonCode.X] = Tuple.Create("joystick button 2", KeyCode.X);             // Xboxコン = Xボタン
                keyString[(int)ButtonCode.Y] = Tuple.Create("joystick button 3", KeyCode.V);             // Xboxコン = Yボタン
                keyString[(int)ButtonCode.R1] = Tuple.Create("joystick button 4", KeyCode.S);            // Xboxコン = RBトリガー
                keyString[(int)ButtonCode.R2] = Tuple.Create("joystick button 0", KeyCode.N);            // 
                keyString[(int)ButtonCode.L1] = Tuple.Create("joystick button 5", KeyCode.W);            // Xboxコン = LBトリガー
                keyString[(int)ButtonCode.L2] = Tuple.Create("joystick button 0", KeyCode.L);            // 
                keyString[(int)ButtonCode.Menu] = Tuple.Create("joystick button 7", KeyCode.Escape);     // Xboxコン = STARTボタン
                keyString[(int)ButtonCode.View] = Tuple.Create("joystick button 6", KeyCode.Escape);     // 現在未割り当て

                ps4KeyString[(int)ButtonCode.A] = Tuple.Create("joystick button 1", KeyCode.LeftShift);  // 
                ps4KeyString[(int)ButtonCode.B] = Tuple.Create("joystick button 2", KeyCode.Space);      // 
                ps4KeyString[(int)ButtonCode.X] = Tuple.Create("joystick button 0", KeyCode.C);          // 
                ps4KeyString[(int)ButtonCode.Y] = Tuple.Create("joystick button 3", KeyCode.V);          // 
                ps4KeyString[(int)ButtonCode.R1] = Tuple.Create("joystick button 5", KeyCode.E);         // 
                ps4KeyString[(int)ButtonCode.R2] = Tuple.Create("joystick button 0", KeyCode.N);         // 
                ps4KeyString[(int)ButtonCode.L1] = Tuple.Create("joystick button 4", KeyCode.Q);         // 
                ps4KeyString[(int)ButtonCode.L2] = Tuple.Create("joystick button 0", KeyCode.L);         // 
                ps4KeyString[(int)ButtonCode.Menu] = Tuple.Create("joystick button 9", KeyCode.P);       // 
                ps4KeyString[(int)ButtonCode.View] = Tuple.Create("joystick button 12", KeyCode.Escape); // 現在未割り当て

                arrowString = new Tuple<string, KeyCode>[(int)ArrowCode.Max];                            // 
                ps4ArrowString = new Tuple<string, KeyCode>[(int)ArrowCode.Max];                         // 

                arrowString[(int)ArrowCode.UpArrow] = Tuple.Create("ArrowY", KeyCode.W);                 // Xboxコン = 十字キー上
                arrowString[(int)ArrowCode.DownArrow] = Tuple.Create("ArrowY", KeyCode.S);               // Xboxコン = 十字キー下
                arrowString[(int)ArrowCode.RightArrow] = Tuple.Create("ArrowX", KeyCode.D);              // Xboxコン = 十字キー右
                arrowString[(int)ArrowCode.LeftArrow] = Tuple.Create("ArrowX", KeyCode.A);               // Xboxコン = 十字キー左

                ps4ArrowString[(int)ArrowCode.UpArrow] = Tuple.Create("ArrowY", KeyCode.W);              // 
                ps4ArrowString[(int)ArrowCode.DownArrow] = Tuple.Create("ArrowY", KeyCode.S);            // 
                ps4ArrowString[(int)ArrowCode.RightArrow] = Tuple.Create("ArrowX", KeyCode.D);           // 
                ps4ArrowString[(int)ArrowCode.LeftArrow] = Tuple.Create("ArrowX", KeyCode.A);            // 

                // 初期状態はXBox
                activeKeyString = keyString;
                activeArrowString = arrowString;

                activePad = GamePad.Keyboad;
            }

            void ModeChange(GamePad _gamePad)
            {
                switch (_gamePad) { 
                    case GamePad.Xbox:
                        activeKeyString = keyString;
                        break;
                    case GamePad.PS4:
                        activeKeyString = ps4KeyString;
                        break;
                }
            }

            public bool GetKeyDown(ButtonCode _buttun)
            {
                var padStringFlag = Input.GetKeyDown(activeKeyString[(int)_buttun].Item1);
                var keyStringFlag = Input.GetKeyDown(activeKeyString[(int)_buttun].Item2);

                PadJudge(keyStringFlag, padStringFlag);

                return keyStringFlag || padStringFlag;
            }

            public bool GetKey(ButtonCode _buttun)
            {
                var padStringFlag = Input.GetKey(activeKeyString[(int)_buttun].Item1);
                var keyStringFlag = Input.GetKey(activeKeyString[(int)_buttun].Item2);

                PadJudge(keyStringFlag, padStringFlag);

                return keyStringFlag || padStringFlag;
            }

            public bool GetKeyUp(ButtonCode _buttun)
            {
                var padStringFlag = Input.GetKeyUp(activeKeyString[(int)_buttun].Item1);
                var keyStringFlag = Input.GetKeyUp(activeKeyString[(int)_buttun].Item2);

                PadJudge(keyStringFlag, padStringFlag);

                return keyStringFlag || padStringFlag;
            }

            private float size = 1f;

            public bool GetArrow(ArrowCode _arrow)
            {
                bool keyFlag = false;
                bool padFlag = false;

                bool flag = false;

                switch (_arrow)
                {
                    case ArrowCode.RightArrow:
                        {
                            float x = Input.GetAxis(activeArrowString[(int)_arrow].Item1);
                            keyFlag = x >= size;
                            padFlag = Input.GetKey(activeArrowString[(int)_arrow].Item2);
                            break;
                        }
                    case ArrowCode.LeftArrow:
                        {
                            float x = Input.GetAxis(activeArrowString[(int)_arrow].Item1);
                            keyFlag = x <= -size;
                            padFlag = Input.GetKey(activeArrowString[(int)_arrow].Item2);
                            break;
                        }
                    case ArrowCode.UpArrow:
                        {
                            float y = Input.GetAxis(activeArrowString[(int)_arrow].Item1);
                            keyFlag = y <= -size;
                            padFlag = Input.GetKey(activeArrowString[(int)_arrow].Item2);
                            break;
                        }
                    case ArrowCode.DownArrow:
                        {
                            float y = Input.GetAxis(activeArrowString[(int)_arrow].Item1);
                            keyFlag = y >= size;
                            padFlag = Input.GetKey(activeArrowString[(int)_arrow].Item2);
                            break;
                        }
                }

                flag = keyFlag || padFlag;

                if (keyFlag)
                {
                    activePad = GamePad.Keyboad;
                }
                if (padFlag)
                {
                    activePad = GamePad.Xbox;
                }

                return flag;
            }
            
            public Vector2 GetLStick()
            {
                Vector2 ret;
                ret.x = Input.GetAxis("Horizontal");
                ret.y = Input.GetAxis("Vertical");

                if (!(ret.x == 0 && ret.y == 0))
                {
                    activePad = GamePad.Xbox;
                }
                return ret;
            }

            public Vector2 GetRStick()
            {
                Vector2 ret;
                ret.x = Input.GetAxis("RHorizontal");
                ret.y = Input.GetAxis("RVertical");

                return ret;
            }

            private void PadJudge(bool _key,bool _pad) 
            {
                if (_key)
                {
                    activePad = GamePad.Keyboad;
                }
                if (_pad)
                {
                    activePad = GamePad.Xbox;
                }
            }
        }
    }
}
