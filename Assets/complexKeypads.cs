using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using KMHelper;
using System;
using System.Threading;
using UnityEngine;

public class complexKeypads : MonoBehaviour
{

    public KMAudio newAudio;
    public KMBombModule module;
    public KMBombInfo info;
    public KMSelectable[] btn;
    public MeshRenderer[] leds;
    public KMRuleSeedable RuleSeedable;
    public string[] symbols;

    public Material off;
    public Material green;
    public Material red;

    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;

    private bool _isSolved = false, _lightsOn = false;
    public int symbolRow = 1;
    private int[] one, two, three, four, five;
    public int[] buttonNumbers = new int[9];
    public string[] buttonStrings = new string[9];
    public int[] list = new int[9];
    public int[] buttonPressOrder = new int[9];
    public int[] symbolSet;

    private KMBombInfoExtensions.KnownPortType[] ports = new KMBombInfoExtensions.KnownPortType[6] { KMBombInfoExtensions.KnownPortType.DVI, KMBombInfoExtensions.KnownPortType.Parallel, KMBombInfoExtensions.KnownPortType.PS2, KMBombInfoExtensions.KnownPortType.RJ45, KMBombInfoExtensions.KnownPortType.Serial, KMBombInfoExtensions.KnownPortType.StereoRCA };
    private int batteriesNeeded;
    private KMBombInfoExtensions.KnownPortType firstNeeded;
    private KMBombInfoExtensions.KnownPortType secondNeeded;

    public int pressIndex = 0;

    public int ruleInUse = 3;

    // Use this for initialization
    void Start()
    {
        _moduleId = _moduleIdCounter++;
        module.OnActivate += Activate;
    }

    private void Awake()
    {
        for (int i = 0; i < 9; i++)
        {
            int j = i;
            btn[i].OnInteract += delegate ()
            {
                handlePress(j);
                return false;
            };
        }       
    }

    // Update is called once per frame
    void Activate()
    {
        Init();
        _lightsOn = true;
    }

    void Init()
    {
        symbolRow = UnityEngine.Random.Range(1, 6);
        Debug.LogFormat("[Complex Keypad #{0}] Random symbol row selected: {1}", _moduleId, symbolRow);
        setupSymbols();
        setupRuleSeed();
        setupButtons();
        setupPressOrder();
        setupLEDS();
    }

    void setupRuleSeed()
    {
        var rnd = RuleSeedable.GetRNG();
        if (rnd.Seed == 1)
        {
            one = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            two = new int[10] { 10, 0, 6, 11, 12, 13, 14, 15, 16, 17 };
            three = new int[10] { 18, 17, 11, 13, 2, 8, 1, 10, 19, 9 };
            four = new int[10] { 19, 15, 18, 1, 4, 11, 0, 7, 17, 20 };
            five = new int[10] { 13, 11, 4, 12, 20, 16, 5, 0, 14, 8 };
            batteriesNeeded = 2; //more than
            firstNeeded = ports[1];
            secondNeeded = ports[0];
        } else
        {
            var options = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var batteryOptions = new int[4] { 1, 2, 3, 4 };
            var rowOptions = new int[5] { 0, 1, 2, 3, 4 };
            var rows = new int[][] { new int[10], new int[10], new int[10], new int[10], new int[10] };

            options.Shuffle(rnd);
            batteryOptions.Shuffle(rnd);
            ports.Shuffle(rnd);
            rowOptions.Shuffle(rnd);

            rows[0] = new int[10] { options[0], options[1], options[2], options[3], options[4], options[5], options[6], options[7], options[8], options[9] };
            rows[1] = new int[10] { options[10], options[11], options[12], options[13], options[14], options[15], options[16], options[17], options[18], options[19] };
            rows[2] = new int[10] { options[20], options[2], options[4], options[6], options[8], options[10], options[12], options[14], options[16], options[18] };
            rows[3] = new int[10] { options[1], options[3], options[5], options[7], options[9], options[11], options[13], options[15], options[17], options[19] };
            rows[4] = new int[10] { options[19], options[7], options[4], options[0], options[9], options[13], options[12], options[20], options[16], options[1] };

            one = rows[rowOptions[0]];
            two = rows[rowOptions[1]];
            three = rows[rowOptions[2]];
            four = rows[rowOptions[3]];
            five = rows[rowOptions[4]];

            batteriesNeeded = batteryOptions[0];
            firstNeeded = ports[2];
            secondNeeded = ports[3];

            
        }
    }

