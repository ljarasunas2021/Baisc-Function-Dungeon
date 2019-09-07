using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject win;
    public GameObject lose;
    public GameObject errorInInput;
    public GameObject retry;
    public Canvas canvas;

    private GameObject errorInInputInstant, winInstant, loseInstant, retryInstant;

    // Instantiate corresponding win / lose UI
    public void InstantiateWinLose(bool winGame)
    {
        if (winGame) winInstant = Instantiate(win, canvas.transform);
        else loseInstant = Instantiate(lose, canvas.transform);
        retryInstant = Instantiate(retry, canvas.transform);
    }

    // Instantiate error message
    public void InstantiateErrorInInput()
    {
        errorInInputInstant = Instantiate(errorInInput, canvas.transform);
        Destroy(errorInInputInstant, 5f);
    }

    // Destroy all of the ui thats currently instantiated
    public void DestroyAllUI()
    {
        if (errorInInputInstant != null) Destroy(errorInInputInstant);
        if (winInstant != null) Destroy(winInstant);
        if (loseInstant != null) Destroy(loseInstant);
        if (retryInstant != null) Destroy(retryInstant);
    }
}
