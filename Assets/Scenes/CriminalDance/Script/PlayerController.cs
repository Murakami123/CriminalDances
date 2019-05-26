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

    /// 【重要】
    /// 右回りの順序で、最後にカードを出したプレイヤー
    private CDPlayer lastEmitPlayer = new CDPlayer();

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

        // 全員待機中表示。
        await AllPlayerDispWaiting(true);

        // 対象者がカード決定するまで待ち。
        foreach (var player in playingPlayerTable.Values)
        {
            if (player.IsPosseCardByType(targetType))
            {
                player.DispWaiting(false);
                var targetCard = await player.Discard_FirstDiscover();
                lastEmitPlayer = player;

                Debug.Log("第一発見者を出し、所持カードから削除する暫定対応");
                player.RemoveHundCardList(targetCard);
            }
        }
        return true;
    }


    public async UniTask<bool> PlayNextTurn()
    {
        var nextPlayer = GetNextPlayer();
        await nextPlayer.Discard();
        lastEmitPlayer = nextPlayer;
        return true;
    }

    public CDPlayer GetNextPlayer()
    {
        // 最後に出した人のプレイヤーNo。
        int lastEmitPlayerNo = 0;
        foreach (var player in playingPlayerTable)
        {
            if (player.Value == lastEmitPlayer)
            {
                lastEmitPlayerNo = player.Key;
            }
        }

        // 次に出す人のプレイヤーNo。
        int nextPlayerNo = lastEmitPlayerNo + 1;
        for (int i = 0; i < 8 /* ← 最大プレイ人数8のため */; i++)
        {
            // 右周りの次（playerNo + 1）の人が存在してたら、その人が次の人。
            if (playingPlayerTable.ContainsKey(nextPlayerNo))
            {
                var nextPlayer = playingPlayerTable[nextPlayerNo];
                return nextPlayer;
            }
            else
            {
                Debug.Log("あがり済みか、存在しない nextPlayerNo:" + nextPlayerNo);
                nextPlayerNo += 1;

                // ifExistEmitPlayerNo が 8 を超えたら、またプレイヤー 1 から再チェック。
                if (nextPlayerNo > 8) nextPlayerNo -= 8;
            }
        }

        // あああああ
        Debug.LogError("ここに来るはずないはず");
        return null;
    }

    /// 全員「待機中...」を表示。
    public async UniTask<bool> AllPlayerDispWaiting(bool isDisp)
    {
        foreach (var player in playingPlayerTable.Values)
        {
            player.DispWaiting(isDisp);
        }
        return true;
    }

    // /// 演出の際など 一時的に、全ユーザの「待機中...」を非表示。
    // public async UniTask<bool> TmpActivateDispWaitingAll(bool isDisp)
    // {
    //     foreach (var player in playingPlayerTable.Values)
    //     {
    //         player.TmpActivateDispWait(isDisp);
    //     }
    //     return true;
    // }


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
