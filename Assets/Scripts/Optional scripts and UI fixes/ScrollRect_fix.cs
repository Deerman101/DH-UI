using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Фикс для ручки скролбара...
/// </summary>
public class ScrollRect_fix : ScrollRect
{
    override protected void LateUpdate()
    {
        base.LateUpdate();
        if (this.verticalScrollbar)
            this.verticalScrollbar.size = 0f;
    }

    override public void Rebuild(CanvasUpdate executing)
    {
        base.Rebuild(executing);
        if (this.verticalScrollbar)
            this.verticalScrollbar.size = 0f;
    }
}
