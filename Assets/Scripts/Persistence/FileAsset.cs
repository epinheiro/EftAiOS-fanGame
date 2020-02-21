using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class FileAsset
{
    // Highlevel class based configuration
    static public readonly string dataPath = "Assets/Data/";
    static public readonly string mapsDataPath = "Assets/Data/Maps/";
    static protected readonly string fileFormat = "json";


    //////////// File functions ////////////
    /// <summary>
    /// Method that gets the Dialog data file in the Assets/Data/ folder
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    static protected string GetMapFile(string fileName) {
        try{
            return GetTextFile(string.Format("{0}{1}.{2}", mapsDataPath, fileName, fileFormat));
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
        try {
            TextAsset textFile = (TextAsset) AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
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

}
