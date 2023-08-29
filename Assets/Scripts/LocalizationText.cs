using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class LocalizationText : MonoBehaviour
{

    [HideInInspector]public string _localizationKey;

    TextMeshProUGUI _textMeshProComponent;

    IEnumerator Start()
    {
        _localizationKey = gameObject.name;
        while (!LocalizationManager.Instance._isReady)
        {
            yield return null;
        }
        AttributionText();
        
    }

    public void AttributionText()
    {
        if(_textMeshProComponent == null)
        {
            _textMeshProComponent = gameObject.GetComponent<TextMeshProUGUI>();
        }
        try
        {
            _textMeshProComponent.text = LocalizationManager.Instance.GetTextForKey(_localizationKey);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}
