using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/**
 * Reads relevant save data from .json files and then formats them into the type given through TDataTypeOnUnformat.
 */
public class FileDataHandler<TDataTypeOnUnformat>
{
    private string dataDirPath = string.Empty; //The path to the directory the file handler will read from.
    private string dataFileName = string.Empty; //The files name.


    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    /// <summary>
    /// Loads game data from the .json file at the path dataDirPath, and the file name dataFileName
    /// </summary>
    /// <returns>The loaded data as the PlayerData type, returns null if no data was loaded</returns>
    public TDataTypeOnUnformat Load()
    {
        //Use Path.Combine to get the full file path.
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        //Player data to be returned
        TDataTypeOnUnformat loadedPlayerData = default(TDataTypeOnUnformat);

        //temp variable to store the text inside Json file
        String dataToLoad = "";
        if (File.Exists(fullPath))
        {
            try
            {
                //try to load data from filepath
                //Read file to file system. Putting this in a using block to ensure it closes when finished
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                //Convert string to PlayerData Type
                loadedPlayerData = JsonConvert.DeserializeObject<TDataTypeOnUnformat>(dataToLoad);

            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when loading data from: " + fullPath + "\nError name: " + e);
            }

        }

        return loadedPlayerData;
    }

    /// <summary>
    /// Saves the game data given by 'data' to the .json file at the path dataDirPath, and the file name dataFileName
    /// </summary>
    /// <returns>The loaded data as the PlayerData type</returns>
    public void Save(TDataTypeOnUnformat data)
    {
        //Use Path.Combine to get the full file path.
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            //Create file directory if it doesnt already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //Serialize playerData into Json
            string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented);


            //Write file to file system. Putting this in a using block to ensure it closes when finished
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                    Debug.Log("Wrote Save data to: " + fullPath);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when saving data to: " + fullPath + "\nError name: " + e);
        }

    }
}
