using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using DG.Tweening;

/// 山札
public class CDBill : MonoBehaviour
{

    //------------------------------------------------------------------
    // public
    //------------------------------------------------------------------
    private Dictionary<int, CardData.CardType> billCardTable = new Dictionary<int, CardData.CardType>();

    /// 人・NPC 合計から、山札（プレイに使うカード）が決まる。
    public void SetBillData(int playerTotalCount)
    {
        var allCardTable = new CDAllCardData().AllCards;
        switch (playerTotalCount)
        {
            case 3:  // 3人対戦
                Move_AllCardToBill(allCardTable, CardData.CardType.FirstDiscoverer, 1); // 第一発見者_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Criminal, 1); // 犯人_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Detective, 1); // 探偵_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Alibi, 1); // アリバイ_1枚
                Move_AllCardToBill_Random(allCardTable, 8); // さらにランダム8枚
                Debug.Log("カード情報セット完了。billCardTable.Count:" + billCardTable.Count);
                break;

            case 4:  // 4人対戦
                Move_AllCardToBill(allCardTable, CardData.CardType.FirstDiscoverer, 1); // 第一発見者_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Criminal, 1); // 犯人_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Detective, 1); // 探偵_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Alibi, 1); // アリバイ_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Planning, 1); // たくらみ_1枚
                Move_AllCardToBill_Random(allCardTable, 11); // さらにランダム11枚
                break;

            case 5:  // 5人対戦
                Move_AllCardToBill(allCardTable, CardData.CardType.FirstDiscoverer, 1); // 第一発見者_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Criminal, 1); // 犯人_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Detective, 1); // 探偵_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Alibi, 2); // アリバイ_2枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Planning, 1); // たくらみ_1枚
                Move_AllCardToBill_Random(allCardTable, 14); // さらにランダム14枚
                break;

            case 6:  // 6人対戦
                Move_AllCardToBill(allCardTable, CardData.CardType.FirstDiscoverer, 1); // 第一発見者_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Criminal, 1); // 犯人_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Detective, 2); // 探偵_2枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Alibi, 2); // アリバイ_2枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Planning, 2); // たくらみ_2枚
                Move_AllCardToBill_Random(allCardTable, 16); // さらにランダム16枚
                break;

            case 7:  // 7人対戦
                Move_AllCardToBill(allCardTable, CardData.CardType.FirstDiscoverer, 1); // 第一発見者_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Criminal, 1); // 犯人_1枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Detective, 2); // 探偵_2枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Alibi, 3); // アリバイ_3枚
                Move_AllCardToBill(allCardTable, CardData.CardType.Planning, 2); // たくらみ_2枚
                Move_AllCardToBill_Random(allCardTable, 19); // さらにランダム19枚
                break;

            case 8:  // 8人対戦
                // 全カード使う。
                billCardTable = allCardTable;
                allCardTable.Clear();
                break;

            default:
                Debug.LogError("参加人数がおかしい。playerTotalCount:" + playerTotalCount);
                break;
        }

        foreach (var card in billCardTable)
        {
            Debug.Log("山札。key:" + card.Key + ", value:" + card.Value);
        }
    }

    [SerializeField] CDHandCard handCardPrefab;
    public async UniTask<List<CDHandCard>> DrawRandomCard(int drawCardCount)
    {

        // カード情報を基に、objの生成を行う。
        var getCardDatas = DrawRandomCardData(drawCount: drawCardCount);
        var getCards = new List<CDHandCard>();
        for (int i = 0; i < getCardDatas.Length; i++)
        {
            var cardObj = Instantiate(handCardPrefab, handCardPrefab.transform.parent);
            cardObj.gameObject.SetActive(true);
            cardObj.SetCardData(getCardDatas[i]);
            getCards.Add(cardObj);
            await UniTask.DelayFrame(1);
        }

        // 山札自体なくなったら非アクティブ。
        DeactivateIfTableEmpty();

        // 生成したカードのリストを返す。カードのドロー、UIの移動はPlayerが行う。
        return getCards;
    }

    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    /// 「全カード」から「山札」に、指定タイプのカードを入れる。
    private void Move_AllCardToBill(Dictionary<int, CardData.CardType> allTable, CardData.CardType type, int moveCount)
    {
        // 引数の枚数だけ 山札に移動。
        for (int i = 0; i < moveCount; i++)
        {
            var cardNo = 0;
            foreach (var card in allTable)
            {
                if (card.Value == type)
                {
                    billCardTable.Add(card.Key, card.Value); // 当該カードを、山札に増やす。
                    cardNo = card.Key;
                    break;
                }
            }
            allTable.Remove(cardNo); // 当該カードを、全カードから消す。
        }
    }

    /// 「全カード」から「山札」に、ランダムなカードを入れる。
    private void Move_AllCardToBill_Random(Dictionary<int, CardData.CardType> allTable, int moveCount)
    {
        // 引数の枚数だけ 山札に移動。
        for (int i = 0; i < moveCount; i++)
        {
            var targetNo = Random.Range(0, allTable.Count); // 0 ~ 31
            var cardNo = 0;
            var j = 0;
            foreach (var card in allTable)
            {
                if (targetNo == j)
                {
                    cardNo = card.Key; // ≠ targetNo。allTable の targetNo 番目のカードの key。
                    billCardTable.Add(card.Key, card.Value); // 当該カードを、山札に増やす。
                    break;
                }
                j++;
            }
            allTable.Remove(cardNo); // 当該カードを、全カードから消す。
        }
    }

    /// 山札からカードを引く(山札のカードごとの key(int) も返せるけど不要のはず)
    private CardData.CardType[] DrawRandomCardData(int drawCount)
    {
        // 引数の枚数だけ引く。
        var getCards = new List<CardData.CardType>();
        for (int i = 0; i < drawCount; i++)
        {
            var targetNo = Random.Range(0, billCardTable.Count);
            var cardNo = 0;
            var j = 0;
            foreach (var card in billCardTable)
            {
                if (targetNo == j)
                {
                    cardNo = card.Key;
                    getCards.Add(billCardTable[card.Key]);
                    break;
                }
                j++;
            }
            billCardTable.Remove(cardNo); // 当該カードを山札から消す。
        }
        return getCards.ToArray();
    }

    // 山札の枚数が0になったら、obj自体非表示にする。
    private void DeactivateIfTableEmpty()
    {
        if (billCardTable.Count <= 0)
        {
            gameObject.SetActive(false);
        }
    }

}
