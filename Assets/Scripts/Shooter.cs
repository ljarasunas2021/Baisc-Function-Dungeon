using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B83.ExpressionParser;
using UnityEngine.UI;
using TMPro;

public class Shooter : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text win;
    public TMP_Text lose;
    public TMP_Text errorInInput;
    public Button retry;
    public Canvas canvas;
    public GameObject projectile;
    public int maxFrames;
    public float speed;

    private TMP_Text errorInInputInstant;

    public void Shoot()
    {
        var parser = new ExpressionParser();
        try
        {
            Expression equation = parser.EvaluateExpression(inputField.text);
            GameObject projectileInstantPositive = Instantiate(projectile, transform.position, Quaternion.identity);
            GameObject projectileInstantNegative = Instantiate(projectile, transform.position, Quaternion.identity);
            if (errorInInputInstant != null) Destroy(errorInInputInstant);
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
                    projectileInstantPositive.transform.position = new Vector3(transform.position.x + frames * speed, transform.position.y, transform.position.z + (float)equation.Value);
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
                    projectileInstantNegative.transform.position = new Vector3(transform.position.x - frames * speed, transform.position.y, transform.position.z + (float)equation.Value);
                }
                catch
                {
                    CreateErrorMessage(projectileInstantPositive, projectileInstantNegative);
                    yield break;
                }
            }

            yield return 0;
        }

        if (GameObject.FindGameObjectsWithTag("Target").Length == 0) Instantiate(win, canvas.transform);
        else Instantiate(lose, canvas.transform);
        Instantiate(retry, canvas.transform);
    }

    private void CreateErrorMessage(GameObject projectileInstantPositive, GameObject projectileInstantNegative)
    {
        errorInInputInstant = Instantiate(errorInInput, canvas.transform);
        Destroy(errorInInputInstant, 5f);
        if (projectileInstantPositive != null) Destroy(projectileInstantPositive);
        if (projectileInstantNegative != null) Destroy(projectileInstantNegative);
    }
}
