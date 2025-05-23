using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoutManagement : MonoBehaviour
{
    SubstitutionPuzzleManager substitutionPuzzleManager;

    //public bool isOnTheWay;
    //public int requiredDays;
    Clock mainClock;
    Dictionary<Pin, int> occupiedPins;
    int randomNum;
    // public int hits; // If each pin instance has their own num of hits, why are we using a global variable here?
    public GameObject scoutReturnPanel;

    public static ScoutManagement Instance { get; private set; }

    private void Awake()
    {

        if (Instance == null) { Instance = this; }

        Clock[] clocks = FindObjectsOfType<Clock>();
        foreach (Clock c in clocks)
        {
            if (c.IsMain) mainClock = c;
        }
    }
    private void OnEnable()
    {
        mainClock.NewDay.AddListener(ScoutsWorking);
    }
    private void OnDisable()
    {
        mainClock.NewDay.RemoveListener(ScoutsWorking);
    }

    // Start is called before the first frame update
    void Start()
    {
        substitutionPuzzleManager = FindObjectOfType<SubstitutionPuzzleManager>();
        occupiedPins = new Dictionary<Pin, int>();
    }

    public void ScoutsWorking()
    {
        foreach(Pin key in occupiedPins.Keys.ToList())
        {
            occupiedPins[key]--;
            Debug.Log("Pin: " + key.gameObject.name + ", Days Left: " + occupiedPins[key]);
            // Expedition is complete. Add phrase, reset pin
            if (key.IsOccupied && occupiedPins[key] <= 0)
            {
                for (int i = 0; i < key.NumOfHits; i++)
                {
                    randomNum = Random.Range(0, 2);
                    switch (randomNum)
                    {
                        case 0:
                            substitutionPuzzleManager.AddSingleExtraPhrase();
                            StartCoroutine(ScoutReturnPanelPopUp(0));
                            break;
                        case 1:
                            substitutionPuzzleManager.RevealRandomUnknownLetter();
                            StartCoroutine(ScoutReturnPanelPopUp(1));
                            break;
                    }
                }
                key.Reset();
            }
        }
       /*if(!isOnTheWay) return;

        if (requiredDays > 0)
        {
            requiredDays -= 1;
            Debug.Log("Days left111: " + requiredDays);
        }
        else if (requiredDays <= 0)
        {
            substitutionPuzzleManager.AddSingleExtraPhrase();
            Debug.Log("Days left222: " + requiredDays);
            isOnTheWay = false;
        }*/

    }

    public void AddOccupiedPin(Pin pin, int difficulty)
    {
        occupiedPins[pin] = difficulty;
    }

    IEnumerator ScoutReturnPanelPopUp(int phraseOrLetter)
    {
        if (phraseOrLetter == 0)
            scoutReturnPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Kangaroo Returned! +1 phrase!";
        else if (phraseOrLetter == 1)
            scoutReturnPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Kangaroo Returned! +1 Letter!";
        
        scoutReturnPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        scoutReturnPanel.SetActive(false);
    }
}
