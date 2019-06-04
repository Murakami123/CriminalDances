using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Async;
using DG.Tweening;

public class CDHandCard : UIMonoBehaviour
{
    //------------------------------------------------------------------
    // public
    //------------------------------------------------------------------

    /// 山札から、カード情報セット。
    public void SetCardData(CardData.CardType cardType)
    {
        thisCardType = cardType;
    }
    /// 配る際、持ち主情報セット。
    public void SetOwner(CDPlayer owner)
    {
        thisCardOwner = owner;
    }

    /// 自分の画面上で、このカードの表が見えるようにする。【0.2秒】
    public async UniRx.Async.UniTask ShowCardToHuman(bool isImmidiate = false)
    {
        // y=90にして見えなくする。
        var rect = cardImage.rectTransform;
        if (!isImmidiate)
        {
            rect.DOLocalRotate(canNotShowRotate, 0.1f);
            await UniTask.Delay(100);
        }

        // 画像差し替え
        cardImage.sprite = CriminalDanceSceneManager.Instance.spriteController.GetCardSprite(thisCardType);

        // y=0にして再び表示。
        if (!isImmidiate)
        {
            rect.DOLocalRotate(Vector3.zero, 0.1f);
            await UniTask.Delay(100);
        }
    }

    /// 自分の画面上で、このカードの裏返して見えなくする。
    public async UniRx.Async.UniTask TurnOverCard()
    {
        // y=90にして見えなくする。
        var rect = cardImage.rectTransform;
        rect.DOLocalRotate(canNotShowRotate, 0.1f);
        await UniTask.Delay(100);

        // 画像差し替え
        cardImage.sprite = CriminalDanceSceneManager.Instance.spriteController.GetCardBackSprite();

        // y=0にして再び表示。
        rect.DOLocalRotate(Vector3.zero, 0.1f);
        await UniTask.Delay(100);
    }

    public void OnTap_Card(bool isDisp)
    {
        Decision();
    }
    public void Decision()
    {
        if (canDiscardTiming)
        {
            if (canDiscardType)
            {
                thisCardOwner.SetDecisionCard(this);
                Debug.Log(" このカードを出すことにした。thisCardType: " + thisCardType);
            }
            else
            {
                Debug.Log("現在このタイプのカードだせない。");
            }
        }
        else
        {
            Debug.Log(" 今のタイミングでカードだせない。");
        }
    }

    private bool canDiscardType; // 現在出せるカードのタイプか否か。
    private bool canDiscardTiming; // 現在カードを出せるタイミングが否か。
    public void SetCanDiscardType(bool _canDiscardType)
    {
        canDiscardType = _canDiscardType;
        UpdateCardColor();
    }
    public void SetCanDiscardTiming(bool _canDiscardTiming)
    {
        canDiscardTiming = _canDiscardTiming;
        UpdateCardColor();
    }

    private Color greeColor = new Color(126f / 255f, 126f / 255f, 126f / 255f, 255f / 255f);
    private Color halfClearColor = new Color(1f, 1f, 1f, 146f / 255f);
    private void UpdateCardColor()
    {
        if (canDiscardTiming)
        {
            if (canDiscardType)
            {
                cardImage.color = Color.white;
            }
            else
            {
                cardImage.color = greeColor;
            }
        }
        else
        {
            cardImage.color = halfClearColor;
        }
    }

    public async UniTask<CDHandCard> Discard()
    {
        await ShowCardToHuman(isImmidiate: true);
        await CriminalDanceSceneManager.Instance.discardBill.DiscardCard(this);
        return this;
    }

    //------------------------------------------------------------------
    // ボタン押す考察。
    // public async UniTask<bool> Method()
    // {
    //     return true;
    // }
    // で呼び出し元から呼ばれたい。
    // しかしボタンは UI上ユーザが任意のタイミングで押せることが多いので、
    // このタイミングでのみ、ボタンが動作が作動するのがよさそう。
    // 
    // 今日考えた指針
    // 1, ボタンはボタンobjについてるクラスのメソッド（public void Hoge(){}）を実行する。
    // 2, そのクラスは、呼び出し元から呼ばれる、ボタン判定開始~押す完了を待つ
    //    public async UniTask<bool> Method(){} を持つ。 
    // 3, 呼び出し元クラス と ボタンクラス、相互に参照するのは仕方がない気がする。いつでも押せるボタンは大変。
    //    以上より下記のようなのが良い気がする。
    //
    //    (呼び出し元) 
    //    public async UniTask<bool> Method()
    //    {
    //        await HogeButton.SetCanTap( true );
    //        var resultData = await HogeButton.GetResultData();
    //        await HogeButton.SetCanTap( false );
    //        return true;
    //    }
    // 
    //    (ボタン側) 
    //    private bool canTap;
    //    public int resultData;
    //    public void OnTapButton(){
    //        if( !canTap ) return;
    //        resultData = hogehoge;
    //    }
    //    public async UniTask<bool> GetResultDataAndRefresh(){
    //        await Unitask.WaitUntil(()=> ( resultData != -1 ));
    //        int tmpData = resultData;
    //        resultData = -1;
    //        return tmpData;
    //    }
    // 
    //------------------------------------------------------------------

    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    private CardData.CardType thisCardType;
    public CardData.CardType CardType { get { return thisCardType; } }
    private CDPlayer thisCardOwner;
    // public CDPlayer CardOwner { get { return thisCardOwner; } }
    [SerializeField] Image cardImage;
    private readonly Vector3 canNotShowRotate = new Vector3(0f, 90f, 0f);


}
