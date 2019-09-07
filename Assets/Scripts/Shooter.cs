using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(FunctionControl))]
public class Shooter : MonoBehaviour
{
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
    private FunctionControl functionControlScript;

    // get necessary components for future reference
    private void Start()
    {
        uIControllerScript = uIController.GetComponent<UIController>();
        functionControlScript = GetComponent<FunctionControl>();
    }

    // if the text has changed, try to visualize.  If you can visualize visualize else don't visualize
    private void Update()
    {
        if (visualizeOn)
        {
            try { if (inputField.text != lastEquation) Visualize(); }
            catch { DeleteVisualizeInstants(); }
        }
    }

    private void Visualize()
    {
        // delete previous visualize projectiles, set last equation, set the appropriate function
        DeleteVisualizeInstants();
        lastEquation = inputField.text;
        functionControlScript.SetFunction(inputField.text);

        // visualize the positive projectile (the one that travels right)
        bool positiveWorking = true;
        for (int frames = 0; frames < visualizerMaxFrames && positiveWorking; frames++)
        {
            float output = (float)functionControlScript.output(frames * visualizerSpeed);
            if (float.IsNaN(output) || float.IsInfinity(output)) positiveWorking = false;
            else visualizeProjectileInstants.Add(Instantiate(visualizeProjectile, new Vector3(transform.position.x + frames * visualizerSpeed, transform.position.y, transform.position.z + output), Quaternion.identity));
        }

        // visualize the negative projectile (the one that travels left)
        bool negativeWorking = true;
        for (int frames = 1; frames < visualizerMaxFrames && negativeWorking; frames++)
        {
            float output = (float)functionControlScript.output(-frames * visualizerSpeed);
            if (float.IsNaN(output) || float.IsInfinity(output)) negativeWorking = false;
            else visualizeProjectileInstants.Add(Instantiate(visualizeProjectile, new Vector3(transform.position.x - frames * visualizerSpeed, transform.position.y, transform.position.z + output), Quaternion.identity));
        }
    }

    // start coroutine where projectile are moved according to equation
    public void Shoot() { StartCoroutine(MoveProjectiles()); }

    private IEnumerator MoveProjectiles()
    {
        // turn off visualization and delete visualize projectiles
        visualizeOn = false;
        DeleteVisualizeInstants();

        // instantiate a positive and negative projectile
        GameObject projectileInstantPositive = Instantiate(projectile, transform.position, Quaternion.identity);
        GameObject projectileInstantNegative = Instantiate(projectile, transform.position, Quaternion.identity);

        // destroy any lingering UI
        uIControllerScript.DestroyAllUI();

        // move positive and negative projectile along curve with error resulting in Error in Input message
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

        // create win / lose UI
        uIControllerScript.InstantiateWinLose(GameObject.FindGameObjectsWithTag("Target").Length == 0);
    }

    // create an error in input message and destroy any projectiles
    private void CreateErrorMessage(GameObject projectileInstantPositive, GameObject projectileInstantNegative)
    {
        uIControllerScript.InstantiateErrorInInput();
        if (projectileInstantPositive != null) Destroy(projectileInstantPositive);
        if (projectileInstantNegative != null) Destroy(projectileInstantNegative);
    }

    // Delete visualize instants
    private void DeleteVisualizeInstants() { foreach (GameObject projectileInstant in visualizeProjectileInstants) Destroy(projectileInstant); }
}
