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
    public float playerHeight;

    private UIController uIControllerScript;
    private TargetController targetControllerScript;
    private bool visualizeOn = true;
    private string lastEquation;
    private List<GameObject> visualizeProjectileInstants = new List<GameObject>();
    private FunctionControl functionControlScript;
    private ControllerController controller;

    // get necessary components for future reference
    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<ControllerController>();
        uIControllerScript = controller.uIController;
        targetControllerScript = controller.targetController;
        functionControlScript = GetComponent<FunctionControl>();
        for (int i = 0; i < 2 * visualizerMaxFrames - 1; i++)
        {
            GameObject visualizeProjectile1 = Instantiate(visualizeProjectile, transform);
            visualizeProjectile1.SetActive(false);
            visualizeProjectileInstants.Add(visualizeProjectile1);
        }
    }

    // if the text has changed, try to visualize.  If you can visualize visualize else don't visualize
    private void Update()
    {
        if (visualizeOn)
        {
            try { if (inputField.text != lastEquation) Visualize(); }
            catch { HideVisualizeInstants(); }
        }
    }

    private void Visualize()
    {
        // delete previous visualize projectiles, set last equation, set the appropriate function
        lastEquation = inputField.text;
        functionControlScript.SetFunction(inputField.text);

        int visualizeProjectileIndex = 0;

        // visualize the positive projectile (the one that travels right)
        bool positiveWorking = true;
        for (int frames = 0; frames < visualizerMaxFrames; frames++)
        {
            GameObject currentVisualizeProjectile = visualizeProjectileInstants[visualizeProjectileIndex];

            if (positiveWorking)
            {
                float output = (float)functionControlScript.output(frames * visualizerSpeed);
                if (float.IsNaN(output) || float.IsInfinity(output)) positiveWorking = false;
                else
                {
                    currentVisualizeProjectile.SetActive(true);
                    currentVisualizeProjectile.transform.position = new Vector3(transform.position.x + frames * visualizerSpeed, transform.position.y + playerHeight, transform.position.z + output);
                }
            }

            if (!positiveWorking) currentVisualizeProjectile.SetActive(false);

            visualizeProjectileIndex++;
        }

        // visualize the negative projectile (the one that travels left)
        bool negativeWorking = true;
        float zeroOutput = (float)functionControlScript.output(0);
        if (!float.IsNaN(zeroOutput) && !float.IsInfinity(zeroOutput))
        {
            for (int frames = 1; frames < visualizerMaxFrames; frames++)
            {
                GameObject currentVisualizeProjectile = visualizeProjectileInstants[visualizeProjectileIndex];

                if (negativeWorking)
                {
                    float output = (float)functionControlScript.output(-frames * visualizerSpeed);
                    if (float.IsNaN(output) || float.IsInfinity(output)) negativeWorking = false;
                    else
                    {
                        currentVisualizeProjectile.SetActive(true);
                        currentVisualizeProjectile.transform.position = new Vector3(transform.position.x - frames * visualizerSpeed, transform.position.y + playerHeight, transform.position.z + output);
                    }
                }

                if (!negativeWorking) currentVisualizeProjectile.SetActive(false);

                visualizeProjectileIndex++;
            }
        }
    }

    // start coroutine where projectile are moved according to equation
    public void Shoot() { StartCoroutine(MoveProjectiles()); }

    private IEnumerator MoveProjectiles()
    {
        // turn off visualization and delete visualize projectiles
        visualizeOn = false;
        HideVisualizeInstants();

        // set correct function
        functionControlScript.SetFunction(inputField.text);
        GameObject projectileInstantPositive = null, projectileInstantNegative = null;

        // instantiate a positive and negative projectile
        try
        {
            float output = (float)functionControlScript.output(0);
            output = Mathf.Clamp(output, -1000, 1000);
            projectileInstantPositive = Instantiate(projectile, transform.position + new Vector3(0, playerHeight, output), Quaternion.identity);
            projectileInstantNegative = Instantiate(projectile, transform.position + new Vector3(0, playerHeight, output), Quaternion.identity);
        }
        catch { }

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
                    else projectileInstantPositive.GetComponent<Rigidbody>().MovePosition(new Vector3(transform.position.x + frames * speed, transform.position.y + playerHeight, transform.position.z + output));
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
                    else projectileInstantNegative.GetComponent<Rigidbody>().MovePosition(new Vector3(transform.position.x - frames * speed, transform.position.y + playerHeight, transform.position.z + output));
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
        uIControllerScript.InstantiateWinLose(!(targetControllerScript.ReturnTargets() > 0));
    }

    // create an error in input message and destroy any projectiles
    private void CreateErrorMessage(GameObject projectileInstantPositive, GameObject projectileInstantNegative)
    {
        uIControllerScript.InstantiateErrorInInput();
        if (projectileInstantPositive != null) Destroy(projectileInstantPositive);
        if (projectileInstantNegative != null) Destroy(projectileInstantNegative);
    }

    // Delete visualize instants
    private void HideVisualizeInstants() { foreach (GameObject projectileInstant in visualizeProjectileInstants) projectileInstant.SetActive(false); }
}
