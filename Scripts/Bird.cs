using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;

public class Bird : MonoBehaviour
{

    public string commonName;
    public string latinName;

    public GameManager gameController;
    public float speed;
    public bool goingLeft;
    public TextMeshPro label;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameManager>();
        label = GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        // Birds FLY
        if (goingLeft)
        {
            transform.position = Vector3.MoveTowards(transform.position, gameController.birdEnd, speed * Time.deltaTime);
        }
        else if (!goingLeft)
        {
            transform.position = Vector3.MoveTowards(transform.position, gameController.birdStart, speed * Time.deltaTime);
        }
    }
}
