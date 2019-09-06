using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject win;
    public GameObject lose;
    public GameObject errorInInput;
    public GameObject retry;
    public Canvas canvas;

    private GameObject errorInInputInstant;

    public void InstantiateWinLose(bool winGame)
    {
        if (winGame) Instantiate(win, canvas.transform);
        else Instantiate(lose, canvas.transform);
        Instantiate(retry, canvas.transform);
    }

    public void InstantiateErrorInInput()
    {
        errorInInputInstant = Instantiate(errorInInput, canvas.transform);
        Destroy(errorInInputInstant, 5f);
    }

    public void DestroyErrorInInputInstant()
    {
        if (errorInInputInstant != null) Destroy(errorInInputInstant);
    }
}
