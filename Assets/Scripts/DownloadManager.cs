using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using UnityEngine;
using System.IO;

public class DownloadManager : MonoBehaviour
{

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void GetVersion(string versionID, string platform)
    {
     using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("a", "a");
                try
                {
                    wc.DownloadFile(@"https://raw.githubusercontent.com/LeagerStudios/LeagerLauncher/main/versions/" + versionID + "-" + platform + ".zip", Application.persistentDataPath + @"/downloading/" + versionID + ".zip");
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }
    }

    public void UnzipGame(string versionID)
    {
      string zipPath = Application.persistentDataPath + @"/downloading/" + versionID + ".zip";
      string extractPath = Application.persistentDataPath + @"/local/" + versionID;

      System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
	DataSaver.DeleteFile(zipPath);
    }

    public void OpenGame(string versionID)
    {
	string location = DataSaver.ReadTxt(Application.persistentDataPath  + @"/local/" + versionID + @"/exeLocation.txt")[0];
        System.Diagnostics.Process.Start(Application.persistentDataPath  + @"/local/" + versionID + @"/" + location, "Verified Launcher, dude did legal.. i think");

    }

}
