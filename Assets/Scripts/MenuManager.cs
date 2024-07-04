using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public DownloadManager downloadManager;
    // Start is called before the first frame update
    void Start()
    {
        downloadManager.GetVersion("00.0.9", "Win");
	downloadManager.UnzipGame("00.0.9");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
