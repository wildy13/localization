using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Numerics;

public class LocalizationManager : MonoBehaviour
{
    [Header("Important String")]
    private const string FILENAME_PREFIX = "text_";
    private const string FILE_EXTENSION = ".json";
    private string FULL_NAME_TEXT_FILE;
    private string path = "https://raw.githubusercontent.com/wildy13/LanguageJson/main/";
    private string FULL_PATH_TEXT_FILE;
    private string LANGUAGE_CHOOSE = "EN";
    private string LOADED_JSON_TEXT = "";

    [Header("Important Bool")]
    [HideInInspector]
    public bool _isReady = false;
    private bool _isFileFound = false;
    private bool _isTryChangeLangRunTime = false;

    [Header("Json Variable")]
    public Dictionary<string, string> _LocalizedDictionary;
    public LocalizationData _loadedData, _loadedLang;


    #region Instance Function
    private static LocalizationManager LocalizationManagerInstace;

    public static LocalizationManager Instance
    {
        get
        {
            if(LocalizationManagerInstace ==  null)
            {
                LocalizationManagerInstace = FindObjectOfType(typeof(LocalizationManager)) as LocalizationManager;
            }
            return LocalizationManagerInstace;
        }
    }
    #endregion Instance Function

    private const string LANGUAGE_PLAYER_PREFS_KEY = "selectedLanguage";

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        LoadSelectedLanguage();
    }

    private void LoadSelectedLanguage()
    {
        int lang = 0;
        string selectedLanguage = PlayerPrefs.GetString(LANGUAGE_PLAYER_PREFS_KEY, LocaleHelper.GetSupportLanguageCode());
        if (selectedLanguage == "EN")
        {
            lang = 0;
        }
        else if (selectedLanguage == "ES")
        {
            lang = 1 ;
        }
        else if (selectedLanguage == "ID")
        {
            lang = 2;
        }

        
        ChangeLanguage(lang);
    }

    private void SaveSelectedLanguage(string languageCode)
    {
        PlayerPrefs.SetString(LANGUAGE_PLAYER_PREFS_KEY, languageCode);
    }

    IEnumerator Start()
    {
        LANGUAGE_CHOOSE = LocaleHelper.GetSupportLanguageCode();
        FULL_NAME_TEXT_FILE = FILENAME_PREFIX + LANGUAGE_CHOOSE.ToLower() + FILE_EXTENSION;

#if UNITY_IOS || UNITY_ANDROID
            FULL_PATH_TEXT_FILE = Path.Combine(path, FULL_NAME_TEXT_FILE);
#else
        FULL_PATH_TEXT_FILE = Path.Combine(path, FULL_NAME_TEXT_FILE);
        
#endif
        yield return StartCoroutine(LoadJsonLanguageData(FULL_PATH_TEXT_FILE));

        _isReady = true;
    }


    IEnumerator LoadJsonLanguageData(string path)
    {
        CheckFileExist();

        yield return new WaitUntil(() => _isFileFound);
        _loadedData = JsonUtility.FromJson<LocalizationData>(LOADED_JSON_TEXT);
        _LocalizedDictionary = new Dictionary<string, string>();

        foreach (LocalalizationItems item in _loadedData.items)
        {
            try
            {
                _LocalizedDictionary.Add(item.key, item.value);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }


    private void CheckFileExist()
    {
            StartCoroutine(LoadFileContent());
    }


   IEnumerator LoadFileContent()
    {
        if (FULL_PATH_TEXT_FILE.Contains("://"))
        {
            UnityWebRequest www = UnityWebRequest.Get(FULL_PATH_TEXT_FILE);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                LOADED_JSON_TEXT = www.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Error loading JSON: " + www.error);
            }
        }
        else
        {
            if (File.Exists(FULL_PATH_TEXT_FILE))
            {
                LOADED_JSON_TEXT = File.ReadAllText(FULL_PATH_TEXT_FILE);
            }
            else
            {
                Debug.LogError("File not found: " + FULL_PATH_TEXT_FILE);
            }
        }
        _isFileFound = true;
    }


    private bool IsFileFinishCreate(FileInfo file)
    {
        FileStream stream = null;
        try
        {
            stream = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
            _isFileFound = true;
            Debug.Log("we succed to find  file");
            return true;
        }

        finally
        {
            if (stream != null)
            {
                stream.Close();
            }
        }
        _isFileFound = false;

            return false;
    }

    public string GetTextForKey(string localizationKey)
    {
        if (_LocalizedDictionary.ContainsKey(localizationKey))
        {
            return _LocalizedDictionary[localizationKey];

        }
        else
        {
            return "Error No Key matching with" + localizationKey;
        }
    }

    IEnumerator SwitchLanguageRunTime(int langChoose)
    {
        if (!_isTryChangeLangRunTime)
        {
            _isTryChangeLangRunTime = true;
            _isFileFound = false;
            _isReady = false;
            
            if(langChoose == 0)
            {
                LANGUAGE_CHOOSE = "EN";
            }
            else if(langChoose == 1)
            {
                LANGUAGE_CHOOSE = "ES";
            }else if(langChoose == 2)
            {
                LANGUAGE_CHOOSE = "ID";
            }

            FULL_NAME_TEXT_FILE = FILENAME_PREFIX + LANGUAGE_CHOOSE.ToLower() + FILE_EXTENSION;
#if UNITY_IOS || UNITY_ANDROID
            FULL_PATH_TEXT_FILE = Path.Combine(path, FULL_NAME_TEXT_FILE);
#else
            FULL_PATH_TEXT_FILE = Path.Combine(path, FULL_NAME_TEXT_FILE);
#endif
            yield return StartCoroutine(LoadJsonLanguageData(FULL_PATH_TEXT_FILE));
            _isReady = true;

            LocalizationText[] arrayText = FindObjectsOfType<LocalizationText>(); 
            for(int i = 0; i<arrayText.Length; i++)
            {
                arrayText[i].AttributionText();
            }
            SaveSelectedLanguage(LANGUAGE_CHOOSE);
            _isTryChangeLangRunTime = false;
        }

    }
    public void ChangeLanguage(int language)
    {
        StartCoroutine(SwitchLanguageRunTime(language));
    }

}
