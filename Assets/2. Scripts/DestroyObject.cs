using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public void DestroyAndUpdatePanel()
    {
        GameUIManager.instance.UpdatePanelState();
        Destroy(this.gameObject);
    }
}