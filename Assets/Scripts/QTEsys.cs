using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class QTEsys : MonoBehaviour
{

    [Header("Event settings")]
    public List<QTEKey> keys = new List<QTEKey>();
    public QTETimeType timeType;
    public float time = 3f;
    public bool failOnWrongKey;
    public QTEPressType pressType;

    [Header("UI")]
    public QTEUI keyboardUI;

    [Header("Event actions")]
    public UnityEvent onStart;
    public UnityEvent onEnd;
    public UnityEvent onSuccess;
    public UnityEvent onFail;

    public GameObject displayBox;
    public GameObject passBox;
    public int waitingForKey;
    public bool correctKey;
    public int countingDown;
    public int keyDownCount = 0;
    public List<string> qte = new List<string>();
    public List<string> keysPressed = new List<string>();

    private void Update()
    {
        if (waitingForKey == 0)
        {
            countingDown = 1;
            StartCoroutine(countdown());

            qte = qteGenerator();
            displayQTE(displayBox, qte);
            waitingForKey = 1;

            string temp = "";
            foreach (string i in qte)
            {
                temp = temp + i;
            }
            Debug.Log(temp);    
        }

        if (Input.anyKeyDown)
        {
            keysPressed.Add((string)Input.inputString);
            keyDownCount++;
            if (keyDownCount == qte.Count)
            {
                for (int i = 0; i < keysPressed.Count; i++)
                {
                    keysPressed[i] = keysPressed[i].ToUpper();
                }
                correctKey = validateQte(keysPressed, qte);
                keysPressed.Clear();
                keyDownCount = 0;
                StartCoroutine(keyPressed());
            }
        }

    }

    public List<string> qteGenerator()
    {
        List<int> l = new List<int>();
        List<string> qte = new List<string>();
        int qteSize = Random.Range(1, 4);
        Debug.Log(qteSize);

        for (int i = 0; i < qteSize; i++)
        {
            print("skahdkashkdhkjl");
            int rand = Random.Range(1, 4);
            l.Add(rand);
        }

        foreach (int i in l)
        {
            if (i == 1) { qte.Add("E"); }
            if (i == 2) { qte.Add("R"); }
            if (i == 1) { qte.Add("T"); }
        }

        return qte;
    }

    public void displayQTE(GameObject displayBox, List<string> qte)
    {
        string qteText = "";

        foreach (string letter in qte)
        {
            qteText = qteText + "[" + letter + "]" + " ";
        }

        displayBox.GetComponent<Text>().text = qteText;
    }

    public bool validateQte(List<string> keys, List<string> qte)
    {
        if (keys.Count != qte.Count) { Debug.Log("error"); }

        for (int i = 0; i < qte.Count; i++)
        {
            if (keys[i] != qte[i])
            {
                return false;
            }
        }

        return true;
    }


    IEnumerator keyPressed()
    {

        if (correctKey)
        {
            countingDown = 2;
            passBox.GetComponent<Text>().text = "passed";
            yield return new WaitForSeconds(1.5f);
            correctKey = false;
            displayBox.GetComponent<Text>().text = "";
            passBox.GetComponent<Text>().text = "";
            yield return new WaitForSeconds(1.5f);
            waitingForKey = 0;
            countingDown = 1;
        }

        else if (!correctKey)
        {
            countingDown = 2;
            passBox.GetComponent<Text>().text = "failed";
            yield return new WaitForSeconds(1.5f);
            correctKey = false;
            displayBox.GetComponent<Text>().text = "";
            passBox.GetComponent<Text>().text = "";
            yield return new WaitForSeconds(1.5f);
            waitingForKey = 0;
            countingDown = 1;
        }
    }

    IEnumerator countdown()
    {
        yield return new WaitForSeconds(3.5f);
        if (countingDown == 1)
        {
            countingDown = 2;
            passBox.GetComponent<Text>().text = "failed";
            yield return new WaitForSeconds(1.5f);
            correctKey = false;
            displayBox.GetComponent<Text>().text = "";
            passBox.GetComponent<Text>().text = "";
            yield return new WaitForSeconds(1.5f);
            waitingForKey = 0;
            countingDown = 1;
        }
    }
}
