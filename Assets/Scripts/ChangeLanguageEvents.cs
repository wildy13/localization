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
        StartCoroutine(LoadLanguageOptions());
    }

    private IEnumerator LoadLanguageOptions()
    {
        string url = "http://190.1.7.100:4005/api/lang/";

        UnityWebRequest request = new UnityWebRequest(url, "GET");

        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        string res = request.downloadHandler.text;
        LocalizationData _loadedData = JsonUtility.FromJson<LocalizationData>(res);
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (LocalizationItems item in _loadedData.items)
        {
            options.Add(new TMP_Dropdown.OptionData(item.value));
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(options);

        langId = PlayerPrefs.GetInt("langId");
        if (langId == -1)
        {
            langId = 0;
        }
        dropdown.value = langId;
    }

    public void DropdownValueChanged()
    {
        langId = dropdown.value;
        LocalizationManager.Instance.ChangeLanguage(langId);
        PlayerPrefs.SetInt("langId", langId);
    }
}
