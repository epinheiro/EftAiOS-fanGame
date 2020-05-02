using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FileAsset
{
    // Highlevel class based configuration
    static public readonly string resourcesMapsDataPath = "Data/Maps/";
    static public readonly string resourcesMaterialsPath = "Materials/SoundParticle/";
    static public readonly string manualFileLastIPPath = "Data/";
    static public string universalSaveName = "save.dat";


    //////////// File functions ////////////
    /// <summary>
    /// Method that gets the Dialog data file in the Assets/Data/ folder
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    static protected string GetMapFile(string fileName) {
        try{
            return GetTextFile(string.Format("{0}{1}", resourcesMapsDataPath, fileName));
        }catch(Exception e){
            throw e;
        }
    }

    /// <summary>
    /// Get an Text File reference, based on the relative path / of the project
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static protected string GetTextFile(string path) {
        // https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadAssetAtPath.html
        // https://docs.unity3d.com/ScriptReference/Resources.Load.html
        try {
            TextAsset textFile = (TextAsset) Resources.Load<TextAsset>(path);
            return textFile.ToString();
        } catch {
            throw new System.IO.FileNotFoundException(string.Format("File not found in {0}", path));
        }
    }

    //////////// Read parsed file functions ////////////
    static public string GetMapTileInfo(string fileName){
        string mapContent = GetMapFile(fileName);
        return mapContent;
    }

    //////////// Get Resources
    static public Material GetMaterialOfSoundParticleByColorName(string colorName){
        return (Material) Resources.Load<Material>(string.Format("{0}SoundParticle{1}", resourcesMaterialsPath, colorName));
    }

    //////////// Generic file for Game Data
    [System.Serializable]
    public struct GameData{
        public string lastIP; // Already used IP address
    }

    static public void SaveGameData(string lastIPUsed){
        string destination = Path.Combine(manualFileLastIPPath, universalSaveName);

        // Get or create FILE
        FileStream file;
        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else {
            Directory.CreateDirectory(manualFileLastIPPath);
            file = File.Create(destination);
        }

        // Get DATA to FILE
        GameData data = new GameData{
            lastIP = lastIPUsed
        };

        // Format BINARY FILE (SAVE)
        BinaryFormatter binForm = new BinaryFormatter();
        binForm.Serialize(file, data);
        file.Close();
        TimeLogger.Log("Save last ip ({0}) file in {1}", lastIPUsed, destination);
    }

    static public GameData LoadGameData(){
        string destination = Path.Combine(manualFileLastIPPath, universalSaveName);

        // Get FILE or throw exception
        FileStream file;
        if (File.Exists(destination)) {
            file = File.OpenRead(destination);
        } else {
            throw new System.Exception(string.Format("File at {0} not found", destination));
        }

        // Get data from BINARY FILE (LOAD)
        BinaryFormatter binForm = new BinaryFormatter();
        GameData data = (GameData) binForm.Deserialize(file);
        file.Close();
        TimeLogger.Log("Loaded file from {0}", destination);

        return data;
    }

}
