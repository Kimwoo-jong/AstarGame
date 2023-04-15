using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFace : MonoBehaviour
{
    private SelectPlayer selectPlayer;
    public Player Player { get; set; }

    private void Start()
    {
        selectPlayer = transform.parent.GetComponent<SelectPlayer>();
    }
    public void OnClickPlayerFace()
    {
        //초상화 UI를 누르면
        selectPlayer.SetSelectFace(this);
    }
}
