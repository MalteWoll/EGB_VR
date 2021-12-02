using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    private List<int> visualizationList = new List<int>(); /* There are three allowed values on the list: 0 for equation visualization, 1 for graph, 2 for interactive VR, used values are deleted */
    private int calculationCounter; /* Multiple calculations are needed for each visualization, so here we keep track of them */
    private int maxCalculations; /* The maximum number of calculations. It should be hardcoded and always the same, just to be prepared this variable exists. */
    private int investmentCounter; /* Same as for the calculations, multiple for each visualization, this keeps track of where we are at the moment */
    private int maxInvestments; /* Maximum number of investments, same as for the calculations above */

    // The classes for the different visualizations are components of the corresponding GameObject
    [SerializeField]
    private GameObject visualizationEquationParent;
    [SerializeField]
    private GameObject visualizationGraphParent;
    [SerializeField]
    private GameObject visualizationInteractiveParent;

    private VisualizationEquation visualizationEquation;
    private VisualizationGraph visualizationGraph;
    private VisualizationInteractive visualizationInteractive;

    public float initialValue;
    public float growthFactor;
    public float speed;
    public float frequency;
    public float maxX;

    private void Start()
    {
        // When first starting this (and the experiment), fill the list for the visualization options, then shuffle it. This way, the order of the visualizations is always randomized.
        // Also, this makes adding more cycles of the process later on easier, just add more numbers to the list. Probably not needed though.
        visualizationList.Add(0); /* 0 = equation */
        visualizationList.Add(1); /* 1 = graph */
        visualizationList.Add(2); /* 2 = interactive */
        visualizationList = Util.shuffleList(visualizationList); /* Randomize the order of the values on the list */

        // Get the different classes for visualization from the components of the GameObjects
        visualizationEquation = visualizationEquationParent.GetComponent<VisualizationEquation>();
        visualizationGraph = visualizationGraphParent.GetComponent<VisualizationGraph>();
        visualizationInteractive = visualizationInteractiveParent.GetComponent<VisualizationInteractive>();

        startVisualization(visualizationList[0]); /* Start the first visualization with the first integer value on the now shuffled list */
    }

    private void Update()
    {
        
    }

    /// <summary>
    /// Activate a GameObject in relation to the value of the argument. The GameObject/number represent a type of visualization.
    /// </summary>
    /// <param name="option"></param>
    private void startVisualization(int option)
    {
        switch(option)
        {
            case 0:
                visualizationEquationParent.SetActive(true);
                break;
            case 1:
                visualizationGraphParent.SetActive(true);
                break;
            case 2:
                visualizationInteractiveParent.SetActive(true);
                break;
            default:
                Debug.LogError("Value " + option + " in switch case in startVisualization(), something went wrong.");
                break;
        }
    }

    private void startCalculation(int counter)
    {

    }

    private void startInvestment(int counter)
    {

    }

    private void saveAndExit()
    {

    }

    public void finishedVisualization()
    {

    }
}
