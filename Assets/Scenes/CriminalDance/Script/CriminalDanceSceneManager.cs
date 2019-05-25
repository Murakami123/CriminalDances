using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GreatScript;
using UniRx.Async;

public class CriminalDanceSceneManager : SingletonMonoBehaviour<CriminalDanceSceneManager>
{
    //------------------------------------------------------------------
    // public
    //------------------------------------------------------------------
    async UniTask Start()
    {
        // NPC人数設定画面の処理が完了するまで待つ。
        windowGameStart.Initilize(btnNo: 0); // デフォでNPC:2人。
        await windowGameStart.WaitDeciedMemberCount();
        SetPlayerCount(windowGameStart.GetNpcCount());

        // 参加人数から山札の中身を確定させる。
        cdBill.SetBillData(playerTotalCount);

        // 参加者全員がカードを引く。
        playerController.Initialize(playerTotalCount);
        await playerController.DrawAllPlayersAtGameStart(cdBill);

        // // 全員の 手札を見るデバッグ機能
        // await UniTask.Delay(500);
        // await playerController.DebugShowAllCards(playerTotalCount);

        // 自分だけの 手札を見る
        await UniTask.Delay(500);
        await playerController.ShowPlayerCards();

        // 第一発見者を持ってる人は出す。他の人は「待機中...」を出す。
        await playerController.TmpActivateDispWaitingAll(true);
        await UniTask.Delay(2500);
        await playerController.Firstdiscovery();
        await playerController.TmpActivateDispWaitingAll(false);

        // 1秒間、事件発生演出。
        await UniTask.Delay(1500);
        await effctIncident.EffectDayo(2f);


        // あとはぐるぐるカードを出し続ける。
        // while (!isFinishGame())
        // {
        //     // 詳細後で。
        //     // await playerController.PlayNextTurn();
        // }

        // とりあえず3秒待って勝敗演出。
        await UniTask.Delay(3000);

        // （デバッグ）2秒後_自分の画面に、勝敗演出。
        await effectWinOrLose.EffectWin();
        await UniTask.Delay(2000);
        await effectWinOrLose.CloseWinOrLoseWindow();

        // // 自分の画面タッチするまで、勝敗演出。
        // await effectWinOrLose.EffectWin();
        // await effectWinOrLose.WaitEffectComplete();

        // リトライしますか？
        // Yes: シーンを再ロード。
        // No : アプリケーションを終了。
        await retryWindow.Open();
    }


    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
    public int playNpcCount { get; private set; }
    public int playerTotalCount { get; private set; }
    [SerializeField] CDWindowGameStart windowGameStart;
    [SerializeField] PlayerController playerController;
    public CardSpriteController spriteController; // 外からGetしかしないからpropertyにしたいけどInspectorから指定できなくなるから苦肉の策。
    [SerializeField] CDBill cdBill; // 山札
    [SerializeField] Text testText;
    [SerializeField] CDEffectIncident effctIncident;
    [SerializeField] CDEffectWinOrLose effectWinOrLose;
    [SerializeField] GameRetryWindow retryWindow;
    public CDDiscardedBill discardBill;
    private void SetPlayerCount(int npcCount)
    {
        playNpcCount = npcCount;
        playerTotalCount = playNpcCount + 1;
        Debug.Log("NPCの人数:" + npcCount + " 人");
        Debug.Log("トータルの人数:" + playerTotalCount + " 人");
        testText.text = "NPCは " + playNpcCount.ToString() + "人です";
    }

    /// ゲーム（対戦）が終わったかを返す。
    /// 終わるまで時計周りでカードを出し続ける。
    private bool isFinishGame()
    {
        // 仮
        return false;

        // 犬が犯人を当てたらゲーム終わり。

        // 全員がすべてのカードを出し終えたらゲーム終わり。

    }

    // カードを準備したらよくシャッフルして、各プレイヤーに4枚ずつくばります。
    // 手札に第一発見者を持っているプレイヤーが第一発見者カードを出して、ゲームが始まります。


    // ゲームフロー

    // ゲームは以下のとおりに行います。

    // 1.カードを１枚出します。

    // 2.カードに書かれたアクションを行います。

    // 3.次の人の手番です（1.に戻る）



    // カードは全部で11種類あります。
    // 以下がカードの種類とアクションの内容です。

    // [第一発見者]
    // このカードを配られた人がスタートプレイヤーです。
    // 出した時に特別な効果はありません。

    // [犯人]
    // 手札が1枚の時だけ出すことができます。
    // このカードを場に出すことができれば勝利です。

    // [アリバイ]
    // このカードを出しても特別な効果はありません。
    // ただし、手札にアリバイがあるとき探偵に指名された時に犯人カードを持っていても「犯人ではない」と答えることができます。

    // [たくらみ]
    // このカードを出すと、出した人の勝利条件が変化し、犯人が勝ったら勝ち、犯人が負けたら負けとなります。
    // 探偵に指名されても「犯人ではない」と答えてください。

    // [探偵]
    // 手札が3枚以下の時に出すことができます。
    // 他のプレイヤーを一人指名してください。
    // その人が犯人カードを持っていれば勝利です。
    // ※例外として、手札が探偵4枚、または探偵3枚犯人1枚の時、探偵カードを出すことができます。ただし探偵としての効果はありません。

    // [いぬ]
    // (第三版で効果が変わりました。
    // 手札に入れず、誰か1人の手札を1枚選びそれが犯人であるかどうかを確認します。全員に見せ、犯人なら勝ち、そうでないなら、戻します。)
    // (旧効果)
    // 他のプレイヤーを1人指名してください。
    // 指名されたプレイヤーは手札を1枚捨てて、いぬカードを手札に入れてください。
    // 捨てられたカードが犯人カードの場合、いぬカードを出した人の勝利です。


    // [目撃者]
    // 他のプレイヤーを1人指名して、その人の手札を全て見ることができます。

    // [情報交換]
    // 全てのプレイヤーは左隣のプレイヤーにカードを1枚同時に渡してください。
    // 手札がない場合は、右隣りから貰うだけとなります。

    // [うわさ]
    // 全てのプレイヤーは右隣りのプレイヤーの手札からカードを1枚引いてください。
    // この時引いたカードを更に隣のプレイヤーに引かれないよう、注意してください。
    // 手札がない場合は、引かれずに、右隣りから引くだけとなります。

    // [取り引き]
    // 他のプレイヤー1人と手札を1枚交換します。
    // カードを交換する際は内容がわからないように裏向けで交換してください。
    // また、どちらかの手札が0枚の場合は、交換は行いません。

    // [一般人]
    // 特別な効果はありません。




    // 勝利条件

    // 勝利条件は以下のとおりです。

    // 探偵が犯人を当てた時、探偵の勝利となります。
    // いぬが犯人を当てた時、いぬの勝利となります。
    // 犯人カードが出された時、犯人とたくらみを出した人の勝利です。






}