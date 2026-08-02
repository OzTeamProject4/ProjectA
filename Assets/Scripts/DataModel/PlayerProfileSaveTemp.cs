using System;
using System.Globalization;
using UnityEngine;

public static class PlayerProfileSaveTemp
{
    private const string AccountCreatedAtKey = "PlayerProfile.AccountCreatedAt";
    private const string LastConnectAtKey = "PlayerProfile.LastConnectAt";
    private const string TotalLoginDaysKey = "PlayerProfile.TotalLoginDays";

    private const string DateTimeFormat = "o";
    private const int FirstLoginDays = 1;

    public static void LoadAndUpdate(DateTime now, out DateTime accountCreatedAt, out DateTime lastConnectAt, out int totalLoginDays)
    {
        if (!TryLoadDateTime(AccountCreatedAtKey, out accountCreatedAt))
        {
            accountCreatedAt = now;
            SaveDateTime(AccountCreatedAtKey, accountCreatedAt);
        }

        if (!TryLoadDateTime(LastConnectAtKey, out lastConnectAt))
        {
            lastConnectAt = now;
            totalLoginDays = FirstLoginDays;
        }
        else
        {
            totalLoginDays = PlayerPrefs.GetInt(TotalLoginDaysKey, FirstLoginDays);

            if (lastConnectAt.Date != now.Date)
            {
                totalLoginDays++;
            }
        }

        SaveDateTime(LastConnectAtKey, now);
        PlayerPrefs.SetInt(TotalLoginDaysKey, totalLoginDays);
        PlayerPrefs.Save();
    }

    private static bool TryLoadDateTime(string key, out DateTime value)
    {
        value = default;

        string savedText = PlayerPrefs.GetString(key, string.Empty);

        if (string.IsNullOrEmpty(savedText))
        {
            return false;
        }

        if (!DateTime.TryParse(savedText, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out value))
        {
            Debug.LogWarning($"[{nameof(PlayerProfileSaveTemp)}:{nameof(TryLoadDateTime)}] '{key}'의 저장 값을 해석하지 못했습니다. 값={savedText}");
            return false;
        }

        return true;
    }

    private static void SaveDateTime(string key, DateTime value)
    {
        PlayerPrefs.SetString(key, value.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
    }
}
