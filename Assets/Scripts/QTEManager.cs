using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

#if UNITY_2019_4_OR_NEWER && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class QTEManager : MonoBehaviour
{


    [Header("Configuration")]
    public float slowMotionTimeScale = 0.1f;

    public List<Button> blist = new List<Button>();
    public Button butPrefab;
    public Transform fragPanel;	
    public Transform ansPanel;

    [HideInInspector]
    public bool isEventStarted;
    private QTEEvent eventData;
    private bool isAllButtonsPressed;
    private bool isFail;
    private bool isEnded;
    private bool isPaused;
    private bool wrongKeyPressed;
    private float currentTime;
    private float smoothTimeUpdate;
    private float rememberTimeScale;
    private List<QTEKey> keys = new List<QTEKey>();
    
    public static System.Random rng = new System.Random();  

    public void Start()
    {
        
    }

    protected void Update()
    {
        if (!isEventStarted || eventData == null || isPaused) return;
        updateTimer();
        if (keys.Count == 0 || isFail)
        {
            doFinally();
        }
        else
        {
            for (int i = 0; i < eventData.keys.Count; i++)
            {

                checkKeyboardInput(eventData.keys[i]);

            }
        }
    }

    public void startEvent(QTEEvent eventScriptable)
    {
        List<string> ls = new List<string>();
        ls.Add("Hey there,");
        ls.Add("correct me if I'm wrong, but");
        ls.Add("isn't EduBeyond the greatest company in the world?");
        
        //generateQTE(ls, parent);
    
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current == null)
        {
            Debug.Log("No keyboard connected. Gamepad input in QTE events is not supported now");
            return;
        }
#endif
        eventData = eventScriptable;
        keys = new List<QTEKey>(eventData.keys);
        if (eventData.onStart != null)
        {
            eventData.onStart.Invoke();
        }
        isAllButtonsPressed = false;
        isEnded = false;
        isFail = false;
        isPaused = false;
        rememberTimeScale = Time.timeScale;
        switch (eventScriptable.timeType)
        {
            case QTETimeType.Slow:
                Time.timeScale = slowMotionTimeScale;
                break;
            case QTETimeType.Paused:
                Time.timeScale = 0;
                break;
        }
        currentTime = eventData.time;
        smoothTimeUpdate = currentTime;
        List<Button> blist = setupGUI(ls, fragPanel);
        StartCoroutine(countDown());
    }

    private IEnumerator countDown()
    {
        isEventStarted = true;
        while (currentTime > 0 && isEventStarted && !isEnded)
        {
            if (eventData.keyboardUI.eventTimerText != null)
            {
                eventData.keyboardUI.eventTimerText.text = currentTime.ToString();
            }
            currentTime--;
            yield return new WaitWhile(() => isPaused);
            yield return new WaitForSecondsRealtime(1f);
        }
        if (!isAllButtonsPressed && !isEnded)
        {
            isFail = true;
            doFinally();
        }
    }

    protected void doFinally()
    {
        if (keys.Count == 0)
        {
            isAllButtonsPressed = true;
        }
        isEnded = true;
        isEventStarted = false;
        Time.timeScale = rememberTimeScale;
        var ui = getUI();
        /*
        if (ui.eventUI != null)
        {
            ui.eventUI.SetActive(false);
        }
        */
        if (eventData.onEnd != null)
        {
            eventData.onEnd.Invoke();
        }
        if (eventData.onFail != null && isFail)
        {
            displayFail(eventData.keyboardUI.passText);
            eventData.onFail.Invoke();
        }
        if (eventData.onSuccess != null && isAllButtonsPressed)
        {
            displayPass(eventData.keyboardUI.passText);
            eventData.onSuccess.Invoke();
        }
        eventData = null;
    }

    protected void OnGUI()
    {
        if (eventData == null || isEnded) return;
        if (Event.current.isKey
            && Event.current.type == EventType.KeyDown
            && eventData.failOnWrongKey
            && !Event.current.keyCode.ToString().Equals("None"))
        {
            wrongKeyPressed = true;

            
            eventData.keys.ForEach(key =>
                wrongKeyPressed = wrongKeyPressed
                && !key.keyboardKey.ToString().Equals(Event.current.keyCode.ToString()));
            

            isFail = wrongKeyPressed;
        }
    }

    protected void updateTimer()
    {
        smoothTimeUpdate -= Time.unscaledDeltaTime;
        var ui = getUI();
        if (ui.eventTimerImage != null)
        {
            ui.eventTimerImage.fillAmount = smoothTimeUpdate / eventData.time;
        }
    }

    public void pause()
    {
        isPaused = true;
    }

    public void play()
    {
        isPaused = false;
    }

