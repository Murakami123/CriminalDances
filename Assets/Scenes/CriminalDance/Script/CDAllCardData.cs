using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// このゲームで使う全カード32枚
public class CDAllCardData
{
    /// 読み取り専用の全カード一覧
    public readonly Dictionary<int, CardData.CardType> AllCards = new Dictionary<int, CardData.CardType>(){
        {0,CardData.CardType.FirstDiscoverer}, // ・第一発見者カード １枚
        {1,CardData.CardType.Criminal}, // ・犯人カード １枚
        {2,CardData.CardType.Detective}, // ・探偵カード ４枚
        {3,CardData.CardType.Detective},
        {4,CardData.CardType.Detective},
        {5,CardData.CardType.Detective},
        {6,CardData.CardType.Alibi}, // ・アリバイカード ５枚
        {7,CardData.CardType.Alibi},
        {8,CardData.CardType.Alibi},
        {9,CardData.CardType.Alibi},
        {10,CardData.CardType.Alibi},
        {11,CardData.CardType.Planning}, // ・たくらみカード ２枚
        {12,CardData.CardType.Planning},
        {13,CardData.CardType.Dog}, // ・いぬカード １枚
        {14,CardData.CardType.Witness}, // ・目撃者カード ３枚
        {15,CardData.CardType.Witness},
        {16,CardData.CardType.Witness},
        {17,CardData.CardType.InformOperation},// ・情報交換カード ３枚
        {18,CardData.CardType.InformOperation},
        {19,CardData.CardType.InformOperation},
        {20,CardData.CardType.Rumor}, // ・うわさカード ４枚
        {21,CardData.CardType.Rumor},
        {22,CardData.CardType.Rumor},
        {23,CardData.CardType.Rumor},
        {24,CardData.CardType.Transaction}, // ・取り引きカード ５枚
        {25,CardData.CardType.Transaction},
        {26,CardData.CardType.Transaction},
        {27,CardData.CardType.Transaction},
        {28,CardData.CardType.Transaction},
        {29,CardData.CardType.NomalPeople}, // ・一般人カード ２枚
        {30,CardData.CardType.NomalPeople},
        {31,CardData.CardType.Boy}, // ・少年カード １枚
    };


}

public class CardData
{
    public enum CardType
    {
        FirstDiscoverer, // 第一発見者
        Criminal, // 犯人
        Detective, // 探偵
        Alibi, // アリバイ
        Planning, // たくらみ
        Dog, // 犬
        Witness, // 目撃者
        InformOperation, // 情報交換
        Rumor, // 噂
        Transaction, // 取引き
        NomalPeople, // 一般人
        Boy, // 少年
    }
}
// カード画像のフォントはHGSSoeiKakupoptai

// ＜犯人は踊るのカード枚数＞
// ・犯人カード １枚
// ・いぬカード １枚
// ・少年カード １枚
// ・第一発見者カード １枚
// ・たくらみカード ２枚
// ・一般人カード ２枚
// ・目撃者カード ３枚
// ・情報操作カード ３枚
// ・探偵カード ４枚
// ・うわさカード ４枚
// ・取り引きカード ５枚
// ・アリバイカード ５枚
// ⇒合計枚数 32枚

