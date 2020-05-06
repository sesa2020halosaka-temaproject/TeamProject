﻿// InputManger
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

        public enum ArrowCoad : int
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
            public InputManager()
            {
                keyString = new Tuple<string, KeyCode>[(int)ButtunCode.Max];
                ps4KeyString = new Tuple<string, KeyCode>[(int)ButtunCode.Max];

                keyString[(int)ButtunCode.A] = Tuple.Create("joystick button 0", KeyCode.LeftShift);
                keyString[(int)ButtunCode.B] = Tuple.Create("joystick button 1", KeyCode.Space);
                keyString[(int)ButtunCode.X] = Tuple.Create("joystick button 2", KeyCode.C);
                keyString[(int)ButtunCode.Y] = Tuple.Create("joystick button 3", KeyCode.V);
                keyString[(int)ButtunCode.R1] = Tuple.Create("joystick button 4", KeyCode.B);
                keyString[(int)ButtunCode.R2] = Tuple.Create("joystick button 0", KeyCode.N);
                keyString[(int)ButtunCode.L1] = Tuple.Create("joystick button 5", KeyCode.M);
                keyString[(int)ButtunCode.L2] = Tuple.Create("joystick button 0", KeyCode.L);
                keyString[(int)ButtunCode.Menu] = Tuple.Create("joystick button 7", KeyCode.Escape);
                keyString[(int)ButtunCode.View] = Tuple.Create("joystick button 6", KeyCode.P);

                ps4KeyString[(int)ButtunCode.A] = Tuple.Create("joystick button 1", KeyCode.LeftShift);
                ps4KeyString[(int)ButtunCode.B] = Tuple.Create("joystick button 2", KeyCode.Space);
                ps4KeyString[(int)ButtunCode.X] = Tuple.Create("joystick button 0", KeyCode.C);
                ps4KeyString[(int)ButtunCode.Y] = Tuple.Create("joystick button 3", KeyCode.V);
                ps4KeyString[(int)ButtunCode.R1] = Tuple.Create("joystick button 5", KeyCode.B);
                ps4KeyString[(int)ButtunCode.R2] = Tuple.Create("joystick button 0", KeyCode.N);
                ps4KeyString[(int)ButtunCode.L1] = Tuple.Create("joystick button 4", KeyCode.M);
                ps4KeyString[(int)ButtunCode.L2] = Tuple.Create("joystick button 0", KeyCode.L);
                ps4KeyString[(int)ButtunCode.Menu] = Tuple.Create("joystick button 9", KeyCode.Escape);
                ps4KeyString[(int)ButtunCode.View] = Tuple.Create("joystick button 12", KeyCode.P);

                // 初期状態はXBox
                activeKeyString = keyString;
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
            public bool GetArrwo(ArrowCoad _arrow)
            {
                float x = Input.GetAxis("ArrowX");
                float y = Input.GetAxis("ArrowY");
                
                switch (_arrow)
                {
                    case ArrowCoad.RightArrow:
                        return x <= size;

                    case ArrowCoad.LeftArrow:
                        return x >= -size;

                    case ArrowCoad.UpArrow:
                        return y <= size;

                    case ArrowCoad.DownArrow:
                        return y >= -size;
                }

                return false;
            }

            //public bool GetArrow(ArrowCoad arrow)
            //{
            //    return false;
            //}

            //public bool GetArrowUp(ArrowCoad arrow)
            //{
            //    return false;
            //}


            //public bool GetArrowDown(ArrowCoad arrow)
            //{
            //    return false;
            //}


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
