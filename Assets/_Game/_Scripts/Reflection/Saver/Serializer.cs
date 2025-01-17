using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public abstract class SaveLoadBase
{
    //Folder name under which you want to save file for e.g. You want to save avatar data
    //Only add folder name not actual directory path.
    public string m_DirPath;
    //Actual file onto which data is save
    //Don't add extension like .json
    public string m_FileName;
}


public static class Serializer
{
    #region Save Data as Binary
    //Save Data
    public static void SaveBinaryData<T>(T classToSave) where T : SaveLoadBase
    {
        //Create the binary formatter
        BinaryFormatter formatter = new BinaryFormatter();
        //Directory path
        string dirPath = Path.Combine(Application.persistentDataPath, classToSave.m_DirPath);
        Debug.Log($"Directory path is {dirPath}");
        //Actucal file name
        string streamPath = Path.Combine(dirPath, classToSave.m_FileName) + ".binary";
        Debug.Log($"Final path is {streamPath}");
        if (!Directory.Exists(dirPath))
        {
            Debug.Log($"Creating {typeof(T).Name} data path as .binary");
            Directory.CreateDirectory(dirPath);
        }
        //Create the stream to write data onto
        FileStream stream = new FileStream(streamPath, FileMode.Create);
        //Get the data
        T data = classToSave;
        //Write the data
        formatter.Serialize(stream, data);
        //Close the stream
        stream.Close();
        Debug.Log($"{typeof(T).Name} saved successfully as .binary");
    }
    //Load Data
    public static T LoadBinaryData<T>(T classToLoad) where T : SaveLoadBase
    {
        //Directory path
        string streamPath = Path.Combine(Path.Combine(Application.persistentDataPath, classToLoad.m_DirPath), classToLoad.m_FileName) + ".binary";
        if (File.Exists(streamPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(streamPath, FileMode.Open);
            SaveLoadBase classData = formatter.Deserialize(stream) as SaveLoadBase;
            stream.Close();
            Debug.Log($"{typeof(T).Name} loaded successfully as .binary");
            return classData as T;
        }
        else
        {
            Debug.Log($"No {typeof(T).Name} Save file found as .binary");
            return null;
        }
    }
    #endregion
    #region Save Data as Json
    public static void SaveJsonData<T>(T classToSave, bool logFileName) where T : SaveLoadBase
    {
        string dirPath = Path.Combine(Application.persistentDataPath, classToSave.m_DirPath);
        if(logFileName)
            Debug.Log($"Directory path is {dirPath}");
        string jsonData = JsonUtility.ToJson(classToSave, true);
        if (!Directory.Exists(dirPath))
        {
            if(logFileName)
             Debug.Log($"Creating {typeof(T).Name} data path  as .json");
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllText(dirPath + "/" + classToSave.m_FileName + ".json", jsonData);
        if (logFileName)
        {
            Debug.Log($"Final path is {dirPath}/classToSave.m_FileName.json");
            Debug.Log($"{typeof(T).Name} saved successfully as .json");
        }
    }
    public static T LoadJsonData<T>(T classToLoad) where T : SaveLoadBase
    {
        string dirPath = Path.Combine(Application.persistentDataPath, classToLoad.m_DirPath);
        string streamPath = Path.Combine(dirPath, classToLoad.m_FileName + ".json");
        if (File.Exists(streamPath))
        {
            string jsonData = File.ReadAllText(dirPath + "/" + classToLoad.m_FileName + ".json");
            SaveLoadBase classData = JsonUtility.FromJson<T>(jsonData);
            return classData as T;
        }
        else
        {
            Debug.Log($"No  {typeof(T).Name} Save file found as .json");
            return null;
        }
    }
    public static void DeleteFile<T>(T classToLoad) where T : SaveLoadBase
    {
        string dirPath = Path.Combine(Application.persistentDataPath, classToLoad.m_DirPath);
        string streamPath = Path.Combine(dirPath, classToLoad.m_FileName + ".json");
        if (File.Exists(streamPath))
        {
            File.Delete(streamPath);
            Debug.Log($"File deleted: {streamPath}");
        }
        else
        {
            Debug.Log($"File not found: {streamPath}");
        }
    }
    #endregion

    public static void OpenFolder<T>(T classToLoad) where T: SaveLoadBase
    {
        
    string dirPath = Path.Combine(Application.persistentDataPath, classToLoad.m_DirPath);
    if (System.IO.Directory.Exists(dirPath))
    {
        Application.OpenURL("file://" + dirPath);
    }
    else
    {
        Debug.LogError("Folder does not exist: " + dirPath);
    }


    }
}