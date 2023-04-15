using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isPlayerTurn { get; set; }              //플레이어의 턴인지 확인
    public bool isStart { get; set; }                   //게임이 시작되었는지 확인
    public bool isOver { get; set; }                    //게임이 종료되었는지 확인

    private TurnControl turnControl;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        isPlayerTurn = true;
        isStart = false;
        isOver = false;

        turnControl = GameUIManager.instance.turnPnl.GetComponent<TurnControl>();
    }

    //게임 스타트 버튼 클릭시 단한번 실행될 함수
    public void OnClickStart()
    {
        GameObject.Find("GameCanvas").gameObject.SetActive(false);
        GameUIManager.instance.selectPnl.SetActive(false);
        GameUIManager.instance.inGamePnl.SetActive(true);

        PlayerManager.instance.SetStart();
    }

    public void TurnChange()
    {
        if (isOver)
        {
            return;
        }

        StartCoroutine(CorTurnChange());
    }
    IEnumerator CorTurnChange()
    {
        yield return new WaitForSeconds(1f);

        isPlayerTurn = !isPlayerTurn;

        //플레이어 턴이면 조작가능 세팅초기화
        if (isPlayerTurn == true)
        {
            PlayerManager.instance.ResetMoveable();
            PlayerManager.instance.InitSelectPlayer();

            yield return StartCoroutine(turnControl.MyTurn());

            GameUIManager.instance.inGamePnl.SetActive(true);
            PlayerManager.instance.ShowTile();
        }
        else
        {
            EnemyManager.instance.InitSelectEnemy();
            GameUIManager.instance.inGamePnl.SetActive(false);

            yield return StartCoroutine(turnControl.EnemyTurn());

            EnemyManager.instance.FindPlayer();

            //카메라 추적 세팅
            Camera.main.gameObject.GetComponent<CameraFollow>().isDrag = false;
        }
    }
    //오프닝 씬 재로드
    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}
