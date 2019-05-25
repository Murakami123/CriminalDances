using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using DG.Tweening;

public class CDDiscardedBill : MonoBehaviour
{
    //------------------------------------------------------------------
    // public
    //------------------------------------------------------------------
    public async UniTask<bool> DiscardCard(CDHandCard discardCard)
    {
        var rect = discardCard.GetComponent<RectTransform>();

        // カードを置く位置
        var randomPos = new Vector3(
            transform.position.x + Random.Range(-posThreshold, posThreshold),
            transform.position.y + Random.Range(-posThreshold, posThreshold),
            0f
        );
        rect.DOMove(randomPos, duraDiscard);

        // // カードを置く際の回転
        var rotateZ = Random.Range(-30f, 30f);
        var ratate = new Vector3(0f, 0f, rotateZ);
        rect.DORotate(ratate, duraDiscard);

        await UniTask.Delay((int)(duraDiscard * 1000));
        return true;
    }

    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    [SerializeField] float posThreshold; // カードを出す位置の揺らぎ範囲
    [SerializeField] float duraDiscard;


}
