using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class FileAsset
{
    // Highlevel class based configuration
    static public readonly string mapsDataPath = "Data/Maps/";
    static public readonly string materialsPath = "Materials/SoundParticle/";

    //////////// File functions ////////////
    /// <summary>
    /// Method that gets the Dialog data file in the Assets/Data/ folder
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    static protected string GetMapFile(string fileName) {
        try{
            return GetTextFile(string.Format("{0}{1}", mapsDataPath, fileName));
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
        return (Material) Resources.Load<Material>(string.Format("{0}SoundParticle{1}", materialsPath, colorName));
    }

}
