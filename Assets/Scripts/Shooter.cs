using System.Collections;
using UnityEngine;
using B83.ExpressionParser;
using TMPro;

public class Shooter : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject projectile;
    public int maxFrames;
    public float speed;
    public GameObject uIController;
    private UIController uIControllerScript;

    private void Start()
    {
        uIControllerScript = uIController.GetComponent<UIController>();
    }

    public void Shoot()
    {
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
                    equation.Parameters["x"].Value = frames * speed;
                    float output = (float)equation.Value;
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
                    equation.Parameters["x"].Value = -frames * speed;
                    float output = (float)equation.Value;
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
}