#if !ENABLE_INPUT_SYSTEM
    private bool isGamePadConnected()
    {
        string[] temp = Input.GetJoystickNames();
        bool result = false;
        if (temp.Length > 0)
        {
            for (int i = 0; i < temp.Length; ++i)
            {
                result = result || !string.IsNullOrEmpty(temp[i]);
            }
        }
        return result;
    }

    public void checkKeyboardInput(QTEKey key)
    {
        if (Input.GetKeyDown(key.keyboardKey))
        {
            keys.Remove(key);
        }
        if (Input.GetKeyUp(key.keyboardKey) && eventData.pressType == QTEPressType.Simultaneously)
        {
            keys.Add(key);
        }
    }

    protected List<Button> setupGUI(List<string> fragments, Transform parent)
    {
        var ui = getUI();
        
        Dictionary<string, int> answerDict = new Dictionary<string, int>();
    	int count = 0;
    	foreach (string fragment in fragments)
    	{
	    answerDict.Add(fragment, count);
	    count++;
    	}
    	Shuffle(fragments);
        foreach (string fragment in fragments)
        {   
            Button b = Instantiate(butPrefab, new Vector3(370, 130, 0), Quaternion.identity);
            b.transform.SetParent(parent);
            b.GetComponentInChildren<Text>().text = fragment;
            RectTransform bRectTrans = b.GetComponent<RectTransform>();
	    b.GetComponent<RectTransform>().sizeDelta = bRectTrans.sizeDelta + new Vector2(10.0f, 		    30.0f);
	    b.GetComponent<AnsButton>().code = answerDict[b.GetComponentInChildren<Text>().text];
            blist.Add(b);
        }
        
        for (int i = 0; i < blist.Count; i++)
        {
            
        }

        if (ui.eventTimerImage != null)
        {
            ui.eventTimerImage.fillAmount = 1;
        }
        /*
        if (ui.eventText != null)
        {
            ui.eventText.text = "";
            eventData.keys.ForEach(key => ui.eventText.text += key.keyboardKey + "+");
            eventData.keyboardUI.eventText.text = ui.eventText.text.Remove(ui.eventText.text.Length - 1);
        }
        */
        if (ui.eventUI != null)
        {
            ui.eventUI.SetActive(true);
        }
        return blist;
    }

    protected QTEUI getUI()
    {
        var ui = eventData.keyboardUI;
#endif
        return ui;
    }
    
    public void displayPass(Text passBox)
    {
    	passBox.text = "passed";
    }
    
    public void displayFail(Text passBox)
    {
    	passBox.text = "failed";
    }
    
    public List<Button> generateQTE(List<string> fragments, Transform parent)
    {
    	Dictionary<string, int> answerDict = new Dictionary<string, int>();
    	int count = 0;
    	foreach (string fragment in fragments)
    	{
	    answerDict.Add(fragment, count);
	    count++;
    	}
    	Shuffle(fragments);
        foreach (string fragment in fragments)
        {   
            Button b = Instantiate(butPrefab, new Vector3(370, 130, 0), Quaternion.identity);
            b.transform.SetParent(parent);
            b.GetComponentInChildren<Text>().text = fragment;
            RectTransform bRectTrans = b.GetComponent<RectTransform>();
	    b.GetComponent<RectTransform>().sizeDelta = bRectTrans.sizeDelta + new Vector2(10.0f, 		    30.0f);
	    b.GetComponent<AnsButton>().code = answerDict[b.GetComponentInChildren<Text>().text];
	    b.onClick.AddListener(butClicked(b, ansPanel));
            blist.Add(b);
        }
        
        for (int i = 0; i < blist.Count; i++)
        {
            
        }
        
        return blist;
    }
    
    public UnityEngine.Events.UnityAction butClicked(Button b, Transform ansPanel)
    {
        b.transform.SetParent(ansPanel);
        Debug.Log("allah");
        return null;
    }
        
    public static void Shuffle<T>( IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
        n--;  
        int k = rng.Next(n + 1);  
        T value = list[k];  
        list[k] = list[n];  
        list[n] = value;  
        }
    }  

}
