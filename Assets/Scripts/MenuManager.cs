using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public string platform;
    public static string persistentDataPath;
    public DownloadManager downloadManager;
    public Button playButton;
    public Text playButtonText;
    public Text loadText;
    public Dropdown dropdown;
    public RectTransform progressBar;

    void Start()
    {
        persistentDataPath = Application.persistentDataPath;

        if (!DataSaver.CheckIfFileExists(Application.persistentDataPath + @"/downloading"))
        {
            DataSaver.CreateFolder(Application.persistentDataPath + @"/downloading");
        }

        if (!DataSaver.CheckIfFileExists(Application.persistentDataPath + @"/local"))
        {
            DataSaver.CreateFolder(Application.persistentDataPath + @"/local");
        }


        UpdatePlayButton(dropdown.value);
    }


    public void UpdatePlayButton(int dropdownValue)
    {
        if (DataSaver.CheckIfFileExists(Application.persistentDataPath + @"/local/" + dropdown.options[dropdownValue].text))
        {
            playButtonText.text = "Play";
        }
        else
        {
            playButtonText.text = "Download";
        }
    }

    public void Play()
    {
        string version = dropdown.options[dropdown.value].text;
        playButton.interactable = false;
        dropdown.interactable = false;
        if (DataSaver.CheckIfFileExists(Application.persistentDataPath + @"/local/" + version))
        {
            downloadManager.OpenGame(version);
        }
        else
        {
            StartCoroutine(DownloadVersion(version));
        }
    }

    public IEnumerator DownloadVersion(string version)
    {
        DownloadManager.downloadCompleted = false;
        DownloadManager.installCompleted = false;
        loadText.text = "Downloading...";
        playButtonText.text = "Downloading";
        downloadManager.GetVersion(version, platform);
        while (!DownloadManager.downloadCompleted)
        {
            if (DownloadManager.error != "")
            {
                break;
            }
            else
            {
                progressBar.sizeDelta = new Vector2(DownloadManager.downloadProgress * 800f, 20f);
            }
            yield return new WaitForEndOfFrame();
        }

        if (DownloadManager.error != "")
        {
            loadText.text = DownloadManager.error;
            DownloadManager.error = "";
        }
        else
        {
            loadText.text = "Installing...";
            downloadManager.UnzipGame(version);

            while (!DownloadManager.installCompleted)
            {
                if (DownloadManager.downloadProgress < 0.9f)
                    DownloadManager.downloadProgress += 0.01f;

                progressBar.sizeDelta = new Vector2(DownloadManager.downloadProgress * 800f, 20f);
                yield return new WaitForSeconds(0.1f);
            }
            progressBar.sizeDelta = new Vector2(800f, 20f);

            loadText.text = "Installation complete";
        }

        UpdatePlayButton(dropdown.value);
        playButton.interactable = true;
        dropdown.interactable = true;
    }





}
