using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * A Data persistence class that should be attached to any objects with data that should be saved.
 */
public interface IDataPersistence
{
    void LoadPlayerData(PlayerData playerData);
    void LoadLevelData(LevelData levelData);
    void SaveData(ref PlayerData playerData, ref LevelData levelData);
}
