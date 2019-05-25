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

    public void Initialize(int totalPlayerCount)
    {
        SetPlayerDates(totalPlayerCount);
    }

    // ゲーム開始時__プレイヤー全員がカードを引く。
    public async UniTask<bool> DrawAllPlayersAtGameStart(CDBill cdBill)
    {
        foreach (var player in playingPlayerTable)
        {
            await player.Value.DrawCard(cdBill, drawCardCount: 4);
        }
        return true;
    }

    ///  カード引いたのち__プレイヤーが人間のカードのみ見えるようにする。
    public async UniTask<bool> ShowPlayerCards()
    {
        foreach (var player in playingPlayerTable)
        {
            if (player.Value.isHuman)
            {
                await player.Value.ShowMeCards();
            }
        }
        return true;
    }

    // （デバッグ機能）カード引いたのち__全員分のカードを見せる
    public async UniTask<bool> DebugShowAllCards()
    {
        // 全員分カード見せる
        foreach (var player in playingPlayerTable)
        {
            await player.Value.ShowMeCards();
        }
        return true;
    }

    /// ゲームスタート__第一発見者が事件を見つける。
    public async UniTask<bool> Firstdiscovery()
    {
        Debug.Log("Firstdiscovery");

        // 対象は第一発見者。
        var targetType = CardData.CardType.FirstDiscoverer;

        // 対象者以外待機中。
        foreach (var player in playingPlayerTable.Values)
        {
            Debug.Log("aaaaaaaa");
            if (!player.IsPosseCardByType(targetType)) player.DispWaiting(true);
        }
        Debug.Log("bbbbb");

        // 対象者がカード決定するまで待ち。
        foreach (var player in playingPlayerTable.Values)
        {
            Debug.Log("cccc");
            if (player.IsPosseCardByType(targetType))
            {
                Debug.Log("ddddd");
                player.DispWaiting(false);
                await player.Discard_FirstDiscover();
                // await UniTask.WaitUntil(() => player.isEmitCardDecision);
                Debug.Log("ddddd");
            }
        }
        return true;
    }


    public async UniTask<bool> PlayNextTurn()
    {

        return true;
    }

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

    /// 演出の際など 一時的に、全ユーザの「待機中...」を非表示。
    public async UniTask<bool> TmpActivateDispWaitingAll(bool isDisp)
    {
        foreach (var player in playingPlayerTable.Values)
        {
            player.TmpActivateDispWait(isDisp);
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

    // 引数：参加人数
    // 処理：ゲーム参加プレイヤーテーブルを作成。
    private void SetPlayerDates(int totalPlayerCount)
    {
        // テーブル作成
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

        // NPC か否か雑にセット。
        var humanPlayrNo = 1;
        playingPlayerTable[humanPlayrNo].SetIsHuman(true);
    }

}
