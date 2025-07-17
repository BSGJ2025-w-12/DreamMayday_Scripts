using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

public class LangageChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetJapanese()
    {
        StartCoroutine(SetLocale("ja"));
    }

    public void SetEnglish()
    {
        StartCoroutine(SetLocale("en"));
    }

    private IEnumerator SetLocale(string code)
    {
        yield return LocalizationSettings.InitializationOperation; // 初期化待ち

        var selected = LocalizationSettings.AvailableLocales.Locales
            .Find(locale => locale.Identifier.Code == code);

        if (selected != null)
        {
            LocalizationSettings.SelectedLocale = selected;
        }
        else
        {
            Debug.LogWarning($"Locale {code} not found.");
        }
    }
}
