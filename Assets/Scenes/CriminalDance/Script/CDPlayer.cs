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
            await hundCards[i].ShowMeCard();
        }
        return true;
    }


    /// 待機中を出す、消す（待機中はカードを出すことができない）
    public void DispWaiting(bool isDisp)
    {
        objWaiting.SetActive(isDisp);

        if (isDisp)
        {

        }
        else
        {

        }
    }

    /// 演出の際など 一時的に「待機中...」を非表示。
    public void TmpActivateDispWait(bool isDisp)
    {
        // objWaiting.SetActive(isDisp);
        objWaiting.GetComponent<Text>().enabled = isDisp;
    }

    /// カードを出す。
    public async UniTask<bool> Discard_FirstDiscover()
    {
        var targetCard = GetCardByType(CardData.CardType.FirstDiscoverer);
        await targetCard.Discard();
        return true;
    }

    public void OnTap_Card()
    {

    }

    public void EmitCard()
    {

    }

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




}
