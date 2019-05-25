using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;

public class PlayerController : MonoBehaviour
{
    //------------------------------------------------------------------
    // public
    //------------------------------------------------------------------
    public void Initialize(int totalPlayerCount)
    {

    }

    public CDPlayer[] GetPlayers(int totalPlayerCount)
    {
        switch (totalPlayerCount)
        {
            case 3: return playersAt3Match;
            case 4: return playersAt4Match;
            case 5: return playersAt5Match;
            case 6: return playersAt6Match;
            case 7: return playersAt7Match;
            case 8: return playersAt8Match;
            default:
                Debug.LogError("totalPlayerCount:" + totalPlayerCount, this);
                return playersAt3Match;
        }
    }

    public async UniTask<bool> DebugShowAllCards(int totalPlayerCount)
    {
        var players = new List<CDPlayer>();
        switch (totalPlayerCount)
        {
            case 3: players.AddRange(playersAt3Match); break;
            case 4: players.AddRange(playersAt4Match); break;
            case 5: players.AddRange(playersAt5Match); break;
            case 6: players.AddRange(playersAt6Match); break;
            case 7: players.AddRange(playersAt7Match); break;
            case 8: players.AddRange(playersAt8Match); break;
        }
        players.AddRange(playersAt3Match);

        // 一人ずつ裏返して見えるようにする。
        for (int i = 0; i < players.Count; i++)
        {
            await players[i].ShowMeCards();
        }

        return true;
    }

    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    [SerializeField] CDPlayer[] playersAt3Match; // 3人対戦の時のそれぞれのplayer
    [SerializeField] CDPlayer[] playersAt4Match; // 4人対戦の時
    [SerializeField] CDPlayer[] playersAt5Match; // 5人対戦の時
    [SerializeField] CDPlayer[] playersAt6Match; // 6人対戦の時
    [SerializeField] CDPlayer[] playersAt7Match; // 7人対戦の時
    [SerializeField] CDPlayer[] playersAt8Match; // 8人対戦の時



}
