using System;
using System.Collections;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class WriteToDatabase : MonoBehaviour
{
    /// <summary>
    /// URL at which the .php file interfacing with the SQL database is stored.
    /// </summary>
    private const string DataStorageURL =
        "https://patternlanguageforgamedesign.com/WRoM/wrom_data_collection.php";

    [SerializeField] private StringVariable _playerID;
    [SerializeField] private StringVariable _versionNum;
    [SerializeField] private DatabaseInt[] _databaseIntsToWrite;
    [SerializeField] private DatabaseFloat[] _databaseFloatsToWrite;
    [SerializeField] private DataBaseRecord[] _databaseStringsToWrite;

    public void PostData()
    {
        foreach (DatabaseInt dbInt in _databaseIntsToWrite)
        {
            StartCoroutine(PostDataRoutine(dbInt.DataName,
                dbInt.DataDescription, dbInt.GetValue()));
        }
        foreach (DatabaseFloat dbFloat in _databaseFloatsToWrite)
        {
            StartCoroutine(PostDataRoutine(dbFloat.DataName,
                dbFloat.DataDescription, dbFloat.GetValue()));
        }
    }

    /// <summary>
    /// Routine to post logged data to online database
    /// </summary>
    private IEnumerator PostDataRoutine(string dataName, string dataDescription, string value)
    {
        WWWForm form = new WWWForm();
        form.AddField("post_data", "true");
        form.AddField("timestamp", DateTime.Now.ToString());
        form.AddField("user_id", _playerID.Value);
        form.AddField("user_ip_address", GetLocalIPv4());
        form.AddField("version_num", _versionNum.Value);
        form.AddField("scene_name", SceneManager.GetActiveScene().name);
        form.AddField("data_name", dataName);
        form.AddField("data_description", dataDescription);
        form.AddField("value", value);

        using (UnityWebRequest www = UnityWebRequest.Post(DataStorageURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("ConnectionError");
                Debug.Log(www.error);
            }
            else if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("ProtocolError");
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Successfully posted score!");
            }
        }
    }

    private string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
    }
}
