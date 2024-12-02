using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InnovaFramework.iGUI
{
    public delegate void iEventCallback(Event evt);
    public class iEvent
    {
        public static event iEventCallback OnMouseDown;
        public static event iEventCallback OnMouseUp;
        public static event iEventCallback OnMouseMove;
        public static event iEventCallback OnMouseDrag;
        public static event iEventCallback OnMouseEnterWindow;
        public static event iEventCallback OnMouseExitWindow;

        public static event iEventCallback OnProcessEvent;

        public static event iEventCallback OnKeyDown;
        public static event iEventCallback OnKeyUp;
        public static event iEventCallback OnRepaint;
        public static event iEventCallback OnScrollWheel;

        private static Dictionary<KeyCode, bool> keys = new Dictionary<KeyCode, bool>()
        {
            { KeyCode.Backspace, false },
            { KeyCode.Delete, false },
            { KeyCode.Tab, false },
            { KeyCode.Clear, false },
            { KeyCode.Return, false },
            { KeyCode.Pause, false },
            { KeyCode.Escape, false },
            { KeyCode.Space, false },
            { KeyCode.Keypad0, false },
            { KeyCode.Keypad1, false },
            { KeyCode.Keypad2, false },
            { KeyCode.Keypad3, false },
            { KeyCode.Keypad4, false },
            { KeyCode.Keypad5, false },
            { KeyCode.Keypad6, false },
            { KeyCode.Keypad7, false },
            { KeyCode.Keypad8, false },
            { KeyCode.Keypad9, false },
            { KeyCode.KeypadPeriod, false },
            { KeyCode.KeypadDivide, false },
            { KeyCode.KeypadMultiply, false },
            { KeyCode.KeypadMinus, false },
            { KeyCode.KeypadPlus, false },
            { KeyCode.KeypadEnter, false },
            { KeyCode.KeypadEquals, false },
            { KeyCode.UpArrow, false },
            { KeyCode.DownArrow, false },
            { KeyCode.RightArrow, false },
            { KeyCode.LeftArrow, false },
            { KeyCode.Insert, false },
            { KeyCode.Home, false },
            { KeyCode.End, false },
            { KeyCode.PageUp, false },
            { KeyCode.PageDown, false },
            { KeyCode.F1, false },
            { KeyCode.F2, false },
            { KeyCode.F3, false },
            { KeyCode.F4, false },
            { KeyCode.F5, false },
            { KeyCode.F6, false },
            { KeyCode.F7, false },
            { KeyCode.F8, false },
            { KeyCode.F9, false },
            { KeyCode.F10, false },
            { KeyCode.F11, false },
            { KeyCode.F12, false },
            { KeyCode.F13, false },
            { KeyCode.F14, false },
            { KeyCode.F15, false },
            { KeyCode.Alpha0, false },
            { KeyCode.Alpha1, false },
            { KeyCode.Alpha2, false },
            { KeyCode.Alpha3, false },
            { KeyCode.Alpha4, false },
            { KeyCode.Alpha5, false },
            { KeyCode.Alpha6, false },
            { KeyCode.Alpha7, false },
            { KeyCode.Alpha8, false },
            { KeyCode.Alpha9, false },
            { KeyCode.Exclaim, false },
            { KeyCode.DoubleQuote, false },
            { KeyCode.Hash, false },
            { KeyCode.Dollar, false },
            { KeyCode.Percent, false },
            { KeyCode.Ampersand, false },
            { KeyCode.Quote, false },
            { KeyCode.LeftParen, false },
            { KeyCode.RightParen, false },
            { KeyCode.Asterisk, false },
            { KeyCode.Plus, false },
            { KeyCode.Comma, false },
            { KeyCode.Minus, false },
            { KeyCode.Period, false },
            { KeyCode.Slash, false },
            { KeyCode.Colon, false },
            { KeyCode.Semicolon, false },
            { KeyCode.Less, false },
            { KeyCode.Equals, false },
            { KeyCode.Greater, false },
            { KeyCode.Question, false },
            { KeyCode.At, false },
            { KeyCode.LeftBracket, false },
            { KeyCode.Backslash, false },
            { KeyCode.RightBracket, false },
            { KeyCode.Caret, false },
            { KeyCode.Underscore, false },
            { KeyCode.BackQuote, false },
            { KeyCode.A, false },
            { KeyCode.B, false },
            { KeyCode.C, false },
            { KeyCode.D, false },
            { KeyCode.E, false },
            { KeyCode.F, false },
            { KeyCode.G, false },
            { KeyCode.H, false },
            { KeyCode.I, false },
            { KeyCode.J, false },
            { KeyCode.K, false },
            { KeyCode.L, false },
            { KeyCode.M, false },
            { KeyCode.N, false },
            { KeyCode.O, false },
            { KeyCode.P, false },
            { KeyCode.Q, false },
            { KeyCode.R, false },
            { KeyCode.S, false },
            { KeyCode.T, false },
            { KeyCode.U, false },
            { KeyCode.V, false },
            { KeyCode.W, false },
            { KeyCode.X, false },
            { KeyCode.Y, false },
            { KeyCode.Z, false },
            { KeyCode.LeftCurlyBracket, false },
            { KeyCode.Pipe, false },
            { KeyCode.RightCurlyBracket, false },
            { KeyCode.Tilde, false },
            { KeyCode.Numlock, false },
            { KeyCode.CapsLock, false },
            { KeyCode.ScrollLock, false },
            { KeyCode.RightShift, false },
            { KeyCode.LeftShift, false },
            { KeyCode.RightControl, false },
            { KeyCode.LeftControl, false },
            { KeyCode.RightAlt, false },
            { KeyCode.LeftAlt, false },
            { KeyCode.LeftApple, false },
            { KeyCode.LeftWindows, false },
            { KeyCode.RightApple, false },
            { KeyCode.RightWindows, false },
            { KeyCode.AltGr, false },
            { KeyCode.Help, false },
            { KeyCode.Print, false },
            { KeyCode.SysReq, false },
            { KeyCode.Break, false },
            { KeyCode.Menu, false },
            { KeyCode.Mouse0, false },
            { KeyCode.Mouse1, false },
            { KeyCode.Mouse2, false },
            { KeyCode.Mouse3, false },
            { KeyCode.Mouse4, false },
            { KeyCode.Mouse5, false },
            { KeyCode.Mouse6, false },
            { KeyCode.JoystickButton0, false },
            { KeyCode.JoystickButton1, false },
            { KeyCode.JoystickButton2, false },
            { KeyCode.JoystickButton3, false },
            { KeyCode.JoystickButton4, false },
            { KeyCode.JoystickButton5, false },
            { KeyCode.JoystickButton6, false },
            { KeyCode.JoystickButton7, false },
            { KeyCode.JoystickButton8, false },
            { KeyCode.JoystickButton9, false },
            { KeyCode.JoystickButton10, false },
            { KeyCode.JoystickButton11, false },
            { KeyCode.JoystickButton12, false },
            { KeyCode.JoystickButton13, false },
            { KeyCode.JoystickButton14, false },
            { KeyCode.JoystickButton15, false },
            { KeyCode.JoystickButton16, false },
            { KeyCode.JoystickButton17, false },
            { KeyCode.JoystickButton18, false },
            { KeyCode.JoystickButton19, false },
            { KeyCode.Joystick1Button0, false },
            { KeyCode.Joystick1Button1, false },
            { KeyCode.Joystick1Button2, false },
            { KeyCode.Joystick1Button3, false },
            { KeyCode.Joystick1Button4, false },
            { KeyCode.Joystick1Button5, false },
            { KeyCode.Joystick1Button6, false },
            { KeyCode.Joystick1Button7, false },
            { KeyCode.Joystick1Button8, false },
            { KeyCode.Joystick1Button9, false },
            { KeyCode.Joystick1Button10, false },
            { KeyCode.Joystick1Button11, false },
            { KeyCode.Joystick1Button12, false },
            { KeyCode.Joystick1Button13, false },
            { KeyCode.Joystick1Button14, false },
            { KeyCode.Joystick1Button15, false },
            { KeyCode.Joystick1Button16, false },
            { KeyCode.Joystick1Button17, false },
            { KeyCode.Joystick1Button18, false },
            { KeyCode.Joystick1Button19, false },
            { KeyCode.Joystick2Button0, false },
            { KeyCode.Joystick2Button1, false },
            { KeyCode.Joystick2Button2, false },
            { KeyCode.Joystick2Button3, false },
            { KeyCode.Joystick2Button4, false },
            { KeyCode.Joystick2Button5, false },
            { KeyCode.Joystick2Button6, false },
            { KeyCode.Joystick2Button7, false },
            { KeyCode.Joystick2Button8, false },
            { KeyCode.Joystick2Button9, false },
            { KeyCode.Joystick2Button10, false },
            { KeyCode.Joystick2Button11, false },
            { KeyCode.Joystick2Button12, false },
            { KeyCode.Joystick2Button13, false },
            { KeyCode.Joystick2Button14, false },
            { KeyCode.Joystick2Button15, false },
            { KeyCode.Joystick2Button16, false },
            { KeyCode.Joystick2Button17, false },
            { KeyCode.Joystick2Button18, false },
            { KeyCode.Joystick2Button19, false },
            { KeyCode.Joystick3Button0, false },
            { KeyCode.Joystick3Button1, false },
            { KeyCode.Joystick3Button2, false },
            { KeyCode.Joystick3Button3, false },
            { KeyCode.Joystick3Button4, false },
            { KeyCode.Joystick3Button5, false },
            { KeyCode.Joystick3Button6, false },
            { KeyCode.Joystick3Button7, false },
            { KeyCode.Joystick3Button8, false },
            { KeyCode.Joystick3Button9, false },
            { KeyCode.Joystick3Button10, false },
            { KeyCode.Joystick3Button11, false },
            { KeyCode.Joystick3Button12, false },
            { KeyCode.Joystick3Button13, false },
            { KeyCode.Joystick3Button14, false },
            { KeyCode.Joystick3Button15, false },
            { KeyCode.Joystick3Button16, false },
            { KeyCode.Joystick3Button17, false },
            { KeyCode.Joystick3Button18, false },
            { KeyCode.Joystick3Button19, false },
            { KeyCode.Joystick4Button0, false },
            { KeyCode.Joystick4Button1, false },
            { KeyCode.Joystick4Button2, false },
            { KeyCode.Joystick4Button3, false },
            { KeyCode.Joystick4Button4, false },
            { KeyCode.Joystick4Button5, false },
            { KeyCode.Joystick4Button6, false },
            { KeyCode.Joystick4Button7, false },
            { KeyCode.Joystick4Button8, false },
            { KeyCode.Joystick4Button9, false },
            { KeyCode.Joystick4Button10, false },
            { KeyCode.Joystick4Button11, false },
            { KeyCode.Joystick4Button12, false },
            { KeyCode.Joystick4Button13, false },
            { KeyCode.Joystick4Button14, false },
            { KeyCode.Joystick4Button15, false },
            { KeyCode.Joystick4Button16, false },
            { KeyCode.Joystick4Button17, false },
            { KeyCode.Joystick4Button18, false },
            { KeyCode.Joystick4Button19, false },
            { KeyCode.Joystick5Button0, false },
            { KeyCode.Joystick5Button1, false },
            { KeyCode.Joystick5Button2, false },
            { KeyCode.Joystick5Button3, false },
            { KeyCode.Joystick5Button4, false },
            { KeyCode.Joystick5Button5, false },
            { KeyCode.Joystick5Button6, false },
            { KeyCode.Joystick5Button7, false },
            { KeyCode.Joystick5Button8, false },
            { KeyCode.Joystick5Button9, false },
            { KeyCode.Joystick5Button10, false },
            { KeyCode.Joystick5Button11, false },
            { KeyCode.Joystick5Button12, false },
            { KeyCode.Joystick5Button13, false },
            { KeyCode.Joystick5Button14, false },
            { KeyCode.Joystick5Button15, false },
            { KeyCode.Joystick5Button16, false },
            { KeyCode.Joystick5Button17, false },
            { KeyCode.Joystick5Button18, false },
            { KeyCode.Joystick5Button19, false },
            { KeyCode.Joystick6Button0, false },
            { KeyCode.Joystick6Button1, false },
            { KeyCode.Joystick6Button2, false },
            { KeyCode.Joystick6Button3, false },
            { KeyCode.Joystick6Button4, false },
            { KeyCode.Joystick6Button5, false },
            { KeyCode.Joystick6Button6, false },
            { KeyCode.Joystick6Button7, false },
            { KeyCode.Joystick6Button8, false },
            { KeyCode.Joystick6Button9, false },
            { KeyCode.Joystick6Button10, false },
            { KeyCode.Joystick6Button11, false },
            { KeyCode.Joystick6Button12, false },
            { KeyCode.Joystick6Button13, false },
            { KeyCode.Joystick6Button14, false },
            { KeyCode.Joystick6Button15, false },
            { KeyCode.Joystick6Button16, false },
            { KeyCode.Joystick6Button17, false },
            { KeyCode.Joystick6Button18, false },
            { KeyCode.Joystick6Button19, false },
            { KeyCode.Joystick7Button0, false },
            { KeyCode.Joystick7Button1, false },
            { KeyCode.Joystick7Button2, false },
            { KeyCode.Joystick7Button3, false },
            { KeyCode.Joystick7Button4, false },
            { KeyCode.Joystick7Button5, false },
            { KeyCode.Joystick7Button6, false },
            { KeyCode.Joystick7Button7, false },
            { KeyCode.Joystick7Button8, false },
            { KeyCode.Joystick7Button9, false },
            { KeyCode.Joystick7Button10, false },
            { KeyCode.Joystick7Button11, false },
            { KeyCode.Joystick7Button12, false },
            { KeyCode.Joystick7Button13, false },
            { KeyCode.Joystick7Button14, false },
            { KeyCode.Joystick7Button15, false },
            { KeyCode.Joystick7Button16, false },
            { KeyCode.Joystick7Button17, false },
            { KeyCode.Joystick7Button18, false },
            { KeyCode.Joystick7Button19, false },
            { KeyCode.Joystick8Button0, false },
            { KeyCode.Joystick8Button1, false },
            { KeyCode.Joystick8Button2, false },
            { KeyCode.Joystick8Button3, false },
            { KeyCode.Joystick8Button4, false },
            { KeyCode.Joystick8Button5, false },
            { KeyCode.Joystick8Button6, false },
            { KeyCode.Joystick8Button7, false },
            { KeyCode.Joystick8Button8, false },
            { KeyCode.Joystick8Button9, false },
            { KeyCode.Joystick8Button10, false },
            { KeyCode.Joystick8Button11, false },
            { KeyCode.Joystick8Button12, false },
            { KeyCode.Joystick8Button13, false },
            { KeyCode.Joystick8Button14, false },
            { KeyCode.Joystick8Button15, false },
            { KeyCode.Joystick8Button16, false },
            { KeyCode.Joystick8Button17, false },
            { KeyCode.Joystick8Button18, false },
            { KeyCode.Joystick8Button19, false },
        };


        public static bool IsKeyPressed(KeyCode key)
        {
            if (keys.ContainsKey(key))
            {
                return keys[key];
            }
            return false;
        }


        public static void ProcessEvent(Event e)
        {
            if (e == null) return;

            OnProcessEvent?.Invoke(e);

            switch (e.type)
            {
                case EventType.MouseDown:
                    {
                        OnMouseDown?.Invoke(e);
                        break;
                    }
                case EventType.MouseUp:
                    {
                        OnMouseUp?.Invoke(e);
                        break;
                    }
                case EventType.MouseMove:
                    {
                        OnMouseMove?.Invoke(e);
                        break;
                    }
                case EventType.MouseDrag:
                    {
                        OnMouseDrag?.Invoke(e);
                        break;
                    }
                case EventType.MouseEnterWindow:
                    {
                        OnMouseEnterWindow?.Invoke(e);
                        break;
                    }
                case EventType.MouseLeaveWindow:
                    {
                        OnMouseExitWindow?.Invoke(e);
                        break;
                    }
                case EventType.KeyDown:
                    {
                        if (keys.ContainsKey(e.keyCode))
                        {
                            keys[e.keyCode] = true;
                        }
                        OnKeyDown?.Invoke(e);
                        break;
                    }
                case EventType.KeyUp:
                    {
                        if (keys.ContainsKey(e.keyCode))
                        {
                            keys[e.keyCode] = false;
                        }
                        OnKeyUp?.Invoke(e);
                        break;
                    }
                case EventType.ScrollWheel:
                    {
                        OnScrollWheel?.Invoke(e);
                        break;
                    }
                case EventType.Repaint:
                    {
                        OnRepaint?.Invoke(e);
                        break;
                    }
            }
        }
    }
}