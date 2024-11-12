using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;


internal class AddressableUtils : Editor
{
    const string assetBundlePath = "Assets/AddressableDatas";

    #region Menu Items
    [InitializeOnEnterPlayMode]
    [MenuItem("Tools/Addressable/Mapping")]
    internal static void Mapping()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (var group in settings.groups)
        {
            foreach (var entry in group.entries)
            {
                if (!entry.AssetPath.Contains("Assets") || entry.AssetPath.Contains("addressableMap")) continue;
                
                //addressableMap.json 경로
                string newPath = entry.AssetPath + "/addressableMap.json";

                //json 파일 생성
                string directory = Application.dataPath + entry.AssetPath.Replace("Assets", "");
                eAddressableType type = (eAddressableType)Enum.Parse(typeof(eAddressableType), group.Name);
                AddressableMapList mapData = MapEntryData(directory, type);
                string mapJsonData = JsonUtility.ToJson(mapData);
                File.WriteAllText(newPath, mapJsonData);

                //json 파일 추가
                AssetDatabase.ImportAsset(newPath);
            }
        }
    }

    [MenuItem("Tools/Addressable/Build")]
    internal static void BuildAddressable()
    {
        Mapping();
        
        //버전 맞추기 & 빌드하기
        AddressableAssetSettingsDefaultObject.Settings.OverridePlayerVersion = Application.version;
        AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);

        //lastBuildData 경로
        string buildDataPath = Application.dataPath + "/lastBuildData.txt";

        //lastBuildData 생성
        List<string> uploadList = result.FileRegistry.GetFilePaths()
            .Where(obj => obj.Contains("ServerData"))
            .ToList();
        string list = string.Join("\n", uploadList);

        //lastBuildData 기록
        File.WriteAllText(buildDataPath, list);
    }

    [MenuItem("Tools/Addressable/Upload")]
    public async static void UploadAddressable()
    {
        //업로드할 에셋 리스트
        string path = Application.dataPath + "/lastBuildData.txt";
        List<string> uploadList = File.ReadAllText(path).Split('\n').ToList();

        //buildTarget Window 고정. 
        string buildTarget = "StandaloneWindows"; 
        
        await DeleteFileinFolder(buildTarget);
        await CreateFolder(buildTarget);

        for (int i = 0; i < uploadList.Count; i++)
        {
            if (uploadList[i].Length > 0)
                await Upload(buildTarget, uploadList[i].Replace('\\', '/'));
        }
    }

    [MenuItem("Tools/Addressable/Build And Upload")]
    internal static void BuildAndUploadAddressable()
    {
        BuildAddressable();
        UploadAddressable();
    }
    #endregion

    /// <summary>
    /// addressableMap.json 데이터파일 생성
    /// </summary>
    /// <param name="dir">entry 경로</param>
    /// <param name="type">addressable group 이름</param>
    /// <returns></returns>
    static AddressableMapList MapEntryData(string dir, eAddressableType type)
    {
        AddressableMapList mapData = new AddressableMapList();

        //폴더 순회
        var dirs = Directory.GetDirectories(dir).ToList();
        foreach (var d in dirs)
        {
            var res = MapEntryData(d, type);
            mapData.AddRange(res.list);
        }

        //파일 순회
        List<string> files = Directory.GetFiles(dir)
            .Where(file => Path.GetExtension(file) != ".meta" && Path.GetExtension(file) != ".spriteatlasv2")
            .ToList();
        foreach (var file in files)
        {
            string extension = Path.GetExtension(file);
            string path = file.Replace(Application.dataPath, "Assets").Replace("\\", "/");
            
            //AddressableMap 생성
            AddressableMap data = new AddressableMap(); 
            data.assetType = extension switch
            {
                ".png" or ".jpg" or ".jpeg" => eAssetType.sprite,
                ".json" => eAssetType.json,
                ".prefab" => eAssetType.prefab,
                ".wav" or ".mp3" or ".ogg" => eAssetType.audio,
                ".ttf" or ".otf" => eAssetType.font,
                ".anim" or ".controller" => eAssetType.animation,
                _ => eAssetType.other,
            };
            data.addressableType = type;
            data.path = path;
            data.key = Path.GetFileNameWithoutExtension(path).ToLower();

            mapData.Add(data);
        }
        return mapData;
    }

    #region 서버 관련. (아직 해독 못해봄)
    public async static Task DeleteFileinFolder(string type)
    {
        var ftpUrl = new Uri("ftp://localhost/web/Addressable/");
        var request = WebRequest.Create(ftpUrl + type + "/" + Application.version) as FtpWebRequest;
        request.Credentials = new NetworkCredential("wocjf84", "password");
        request.Method = WebRequestMethods.Ftp.ListDirectory;
        request.KeepAlive = false;
        request.UseBinary = true;

        try
        {
            // 이제 디렉토리를 만든다고 FTP로 요청을 합니다.
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            var response = res.GetResponseStream();
            var reader = new StreamReader(response);
            var list = reader.ReadToEnd().Split('\n');
            reader.Close();
            response.Close();
            res.Dispose();
            for (int i = 0; i < list.Length; i++)
            {
                var request2 = WebRequest.Create(ftpUrl + type + "/" + list[i]) as FtpWebRequest;
                request2.Credentials = new NetworkCredential("wocjf84", "password");
                request2.Method = WebRequestMethods.Ftp.DeleteFile;
                request2.KeepAlive = false;
                request2.UseBinary = true;
                FtpWebResponse res2 = (FtpWebResponse)await request2.GetResponseAsync();
                res2.Dispose();
            }
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        //ICLogger.Log("DeleteFolder ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }

    public async static Task DeleteFolder(string type)
    {
        await DeleteFileinFolder(type);
        var ftpUrl = new Uri("ftp://localhost/web/Addressable/");
        var request = WebRequest.Create(ftpUrl + type + "/" + Application.version) as FtpWebRequest;

        request.Credentials = new NetworkCredential("wocjf84", "password");
        request.Method = WebRequestMethods.Ftp.RemoveDirectory;
        request.KeepAlive = false;
        request.UseBinary = true;

        try
        {
            // 이제 디렉토리를 만든다고 FTP로 요청을 합니다.
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            res.Dispose();
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        //ICLogger.Log("DeleteFolder ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }

    public async static Task CreateFolder(string type)
    {
        var ftpUrl = new Uri("ftp://localhost/web/Addressable/");
        var request = WebRequest.Create(ftpUrl + type + "/" + Application.version) as FtpWebRequest;

        request.Credentials = new NetworkCredential("wocjf84", "password");
        request.Method = WebRequestMethods.Ftp.MakeDirectory;
        request.KeepAlive = false;
        request.UseBinary = true;

        try
        {
            // 이제 디렉토리를 만든다고 FTP로 요청을 합니다.
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            res.Dispose();
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        //ICLogger.Log("CreateFolders ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }

    public async static Task Upload(string type, string path)
    {
        var target = File.ReadAllBytes(path);
        var ftpUrl = new Uri("ftp://localhost/web/Addressable/");
        var request = WebRequest.Create(ftpUrl + type + "/" + Application.version + "/" + path.Split('/').Last()) as FtpWebRequest;

        request.Credentials = new NetworkCredential("wocjf84", "password");
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.KeepAlive = false;
        request.UseBinary = true;
        request.ContentLength = target.Length;

        using (var ftpStream = request.GetRequestStream())
        {
            await ftpStream.WriteAsync(target, 0, target.Length);
            ftpStream.Close();
        }
        try
        {
            // 이제 디렉토리를 만든다고 FTP로 요청을 합니다.
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            res.Dispose();
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        //ICLogger.Log("CreateFolders ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }
    #endregion

}