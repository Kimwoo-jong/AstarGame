using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnControl : MonoBehaviour
{
    private Text turnTxt;
    private RectTransform rt;

    private void Awake()
    {
        turnTxt = transform.GetChild(0).GetComponent<Text>();
        rt = GetComponent<RectTransform>();
        rt.localPosition = new Vector3(10000, 10000, 10000);
    }

    public IEnumerator MyTurn()
    {
        turnTxt.text = "My Turn";
        turnTxt.color = Color.white;

        rt.localPosition = Vector3.zero;

        yield return new WaitForSeconds(3f);

        rt.localPosition = new Vector3(10000, 10000, 10000);
    }
    public IEnumerator EnemyTurn()
    {
        turnTxt.text = "Enemy Turn";
        turnTxt.color = Color.red;

        rt.localPosition = Vector3.zero;

        yield return new WaitForSeconds(3f);

        rt.localPosition = new Vector3(10000, 10000, 10000);
    }
}