using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


    public GameObject centre;






    void Start () {
        Button btn = centre.GetComponent<Button>();
        btn.onClick.AddListener(Clique);
	}
	
	void Update () {

	}
    public void Clique()
    {
        transform.Rotate(Vector3.forward * -30);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        octogone = 
    }

}
