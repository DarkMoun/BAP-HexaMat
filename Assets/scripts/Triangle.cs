using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Triangle : MonoBehaviour {
    public class octogone
    {
        GameObject triangleun;
        GameObject triangledeux;
        GameObject triangletrois;
        GameObject trianglequatre;
        GameObject trianglecinq;
        GameObject trianglesix;
    }

    public int number;

    private Color initialColor;
    private bool mouseOver = false; //true when the mouse is over this triangle
    private List<GameObject> neighbours; //list of this triangle's neighbour

    // Use this for initialization
    void Start()
    {
        initialColor = this.GetComponent<Image>().color;
        //initialise the triangle's number to a random value between -10 and 10
        number = (int)(Random.value * 20 - 10);
        this.GetComponentInChildren<TextMeshProUGUI>().text = number.ToString();
        //rotate the text to be horizontal
        this.GetComponentInChildren<TextMeshProUGUI>().gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))  //if the left button is released (anywhere)
        {
            if (mouseOver)
            {
                this.GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                this.GetComponent<Image>().color = initialColor;
            }
        }
        //to stop triangle selection without calculating, press space
        if (Input.GetKeyDown(KeyCode.Space))    //if space bar is pressed
        {
            this.GetComponent<Image>().color = initialColor;
        }
    }

    private void OnMouseEnter() //called the frame where mouse enters this triangle
    {
        mouseOver = true;

        //if the left click is pressed
        if (Input.GetMouseButton(0))
        {
            //if this triangle isn't selected and there are less than 5 selected triangles
            if (Game.selected.Count < 5 && !Game.selected.Contains(this.gameObject))
            {
                //if this triangle is a neighbour of the last selected triangle
                if (Game.last.GetComponent<Triangle>().neighbours.Contains(this.gameObject))
                {
                    //select this triangle
                    this.GetComponent<Image>().color = Color.red;
                    Game.selected.Add(this.gameObject);
                    Game.last = this.gameObject;
                }
            }
        }
        else
        {
            this.GetComponent<Image>().color = Color.yellow;
        }
    }

    private void OnMouseExit() //called the frame where mouse exits this triangle
    {
        mouseOver = false;
        //if the left click isn't pressed
        if (!Input.GetMouseButton(0))
        {
            this.GetComponent<Image>().color = initialColor;
        }
    }
    
    private void OnMouseDown() //called the frame where this triangle is clicked
    {
        //if it was left click and the triangle wasn't already selected
        if (mouseOver && Input.GetMouseButtonDown(0) && !Game.selected.Contains(this.gameObject))
        {
            //select the triangle
            this.GetComponent<Image>().color = Color.red;
            Game.selected.Add(this.gameObject);
            Game.last = this.gameObject;
        }
    }
    
    public List<GameObject> Neighbours
    {
        get
        {
            return neighbours;
        }
    }

    public void InitaliseNeighbour()
    {
        neighbours = new List<GameObject>();

        //possible positions of neighbour
        //(the point opposing the horizontal side is "up")
        Vector3 point1 = this.GetComponent<RectTransform>().localPosition + this.GetComponent<RectTransform>().up * 69;     //down of the triangle
        Vector3 point2 = this.GetComponent<RectTransform>().localPosition + this.GetComponent<RectTransform>().right * 40;  //left of the triangle
        Vector3 point3 = this.GetComponent<RectTransform>().localPosition + this.GetComponent<RectTransform>().right * -40; //right of the triangle
        float dist1, dist2, dist3;
        foreach (GameObject tri in Game.triangles)
        {
            //if the triangle is different to this one
            if (!Object.ReferenceEquals(tri, this.gameObject))
            {
                //calculate the distance between the triangle and the 3 points
                dist1 = (tri.GetComponent<RectTransform>().localPosition - point1).magnitude;
                dist2 = (tri.GetComponent<RectTransform>().localPosition - point2).magnitude;
                dist3 = (tri.GetComponent<RectTransform>().localPosition - point3).magnitude;
                //if the triangle is close to one of these points
                if (dist1 < 30 || dist2 < 30 || dist3 < 30)
                {
                    //add it as neighbour
                    neighbours.Add(tri);
                }
            }
        }
    }
}
