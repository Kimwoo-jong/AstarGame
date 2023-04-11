using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    Text txtTitle1;                                 //텍스트 오브젝트 1
    Text txtTitle2;                                 //텍스트 오브젝트 2

    Color textColor1;                               //텍스트 오브젝트 1의 색상
    Color textColor2;                               //텍스트 오브젝트 1의 색상
    
    //색상값을 지정하기 위한 float 변수
    private float txtR1 = 0f, txtG1 = 0f, txtB1 = 0f;
    private float txtR2 = 255f, txtG2 = 255f, txtB2 = 255f;
    
    //색상 변경 확인을 위한 bool 변수
    private bool isChange;

    private void Awake()
    {
        txtTitle1 = transform.GetChild(0).GetComponent<Text>();
        txtTitle2 = transform.GetChild(1).GetComponent<Text>();

        textColor1 = Color.black;
        textColor2 = Color.white;

        StartCoroutine(ChangeColor());
    }
    //게임 이름 Text의 색을 서로 교차되며 변하도록 한다.
    IEnumerator ChangeColor()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.01f);

            if(textColor1 == Color.black || textColor1 == Color.white)
            {
                isChange = !isChange;
            }

            if(isChange)
            {
                txtR1++; txtG1++; txtB1++;
                txtR2--; txtG2--; txtB2--;
            }
            else
            {
                txtR1--; txtG1--; txtB1--;
                txtR2++; txtG2++; txtB2++;
            }

            textColor1 = new Color(txtR1 / 255f, txtG1 / 255f, txtB1 / 255f, 1f);
            textColor2 = new Color(txtR2 / 255f, txtG2 / 255f, txtB2 / 255f, 1f);

            txtTitle1.color = textColor1;
            txtTitle2.color = textColor2;
        }
    }
}
