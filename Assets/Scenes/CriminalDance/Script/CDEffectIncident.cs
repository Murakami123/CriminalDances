using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;

public class CDEffectIncident : MonoBehaviour
{
    //------------------------------------------------------------------
    // public
    //------------------------------------------------------------------
    public async UniTask<bool> EffectDayo(float duraTime)
    {
        // 演出時間
        gameObject.SetActive(true);
        await UniTask.Delay((int)(duraTime * 1000));
        gameObject.SetActive(false);
        return true;
    }
    //------------------------------------------------------------------
    // private
    //------------------------------------------------------------------
}
