using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int stagePoint;
    public int totalPoint;
    public int stageIndex;
    public int health;

    public PlayerController player;
    public GameObject[] stages;

    //UI
    public Image[] UIHealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject restartBtn;

    void Update() {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage() {
        //Change Stage
        if(stageIndex < stages.Length - 1) {
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

    void OnTriggerEnter2D(Collider2D collision) 
    {
        //Health Down
        HealthDown();

        if (collision.gameObject.tag == "Player") {
            if(health > 1) {
                //Player Reposition
                PlayerReposition();
            }

        }
    }

    void PlayerReposition() {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }

    public void Restart() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
