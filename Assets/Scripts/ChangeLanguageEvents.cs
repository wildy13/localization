using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class ChangeLanguageEvents : MonoBehaviour
{
    public LocalizationManager localizationManager;
    TMP_Dropdown dropdown;
    int langId;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        LoadLanguageOptions();
    }

    private void LoadLanguageOptions()
    {
        string path = "https://raw.githubusercontent.com/wildy13/LanguageJson/main/language.json";
        StartCoroutine(LoadLanguageOptionsCoroutine(path));
    }

    private IEnumerator LoadLanguageOptionsCoroutine(string path)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string LOADED_JSON_LANG = www.downloadHandler.text;
            LocalizationData _loadedLang = JsonUtility.FromJson<LocalizationData>(LOADED_JSON_LANG);

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (LocalizationItems item in _loadedLang.items)
            {
                options.Add(new TMP_Dropdown.OptionData(item.value));
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(options);

            // Set the value of the dropdown based on PlayerPrefs or system language
            langId = PlayerPrefs.GetInt("langId", -1);
            if (langId == -1)
            {
                string systemLanguageCode = LocaleHelper.GetSupportLanguageCode();
                langId = GetLanguageIndex(systemLanguageCode);
            }
            dropdown.value = langId;
        }
        else
        {
            Debug.LogError("Error loading JSON: " + www.error);
        }
    }

    private int GetLanguageIndex(string languageCode)
    {
        List<TMP_Dropdown.OptionData> options = dropdown.options;
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].text.Equals(languageCode))
            {
                return i;
            }
        }
        return 0; 
    }

    public void DropdownValueChanged()
    {
        langId = dropdown.value;
        LocalizationManager.Instance.ChangeLanguage(langId);
        PlayerPrefs.SetInt("langId", langId);
    }
}
