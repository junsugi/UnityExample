using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerTest : MonoBehaviour
{
    public int stagePoint;  // 해당 스테이지에서 먹은 코인 점수
    public int totalPoint;  // 총 스테이지에서 먹은 코인 점수
    public int stageIndex;  // 몇 스테이지인지 저장
    public int health;      // 캐릭터 체력이 몇인지

    public TestScript player;
    public GameObject[] stages;

    //UI
    public Image[] UIHealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject restartBtn;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage() {
        //Change Stage
        if (stageIndex < stages.Length - 1) {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else {  //Game Clear
            Time.timeScale = 0;
            Debug.Log("게임 클리어");
            restartBtn.SetActive(true);

            Text btnText = restartBtn.GetComponentInChildren<Text>();
            btnText.text = "다깼네? 다시 해";
        }

        //Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    void PlayerReposition() {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }

    public void Restart() {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void HealthDown() {
        if (health > 1) {
            health--;
            UIHealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else {
            //All HealthUI Off
            UIHealth[0].color = new Color(1, 0, 0, 0.4f);
            //Player Die Effect
            player.OnDie();
            //Result UI
            Debug.Log("죽음");
            //Retry Button UI
            restartBtn.SetActive(true);
            Text btnText = restartBtn.GetComponentInChildren<Text>();
            btnText.text = "죽었어, 다시 해";
        }
    }
    void OnTriggerEnter2D(Collider2D collision) {
        //Health Down
        HealthDown();

        if (collision.gameObject.tag == "Player") {
            Debug.Log(health);
            if (health > 0) {
                //Player Reposition
                PlayerReposition();
            }

        }
    }
}
