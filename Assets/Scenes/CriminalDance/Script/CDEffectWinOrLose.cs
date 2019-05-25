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
    public async UniTask<bool> EffectWin()
    {
        gameObject.SetActive(true);
        objWin.SetActive(true);

        // 演出終わった後に Close ボタンを押せるようにする。
        ActivateCloseBtn();
        return true;
    }

    public async UniTask<bool> EffectLose()
    {
        gameObject.SetActive(true);
        objWin.SetActive(false);

        // 演出終わった後に Close ボタンを押せるようにする。
        ActivateCloseBtn();
        return true;
    }

    public void OnTap_CloseBtn()
    {
        CloseWinOrLoseWindow().Forget();
    }

    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    private async UniTask<bool> CloseWinOrLoseWindow()
    {
        gameObject.SetActive(false);
        return true;
    }

    [SerializeField] GameObject closeBtn;
    private void ActivateCloseBtn()
    {
        closeBtn.SetActive(true);
    }
}
