using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI healText;
    [HideInInspector] public bool[] skillActivated;             //used to enable any heal or damage value text.
    public TextMeshProUGUI damageText;
    public List<TextMeshProUGUI> damageTextList;                    //need a list because a skill can hit multiple times

    public static GameUI instance;
    Vector2 originalTextPos;

    //consts
    public int Heal { get; } = 0;
    public int Damage { get; } = 1;
    public int Support { get; } = 2;    //used for buffs and debuffs

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        skillActivated = new bool[3];
        for (int i = 0; i < skillActivated.Length; i++)
            skillActivated[i] = false;

        healText.enabled = false;
        damageText.enabled = false;
        originalTextPos = healText.transform.position;
        damageTextList = new List<TextMeshProUGUI>();
        //TextMeshProUGUI text = damageText;
       //text.transform.parent = transform;
        //damageTextList.Add(Instantiate(damageText, transform));
    }

    // Update is called once per frame
    void Update()
    {
        if (skillActivated[Heal])
        {
            StartCoroutine(DisplayText(healText, originalTextPos, Heal));   
        }

        if (skillActivated[Damage])
        {
            StartCoroutine(DisplayText(damageTextList, Damage));
        }
    }


    IEnumerator DisplayText(TextMeshProUGUI textGui, Vector2 originalPos, int skillType)
    {
       
        textGui.enabled = true;
        //Vector2 currentPos = textGui.transform.position;
        //Debug.Log("Current pos: " + currentPos);
       
        while (textGui.alpha > 0 && skillActivated[skillType])
        {
            textGui.alpha -= 0.2f * Time.deltaTime;
            textGui.transform.position = new Vector2(textGui.transform.position.x, textGui.transform.position.y + 10 * Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
        }
         
        skillActivated[skillType] = false;
        textGui.transform.position = originalPos;
        textGui.alpha = 1;
        textGui.enabled = false;
        

    }

    IEnumerator DisplayText(List<TextMeshProUGUI> textGui, int skillType)
    {
        for (int i = 0; i < textGui.Count; i++)
        {
            textGui[i].enabled = true;
            //Vector2 currentPos = textGui.transform.position;
            //Debug.Log("Current pos: " + currentPos);

            while (textGui[i].alpha > 0.01f)
            {
                textGui[i].alpha -= 0.2f * Time.deltaTime;
                textGui[i].transform.position = new Vector2(textGui[i].transform.position.x, textGui[i].transform.position.y + 10 * Time.deltaTime);
                yield return new WaitForSeconds(0.1f);
            }
            //Destroy(textGui[i]);
            //textGui.RemoveAt(i);
            //i--;
        }
        skillActivated[skillType] = false;
        //clear list
        for (int i = 0; i < textGui.Count; i++)
        {
            Destroy(textGui[i]);
            //textGui.RemoveAt(i);
            //i--;
        }
        textGui.Clear();
        textGui.Capacity = 0;

        //textGui.transform.position = originalPos;
        //textGui.alpha = 1;
        //textGui.enabled = false;


    }
}
