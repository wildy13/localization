using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocaleHelper
{
    public static string GetSupportLanguageCode()
    {
        SystemLanguage lang = Application.systemLanguage;
        switch(lang)
        {
            case SystemLanguage.English:
                return LocaleApplication.EN;
            case SystemLanguage.Indonesian:
                return LocaleApplication.ID;
            case SystemLanguage.Spanish:
                return LocaleApplication.ES;
            default:
                return GetDefaultSupportLanguageCode();
        }
    }

    static string GetDefaultSupportLanguageCode()
    {
        return LocaleApplication.EN;
    }
}
