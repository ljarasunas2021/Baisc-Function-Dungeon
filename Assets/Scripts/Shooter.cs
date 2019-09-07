using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Shooter : MonoBehaviour
{
    public FunctionControl functionControlScript;
    public TMP_InputField inputField;
    public GameObject projectile;
    public GameObject visualizeProjectile;
    public float speed;
    public float visualizerSpeed;
    public int visualizerMaxFrames;
    public GameObject uIController;
    private UIController uIControllerScript;
    private bool visualizeOn = true;
    private string lastEquation;
    private List<GameObject> visualizeProjectileInstants = new List<GameObject>();

    private void Start()
    {
        uIControllerScript = uIController.GetComponent<UIController>();
    }

    private void Update()
    {
        if (visualizeOn)
        {
            try
            {
                if (inputField.text != lastEquation)
                {
                    Visualize();
                }
            }
            catch
            {
                DeleteVisualizeInstants();
            }
        }
    }

    private void Visualize()
    {
        DeleteVisualizeInstants();
        lastEquation = inputField.text;
        functionControlScript.SetFunction(inputField.text);

        bool positiveWorking = true;
        for (int frames = 0; frames < visualizerMaxFrames && positiveWorking; frames++)
        {
            //if (inputField.text.Contains("x")) equation.Parameters["x"].Value = frames * visualizerSpeed;
            float output = (float)functionControlScript.output(frames * visualizerSpeed);
            if (float.IsNaN(output) || float.IsInfinity(output)) positiveWorking = false;
            else visualizeProjectileInstants.Add(Instantiate(visualizeProjectile, new Vector3(transform.position.x + frames * visualizerSpeed, transform.position.y, transform.position.z + output), Quaternion.identity));
        }

        bool negativeWorking = true;
        for (int frames = 1; frames < visualizerMaxFrames && negativeWorking; frames++)
        {
            //if (inputField.text.Contains("x")) equation.Parameters["x"].Value = -frames * visualizerSpeed;
            float output = (float)functionControlScript.output(-frames * visualizerSpeed);
            if (float.IsNaN(output) || float.IsInfinity(output)) negativeWorking = false;
            else visualizeProjectileInstants.Add(Instantiate(visualizeProjectile, new Vector3(transform.position.x - frames * visualizerSpeed, transform.position.y, transform.position.z + output), Quaternion.identity));
        }
    }

    public void Shoot()
    {
        visualizeOn = false;
        DeleteVisualizeInstants();

        var parser = new ExpressionParser();
        try
        {
            Expression equation = parser.EvaluateExpression(inputField.text);
            GameObject projectileInstantPositive = Instantiate(projectile, transform.position, Quaternion.identity);
            GameObject projectileInstantNegative = Instantiate(projectile, transform.position, Quaternion.identity);
            uIControllerScript.DestroyErrorInInputInstant();
            StartCoroutine(MoveProjectile(projectileInstantPositive, projectileInstantNegative, equation));
        }
        catch
        {
            CreateErrorMessage(null, null);
        }
    }

    private IEnumerator MoveProjectile(GameObject projectileInstantPositive, GameObject projectileInstantNegative, Expression equation)
    {
        for (int frames = 0; projectileInstantPositive != null || projectileInstantNegative != null; frames++)
        {
            if (projectileInstantPositive != null)
            {
                try
                {
                    float output = (float)functionControlScript.output(frames * speed);
                    if (float.IsNaN(output) || float.IsInfinity(output)) Destroy(projectileInstantPositive);
                    else projectileInstantPositive.transform.position = new Vector3(transform.position.x + frames * speed, transform.position.y, transform.position.z + output);
                }
                catch
                {
                    CreateErrorMessage(projectileInstantPositive, projectileInstantNegative);
                    yield break;
                }
            }

            if (projectileInstantNegative != null)
            {
                try
                {
                    float output = (float)functionControlScript.output(-frames * speed);
                    if (float.IsNaN(output) || float.IsInfinity(output)) Destroy(projectileInstantNegative);
                    else projectileInstantNegative.transform.position = new Vector3(transform.position.x - frames * speed, transform.position.y, transform.position.z + output);
                }
                catch
                {
                    CreateErrorMessage(projectileInstantPositive, projectileInstantNegative);
                    yield break;
                }
            }

            yield return 0;
        }

        uIControllerScript.InstantiateWinLose(GameObject.FindGameObjectsWithTag("Target").Length == 0);
    }

    private void CreateErrorMessage(GameObject projectileInstantPositive, GameObject projectileInstantNegative)
    {
        uIControllerScript.InstantiateErrorInInput();
        if (projectileInstantPositive != null) Destroy(projectileInstantPositive);
        if (projectileInstantNegative != null) Destroy(projectileInstantNegative);
    }

    private void DeleteVisualizeInstants()
    {
        foreach (GameObject projectileInstant in visualizeProjectileInstants) Destroy(projectileInstant);
    }
}
