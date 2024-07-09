using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public string platform;
    public static string persistentDataPath;
    public static bool internet;
    public DownloadManager downloadManager;
    public Button playButton;
    public Text playButtonText;
    public Text loadText;
    public Dropdown dropdown;
    public RectTransform progressBar;
    public Image background;
    public GameObject noInternet;

    void Start()
    {
        internet = IsInternetAvailable();
        persistentDataPath = Application.persistentDataPath;

        if (!DataSaver.CheckIfFileExists(Application.persistentDataPath + @"/downloading"))
        {
            DataSaver.CreateFolder(Application.persistentDataPath + @"/downloading");
        }

        List<string> options = new List<string>();

        if (!DataSaver.CheckIfFileExists(Application.persistentDataPath + @"/local"))
        {
            DataSaver.CreateFolder(Application.persistentDataPath + @"/local");
        }
        else
        {
            options = new List<string>(DataSaver.FileNamesInside(Application.persistentDataPath + @"/local"));
        }

        if (internet)
        {
            string latest = downloadManager.GetLatestVersion(platform);
            if(!options.Contains(latest)) options.Insert(0, latest);
        }
        else if(options.Count > 0)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.value = 0;
            dropdown.RefreshShownValue();

            UpdatePlayButton(0);

            if (internet)
            {
                downloadManager.DownloadLatestBackground();
                background.sprite = LoadNewSprite(persistentDataPath + @"/background.png");
            }
        }
        else
        {

        }
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



    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Sprite NewSprite;
        Texture2D SpriteTexture = LoadTexture(FilePath);
        NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);

        return NewSprite;
    }

    public Texture2D LoadTexture(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (System.IO.File.Exists(FilePath))
        {
            FileData = System.IO.File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2)
            {
                filterMode = FilterMode.Point
            };
            if (Tex2D.LoadImage(FileData))       
                return Tex2D;                 
        }
        return null;                    
    }


    public static bool IsInternetAvailable()
    {
        try
        {
            using (var ping = new System.Net.NetworkInformation.Ping())
            {
                var reply = ping.Send("google.com", 2000);
                return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
            }
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
