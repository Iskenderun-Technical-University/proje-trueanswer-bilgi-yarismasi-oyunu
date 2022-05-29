using System;
using UnityEngine;
using System.Collections.Generic;

public class GameMultiLang : MonoBehaviour
{
    public static GameMultiLang Instance;

    public static Dictionary<String, String> Fields;

    //[SerializeField] string defaultLang = "en";


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
		LoadLanguage();
    }

    private void LoadLanguage()
    {
        if (Fields == null)
            Fields = new Dictionary<string, string>();

        Fields.Clear();
		
        //string lang = PlayerPrefs.GetString("_language", defaultLang);
        //Debug.Log("kang: " + lang);
        string lang = PlayerPrefs.GetString(GameRecords.SaveDefaultLanguage);
		if(lang =="" ||lang == null)
		{
            PlayerPrefs.SetString(GameRecords.SaveDefaultLanguage,"en");
            lang = "en";
        }

        //if (PlayerPrefs.GetInt("_language_index", -1) == -1)
           // PlayerPrefs.SetInt("_language_index", 0);

        //string allTexts = (Resources.Load(@"Languages/" + lang) as TextAsset).text; //without (.txt)
        string allTexts = (Resources.Load(@"Languages/" + lang) as TextAsset).text; //without (.txt)

        string[] lines = allTexts.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        string key, value;

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].IndexOf("=") >= 0 && !lines[i].StartsWith("#"))
            {
                key = lines[i].Substring(0, lines[i].IndexOf("="));
                value = lines[i].Substring(lines[i].IndexOf("=") + 1,
                    lines[i].Length - lines[i].IndexOf("=") - 1).Replace("\\n", Environment.NewLine);
                Fields.Add(key, value);
            }
        }
    }

    public static string GetTraduction(string key)
    {
        if (!Fields.ContainsKey(key))
        {
            Debug.LogError("There is no key with name: [" + key + "] in your text files");
            return null;
        }

        return Fields[key];
    }


}
