using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using UnityEngine;
using System.IO;
using System.IO.Compression;

public class DownloadManager : MonoBehaviour
{
    public static float downloadProgress = 0f;
    public static bool downloadCompleted = false;
    public static bool installCompleted = false;
    public static string error = "";

    public void GetVersion(string versionID, string platform)
    {
        using (WebClient webClient = new WebClient())
        {
            string url = @"https://raw.githubusercontent.com/LeagerStudios/LeagerLauncher/main/versions/" + versionID + "-" + platform + ".zip";
            webClient.Headers.Add("a", "a");
            try
            {
                if (!RemoteFileExists(url))
                    throw new Exception("Version is not Available for download");
                webClient.DownloadFileAsync(new Uri(url), MenuManager.persistentDataPath + @"/downloading/" + versionID + ".zip");
                webClient.DownloadProgressChanged += (s, e) =>
                {
                    downloadProgress = e.ProgressPercentage / 200f;
                };
                webClient.DownloadFileCompleted += (s, e) =>
                {
                    downloadCompleted = true;
                };
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }
    }

    public string GetLatestVersion(string platform)
    {
        using (WebClient webClient = new WebClient())
        {
            string url = @"https://raw.githubusercontent.com/LeagerStudios/LeagerLauncher/main/lastGameVersion.txt";
            webClient.Headers.Add("a", "a");
            try
            {
                string ver = webClient.DownloadString(url);
                return ver;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }
    }

    public void UnzipGame(string versionID)
    {
        string zipPath = MenuManager.persistentDataPath + @"/downloading/" + versionID + ".zip";
        string extractPath = MenuManager.persistentDataPath + @"/local/" + versionID;

        ThreadStart unziperStart = new ThreadStart(() => ExtractToDirectoryAsync(zipPath, extractPath));
        Thread unzipper = new Thread(unziperStart);
        unzipper.Start();
    }

    public void OpenGame(string versionID)
    {
        string location = DataSaver.ReadTxt(MenuManager.persistentDataPath + @"/local/" + versionID + @"/exeLocation.txt")[0];
        System.Diagnostics.Process.Start(MenuManager.persistentDataPath + @"/local/" + versionID + @"/" + location, "Verified Launcher, dude did legal.. i think");
        Application.Quit();
    }

    public void DownloadLatestBackground()
    {
        using (WebClient webClient = new WebClient())
        {
            string url = @"https://raw.githubusercontent.com/LeagerStudios/LeagerLauncher/main/background.png";
            webClient.Headers.Add("a", "a");
            try
            {
                webClient.DownloadFile(new Uri(url), MenuManager.persistentDataPath + @"/background.png");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }

    public static void ExtractToDirectoryAsync(string zipPath, string extractPath)
    {
        ZipFile.ExtractToDirectory(zipPath, extractPath);
        downloadProgress = 0.95f;
        DataSaver.DeleteFile(zipPath);
        downloadProgress = 1f;
        installCompleted = true;
    }

    public bool RemoteFileExists(string url)
    {
        try
        {
         
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        
            request.Method = "HEAD";
       
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
         
            response.Close();
            return (response.StatusCode == HttpStatusCode.OK);
        }
        catch
        {
            return false;
        }
    }
}
