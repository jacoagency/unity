using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class MDB_PlayerData : MonoBehaviour
{
	//===============================================================
	public static MDB_PlayerData Instance;
	//===============================================================

	// same data structure as in local player data (top10 scoreboard is handled in other script)
	[HideInInspector] public static string MDB_WalletAddress;
	[HideInInspector] public static string MDB_GamerTag;

	[HideInInspector] public static int MDB_LastScore;
	[HideInInspector] public static int MDB_HighScore;

	[HideInInspector] public static int MDB_TimesPlayed;
	[HideInInspector] public static string MDB_LastPlayedDateString;
	[HideInInspector] public static string MDB_LastPlayedTxidString;

	//===============================================================
	private float _fakeResponseTime = 0.5f;

	//===============================================================
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	// si no existe el usuario con la WalletAddress que paga para jugar, es porque es necesario crear un nuevo usuario en mongoDB
	public void SetupDefaultNewDB()
	{
		MDB_WalletAddress = "aSolWalletAddress";
		MDB_GamerTag = "gamerTag";

		MDB_LastScore = 0;
		MDB_HighScore = 0;

		MDB_TimesPlayed = 0;
		MDB_LastPlayedDateString = "date";
		MDB_LastPlayedTxidString = "txid";
	}

	// base de datos falsa para prueba
	private void SetupFakeDB()
	{
		MDB_WalletAddress = "aSolWalletAddress";
		MDB_GamerTag = "Billy";

		MDB_LastScore = 13;
		MDB_HighScore = 34;

		MDB_TimesPlayed = 4;
		MDB_LastPlayedDateString = "date";
		MDB_LastPlayedTxidString = "txid";
	}



	//=============================================================== PUSH / SETTERS

	// push new username (GamerTag) se sobreescribe cada vez que se abre el link del juego
	public IEnumerator MDB_PUSH_username()
	{
		yield return new WaitForSeconds(_fakeResponseTime);
		MDB_GamerTag = LocalPlayerData.GamerTag;
	}

	// cada vez que se paga se registra
	public IEnumerator MDB_PUSH_registerLastPlayed()
	{
		yield return new WaitForSeconds(_fakeResponseTime);
		MDB_TimesPlayed = LocalPlayerData.TimesPlayed;
		MDB_LastPlayedDateString = LocalPlayerData.LastPlayedDateString;
		MDB_LastPlayedTxidString = LocalPlayerData.LastPlayedTxidString;
	}

	// lo utiliza el LocalPlayerData cada que el usuario pierde 1 vida
	public IEnumerator MDB_PUSH_LastScore()
	{
		yield return new WaitForSeconds(_fakeResponseTime);
		MDB_LastScore = LocalPlayerData.LastScore;
	}

	// lo utiliza el LocalPlayerData cada que el usuario pierde 1 vida (solamente si hay un nuevo highscore)
	public IEnumerator MDB_PUSH_HighScore()
	{
		yield return new WaitForSeconds(_fakeResponseTime);
		MDB_HighScore = LocalPlayerData.HighScore;
	}


	// estas funciones son falsas! simplemente regresan el valor que esta escrito en este mismo script de GamerTag, LastScore... etc ....
	//=============================================================== GET
	public static string MDB_GET_Gamertag()
	{
		return MDB_GamerTag;
	}

	public static int MDB_GET_LastScore()
	{
		return MDB_LastScore;
	}

	public static int MDB_GET_HighScore()
	{
		return MDB_HighScore;
	}

	public static int MDB_GET_TimesPlayed()
	{
		return MDB_TimesPlayed; 
	}

	public static string MDB_GET_LastPlayedDate()
	{
		return MDB_LastPlayedDateString;
	}

	public static string MDB_GET_LastPlayedTxid()
	{
		return MDB_LastPlayedTxidString;
	}
}

public class MDB_Fake_manager
{
	private static readonly HttpClient client = new HttpClient();
	private const string API_BASE_URL = "http://localhost:3000";

	public static async Task<bool> SavePlayerData(LocalPlayerData playerData)
	{
		try
		{
			var json = JsonConvert.SerializeObject(playerData);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await client.PostAsync($"{API_BASE_URL}/players", content);
			return response.IsSuccessStatusCode;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error saving player data: {ex.Message}");
			return false;
		}
	}

	public static async Task<LocalPlayerData> LoadPlayerData(string playerId)
	{
		try
		{
			var response = await client.GetAsync($"{API_BASE_URL}/players/{playerId}");
			if (response.IsSuccessStatusCode)
			{
				var json = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<LocalPlayerData>(json);
			}
			return null;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error loading player data: {ex.Message}");
			return null;
		}
	}

	// Implement other methods (UpdatePlayerData, DeletePlayerData) similarly
}