    void setupLEDS()
    {
        for (int i = 0; i < 9; i++)
        {
            leds[i].material = off;
        }
    }



    void setupSymbols()
    {
        symbols[0] = "α";
        symbols[1] = "ε";
        symbols[2] = "θ";
        symbols[3] = "ψ";
        symbols[4] = "μ";
        symbols[5] = "Ξ";
        symbols[6] = "ζ";
        symbols[7] = "σ";
        symbols[8] = "β";
        symbols[9] = "Δ";
        symbols[10] = "π";
        symbols[11] = "ω";
        symbols[12] = "δ";
        symbols[13] = "Γ";
        symbols[14] = "η";
        symbols[15] = "م";
        symbols[16] = "⊃";
        symbols[17] = "κ";
        symbols[18] = "φ";
        symbols[19] = "נ";
        symbols[20] = "ن";
    }
    void setupButtons()
    {
        buttonNumbers[0] = UnityEngine.Random.Range(0, 10);
        buttonNumbers[1] = UnityEngine.Random.Range(0, 10);
        do
        {
            buttonNumbers[1] = UnityEngine.Random.Range(0, 10);
        } while (buttonNumbers[1] == buttonNumbers[0]);
        buttonNumbers[2] = UnityEngine.Random.Range(0, 10);
        do
        {
            buttonNumbers[2] = UnityEngine.Random.Range(0, 10);
        } while (buttonNumbers[2] == buttonNumbers[0] || buttonNumbers[2] == buttonNumbers[1]);
        buttonNumbers[3] = UnityEngine.Random.Range(0, 10);
        do
        {
            buttonNumbers[3] = UnityEngine.Random.Range(0, 10);
        } while (buttonNumbers[3] == buttonNumbers[0] || buttonNumbers[3] == buttonNumbers[1] || buttonNumbers[3] == buttonNumbers[2]);
        buttonNumbers[4] = UnityEngine.Random.Range(0, 10);
        do
        {
            buttonNumbers[4] = UnityEngine.Random.Range(0, 10);
        } while (buttonNumbers[4] == buttonNumbers[0] || buttonNumbers[4] == buttonNumbers[1] || buttonNumbers[4] == buttonNumbers[2] || buttonNumbers[4] == buttonNumbers[3]);
        buttonNumbers[5] = UnityEngine.Random.Range(0, 10);
        do
        {
            buttonNumbers[5] = UnityEngine.Random.Range(0, 10);
        } while (buttonNumbers[5] == buttonNumbers[0] || buttonNumbers[5] == buttonNumbers[1] || buttonNumbers[5] == buttonNumbers[2] || buttonNumbers[5] == buttonNumbers[3] || buttonNumbers[5] == buttonNumbers[4]);
        buttonNumbers[6] = UnityEngine.Random.Range(0, 10);
        do
        {
            buttonNumbers[6] = UnityEngine.Random.Range(0, 10);
        } while (buttonNumbers[6] == buttonNumbers[0] || buttonNumbers[6] == buttonNumbers[1] || buttonNumbers[6] == buttonNumbers[2] || buttonNumbers[6] == buttonNumbers[3] || buttonNumbers[6] == buttonNumbers[4] || buttonNumbers[6] == buttonNumbers[5]);
        buttonNumbers[7] = UnityEngine.Random.Range(0, 10);
        do
        {
            buttonNumbers[7] = UnityEngine.Random.Range(0, 10);
        } while (buttonNumbers[7] == buttonNumbers[0] || buttonNumbers[7] == buttonNumbers[1] || buttonNumbers[7] == buttonNumbers[2] || buttonNumbers[7] == buttonNumbers[3] || buttonNumbers[7] == buttonNumbers[4] || buttonNumbers[7] == buttonNumbers[5] || buttonNumbers[7] == buttonNumbers[6]);
        buttonNumbers[8] = UnityEngine.Random.Range(0, 10);
        do
        {
            buttonNumbers[8] = UnityEngine.Random.Range(0, 10);
        } while (buttonNumbers[8] == buttonNumbers[0] || buttonNumbers[8] == buttonNumbers[1] || buttonNumbers[8] == buttonNumbers[2] || buttonNumbers[8] == buttonNumbers[3] || buttonNumbers[8] == buttonNumbers[4] || buttonNumbers[8] == buttonNumbers[5] || buttonNumbers[8] == buttonNumbers[6] || buttonNumbers[8] == buttonNumbers[7]);


        switch (symbolRow)
        {
            case 1:
                symbolSet = one;
                break;
            case 2:
                symbolSet = two;
                break;
            case 3:
                symbolSet = three;
                break;
            case 4:
                symbolSet = four;
                break;
            case 5:
                symbolSet = five;
                break;
            default:
                symbolSet = one;
                Debug.LogFormat("[Complex Keypad #{0}] Symbol row not selected correctly! Random row returned {1}", _moduleId, symbolRow);
                break;
        }



        for (int i = 0; i < 9; i++)
        {
            list[i] = symbolSet[buttonNumbers[i]];
        }

        for (int i = 0; i < 9; i++)
        {
            buttonStrings[i] = symbols[list[i]];
        }

        if (info.GetBatteryCount() > batteriesNeeded && info.IsPortPresent(firstNeeded))
        {
            ruleInUse = 1;
            Debug.LogFormat("[Complex Keypad #{0}] Battery count: {1}. Contains "+firstNeeded.ToString()+": True. Rule 1 in use (disregard chart, press left - right on module)", _moduleId, info.GetBatteryCount());
        }
        else if (info.IsPortPresent(secondNeeded) && info.IsIndicatorOn(KMBombInfoExtensions.KnownIndicatorLabel.BOB))
        {
            ruleInUse = 2;
            Debug.LogFormat("[Complex Keypad #{0}] Battery count: {1}. Contains " + secondNeeded.ToString() +": True. Lit indicator BOB: True. Rule 2 in use (press right - left according to chart)", _moduleId, info.GetBatteryCount());
        }
        else
        {
            ruleInUse = 3;
            Debug.LogFormat("[Complex Keypad #{0}] Battery count: {1}. Does not contain match either prior rule. Rule 3 in use (press left - right according to chart)", _moduleId, info.GetBatteryCount());
        }

        for (int i = 0; i < 9; i++)
        {
            btn[i].GetComponentInChildren<TextMesh>().text = buttonStrings[i];
        }

    }
    void setupPressOrder()
    {
        switch (ruleInUse)
        {
            case 1:
                for (int i = 0; i < 9; i++)
                {
                    buttonPressOrder[i] = i;
                }
                Debug.LogFormat("[Complex Keypad #{0}] Button press order: 1:{1}({2}), 2:{3}({4}), 3:{5}({6}), 4:{7}({8}), 5:{9}({10}), 6:{11}({12}), 7:{13}({14}), 8:{15}({16}), 9:{17}({18})", _moduleId, buttonPressOrder[0], buttonStrings[buttonPressOrder[0]], buttonPressOrder[1], buttonStrings[buttonPressOrder[1]], buttonPressOrder[2], buttonStrings[buttonPressOrder[2]], buttonPressOrder[3], buttonStrings[buttonPressOrder[3]], buttonPressOrder[4], buttonStrings[buttonPressOrder[4]], buttonPressOrder[5], buttonStrings[buttonPressOrder[5]], buttonPressOrder[6], buttonStrings[buttonPressOrder[6]], buttonPressOrder[7], buttonStrings[buttonPressOrder[7]], buttonPressOrder[8], buttonStrings[buttonPressOrder[8]]);
                break;
            case 2:
                int[] reverseSet = symbolSet;
                for (int i = 0; i < reverseSet.Length / 2; i++)
                {
                    int tmp = reverseSet[i];
                    reverseSet[i] = reverseSet[reverseSet.Length - i - 1];
                    reverseSet[reverseSet.Length - i - 1] = tmp;
                }
                int removal = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (!list.Contains(reverseSet[i]))
                    {
                        removal = i;
                        break;
                    }
                }
                Debug.LogFormat("[Complex Keypad #{0}] Symbol removed from manual's list: {1}", _moduleId, symbols[reverseSet[removal]]);
                int[] btnSymbolSet = new int[9];
                bool t = false;
                for (int i = 0; i < 10; i++)
                {
                    if (i == removal)
                    {
                        t = true;
                        continue;
                    }
                    if (!t)
                    {
                        btnSymbolSet[i] = reverseSet[i];
                    }
                    else
                    {
                        btnSymbolSet[i - 1] = reverseSet[i];
                    }
                }
                foreach (int i in btnSymbolSet)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (list[j] == i)
                        {
                            buttonPressOrder[Array.IndexOf(btnSymbolSet, i)] = j;
                            break;
                        }
                    }
                }
                Debug.LogFormat("[Complex Keypad #{0}] Button press order: 1:{1}({2}), 2:{3}({4}), 3:{5}({6}), 4:{7}({8}), 5:{9}({10}), 6:{11}({12}), 7:{13}({14}), 8:{15}({16}), 9:{17}({18})", _moduleId, buttonPressOrder[0], buttonStrings[buttonPressOrder[0]], buttonPressOrder[1], buttonStrings[buttonPressOrder[1]], buttonPressOrder[2], buttonStrings[buttonPressOrder[2]], buttonPressOrder[3], buttonStrings[buttonPressOrder[3]], buttonPressOrder[4], buttonStrings[buttonPressOrder[4]], buttonPressOrder[5], buttonStrings[buttonPressOrder[5]], buttonPressOrder[6], buttonStrings[buttonPressOrder[6]], buttonPressOrder[7], buttonStrings[buttonPressOrder[7]], buttonPressOrder[8], buttonStrings[buttonPressOrder[8]]);
                break;
            case 3:
                int removalX = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (!list.Contains(symbolSet[i]))
                    {
                        removalX = i;
                        break;
                    }
                }
                Debug.LogFormat("[Complex Keypad #{0}] Symbol removed from manual's list: {1}", _moduleId, symbols[symbolSet[removalX]]);
                int[] btnSymbolSetX = new int[9];
                bool tX = false;
                for (int i = 0; i < 10; i++)
                {
                    if (i == removalX)
                    {
                        tX = true;
                        continue;
                    }
                    if (!tX)
                    {
                        btnSymbolSetX[i] = symbolSet[i];
                    }
                    else
                    {
                        btnSymbolSetX[i - 1] = symbolSet[i];
                    }
                }
                foreach (int i in btnSymbolSetX)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (list[j] == i)
                        {
                            buttonPressOrder[Array.IndexOf(btnSymbolSetX, i)] = j;
                            break;
                        }
                    }
                }
                Debug.LogFormat("[Complex Keypad #{0}] Button press order: 1:{1}({2}), 2:{3}({4}), 3:{5}({6}), 4:{7}({8}), 5:{9}({10}), 6:{11}({12}), 7:{13}({14}), 8:{15}({16}), 9:{17}({18})", _moduleId, buttonPressOrder[0], buttonStrings[buttonPressOrder[0]], buttonPressOrder[1], buttonStrings[buttonPressOrder[1]], buttonPressOrder[2], buttonStrings[buttonPressOrder[2]], buttonPressOrder[3], buttonStrings[buttonPressOrder[3]], buttonPressOrder[4], buttonStrings[buttonPressOrder[4]], buttonPressOrder[5], buttonStrings[buttonPressOrder[5]], buttonPressOrder[6], buttonStrings[buttonPressOrder[6]], buttonPressOrder[7], buttonStrings[buttonPressOrder[7]], buttonPressOrder[8], buttonStrings[buttonPressOrder[8]]);
                break;
        }
    }
    void resetGame()
    {
        setupButtons();
        setupPressOrder();
        setupLEDS();
    }
    void handlePress(int btnIndex)
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, btn[btnIndex].transform);
        if (!_lightsOn || _isSolved) return;

        if(buttonPressOrder[pressIndex] == btnIndex)
        {
            pressIndex++;
            leds[btnIndex].material = green;
            Debug.LogFormat("[Complex Keypad #{0}] Correct button pressed. Input recieved: button at index {1}.",_moduleId, btnIndex);
        } else
        {
            module.HandleStrike();
            Debug.LogFormat("[Complex Keypad #{0}] Incorrect button pressed. Expected: {1}. Recieved: {2}.",_moduleId, buttonPressOrder[pressIndex], btnIndex);
            Debug.LogFormat("[Complex Keypad #{0}] If you feel that this strike is an error, please contact AAces as soon as possible so we can get this error sorted out. Have a copy of this log file handy. Discord: AAces#0908", _moduleId);
            pressIndex = 0;
            for (int i = 0; i < 9; i++)
            {
                leds[i].material = red;
            }
            StartCoroutine(RedLights());
            return;
        }
        if(pressIndex == 9)
        {
            module.HandlePass();
            _isSolved = true;
            Debug.LogFormat("[Complex Keypad #{0}] Module solved!",_moduleId);
            newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, btn[btnIndex].transform);
        }
    }
    IEnumerator RedLights()
    {
        for (int i = 0; i < 9; i++)
        {
            leds[i].material = red;
        }
        yield return new WaitForSeconds(2.0f);
        resetGame();
    }
