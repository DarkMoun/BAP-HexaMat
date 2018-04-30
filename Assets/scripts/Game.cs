using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game : MonoBehaviour {

    public static List<GameObject> triangles;   //all triangles
    public static List<GameObject> buttons;     //all rotation buttons
    public static List<GameObject> selected;    //all selected triangles (red)
    public static GameObject last;              //last selected triangle

    public List<string> operators;  //list of operators (can be set in editor)

    //lists used to calculate
    private List<float> results;
    private List<GameObject> tmpSelected;
    private List<string> allOperations;
    private List<string> tmpOperations;
    private List<string> elements;
    private Stack<string> calculateLeft;
    private Stack<string> calculateRight;
    string currentElem;

    // Use this for initialization
    void Start () {
        //lists initialisation
        triangles = new List<GameObject>();
        buttons = new List<GameObject>();
        selected = new List<GameObject>();
        //operators = new List<string>();
        results = new List<float>();
        allOperations = new List<string>();
        tmpOperations = new List<string>();
        calculateLeft = new Stack<string>();
        calculateRight = new Stack<string>();

        //put all triangles in "triangles"
        foreach (Triangle tri in this.GetComponentsInChildren<Triangle>())
        {
            triangles.Add(tri.gameObject);
        }
        //set all triangles' neighbours
        foreach(GameObject tri in triangles)
        {
            tri.GetComponent<Triangle>().InitaliseNeighbour();
        }
        bool ok;
        //set all buttons' triangles
        foreach(tourne_triangles tt in this.GetComponentsInChildren<tourne_triangles>())
        {
            ok = tt.InitialiseTriangles();
            //if the button doesn't have 6 triangles, hide it
            //(can be a problem if the button is not at the center of the hexagon or if the triangles are too far)
            if (ok)
            {
                buttons.Add(tt.gameObject);
            }
            else
            {
                tt.gameObject.SetActive(false);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        //if the left button is released (anywhere)
        if (Input.GetMouseButtonUp(0))
        {
            int nbSelected = selected.Count;
            //if there are at least 3 selected triangles
            if (nbSelected > 2)
            {
                if(operators.Count == 0)
                {
                    Debug.Log("Operators list is empty.");
                }
                else // if the operators list isn't empty
                {
                    //calculate all possible results
                    GenerateAllOperations(allOperations, selected);
                    CalculateAllOperations(allOperations, results);

                    //when all possible results are calculated, check if the last selected number is among them
                    if (results.Contains((float)last.GetComponent<Triangle>().number))
                    {
                        //correct answer
                        Debug.Log("Correct");
                        //replace all selected numbers to new random numbers
                        foreach(GameObject tri in selected)
                        {
                            tri.GetComponent<Triangle>().number = (int)(Random.value * 20 - 10);
                            tri.GetComponentInChildren<TextMeshProUGUI>().text = tri.GetComponent<Triangle>().number.ToString();
                        }
                    }
                    else
                    {
                        //wrong answer
                        Debug.Log("Wrong");
                    }
                }
            }
            else if(nbSelected != 0)    //if there are less than 3 selected numbers but at least 1
            {
                Debug.Log("Less than 3 selected.");
            }
            //clear the selected list after all operations when the left clicked is released
            selected.Clear();
        }
        //to stop triangle selection without calculating, press space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            selected.Clear();
        }
	}

    private void GenerateAllOperations(List<string> allOp, List<GameObject> selectedTriangles)
    {
        allOp.Clear();
        int nbSelected = selectedTriangles.Count;
        tmpSelected = new List<GameObject>(selectedTriangles); //copy the list
        //remove the first and the last number of the list
        tmpSelected.RemoveAt(nbSelected - 1);
        allOp.Add(tmpSelected[0].GetComponent<Triangle>().number.ToString()); //add the first number to the result list
        tmpSelected.RemoveAt(0);
        //for each remaining numbers
        for (int i = 0; i < nbSelected - 2; i++)
        {
            tmpOperations.Clear();
            int nextNumber = tmpSelected[i].GetComponent<Triangle>().number; //next number to be used to calculate (start from the second in the selected list)
            foreach (string operation in allOp)
            {
                //generate all possible operations between "nextNumber" and all operations in tmpOperations
                foreach (string op in operators)
                {
                    tmpOperations.Add(string.Concat(operation, " ", op, " ", nextNumber.ToString()));
                }
            }
            //save all new operations in the list "allOp"
            allOp = new List<string>(tmpOperations);
        }
    }

    private void CalculateAllOperations(List<string> allOp, List<float> operationResults)
    {
        foreach (string operation in allOp)
        {
            calculateLeft = new Stack<string>(operation.Split(' ')); //split by ' ' the operation and put all elements in a stack
            int nbElem;
            int nbOperators = calculateLeft.Count / 2;
            for (int i = 0; i < nbOperators; i++) //for each operators
            {
                nbElem = calculateLeft.Count; //nb numbers + nb operators
                //check the presence of each operator by priority
                if (calculateLeft.Contains("*") || calculateLeft.Contains("/"))
                {
                    //browse the list to find "*" or "/"
                    for (int j = 0; j < nbElem; j++)
                    {
                        currentElem = calculateLeft.Pop(); //take an element from the left stack
                        //if the element is "*" or "/" calculate and put the result in the right stack and stop browsing
                        if (currentElem == "*" || currentElem == "/")
                        {
                            //take one number from each stack
                            float a, b;
                            float.TryParse(calculateRight.Pop(), out a);
                            float.TryParse(calculateLeft.Pop(), out b);
                            //calculate and put the result in the right stack
                            calculateRight.Push(Calculate(a, currentElem, b).ToString());
                            //move all elements from the right stack to the left stack
                            int nbRight = calculateRight.Count;
                            for (int k = 0; k < nbRight; k++)
                            {
                                calculateLeft.Push(calculateRight.Pop());
                            }
                            break; //stop looking for "*" or "/"
                        }
                        //else put the element in the right stack and keep browsing
                        else
                        {
                            calculateRight.Push(currentElem);
                        }
                    }
                }
                else if (calculateLeft.Contains("+") || calculateLeft.Contains("-"))
                {
                    //browse the list to find "+" or "-"
                    for (int j = 0; j < nbElem; j++)
                    {
                        currentElem = calculateLeft.Pop(); //take an element from the left stack
                        //if the element is "+" or "-" calculate and put the result in the right stack and stop browsing
                        if (currentElem == "+" || currentElem == "-")
                        {
                            //take one number from each stack
                            float a, b;
                            float.TryParse(calculateRight.Pop(), out a);
                            float.TryParse(calculateLeft.Pop(), out b);
                            //calculate and put the result in the right stack
                            calculateRight.Push(Calculate(a, currentElem, b).ToString());
                            //move all elements from the right stack to the left stack
                            int nbRight = calculateRight.Count;
                            for (int k = 0; k < nbRight; k++)
                            {
                                calculateLeft.Push(calculateRight.Pop());
                            }
                            break; //stop looking for "+" or "-"
                        }
                        //else put the element in the right stack and keep browsing
                        else
                        {
                            calculateRight.Push(currentElem);
                        }
                    }
                }
                else
                {
                    Debug.Log("An operator is in the list but isn't recognized in the code. Add a case \"if calculateLeft.Contains(<operator>)\" in the fonction \"CalculateAllOperations\" of the script \"Game\".");
                }
            }
            //add the result of the operation to the list
            float result;
            float.TryParse(calculateLeft.Pop(), out result);
            operationResults.Add(result);
        }
    }

    //calculate the operation "a op b" with "op" the operator
    private float Calculate(float a, string op, float b)
    {
        float result = Mathf.Infinity;
        switch (op)
        {
            //list of defined operations depending on the operator
            case "+":
                result = a + b;
                break;

            case "-":
                result = a - b;
                break;

            case "*":
                result = a * b;
                break;

            case "/":
                if(b != 0)
                {
                    result = a / b;
                }
                break;

            default: //if op isn't one of the cases above
                Debug.Log(string.Concat("The operator \"", op, "\" is wrong or doesn't exist. Correct you operators list or add a case for the new operator in the method \"Calculate\" of the script \"Game\"."));
                break;
        }
        return result;
    }
}
