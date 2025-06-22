using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using System;
using System.Reflection;

public class KeyBindManager : MonoBehaviour
{
    public static KeyCode dashKey = KeyCode.C;
    public static KeyCode attackKey = KeyCode.X;
    public static KeyCode jumpKey = KeyCode.Z;
    public static KeyCode heavyKey = KeyCode.F;

    public InputField inputfieldDash;
    public InputField inputfieldAttack;
    public InputField inputfieldJump;
    public InputField inputfieldHeavy;

    public float shortKeyWidth = 160f;
    public float longKeyWidth = 200f;

    /*
    private float pivotXJump = -4.2f;
    private float pivotXDash = -147.3f;
    private float pivotXAttack = -71.1f;
    */

    private float pivotX = 0.5f;
    private float pivotY = 0.5f;

    private void Start()
    {
        SetPivot(inputfieldJump, pivotX, pivotY);
        SetPivot(inputfieldDash, pivotX, pivotY);
        SetPivot(inputfieldAttack, pivotX, pivotY);
        SetPivot(inputfieldHeavy, pivotX, pivotY);

        LoadKeyBindings();
    }

    private void Update()
    {
        DetectKeyInput();
    }

    private void DetectKeyInput()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (!Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Backspace))
                    {
                        if (IsKeyAlreadyAssigned(key))
                        {
                            /*
                            if (inputfieldDash.isFocused)
                            {
                                
                                inputfieldDash.text = "Denied";
                                AdjustInputFieldWidth(inputfieldDash, "Denied");
                            }
                            else if (inputfieldAttack.isFocused)
                            { 
                                inputfieldAttack.text = "Denied";
                                AdjustInputFieldWidth(inputfieldAttack, "Denied");
                            }
                            else if (inputfieldJump.isFocused)
                            {
                                inputfieldJump.text = "Denied";
                                AdjustInputFieldWidth(inputfieldJump, "Denied");
                            }
                            */
                            if (inputfieldDash.isFocused)
                            {
                                inputfieldDash.text = string.Empty;
                                dashKey = KeyCode.None;
                            }
                            else if (inputfieldAttack.isFocused)
                            {
                                inputfieldAttack.text = string.Empty;
                                attackKey = KeyCode.None;
                            }
                            else if (inputfieldJump.isFocused)
                            {
                                inputfieldJump.text = string.Empty;
                                jumpKey = KeyCode.None;
                            }
                            else if(inputfieldHeavy.isFocused)
                            {
                                inputfieldHeavy.text = string.Empty;
                                heavyKey = KeyCode.None;
                            }
                            return;
                        }
                        if (inputfieldDash.isFocused)
                            SetDashKey(key);
                        else if (inputfieldAttack.isFocused)
                            SetAttackKey(key);
                        else if (inputfieldJump.isFocused)
                            SetJumpKey(key);
                        else if(inputfieldHeavy.isFocused)
                            SetHeavyKey(key);
                    }

                    if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        if (inputfieldDash.isFocused)
                        {
                            inputfieldDash.text = string.Empty;
                            dashKey = KeyCode.None;
                            inputfieldDash.placeholder.gameObject.SetActive(false);
                        }
                        else if (inputfieldAttack.isFocused)
                        {
                            inputfieldAttack.text = string.Empty;
                            attackKey = KeyCode.None;
                            inputfieldAttack.placeholder.gameObject.SetActive(false);
                        }
                        else if (inputfieldJump.isFocused)
                        { 
                            inputfieldJump.text = string.Empty;
                            jumpKey = KeyCode.None;
                            inputfieldJump.placeholder.gameObject.SetActive(false);
                        }
                        else if (inputfieldHeavy.isFocused)
                        {
                            inputfieldHeavy.text = string.Empty;
                            heavyKey = KeyCode.None;
                            inputfieldHeavy.placeholder.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    private void SetDashKey(KeyCode key)
    {
        dashKey = key;
        string keyDisplay = KeyToString(key);
        inputfieldDash.text = keyDisplay;
        AdjustInputFieldWidth(inputfieldDash, keyDisplay);
        SaveKeyBindings();
        Debug.Log("Dash Key Set: " + dashKey);
    }

    private void SetAttackKey(KeyCode key)
    {
        attackKey = key;
        string keyDisplay = KeyToString(key);
        inputfieldAttack.text = keyDisplay;
        AdjustInputFieldWidth(inputfieldAttack, keyDisplay);
        SaveKeyBindings();
        Debug.Log("Attack Key Set: " + attackKey);
    }

    private void SetJumpKey(KeyCode key)
    {
        jumpKey = key;
        string keyDisplay = KeyToString(key);
        inputfieldJump.text = keyDisplay;
        AdjustInputFieldWidth(inputfieldJump, keyDisplay);
        SaveKeyBindings();
        Debug.Log("Jump Key Set: " + jumpKey);
    }

    private void SetHeavyKey(KeyCode key)
    {
        heavyKey = key;
        string keyDisplay = KeyToString(key);
        inputfieldHeavy.text = keyDisplay;
        AdjustInputFieldWidth(inputfieldHeavy, keyDisplay);
        SaveKeyBindings();
        Debug.Log("HeavyAttack Key Set: " + jumpKey);
    }

    private void SaveKeyBindings()
    {
        PlayerPrefs.SetInt("DashKey", (int)dashKey);
        PlayerPrefs.SetInt("AttackKey", (int)attackKey);
        PlayerPrefs.SetInt("JumpKey", (int)jumpKey);
        PlayerPrefs.SetInt("HeavyKey", (int)heavyKey);
        PlayerPrefs.Save();
        Debug.Log("Key bindings saved.");
    }

    private void LoadKeyBindings()
    {
        dashKey = (KeyCode)PlayerPrefs.GetInt("DashKey", (int)KeyCode.C);
        attackKey = (KeyCode)PlayerPrefs.GetInt("AttackKey", (int)KeyCode.X);
        jumpKey = (KeyCode)PlayerPrefs.GetInt("JumpKey", (int)KeyCode.Z);
        heavyKey = (KeyCode)PlayerPrefs.GetInt("HeavyKey", (int)KeyCode.F);

        string dashDisplay = KeyToString(dashKey);
        string attackDisplay = KeyToString(attackKey);
        string jumpDisplay = KeyToString(jumpKey);
        string HeavyDisplay = KeyToString(heavyKey);

        inputfieldDash.text = dashDisplay;
        inputfieldAttack.text = attackDisplay;
        inputfieldJump.text = jumpDisplay;
        inputfieldHeavy.text = HeavyDisplay;

        AdjustInputFieldWidth(inputfieldDash, dashDisplay);
        AdjustInputFieldWidth(inputfieldAttack, attackDisplay);
        AdjustInputFieldWidth(inputfieldJump, jumpDisplay);
        AdjustInputFieldWidth(inputfieldHeavy, HeavyDisplay);

        Debug.Log("Key bindings loaded: Dash[" + dashKey + "] Attack[" + attackKey + "] Jump[" + jumpKey + "] + [" + heavyKey + "]");
    }

    private bool IsKeyAlreadyAssigned(KeyCode key)
    {
        return key == dashKey || key == attackKey || key == jumpKey || key == heavyKey;
    }

    private string KeyToString(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Space: return "Space";
            case KeyCode.LeftShift: return "Shift";
            case KeyCode.RightShift: return "Shift";
            case KeyCode.Tab: return "Tab";
            case KeyCode.LeftControl: return "LCtrl";
            case KeyCode.RightControl: return "RCtrl";
            case KeyCode.LeftAlt: return "LAlt";
            case KeyCode.RightAlt: return "RAlt";

            case KeyCode.UpArrow: return "Up Arrow";
            case KeyCode.DownArrow: return "Down Arrow";
            case KeyCode.LeftArrow: return "Left Arrow";
            case KeyCode.RightArrow: return "Right Arrow";

            case KeyCode.A: return "A";
            case KeyCode.B: return "B";
            case KeyCode.C: return "C";
            case KeyCode.D: return "D";
            case KeyCode.E: return "E";
            case KeyCode.F: return "F";
            case KeyCode.G: return "G";
            case KeyCode.H: return "H";
            case KeyCode.I: return "I";
            case KeyCode.J: return "J";
            case KeyCode.K: return "K";
            case KeyCode.L: return "L";
            case KeyCode.M: return "M";
            case KeyCode.N: return "N";
            case KeyCode.O: return "O";
            case KeyCode.P: return "P";
            case KeyCode.Q: return "Q";
            case KeyCode.R: return "R";
            case KeyCode.S: return "S";
            case KeyCode.T: return "T";
            case KeyCode.U: return "U";
            case KeyCode.V: return "V";
            case KeyCode.W: return "W";
            case KeyCode.X: return "X";
            case KeyCode.Y: return "Y";
            case KeyCode.Z: return "Z";

            case KeyCode.Alpha0: return "0";
            case KeyCode.Alpha1: return "1";
            case KeyCode.Alpha2: return "2";
            case KeyCode.Alpha3: return "3";
            case KeyCode.Alpha4: return "4";
            case KeyCode.Alpha5: return "5";
            case KeyCode.Alpha6: return "6";
            case KeyCode.Alpha7: return "7";
            case KeyCode.Alpha8: return "8";
            case KeyCode.Alpha9: return "9";

            case KeyCode.F1: return "F1";
            case KeyCode.F2: return "F2";
            case KeyCode.F3: return "F3";
            case KeyCode.F4: return "F4";
            case KeyCode.F5: return "F5";
            case KeyCode.F6: return "F6";
            case KeyCode.F7: return "F7";
            case KeyCode.F8: return "F8";
            case KeyCode.F9: return "F9";
            case KeyCode.F10: return "F10";
            case KeyCode.F11: return "F11";
            case KeyCode.F12: return "F12";

            case KeyCode.Mouse0: return "LMB";
            case KeyCode.Mouse1: return "RMB";
            case KeyCode.Mouse2: return "MMB";

            case KeyCode.Delete: return "Del";
            case KeyCode.Insert: return "Insert";
            case KeyCode.Home: return "Home";
            case KeyCode.End: return "End";
            case KeyCode.PageUp: return "PgUp";
            case KeyCode.PageDown: return "PgDn";


            case KeyCode.CapsLock: return "Caps";
            case KeyCode.ScrollLock: return "ScrLk";
            case KeyCode.Numlock: return "NumLk";

            case KeyCode.Print: return "PrtSc";
            case KeyCode.Pause: return "Pause";

            case KeyCode.LeftWindows: return "LWin";
            case KeyCode.RightWindows: return "RWin";

            case KeyCode.Keypad0: return "Num0";
            case KeyCode.Keypad1: return "Num1";
            case KeyCode.Keypad2: return "Num2";
            case KeyCode.Keypad3: return "Num3";
            case KeyCode.Keypad4: return "Num4";
            case KeyCode.Keypad5: return "Num5";
            case KeyCode.Keypad6: return "Num6";
            case KeyCode.Keypad7: return "Num7";
            case KeyCode.Keypad8: return "Num8";
            case KeyCode.Keypad9: return "Num9";

            case KeyCode.KeypadPeriod: return "Num.";
            case KeyCode.KeypadDivide: return "Num/";
            case KeyCode.KeypadMultiply: return "Num*";
            case KeyCode.KeypadMinus: return "Num-";
            case KeyCode.KeypadPlus: return "Num+";
            case KeyCode.KeypadEnter: return "Enter";
            case KeyCode.KeypadEquals: return "Num=";

            case KeyCode.LeftBracket: return "LBrk";
            case KeyCode.RightBracket: return "RBrk";
            case KeyCode.Semicolon: return "Semi";
            case KeyCode.Quote: return "Quote";
            case KeyCode.Comma: return "Comma";
            case KeyCode.Period: return "Dot";
            case KeyCode.Slash: return "Slash";
            case KeyCode.Backslash: return "Bsla";
            case KeyCode.Tilde: return "Tlde";
            case KeyCode.BackQuote: return "Bqte";
            case KeyCode.Equals: return "Equal";
            case KeyCode.Minus: return "Minus";

            case KeyCode.AltGr: return "AltG";
            case KeyCode.Help: return "Help";


            default: return "?";
        }
    }

    private void AdjustInputFieldWidth(InputField field, string keyName)
    {

        RectTransform rect = field.GetComponent<RectTransform>();
        Vector2 originalPosition = rect.anchoredPosition;
        float newWidth = (keyName.Length > 1) ? longKeyWidth : shortKeyWidth;
        rect.sizeDelta = new Vector2(newWidth, rect.sizeDelta.y);
        rect.anchoredPosition = originalPosition;

        //es aq iyos just incase imis kodia rom marjvniv gaizardos marto size
        /*
        RectTransform rect = field.GetComponent<RectTransform>();
        Vector2 originalPosition = rect.anchoredPosition;
        rect.pivot = new Vector2(0f, rect.pivot.y);
        float newWidth = (keyName.Length > 1) ? longKeyWidth : shortKeyWidth;
        rect.sizeDelta = new Vector2(newWidth, rect.sizeDelta.y);

        if (keyName == "dashDisplay")
        {
            rect.anchoredPosition = new Vector2(104.7f, -10.35205f);
        }
        else if (keyName == "attackDisplay")
        {
            rect.anchoredPosition = new Vector2(-3.200005f, -10.89999f);
        }
        else if (keyName == "JumpDisplay")
        {
            rect.anchoredPosition = new Vector2(-103.7f, -9.099991f);
        }
        */
    }

    private void SetPivotToLeft(InputField field)
    {
        RectTransform rect = field.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0f, rect.pivot.y);
    }

    private void SetPivot(InputField field, float pivotX, float pivotY)
    {
        RectTransform rect = field.GetComponent<RectTransform>();
        rect.pivot = new Vector2(pivotX, pivotY);
    }
}