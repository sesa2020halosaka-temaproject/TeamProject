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
        public enum ButtunCode:int
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

            public InputManager()
            {
                keyString = new Tuple<string, KeyCode>[(int)ButtunCode.Max];
                ps4KeyString = new Tuple<string, KeyCode>[(int)ButtunCode.Max];

                keyString[(int)ButtunCode.A] = Tuple.Create("joystick button 0", KeyCode.Return);        // Xboxコン = Aボタン
                keyString[(int)ButtunCode.B] = Tuple.Create("joystick button 1", KeyCode.Escape);        // Xboxコン = Bボタン
                keyString[(int)ButtunCode.X] = Tuple.Create("joystick button 2", KeyCode.X);             // Xboxコン = Xボタン
                keyString[(int)ButtunCode.Y] = Tuple.Create("joystick button 3", KeyCode.V);             // Xboxコン = Yボタン
                keyString[(int)ButtunCode.R1] = Tuple.Create("joystick button 4", KeyCode.S);            // Xboxコン = RBトリガー
                keyString[(int)ButtunCode.R2] = Tuple.Create("joystick button 0", KeyCode.N);            // 
                keyString[(int)ButtunCode.L1] = Tuple.Create("joystick button 5", KeyCode.W);            // Xboxコン = LBトリガー
                keyString[(int)ButtunCode.L2] = Tuple.Create("joystick button 0", KeyCode.L);            // 
                keyString[(int)ButtunCode.Menu] = Tuple.Create("joystick button 7", KeyCode.Escape);     // Xboxコン = STARTボタン
                keyString[(int)ButtunCode.View] = Tuple.Create("joystick button 6", KeyCode.Escape);     // 現在未割り当て

                ps4KeyString[(int)ButtunCode.A] = Tuple.Create("joystick button 1", KeyCode.LeftShift);  // 
                ps4KeyString[(int)ButtunCode.B] = Tuple.Create("joystick button 2", KeyCode.Space);      // 
                ps4KeyString[(int)ButtunCode.X] = Tuple.Create("joystick button 0", KeyCode.C);          // 
                ps4KeyString[(int)ButtunCode.Y] = Tuple.Create("joystick button 3", KeyCode.V);          // 
                ps4KeyString[(int)ButtunCode.R1] = Tuple.Create("joystick button 5", KeyCode.E);         // 
                ps4KeyString[(int)ButtunCode.R2] = Tuple.Create("joystick button 0", KeyCode.N);         // 
                ps4KeyString[(int)ButtunCode.L1] = Tuple.Create("joystick button 4", KeyCode.Q);         // 
                ps4KeyString[(int)ButtunCode.L2] = Tuple.Create("joystick button 0", KeyCode.L);         // 
                ps4KeyString[(int)ButtunCode.Menu] = Tuple.Create("joystick button 9", KeyCode.P);       // 
                ps4KeyString[(int)ButtunCode.View] = Tuple.Create("joystick button 12", KeyCode.Escape); // 現在未割り当て

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
            }

            void MoadChange(GamePad _gamePad)
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

            public bool GetKeyDown(ButtunCode _buttun)
            {
                var keyStringFlag =
                    Input.GetKeyDown(activeKeyString[(int)_buttun].Item1) ||
                    Input.GetKeyDown(activeKeyString[(int)_buttun].Item2);

                return keyStringFlag;
            }

            public bool GetKey(ButtunCode _buttun)
            {
                var keyStringFlag =
                    Input.GetKey(activeKeyString[(int)_buttun].Item1) ||
                    Input.GetKey(activeKeyString[(int)_buttun].Item2);

                return keyStringFlag;
            }

            public bool GetKeyUp(ButtunCode _buttun)
            {
                var keyStringFlag =
                    Input.GetKeyUp(activeKeyString[(int)_buttun].Item1) ||
                    Input.GetKeyUp(activeKeyString[(int)_buttun].Item2);

                return keyStringFlag;
            }

            private float size = 1f;

            public bool GetArrow(ArrowCode _arrow)
            {
                switch (_arrow)
                {
                    case ArrowCode.RightArrow:
                        {
                            float x = Input.GetAxis(activeArrowString[(int)_arrow].Item1);
                            return x >= size || Input.GetKey(activeArrowString[(int)_arrow].Item2);
                        }
                    case ArrowCode.LeftArrow:
                        {
                            float x = Input.GetAxis(activeArrowString[(int)_arrow].Item1);
                            return x <= -size || Input.GetKey(activeArrowString[(int)_arrow].Item2);
                        }
                    case ArrowCode.UpArrow:
                        {
                            float y = Input.GetAxis(activeArrowString[(int)_arrow].Item1);
                            return y <= -size || Input.GetKey(activeArrowString[(int)_arrow].Item2);
                        }
                    case ArrowCode.DownArrow:
                        {
                            float y = Input.GetAxis(activeArrowString[(int)_arrow].Item1);
                            return y >= size || Input.GetKey(activeArrowString[(int)_arrow].Item2);
                        }
                }
                return false;
            }
            
            public Vector2 GetLStick()
            {
                Vector2 ret;
                ret.x = Input.GetAxis("Horizontal");
                ret.y = Input.GetAxis("Vertical");
                return ret;
            }

            public Vector2 GetRStick()
            {
                Vector2 ret;
                ret.x = Input.GetAxis("RHorizontal");
                ret.y = Input.GetAxis("RVertical");
                return ret;
            }
        }
    }
}
