using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Bird currentBird;

    public Vector3 birdStart;
    public Vector3 birdEnd;
    public GameObject loseScreen;

    private TextMeshProUGUI text;
    private string remainingWord;
    private string currentWord;
    private bool allowInput;
    public GameObject[] birdList;
    private float time;
    private int level;
    private bool lost;

    private string[] starters = new string[] { "Look!", "Oh, boy!", "Wow!" };
    private Dictionary<string, string> birdFact = new Dictionary<string, string>(){
        {"Canada Goose", "Canada goose mate for life, which can be anywhere from 10 to 25 years!"},
        {"Bittern", "Bitterns have a loud booming call that can be heard for half a mile!"},
        {"Kestrel", "Studies have shown that kestrels can se ultraviolet light!"},
        {"Blackbird", "Blackbirds like to sing after rain!"},
        {"Bullfinch", "Bullfinches can be taught to imitate a whistle!"},
        {"Buzzard", "Buzzards are known to decorate their nests with foliage!"},
        {"Cuckoo", "Only male cuckoos make the iconic 'cuckoo' sound!"},
        {"Turtle Dove", "Turtle doves are mentioned in the Song of Solomon!"},
        {"Eider", "The scientific name for eider means 'softest down body'!"},
        {"Goldfinch", "Goldfinches are some of the strictest vegetarians in the bird world!"},
        {"Jay", "Jays can mimic the sounds of other birds and animals!"},
        {"Kingfisher", "Many of the world's kingfishers don't eat fish and rarely go near water!"},
        {"Magpie", "A magpie's tail makes up half of its total length!"},
        {"Mallard", "Mallards are the ancestors for almost all domestic ducks!"},
        {"Nuthatch", "Nuthatches will often store seed for later retrieval!"},
        {"Snowy Owl", "Snowy owls have an average wingspan of 4-5 feet!"},
        {"Puffin", "Puffins can flap their wings up to 400 times a minute!"},
        {"Quail", "Quails are a prey species and have an average lifespan of less than a year!"},
        {"Raven", "Ravens love to play and have been seen using snow-covered roofs as slides!"},
        {"Robin", "Robins have been known to eat fermented berries and exhibit drunken behavior!"},
        {"Sparrow", "City sparrows use cigarette butts to drive parasitic mites away from their nests!"},
        {"Arctic Tern", "In its lifespan, an arctic tern flies the equivalent of three round trips to the moon!"},
        {"Waxwing", "The name 'waxwing' comes from a waxy red secretion on the tips of their wings!"},
        {"Wren", "There is fossil evidence of wrens from the last ice age, 10,000-120,000 years ago!"}
    };
    private string currentStarter;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        birdList = GameObject.FindGameObjectsWithTag("Bird");
        currentBird = FindObjectOfType<Bird>();
        currentWord = currentBird.latinName;
        remainingWord = currentWord;
        allowInput = true;
        time = 0;
        lost = false;
        level = 1;

        foreach (GameObject b in birdList)
        {
            b.SetActive(false);
        }

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            loseScreen = GameObject.Find("LoseScreen");
            loseScreen.SetActive(false);

            text = GameObject.Find("Dialog").GetComponent<TextMeshProUGUI>();
            SetDialog();
        }

        StartCoroutine(SpawnBird());
    }

    // Update is called once per frame
    void Update()
    {
        // For birds flying in main menu
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            time += Time.deltaTime;

            if (time >= 10f)
            {
                int left = Random.Range(0, 2);

                time = 0;
                level += 1;

                if (left == 0)
                    StartCoroutine(SpawnBird());
                else
                    StartCoroutine(SpawnBirdLeft());
            }
        }
        else
        {
            // Display the lost screen if lost
            if (lost)
            {
                loseScreen.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {

                if (allowInput)
                {
                    GetInput();
                    SetDialog();
                }

                time += Time.deltaTime;

                if (time >= 10f)
                {
                    // Randomly decide which side bird comes from
                    int left = Random.Range(0, 2);

                    time = 0;
                    level += 1;

                    if (left == 0)
                        StartCoroutine(SpawnBird());
                    else
                        StartCoroutine(SpawnBirdLeft());

                    // Increase difficulty
                    if (level < 5)
                        currentBird.speed = 2f;
                    else if (level >= 5 && level < 10)
                        currentBird.speed = 2.5f;
                    else if (level >= 10 && level < 15)
                        currentBird.speed = 3f;
                    else if (level >= 15 && level < 20)
                        currentBird.speed = 3.5f;
                    else
                        currentBird.speed += 4f;

                    allowInput = true;
                }
            }
        }
    }

    // Set dialog box to include both latin and common names
    private void SetDialog()
    {
        //if (SceneManager.GetActiveScene().name != "MainMenu")
        //{
            if (allowInput)
            {
                // Update the colors to match what the player has typed
                text.text = currentStarter + " It's <color=#5dd644><i>" +
                (currentWord.Remove(currentWord.Length - remainingWord.Length, remainingWord.Length)) + "</color>" +
                remainingWord +
                "</i>! They are more commonly known as <i>" + currentBird.commonName + "</i>.";
            }
            else
                text.text = "Did you know... " + birdFact[currentBird.commonName];

            currentBird.label.text = "<color=#5dd644>" +
                (currentWord.Remove(currentWord.Length - remainingWord.Length, remainingWord.Length)) + "</color>" +
                remainingWord;
       // }
    }

    // Get player input
    private void GetInput()
    {
        if (Input.anyKeyDown)
        {
            string inKey = Input.inputString;

            if (inKey.Length == 1)
            {
                EnterLetter(inKey);
            }
        }
    }

    private void EnterLetter(string letter)
    {
        if ((remainingWord.IndexOf(letter.ToLower()) == 0) || (remainingWord.IndexOf(letter.ToUpper()) == 0))
        {
            SetDialog();
            remainingWord = remainingWord.Remove(0, 1);

            //Check if word is complete
            if (remainingWord.Length == 0)
            {
                allowInput = false;
            }
        }
        else
        {
            // Reset word
            remainingWord = currentWord;
            SetDialog();
        }
    }

    // Spawn a bird every 10 seconds
    IEnumerator SpawnBird()
    {
        birdStart = new Vector3(10f, Random.Range(-1f, 2.4f), -1f);
        birdEnd = new Vector3(-10f, Random.Range(-1f, 2.4f), -1f);

        var ind = Random.Range(0, birdList.Length);
        GameObject obj = Instantiate(birdList[ind], birdStart, new Quaternion());
        obj.SetActive(true);

        currentBird = obj.GetComponent<Bird>();
        currentWord = obj.GetComponent<Bird>().latinName;
        remainingWord = currentWord;
        currentStarter = starters[Random.Range(0, starters.Length)];
        //SetDialog();

        // Increase difficulty
        if (level < 5)
            yield return new WaitForSeconds(9.9f);
        else if (level >= 5 && level < 10)
            yield return new WaitForSeconds(8f);
        else if (level >= 10 && level < 15)
            yield return new WaitForSeconds(6.6f);
        else if (level >= 15 && level < 20)
            yield return new WaitForSeconds(5.8f);
        else
            yield return new WaitForSeconds(3.5f);

        // If the word is not fully typed, you lose
        if (remainingWord.Length != 0)
        {
            lost = true;
        }

        Destroy(obj);
    }

    IEnumerator SpawnBirdLeft()
    {
        birdEnd = new Vector3(10f, Random.Range(-1f, 2.4f), -1f);
        birdStart = new Vector3(-10f, Random.Range(-1f, 2.4f), -1f);

        var ind = Random.Range(0, birdList.Length);
        GameObject obj = Instantiate(birdList[ind], birdStart, new Quaternion());
        obj.GetComponent<SpriteRenderer>().flipX = true;
        obj.SetActive(true);

        currentBird = obj.GetComponent<Bird>();
        currentWord = obj.GetComponent<Bird>().latinName;
        remainingWord = currentWord;
        currentStarter = starters[Random.Range(0, starters.Length)];
        //SetDialog();

        // Increase difficulty
        if (level < 5)
            yield return new WaitForSeconds(9.9f);
        else if (level >= 5 && level < 10)
            yield return new WaitForSeconds(8f);
        else if (level >= 10 && level < 15)
            yield return new WaitForSeconds(6.6f);
        else if (level >= 15 && level < 20)
            yield return new WaitForSeconds(5.8f);
        else
            yield return new WaitForSeconds(3.5f);

        // If the word is not fully typed, you lose
        if (remainingWord.Length != 0)
        {
            lost = true;
        }

        Destroy(obj);
    }
}
