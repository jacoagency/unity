using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class BackendManager : MonoBehaviour
{
    private const string BASE_URL = "http://localhost:3000"; // Update if your server is running on a different port or address

    // Get all players
    public void GetAllPlayers(System.Action<string> callback)
    {
        StartCoroutine(GetRequest($"{BASE_URL}/players", callback));
    }

    // Create a new player
    public void CreatePlayer(string name, int score, int level, int experience, int coins, System.Action<string> callback)
    {
        string jsonBody = JsonUtility.ToJson(new PlayerData { name = name, score = score, level = level, experience = experience, coins = coins });
        StartCoroutine(PostRequest($"{BASE_URL}/players", jsonBody, callback));
    }

    // Update player stats
    public void UpdatePlayerStats(string playerId, int score, int level, int experience, int coins, System.Action<string> callback)
    {
        string jsonBody = JsonUtility.ToJson(new PlayerData { score = score, level = level, experience = experience, coins = coins });
        StartCoroutine(PatchRequest($"{BASE_URL}/players/{playerId}", jsonBody, callback));
    }

    // Get top players
    public void GetTopPlayers(int limit, System.Action<string> callback)
    {
        StartCoroutine(GetRequest($"{BASE_URL}/players/top/{limit}", callback));
    }

    // Get player rank
    public void GetPlayerRank(string playerId, System.Action<string> callback)
    {
        StartCoroutine(GetRequest($"{BASE_URL}/players/{playerId}/rank", callback));
    }

    // Helper method for GET requests
    private IEnumerator GetRequest(string url, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                callback(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }

    // Helper method for POST requests
    private IEnumerator PostRequest(string url, string jsonBody, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                callback(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }

    // Helper method for PATCH requests
    private IEnumerator PatchRequest(string url, string jsonBody, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, jsonBody))
        {
            webRequest.method = "PATCH";
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                callback(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public string name;
    public int score;
    public int level;
    public int experience;
    public int coins;
}