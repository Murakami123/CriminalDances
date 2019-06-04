using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;

/// 詳細な演出は後で作ろう
public class CDEffectWinOrLose : MonoBehaviour
{

    //------------------------------------------------------------------
    // public
    //------------------------------------------------------------------
    [SerializeField] GameObject objWin;
    [SerializeField] GameObject objLose;
    [SerializeField] GameObject objBystander;
    public async UniTask<bool> EffectWin()
    {
        gameObject.SetActive(true);
        objWin.SetActive(true);

        // 演出終わった後に Close ボタンを押せるようにする。
        ActivateCloseBtn();

        // 完了
        return true;
    }

    public async UniTask<bool> EffectLose()
    {
        gameObject.SetActive(true);
        objLose.SetActive(false);

        // 演出終わった後に Close ボタンを押せるようにする。
        ActivateCloseBtn();

        // 完了
        return true;
    }

    public async UniTask<bool> EffectBystander()
    {
        gameObject.SetActive(true);
        objBystander.SetActive(false);

        // 演出終わった後に Close ボタンを押せるようにする。
        ActivateCloseBtn();

        // 完了
        return true;
    }

    public void OnTap_CloseBtn()
    {
        CloseWinOrLoseWindow().Forget();
    }

    private bool isEffectFinish = false;
    public async UniTask<bool> WaitEffectComplete()
    {
        await UniTask.WaitUntil(() => isEffectFinish);
        return true;
    }

    public async UniTask<bool> CloseWinOrLoseWindow()
    {
        gameObject.SetActive(false);
        // 演出完了フラグ。
        isEffectFinish = true;
        return true;
    }

    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    [SerializeField] GameObject closeBtn;
    private void ActivateCloseBtn()
    {
        closeBtn.SetActive(true);
    }
}
