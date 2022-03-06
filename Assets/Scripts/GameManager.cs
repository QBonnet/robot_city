using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private const string m_playerIdPrefix = "Player";
    private static Dictionary<string, Player> m_players = new Dictionary<string, Player>();
    public MatchSettings m_matchSettings;

    public static GameManager m_instance;

    public delegate void OnPlayerKilledCallback(string player, string source);
    public OnPlayerKilledCallback m_onPlayerKilledCallback;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            return;
        }
        Debug.LogError("gamemanager > 1");
    }

    public static void RegisterPlayer(string netID, Player player)
    {
        string playerId = m_playerIdPrefix + netID;
        m_players.Add(playerId, player);
        player.transform.name = playerId;
    }

    public static void UnregisterPlayer(string playerId)
    {
        m_players.Remove(playerId);
    }

    public static Player GetPlayer(string playerId)
    {
        return m_players[playerId];
    }

    public static Player[] GetAllPlayers()
    {
        return m_players.Values.ToArray();
    }
}
