using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI healText;
    public bool skillActivated;             //used to enable any heal or damage value text.

    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        skillActivated = false;
        healText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (skillActivated)
        {

            //healText.enabled = true;
            StartCoroutine(DisplayDamageText(healText));
            
        }
    }


    IEnumerator DisplayDamageText(TextMeshProUGUI textGui)
    {
       
        textGui.enabled = true;
        Vector2 currentPos = textGui.transform.position;
        

        while (textGui.alpha > 0 && skillActivated)
        {
            textGui.alpha -= 0.1f * Time.deltaTime;
            textGui.transform.position = new Vector2(textGui.transform.position.x, textGui.transform.position.y + 2 * Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
        }

        skillActivated = false;
        textGui.transform.position = currentPos;
        textGui.alpha = 1;
        textGui.enabled = false;
        

    }
}
