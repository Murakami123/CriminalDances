using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// 村上スクリプト
public abstract class UIMonoBehaviour : MonoBehaviour
{
    //------------------------------------------------------------------
    // 継承先で rectTransform で RectTranform にアクセスできる
    //------------------------------------------------------------------
    private RectTransform rect;
    protected RectTransform rectTransform
    {
        get
        {
            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
                if (rect == null) Debug.LogError("RectTransformがアタッチされてないかも");
            }
            return rect;
        }
    }

    //------------------------------------------------------------------
    // 継承先で thisImage で Image にアクセスできる
    //------------------------------------------------------------------
    private Image img;
    protected Image thisImage
    {
        get
        {
            if (img == null)
            {
                img = GetComponent<Image>();
                if (img == null) Debug.LogError("Imageがアタッチされてないかも");
            }
            return img;
        }
    }
}
