using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx.Async;
using System;

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

    private void UpdateCanDiscardType()
    {
        // 最初にリセット
        ResetCanDiscardTypeCards();

        // まず全種類のカードを出せる仮定
        var allCardTypeList = new List<CardData.CardType>();
        {
            var i = 0;
            foreach (var type in Enum.GetValues(typeof(CardData.CardType)))
            {
                allCardTypeList.Add((CardData.CardType)type);
                i++;
            }
        }
        Debug.Log("allCardTypeList.Count：" + allCardTypeList.Count);

        // 探偵_残り4枚以上で出せない。
        if (hundCards.Count >= 4)
        {
            for (int i = 0; i < allCardTypeList.Count; i++)
            {
                if (allCardTypeList[i] == CardData.CardType.Detective)
                {
                    allCardTypeList.Remove(allCardTypeList[i]);
                    Debug.Log("ああああ");
                }
            }
        }
        Debug.Log("allCardTypeList.Count：" + allCardTypeList.Count);

        // 犯人_残り2枚以上で出せない。
        if (hundCards.Count >= 2)
        {
            for (int i = 0; i < allCardTypeList.Count; i++)
            {
                if (allCardTypeList[i] == CardData.CardType.Criminal)
                {
                    allCardTypeList.Remove(allCardTypeList[i]);
                    Debug.Log("いいいい");
                }
            }
        }
        Debug.Log("allCardTypeList.Count：" + allCardTypeList.Count);

        // 他全部いつでも出せる
        // カード情報更新
        SetCanDiscardTypeCards(allCardTypeList.ToArray());
    }

    /// 現在出すことのできるカードタイプ。
    // （第一発見者、犯人、探偵などは特定のタイミングでしか出せない）
    private void SetCanDiscardTypeCards(CardData.CardType[] canDiscardTypes)
    {
        //  一旦falseにする
        for (int i = 0; i < hundCards.Count; i++)
        {
            hundCards[i].SetCanDiscardType(false);
        }

        // 該当カードのみ true にする
        for (int i = 0; i < canDiscardTypes.Length; i++)
        {
            for (int j = 0; j < hundCards.Count; j++)
            {
                var canDiscard = (canDiscardTypes[i] == hundCards[j].CardType);
                if (canDiscard)
                {
                    hundCards[j].SetCanDiscardType(canDiscard);
                }
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

        // 出すカードを確定させる
        // CDHandCard targetCard = null;
        if (isHuman)
        {
            Debug.Log("このプレイヤーは人、カード決定を待つ。");

            // 出せるカード出せないカード判定

            UpdateCanDiscardType();

            SetCanDiscardTiming(true); // カードを選択できるタイミングである。
            await UniTask.WaitUntil(() => (decisionCard != null)); // カード決定まで待ち。
            SetCanDiscardTiming(false); // カードを選択できるタイミング終わり。
        }
        else
        {
            Debug.Log("このプレイヤーはNPC、適当にカードを選ぶ。");
            decisionCard = hundCards[UnityEngine.Random.Range(0, hundCards.Count)];
        }

        // カード出す。
        var discardedCard = await decisionCard.Discard();
        switch (discardedCard.CardType)
        {
            // 犯人出した！私の勝ち！
            case CardData.CardType.Criminal:
                if (isHuman)
                {
                    CriminalDanceSceneManager.Instance.SetPlayerFinishType(PlayerFinishType.Win);
                }
                break;

            default: break;
        }

        RemoveHundCardList(decisionCard);
        decisionCard = null; // カード出し終えたので消す。
        return true;
    }

    private CDHandCard decisionCard; // 出すことを決めたカード。
    public void SetDecisionCard(CDHandCard card)
    {
        decisionCard = card;
    }



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
