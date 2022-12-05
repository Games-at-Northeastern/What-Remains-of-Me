using System;
using System.Collections;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WriteToDatabase : MonoBehaviour
{
    public bool QuitAfterPost = false;

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

    private IEnumerator PostDataRoutineStart()
    {
        for (int i = 0; i < _databaseIntsToWrite.Length; i++)
        {
            DatabaseInt dbInt = _databaseIntsToWrite[i];
            yield return StartCoroutine(PostDataRoutine(dbInt.DataName,
                dbInt.DataDescription, dbInt.GetValue(), false));
        }
        for (int i = 0; i < _databaseIntsToWrite.Length; i++)
        {
            DatabaseFloat dbFloat = _databaseFloatsToWrite[i];
            yield return StartCoroutine(PostDataRoutine(dbFloat.DataName,
                dbFloat.DataDescription, dbFloat.GetValue(), true));
        }
    }

    private void PostData()
    {
        foreach (DatabaseInt dbInt in _databaseIntsToWrite)
        {
            StartCoroutine(PostDataRoutine(dbInt.DataName,
                dbInt.DataDescription, dbInt.GetValue(), false));
        }
        foreach (DatabaseFloat dbFloat in _databaseFloatsToWrite)
        {
            StartCoroutine(PostDataRoutine(dbFloat.DataName,
                dbFloat.DataDescription, dbFloat.GetValue(), true));
        }
    }

    public void PostDataAndQuit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            QuitAfterPost = true;
            StartCoroutine(PostDataRoutineStart());
        }
    }
    public void PostDataAndContinue()
    {
        QuitAfterPost = false;
        PostData();
    }

    /// <summary>
    /// Routine to post logged data to online database
    /// </summary>
    private IEnumerator PostDataRoutine(string dataName, string dataDescription, string value, bool quitAfter)
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

        if (QuitAfterPost && quitAfter)
        {
            Application.Quit();
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
