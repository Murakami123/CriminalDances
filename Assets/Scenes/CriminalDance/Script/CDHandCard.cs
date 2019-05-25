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

    /// カード情報セット。
    public void SetCardData(CardData.CardType cardType){
        thisCardType = cardType;
    }

    public void OnTap_DispCard( bool isDisp ){
        if( isDisp ){
            ShowMeCard().Forget();
        } else {
            TurnOverCard().Forget();
        }
    }

    /// 自分の画面上で、このカードの表が見えるようにする。【0.2秒】
    public async UniRx.Async.UniTask ShowMeCard(){
        // y=90にして見えなくする。
        var rect = cardImage.rectTransform;
        rect.DOLocalRotate(canNotShowRotate, 0.1f);
        await UniTask.Delay(100);

        // 画像差し替え
        cardImage.sprite = CriminalDanceSceneManager.Instance.spriteController.GetCardSprite( thisCardType );

        // y=0にして再び表示。
        rect.DOLocalRotate(Vector3.zero, 0.1f);
        await UniTask.Delay(100);
    }

    /// 自分の画面上で、このカードの裏返して見えなくする。
    public async UniRx.Async.UniTask TurnOverCard(){
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

    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    private CardData.CardType thisCardType;
    [SerializeField] Image cardImage;
    private readonly Vector3 canNotShowRotate = new Vector3( 0f, 90f, 0f);


}
