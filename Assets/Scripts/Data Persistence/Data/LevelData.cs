[System.Serializable]
/// <summary>
///Formats the game data into a readable format for the save and load system.
/// </summary>
#pragma warning disable IDE1006 // Naming Styles
public class LevelData
{
    public string sceneName;

    public SerializableDictionary<string, float> outletCleanEnergy; //Saves all the outlets GUID's and clean energy values as a percent 0-1.0f
    public SerializableDictionary<string, float> outletVirusEnergy; //Saves all the outlets GUID's and virus energy values as a percent 0-1.0f
    public SerializableDictionary<string, float> outletMaxEnergy;   //Saves all the outlets GUID's and max energy values as a percent 0-1.0f

    //public SerializableDictionary<string, Vector3> platformPositions;   //Saves all the platform GUID's and platform positions into a dictionary.

    //implement future tile info here

    //implement future npc stuff here

    /**
     * When a new game is created, all values in the constructor should be read into the game
     * This will give the player the below starting values.
     */
    public LevelData()
    {
        outletCleanEnergy = new SerializableDictionary<string, float>();
        outletVirusEnergy = new SerializableDictionary<string, float>();
        outletMaxEnergy = new SerializableDictionary<string, float>();
    }
}
#pragma warning restore IDE1006 // Naming Styles
