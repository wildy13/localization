using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class ChangeLanguageEvents : MonoBehaviour
{
    public LocalizationManager localizationManager;
    TMP_Dropdown dropdown;
    int id_language_Choose;

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

            foreach (LocalalizationItems item in _loadedLang.items)
            {
                options.Add(new TMP_Dropdown.OptionData(item.value));
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(options);

            // Set the value of the dropdown based on PlayerPrefs or system language
            id_language_Choose = PlayerPrefs.GetInt(Application.productName + "id_language_Choose", -1);
            if (id_language_Choose == -1)
            {
                string systemLanguageCode = LocaleHelper.GetSupportLanguageCode();
                id_language_Choose = GetLanguageIndex(systemLanguageCode);
            }
            dropdown.value = id_language_Choose;
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
        id_language_Choose = dropdown.value;
        Debug.Log(id_language_Choose);
        LocalizationManager.Instance.ChangeLanguage(id_language_Choose);
        PlayerPrefs.SetInt(Application.productName + "id_language_Choose", id_language_Choose);
    }
}
