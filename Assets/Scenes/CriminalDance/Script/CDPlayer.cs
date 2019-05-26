using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx.Async;

/// Player 一人当たりのカード所持位置を管理。
public class CDPlayer : MonoBehaviour
{
    //------------------------------------------------------------------
    // public
    //------------------------------------------------------------------
    public async UniTask<bool> DrawCard(CDBill bill, int drawCardCount)
    {
        // カード所持情報更新
        hundCards = await bill.DrawRandomCard(drawCardCount);

        // 入手したカードを手元に移動
        for (int i = 0; i < hundCards.Count; i++)
        {
            hundCards[i].SetOwner(this);
            hundCards[i].transform.SetParent(gameObject.transform, true);
            var cardRect = hundCards[i].GetComponent<RectTransform>();
            cardRect.localScale = Vector3.one;
            cardRect.DOLocalMove(cardPos[i].localPosition, duraAddCard);
            await UniTask.Delay((int)duraAddCard * 1000);
        }
        return true;
    }

    public async UniTask<bool> ShowMeCards()
    {
        // それぞれ裏返して見えるようにする。
        for (int i = 0; i < hundCards.Count; i++)
        {
            // Debug.Log("ShowMeCard");
            await hundCards[i].ShowCardToHuman();
        }
        return true;
    }


    /// 待機中を出す、消す（待機中はカードを出すことができない）
    public void DispWaiting(bool isDisp)
    {
        objWaiting.SetActive(isDisp);
    }

    // /// 演出の際など 一時的に「待機中...」を非表示。
    // public void TmpActivateDispWait(bool isDisp)
    // {
    //     // objWaiting.SetActive(isDisp);
    //     objWaiting.GetComponent<Text>().enabled = isDisp;
    // }

    // 引数のタイプのカードを持っているか
    public bool IsPosseCardByType(CardData.CardType type)
    {
        for (int i = 0; i < hundCards.Count; i++)
        {
            if (hundCards[i].CardType == type)
            {
                return true;
            }
        }
        return false;
    }

    // 引数のタイプのカードを持っていたら返す。
    public CDHandCard GetCardByType(CardData.CardType type)
    {
        if (IsPosseCardByType(type))
        {
            for (int i = 0; i < hundCards.Count; i++)
            {
                if (hundCards[i].CardType == type)
                {
                    return hundCards[i];
                }

            }
            Debug.LogError("ここにはこない");
            return null;
        }
        else
        {
            Debug.LogError("このタイプのカード持ってない。type:" + type);
            return null;
        }
    }

    /// 現在出すことのできるカードタイプ。
    // （第一発見者、犯人、探偵などは特定のタイミングでしか出せない）
    public void SetCanDiscardTypeCards(CardData.CardType[] canDiscardTypes)
    {
        for (int i = 0; i < canDiscardTypes.Length; i++)
        {
            for (int j = 0; j < hundCards.Count; j++)
            {
                var canDiscard = (canDiscardTypes[i] == hundCards[j].CardType);
                hundCards[j].SetCanDiscardType(canDiscard);
            }
        }
    }

    // 全種類のカードを出せるようにする。
    public void ResetCanDiscardTypeCards()
    {
        for (int j = 0; j < hundCards.Count; j++)
        {
            hundCards[j].SetCanDiscardType(true);
        }
    }

    // カードを出せるタイミングフラグ切り替え。
    public void SetCanDiscardTiming(bool canDiscardTiming)
    {
        for (int j = 0; j < hundCards.Count; j++)
        {
            hundCards[j].SetCanDiscardTiming(canDiscardTiming);
        }
    }

    /// プレイヤーがカードを選んで出す。（必要なら出せるタイプは先にセットしておく）
    public async UniTask<bool> Discard()
    {
        CDHandCard targetCard = null;
        if (isHuman)
        {
            Debug.Log("このプレイヤーは人、カード決定を待つ。");

            Debug.Log("仮で全種類のカード出せる状況");
            ResetCanDiscardTypeCards();

            SetCanDiscardTiming(true); // カードを選択できるタイミングである。
            await UniTask.WaitUntil(() => (lastDecisionCard != null)); // カード決定まで待ち。
            targetCard = lastDecisionCard;
            Debug.Log("lastDecisionCard.CardType：" + lastDecisionCard.CardType);
            SetCanDiscardTiming(false); // カードを選択できるタイミング終わり。
        }
        else
        {
            Debug.Log("このプレイヤーはNPC、適当にカードを選ぶ。");
            targetCard = hundCards[Random.Range(0, hundCards.Count)];
        }

        // 出したらそのカードは削除。
        await targetCard.Discard();
        lastDecisionCard = null; // ここで消すのはおかしい気がする仮。
        RemoveHundCardList(targetCard);
        return true;
    }

    private CDHandCard lastDecisionCard; // 最後に出したカード。
    public void SetLastEmitCard(CDHandCard card)
    {
        lastDecisionCard = card;
    }
    // public CDHandCard GetLastEmitCard()
    // {
    //     return lastDecisionCard;
    // }

    //------------------------------------------------------------------
    // public_カードの効果。
    //------------------------------------------------------------------
    /// 第一発見者_ゲーム開始時、強制で出さなきゃいけない。
    public async UniTask<CDHandCard> Discard_FirstDiscover()
    {
        var targetCard = GetCardByType(CardData.CardType.FirstDiscoverer);
        await targetCard.Discard();
        return targetCard;
    }


    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    [SerializeField] RectTransform[] cardPos; // 0~3。カード1~4枚目のPosを指定。
    [SerializeField] float duraAddCard; // カード追加の際の移動時間。
    private List<CDHandCard> hundCards = new List<CDHandCard>();
    [SerializeField] GameObject objWaiting;
    public bool isEmitCardDecision { get; private set; }
    public bool isHuman { get; private set; }
    public void SetIsHuman(bool _isHuman)
    {
        isHuman = _isHuman;
    }

    /// 後でprivateにする。
    public void RemoveHundCardList(CDHandCard hundCard)
    {
        if (hundCards.Contains(hundCard))
        {
            hundCards.Remove(hundCard);
        }
        else
        {
            Debug.LogError("このカードは所持カードリストにない。hundCard：" + hundCard + ", hundCard.CardType:" + hundCard.CardType);
        }
    }



}
