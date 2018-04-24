using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class tourne_triangles : MonoBehaviour {
    public class octogone
    {
        GameObject triangleun;
        GameObject triangledeux;
        GameObject triangletrois;
        GameObject trianglequatre;
        GameObject trianglecinq;
        GameObject trianglesix;
    }

    private bool rotate = false;
    private List<GameObject> triangles;
    private float rotateCount;

    void Start () {
        Button btn = this.gameObject.GetComponent<Button>();
        btn.onClick.AddListener(Clique);
	}
	
	void Update () {
        if (rotate)
        {
            foreach(GameObject triangle in triangles)
            {
                //rotate the triangle
                triangle.transform.RotateAround(this.transform.position, Vector3.forward, -120 * Time.deltaTime);
                //rotate the text to keep it horizontal
                triangle.GetComponentInChildren<TextMeshProUGUI>().gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            rotateCount += 120 * Time.deltaTime; //total rotation angle

            //if the total angle is almost 60
            if (rotateCount > 60 - 120 * Time.deltaTime)
            {
                foreach (GameObject triangle in triangles)
                {
                    //finishes the rotation
                    triangle.transform.RotateAround(this.transform.position, Vector3.forward, -(60 - rotateCount));
                    triangle.GetComponentInChildren<TextMeshProUGUI>().gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                rotate = false; //stop the rotation
                //set new triangles for all buttons
                foreach(GameObject button in Game.buttons)
                {
                    button.GetComponent<tourne_triangles>().InitialiseTriangles();
                }
                //set new neighbours for all triangles
                foreach (GameObject triangle in Game.triangles)
                {
                    triangle.GetComponent<Triangle>().InitaliseNeighbour();
                }
            }
        }
	}

    //on click start rotation
    public void Clique()
    {
        rotate = true;
        rotateCount = 0;
        //transform.Rotate(Vector3.forward * -30);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //octogone = 
    }

    //put the triangles around this boutton in a list
    public bool InitialiseTriangles()
    {
        triangles = new List<GameObject>();
        foreach(GameObject triangle in Game.triangles)
        {
            //distance between the triangle and the button
            float distance = (triangle.GetComponent<RectTransform>().localPosition - this.GetComponent<RectTransform>().localPosition).magnitude;
            if (distance < 60)
            {
                triangles.Add(triangle);
            }
        }
        //return true if the hexagone has 6 triangles
        if(triangles.Count == 6)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