// public Dictionary<int, CardData.CardType> AllCards = new Dictionary<int, CardData.CardType>(){
//     {0,CardData.CardType.FirstDiscoverer}, // ・第一発見者カード １枚
//     {1,CardData.CardType.Criminal}, // ・犯人カード １枚
//     {2,CardData.CardType.Detective}, // ・探偵カード ４枚
//     {3,CardData.CardType.Detective},
//     {4,CardData.CardType.Detective},
//     {5,CardData.CardType.Detective},
//     {6,CardData.CardType.Alibi}, // ・アリバイカード ５枚
//     {7,CardData.CardType.Alibi},
//     {8,CardData.CardType.Alibi},
//     {9,CardData.CardType.Alibi},
//     {10,CardData.CardType.Alibi},
//     {11,CardData.CardType.Planning}, // ・たくらみカード ２枚
//     {12,CardData.CardType.Planning},
//     {13,CardData.CardType.Dog}, // ・いぬカード １枚
//     {14,CardData.CardType.Witness}, // ・目撃者カード ３枚
//     {15,CardData.CardType.Witness},
//     {16,CardData.CardType.Witness},
//     {17,CardData.CardType.InformOperation},// ・情報操作カード ３枚
//     {18,CardData.CardType.InformOperation},
//     {19,CardData.CardType.InformOperation},
//     {20,CardData.CardType.Rumor}, // ・うわさカード ４枚
//     {21,CardData.CardType.Rumor},
//     {22,CardData.CardType.Rumor},
//     {23,CardData.CardType.Rumor},
//     {24,CardData.CardType.Transaction}, // ・取り引きカード ５枚
//     {25,CardData.CardType.Transaction},
//     {26,CardData.CardType.Transaction},
//     {27,CardData.CardType.Transaction},
//     {28,CardData.CardType.Transaction},
//     {29,CardData.CardType.NomalPeople}, // ・一般人カード ２枚
//     {30,CardData.CardType.NomalPeople},
//     {31,CardData.CardType.Boy}, // ・少年カード １枚
// };

// public Dictionary<int, CardData.CardType> AllCards = new Dictionary<int, CardData.CardType>();
// public void Refresh(){
//     AllCards.Clear();
//     AllCards.Add( 0,CardData.CardType.FirstDiscoverer); // ・第一発見者カード １枚
//     AllCards.Add( 1,CardData.CardType.Criminal); // ・犯人カード １枚
//     AllCards.Add( 2,CardData.CardType.Detective); // ・探偵カード ４枚
//     AllCards.Add( 3,CardData.CardType.Detective);
//     AllCards.Add( 4,CardData.CardType.Detective);
//     AllCards.Add( 5,CardData.CardType.Detective);
//     AllCards.Add( 6,CardData.CardType.Alibi); // ・アリバイカード ５枚
//     AllCards.Add( 7,CardData.CardType.Alibi);
//     AllCards.Add( 8,CardData.CardType.Alibi);
//     AllCards.Add( 9,CardData.CardType.Alibi);
//     AllCards.Add( 10,CardData.CardType.Alibi);
//     AllCards.Add( 11,CardData.CardType.Planning); // ・たくらみカード ２枚
//     AllCards.Add( 12,CardData.CardType.Planning);
//     AllCards.Add( 13,CardData.CardType.Dog); // ・いぬカード １枚
//     AllCards.Add( 14,CardData.CardType.Witness); // ・目撃者カード ３枚
//     AllCards.Add( 15,CardData.CardType.Witness);
//     AllCards.Add( 16,CardData.CardType.Witness);
//     AllCards.Add( 17,CardData.CardType.InformOperation); //・情報操作カード ３枚
//     AllCards.Add( 18,CardData.CardType.InformOperation);
//     AllCards.Add( 19,CardData.CardType.InformOperation);
//     AllCards.Add( 20,CardData.CardType.Rumor); // ・うわさカード ４枚
//     AllCards.Add( 21,CardData.CardType.Rumor);
//     AllCards.Add( 22,CardData.CardType.Rumor);
//     AllCards.Add( 23,CardData.CardType.Rumor);
//     AllCards.Add( 24,CardData.CardType.Transaction); // ・取り引きカード ５枚
//     AllCards.Add( 25,CardData.CardType.Transaction);
//     AllCards.Add( 26,CardData.CardType.Transaction);
//     AllCards.Add( 27,CardData.CardType.Transaction);
//     AllCards.Add( 28,CardData.CardType.Transaction);
//     AllCards.Add( 29,CardData.CardType.NomalPeople); // ・一般人カード ２枚
//     AllCards.Add( 30,CardData.CardType.NomalPeople);
//     AllCards.Add( 31,CardData.CardType.Boy); // ・少年カード １枚        
// }