using UnityEngine;

public class Timer : MonoBehaviour
{
    private float remainingTime;
    public float RemainingTime => remainingTime;

    public void OnInit(float levelTime)
    {
        remainingTime = levelTime;
        UIManager.Instance.GetUI<CanvasGamePlay>().UpdateTimer(remainingTime);
    }
    private void Update()
    {
        TimeTick();
    }
    private void TimeTick()
    {
        if (GameManager.IsState(GameState.GamePlay))
        {
            remainingTime -= Time.deltaTime;
            UIManager.Instance.GetUI<CanvasGamePlay>().UpdateTimer(remainingTime);
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                GameManager.Instance.EndGame(false);
            }
        }
    }
}
