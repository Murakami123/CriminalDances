using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpriteController : MonoBehaviour
{
    // カード裏側画像。
    [SerializeField] Sprite sprBackSide;
    
    // カードごとの表側画像。
    [SerializeField] Sprite sprFirstDiscoverer;
    [SerializeField] Sprite sprDog;
    [SerializeField] Sprite sprBoy;
    [SerializeField] Sprite sprCriminal;
    [SerializeField] Sprite sprDetective;
    [SerializeField] Sprite sprAlibi;
    [SerializeField] Sprite sprPlanning;
    [SerializeField] Sprite sprWitness;
    [SerializeField] Sprite sprInformOperation;
    [SerializeField] Sprite sprRumor;
    [SerializeField] Sprite sprTransaction;
    [SerializeField] Sprite sprNomalPeople;
    public Sprite GetCardBackSprite(){
        return sprBackSide;
    }

    public Sprite GetCardSprite( CardData.CardType type){
        switch( type ){
            case  CardData.CardType.FirstDiscoverer: return sprFirstDiscoverer;
            case  CardData.CardType.Criminal: return sprCriminal;
            case  CardData.CardType.Detective: return sprDetective;
            case  CardData.CardType.Alibi: return sprAlibi;
            case  CardData.CardType.Planning: return sprPlanning;
            case  CardData.CardType.Dog: return sprDog;
            case  CardData.CardType.Witness: return sprWitness;
            case  CardData.CardType.InformOperation: return sprInformOperation;
            case  CardData.CardType.Rumor: return sprRumor;
            case  CardData.CardType.Transaction: return sprTransaction;
            case  CardData.CardType.NomalPeople: return sprNomalPeople;
            case  CardData.CardType.Boy: return sprBoy;
            default:  break;
        }
        return null;
    }

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

}
