using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Text;

public class LocalizationManager : MonoBehaviour
{
    [Header("Important String")]
    private int defaultLangId = 0;

    [Header("Important Bool")]
    [HideInInspector]
    public bool _isReady = false;
    private bool _isTryChangeLangRunTime = false;

    [Header("Json Variable")]
    public Dictionary<string, string> _LocalizedDictionary;
    public LocalizationData _loadedData;


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
        int selectedLanguage = PlayerPrefs.GetInt("langId");
        ChangeLanguage(selectedLanguage);
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(LoadJsonLanguageData(defaultLangId));

        _isReady = true;
    }

    IEnumerator LoadJsonLanguageData(int langId)
    {
        string url = "http://190.1.7.100:4005/api/lang/" + langId;

        UnityWebRequest request = new UnityWebRequest(url, "GET");

        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        string res = request.downloadHandler.text;
        _loadedData = JsonUtility.FromJson<LocalizationData>(res);
        _LocalizedDictionary = new Dictionary<string, string>();

        foreach (LocalizationItems item in _loadedData.items)
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

    IEnumerator SwitchLanguageRunTime(int langId)
    {
        if (!_isTryChangeLangRunTime)
        {
            _isTryChangeLangRunTime = true;
            _isReady = false;

            yield return StartCoroutine(LoadJsonLanguageData(langId));
            _isReady = true;

            LocalizationText[] arrayText = FindObjectsOfType<LocalizationText>(); 
            for(int i = 0; i<arrayText.Length; i++)
            {
                arrayText[i].AttributionText();
            }
            
            _isTryChangeLangRunTime = false;
        }

    }
    public void ChangeLanguage(int langId)
    {
        StartCoroutine(SwitchLanguageRunTime(langId));
    }

}
