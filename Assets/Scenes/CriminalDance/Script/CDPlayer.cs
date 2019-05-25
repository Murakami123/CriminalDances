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
        myCards = await bill.DrawRandomCard(drawCardCount);

        // 入手したカードを手元に移動
        for (int i = 0; i < myCards.Count; i++)
        {
            myCards[i].transform.SetParent(gameObject.transform, true);
            var cardRect = myCards[i].GetComponent<RectTransform>();
            cardRect.localScale = Vector3.one;
            cardRect.DOLocalMove(cardPos[i].localPosition, duraAddCard);
            await UniTask.Delay((int)duraAddCard * 1000);
        }
        return true;
    }

    public async UniTask<bool> ShowMeCards()
    {
        // それぞれ裏返して見えるようにする。
        for (int i = 0; i < myCards.Count; i++)
        {
            // Debug.Log("ShowMeCard");
            await myCards[i].ShowMeCard();
        }
        return true;
    }

    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    [SerializeField] RectTransform[] cardPos; // 0~3。カード1~4枚目のPosを指定。
    [SerializeField] float duraAddCard; // カード追加の際の移動時間。
    private List<CDHandCard> myCards = new List<CDHandCard>();

}
