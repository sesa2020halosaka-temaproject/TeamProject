// InputManger
// UnityInputのラッピング
// 一括管理して変更を楽にする

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


        class InputManager : Singleton<InputManager>
        {
            static private Tuple<string,KeyCode>[] keyString;

            public InputManager()
            {
                keyString = new Tuple<string, KeyCode>[(int)ButtunCode.Max];

                keyString[(int)ButtunCode.A] = Tuple.Create("joystick button 0", KeyCode.Space);
                keyString[(int)ButtunCode.B] = Tuple.Create("joystick button 1", KeyCode.LeftShift); 
                keyString[(int)ButtunCode.X] = Tuple.Create("joystick button 2", KeyCode.C); 
                keyString[(int)ButtunCode.Y] = Tuple.Create("joystick button 3", KeyCode.V); 
                keyString[(int)ButtunCode.R1] = Tuple.Create("joystick button 4", KeyCode.B); 
                keyString[(int)ButtunCode.R2] = Tuple.Create("joystick button 0", KeyCode.N); 
                keyString[(int)ButtunCode.L1] = Tuple.Create("joystick button 5", KeyCode.M); 
                keyString[(int)ButtunCode.L2] = Tuple.Create("joystick button 0", KeyCode.L);
                keyString[(int)ButtunCode.Menu] = Tuple.Create("joystick button 7", KeyCode.Escape);
                keyString[(int)ButtunCode.View] = Tuple.Create("joystick button 6", KeyCode.P);
            }

            public bool GetKeyDown(ButtunCode _buttun)
            {
                var keyStringFlag =
                    Input.GetKeyDown(keyString[(int)_buttun].Item1) ||
                    Input.GetKeyDown(keyString[(int)_buttun].Item2);

                return keyStringFlag;
            }

            public bool GetKey(ButtunCode _buttun)
            {
                var keyStringFlag =
                    Input.GetKey(keyString[(int)_buttun].Item1) ||
                    Input.GetKey(keyString[(int)_buttun].Item2);

                return keyStringFlag;
            }

            public bool GetKeyUp(ButtunCode _buttun)
            {
                var keyStringFlag =
                    Input.GetKeyUp(keyString[(int)_buttun].Item1) ||
                    Input.GetKeyUp(keyString[(int)_buttun].Item2);

                return keyStringFlag;
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
