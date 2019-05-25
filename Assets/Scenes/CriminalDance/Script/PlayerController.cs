using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;

public class PlayerController : MonoBehaviour
{
    //------------------------------------------------------------------
    // public
    //------------------------------------------------------------------

    /// 【重要】
    /// ゲームを遊んでいるプレイヤー一覧。
    /// 試合中プレイヤーのあがりが予想されるため、Listでなく プレイヤーNoをDicで保存。
    /// Dictionary <n, プレイヤー(n)>。n=2 だったら プレイヤー2 を示す。
    private Dictionary<int, CDPlayer> playingPlayerTable = new Dictionary<int, CDPlayer>();

    // 引数：参加人数
    // 処理：ゲーム参加プレイヤーテーブルを作成。
    public void Initialize(int totalPlayerCount)
    {
        CDPlayer[] targetArray = null;
        switch (totalPlayerCount)
        {
            case 3: targetArray = playersAt3Match; break;
            case 4: targetArray = playersAt4Match; break;
            case 5: targetArray = playersAt5Match; break;
            case 6: targetArray = playersAt6Match; break;
            case 7: targetArray = playersAt7Match; break;
            case 8: targetArray = playersAt8Match; break;
            default: Debug.LogError("targetArray:" + targetArray); break; ;
        }
        for (int i = 0; i < targetArray.Length; i++) playingPlayerTable.Add(i + 1, targetArray[i]);

    }

    // プレイヤー全員が、ゲーム開始時にカードを引く。
    public async UniTask<bool> DrawAllPlayersAtGameStart(CDBill cdBill)
    {
        foreach (var player in playingPlayerTable)
        {
            await player.Value.DrawCard(cdBill, drawCardCount: 4);
        }
        return true;
    }

    // // プレイ中の全員を返す。
    // public CDPlayer[] GetPlayers(int totalPlayerCount)
    // {
    //     switch (totalPlayerCount)
    //     {
    //         case 3: return playersAt3Match;
    //         case 4: return playersAt4Match;
    //         case 5: return playersAt5Match;
    //         case 6: return playersAt6Match;
    //         case 7: return playersAt7Match;
    //         case 8: return playersAt8Match;
    //         default:
    //             Debug.LogError("totalPlayerCount:" + totalPlayerCount, this);
    //             return playersAt3Match;
    //     }
    // }


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

    public async UniTask<bool> PlayNextTurn()
    {

        return true;
    }



    // private int cardEmitNo;
    // private CDPlayer lastEmitPlayer;

    public CDPlayer GetNextPlayer(int lastEmitPlayerNo)
    {
        CDPlayer nextPlayer = null;
        int ifExistEmitPlayerNo; // もしこの番号の人が上がってなかったらこの人が出す。

        ifExistEmitPlayerNo = lastEmitPlayerNo;
        for (int i = 0; i < 8 /* ← 最大プレイ人数8のため */; i++)
        {
            ifExistEmitPlayerNo = +1;

            // ifExistEmitPlayerNo が 8 を超えたら、またプレイヤー 1 から再チェック。
            if (ifExistEmitPlayerNo > 8)
            {
                ifExistEmitPlayerNo = -8;
            }

            // 右周りの次（playerNo + 1）の人が存在してたら、その人が次の人。
            if (playingPlayerTable.ContainsKey(ifExistEmitPlayerNo))
            {
                nextPlayer = playingPlayerTable[ifExistEmitPlayerNo];
                return nextPlayer;
            }
        }

        Debug.LogError("ifExistEmitPlayerNo:" + ifExistEmitPlayerNo + ", nextPlayer== null: " + nextPlayer == null);
        return null;
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
