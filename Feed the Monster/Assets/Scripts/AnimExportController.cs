using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimExportController : MonoBehaviour
{
    public List<GameObject> monsters;
    public List<MonsterState> states;

    public enum MonsterState
    {
        Idle,
        Happy,
        Sad,
        Eating,
        EatGood,
        EatBad,
        Finished
    }

    MonsterState monsterState;


    float mTimeAlive;
    float idleStateTimer = 0;
    int curmonnum;
    int curstate;

    public Animator animController;
    public GameObject selectedMonster;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject mo in monsters)
        {
            mo.SetActive(false);
        }

        changeMonsterFocus(5);
       

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            iterateMonster();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            iterateState();
        }
    }

    public void iterateMonster()
    {
        curmonnum++;
        if (curmonnum >= monsters.Count)
        {
            curmonnum = 0;
        }
        changeMonsterFocus(curmonnum);
    }

    public void iterateState()
    {
        curstate++;
        if (curstate >= states.Count)
        {
            curstate = 0;
        }
        Debug.Log(states[curstate]);
        SetMonsterState(states[curstate]);
    }

    public void changeMonsterFocus(int monnum)
    {
        curmonnum = monnum;

        if (selectedMonster != null)
        {
            selectedMonster.SetActive(false);
        }

        selectedMonster = monsters[monnum];
        Camera.main.transform.position = new Vector3(selectedMonster.transform.position.x, selectedMonster.transform.position.y, -10);
        selectedMonster.SetActive(true);
        if (selectedMonster != null)
        {
            animController = selectedMonster.GetComponentInChildren<Animator>();



            animController.SetInteger("EatState", 0);
            animController.SetInteger("IdleState", 0);
            animController.SetInteger("EmotionState", 0);

      
        }
        else
        {
            Debug.Log("no monster found");
        }
        // move camera to look at target monster
        //update animation controller to point to target monster.

        SetMonsterState(states[curstate]);
    }

    public void changeMonsterAnimation()
    {

    }




    public void SetMonsterState(MonsterState monsterState)
    {
       


        this.monsterState = monsterState;

        idleStateTimer = 0;


        switch (monsterState)
        {
            case MonsterState.Idle:
                animController.SetInteger("IdleState", 0);
                animController.SetInteger("EmotionState", 0);
                animController.SetInteger("EatState", 0);
                break;

            case MonsterState.Happy:
                animController.SetInteger("IdleState", 0);
                //			animController.SetInteger ("EmotionState", 2);
                
                {
                    int state = 2;
                    animController.SetInteger("EmotionState", state);
                }
                break;
            case MonsterState.Sad:
                animController.SetInteger("IdleState", 0);
                //			animController.SetInteger ("EmotionState", 3);
               
                {
                    int state = 3;
                    animController.SetInteger("EmotionState", state);
                }
                break;
            case MonsterState.Eating:
                animController.SetInteger("EatState", 1);
                animController.SetInteger("EmotionState", 0);
                animController.SetInteger("IdleState", 0);
                break;
            case MonsterState.EatGood:

                //			animController.SetInteger ("EatState", 2);
                
                {
                    animController.SetInteger("EmotionState", 0);
                    int state = 2;

                    animController.SetInteger("EatState", state);
                }
                break;
            case MonsterState.EatBad:
                //int state = UnityEngine.Random.Range (3, 5);
                
                {
                    animController.SetInteger("EmotionState", 0);
                    int state = 4;

                    animController.SetInteger("EatState", state);
                }
                break;
          

        }
    }

}
