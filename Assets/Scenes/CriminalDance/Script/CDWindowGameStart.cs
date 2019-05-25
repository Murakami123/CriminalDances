using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx.Async;

public class CDWindowGameStart : MonoBehaviour
{

    //------------------------------------------------------------------
    // public
    //------------------------------------------------------------------
    public void Initilize(int btnNo)
    {
        gameObject.SetActive(true);
        OnTap_NpcCount(btnNo);
    }

    public async UniTask<bool> WaitDeciedMemberCount()
    {
        await UniTask.WaitUntil(() => isDecidedMemberCount);
        return true;
    }

    /// NPC人数確定後、決まった人数を返す。
    public int GetNpcCount()
    {
        return playNpcCountTmp;
    }

    // NPC人数設定ボタン
    public void OnTap_NpcCount(int btnNo)
    {
        // NPC人数を変更
        playNpcCountTmp = btnNo + 2;

        // 当該のボタンを派手にする。
        SetBtnFlashy(btnNo);
    }

    // ゲーム開始！ボタン
    public void OnTap_CardGameStart()
    {
        isDecidedMemberCount = true;
        // CriminalDanceSceneManager.Instance.SetPlayNpcCount(npcCount: playNpcCountTmp);
    }
    private bool isDecidedMemberCount;

    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    private int playNpcCountTmp; // 人数が確定後、SceneManagerに渡すまで暫定的に持っておく人数。

    // [btnNo:npc人数] 0:2人, 1:3人, 2:4人, 3:5人, 4:6人, 5:7人 
    [SerializeField] Button[] btnNpcCountSet;
    private readonly Color FlashyColor = new Color(255f / 255f, 241f, 124f / 255f);
    private Tweener movingTween;
    /// 選択したボタンを派手にする
    private void SetBtnFlashy(int btnNo)
    {
        movingTween.Complete();
        for (int i = 0; i < btnNpcCountSet.Length; i++)
        {
            if (btnNo == i)
            {
                // 派手にする
                btnNpcCountSet[btnNo].Select(); // これしないとボタンが選択状態にならない。
                var image = btnNpcCountSet[btnNo].GetComponent<Image>();
                // image.color = FlashyColor; // Buttonコンポーネントの機能で上書きされるから色変えられない。
                movingTween = image.rectTransform.DOLocalMoveY(image.rectTransform.localPosition.y + 10f, 0.4f).SetLoops(10000, LoopType.Yoyo);
            }
            else
            {
                // 地味にする
                var image = btnNpcCountSet[btnNo].GetComponent<Image>();
                // image.color = Color.white; // Buttonコンポーネントの機能で上書きされるから色変えられない。
            }
        }
    }
}
