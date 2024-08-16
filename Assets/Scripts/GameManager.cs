using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] private NetworkManager networkManager;

    [SerializeField]private List<GameObject> PlayerList = new List<GameObject>();
    [SerializeField] private List<Transform> SpawnPositions = new List<Transform>();

    [SerializeField] private GameObject CountDownText;

    [SerializeField] private List<GameObject> ScoreList = new List<GameObject>();
    [SerializeField]private List<int> playerScores = new List<int>();
    [SerializeField] private TextMeshProUGUI Roundtext;
    private int round = 1;
    
    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CountDownText.SetActive(false);
    }

    public void UpdatePlayerList(GameObject Player)
    {
        PlayerList.Add(Player);
        
        if (PlayerList.Count >= 2)
        {
            //Start game
            StartCountdownRPC(4);
        }
    }


    [Rpc(SendTo.Server)]
    private void StartGameRPC()
    {
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerList[i].GetComponent<CharacterController>().enabled = false;
            PlayerList[i].transform.position = SpawnPositions[i].transform.position;
            PlayerList[i].GetComponent<CharacterController>().enabled = true;
            PlayerList[i].GetComponent<playerHealth>().CanTakeDamage = true;
            PlayerList[i].GetComponent<player>().CanMove = true;
            PlayerList[i].GetComponent<playerHealth>().resetHP();
        }
        
    }

    public void RoundEnd(int playerId)
    {
        if (IsServer)
        {
            int PlayerID = 0;
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if ((int)PlayerList[i].GetComponent<NetworkObject>().NetworkObjectId != playerId)
                {
                    PlayerID = i;
                }
            }
            UpdateScoreValuRPC(PlayerID);
            StartCountdownRPC(5);
        }
    }

    [Rpc(SendTo.Server)]
    private void UpdateScoreValuRPC(int playerId)
    {
        round++;
        playerScores[playerId]++;
        
       
        UpdateScoreTextRPC(playerId, round,playerScores[playerId]);

    }

    [Rpc(SendTo.Everyone)]
    private void UpdateScoreTextRPC(int PlayerID, int round,int score)
    {
        Roundtext.text = "Round " + round;
        ScoreList[PlayerID].GetComponent<TMP_Text>().text = score.ToString();
    }

    [Rpc(SendTo.Everyone)]
    private void StartCountdownRPC(float TimerValue)
    {
        foreach (GameObject player in PlayerList)
        {
            player.GetComponent<player>().CanMove = false;
        }
        StartCoroutine("GameStartCountdown", TimerValue);
    }
    private IEnumerator GameStartCountdown(float TimerValue)
    {
        
        CountDownText.SetActive(true);
        while (TimerValue > 0)
        {
            TimerValue--;
            CountDownText.GetComponent<TMP_Text>().text = TimerValue.ToString();
            yield return new WaitForSeconds(1);
        }
        CountDownText.SetActive(false);
        
        StartGameRPC();
    }

    
}
