using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * A Data persistence class that should be attached to any objects with data that should be saved.
 */
public interface IDataPersistence
{
    void LoadData(GameData data);
    void SaveData(ref GameData data);
}
