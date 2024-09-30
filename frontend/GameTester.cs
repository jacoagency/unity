using UnityEngine;
using UnityEngine.UI;

public class GameTester : MonoBehaviour
{
    private BackendManager backendManager;
    public Text outputText;

    void Start()
    {
        backendManager = GetComponent<BackendManager>();
    }

    public void CreatePlayer()
    {
        backendManager.CreatePlayer("TestPlayer", 0, 1, 0, 100, (response) =>
        {
            Debug.Log("Player created: " + response);
            outputText.text = "Player created: " + response;
        });
    }

    public void GetAllPlayers()
    {
        backendManager.GetAllPlayers((response) =>
        {
            Debug.Log("All players: " + response);
            outputText.text = "All players: " + response;
        });
    }

    public void UpdatePlayerStats()
    {
        // Replace "playerId" with an actual player ID from your database
        string playerId = "replace_with_actual_player_id";
        backendManager.UpdatePlayerStats(playerId, 100, 2, 50, 200, (response) =>
        {
            Debug.Log("Player updated: " + response);
            outputText.text = "Player updated: " + response;
        });
    }

    public void GetTopPlayers()
    {
        backendManager.GetTopPlayers(5, (response) =>
        {
            Debug.Log("Top players: " + response);
            outputText.text = "Top players: " + response;
        });
    }
}