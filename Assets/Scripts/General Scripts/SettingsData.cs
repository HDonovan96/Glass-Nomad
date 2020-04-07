using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// SettingsData handles the updating of settings to the file path that is
/// passed through parameters.
/// </summary>
[Serializable]
public class SettingsData
{
    private AudioMixer audioMixer;

    public FullScreenMode fullscreen = FullScreenMode.Windowed;
    public int[] resolution = new int[2] { Screen.width, Screen.height };
    public int quality = QualitySettings.GetQualityLevel();
    public float fieldOfView = Camera.main.fieldOfView;
    public float volume = 1f;

    public void UpdateSettings(FullScreenMode fs, int[] res, int qual, float fov)
    {
        Screen.SetResolution(res[0], res[1], fs);
        QualitySettings.SetQualityLevel(qual);
        Camera.main.fieldOfView = fov;
    }
    public SettingsData(AudioMixer mixer)
    {
        audioMixer = mixer;
        if (audioMixer.GetFloat("volume", out float vol))
        {
            volume = vol;
        }
    }
}

/// <summary>
/// SaveLoadSettings holds an instance of the SettingsData class. It is static so it can be
/// accessed anywhere without a reference.
/// </summary>
public static class SaveLoadSettings
{
    private static SettingsData settings;

    public static void SaveData(string filePath, AudioMixer mixer)
    {
        Debug.Log("Saving settings...");
        settings = new SettingsData(mixer);
        string jsonData = JsonUtility.ToJson(settings, true);
        File.WriteAllText(filePath, jsonData);
    }

    public static void LoadData(string filePath)
    {
        Debug.Log("Loading settings...");
        try
        {
            settings = JsonUtility.FromJson<SettingsData>(File.ReadAllText(filePath));
            settings.UpdateSettings(settings.fullscreen, settings.resolution, settings.quality, settings.fieldOfView);
            Debug.Log("Settings loaded.");
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("File doesn't currently exist.");
        }
    }
}