#pragma warning disable 414
	private string TwitchHelpMessage = "Use !{0} press TL TM TR to press the top left button, the top middle button or the top right button. You can also use numbers, the keys are numbered in reading order starting from 1";
#pragma warning restore 414
	public KMSelectable[] ProcessTwitchCommand(string command)
	{
		if (!command.Trim().ToLowerInvariant().StartsWith("press")) return null;

		command = command.Substring(6);
		string[] parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		bool NoSpace = false;
		if (parts.Length == 1)
		{
			NoSpace = true;
		}
		List<int> ButtonsPressed = new List<int> { };
		List<KMSelectable> Buttons = new List<KMSelectable> { };
		if (Regex.IsMatch(command.ToUpperInvariant(), "((T|M|B)(L|M|R) )+"))
		{
			if (NoSpace) return null;
			foreach (string part in parts)
			{
				int num = 0;
				switch (part)
				{
					case "TL":
						num = 0;
						break;
					case "TM":
						num = 1;
						break;
					case "TR":
						num = 2;
						break;
					case "ML":
						num = 3;
						break;
					case "MM":
						num = 4;
						break;
					case "MR":
						num = 5;
						break;
					case "BL":
						num = 6;
						break;
					case "BM":
						num = 7;
						break;
					case "BR":
						num = 8;
						break;
				}
				if (ButtonsPressed.Contains(num)) continue;

				ButtonsPressed.Add(num);
				Buttons.Add(btn[num]);
			}

			return Buttons.ToArray();
		} else
		{
			foreach (Match buttonIndexString in Regex.Matches(command, "[1-9]"))
			{
				int buttonIndex;
				if (!int.TryParse(buttonIndexString.Value, out buttonIndex))
				{
					continue;
				}

				buttonIndex--;

				if (buttonIndex >= 0 && buttonIndex < 9)
				{
					if (ButtonsPressed.Contains(buttonIndex))
						continue;

					ButtonsPressed.Add(buttonIndex);
					Buttons.Add(btn[buttonIndex]);
				}
			}

			if (Buttons.Count == 0) return null;
			else return Buttons.ToArray();
		}
	}
}

public static class Extensions
{

    // Fisher-Yates Shuffle

    public static IList<T> Shuffle<T>(this IList<T> list, MonoRandom rnd)
    {

        int i = list.Count;

        while (i > 1)
        {

            int index = rnd.Next(i);

            i--;

            T value = list[index];

            list[index] = list[i];

            list[i] = value;

        }

        return list;

    }

}