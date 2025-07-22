using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (GameManager.IsState(GameState.Setting) || GameManager.IsState(GameState.Finish))
        {
            return; // Game tạm dừng thì khong nhan Input
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null)
            {
                GameEvents.OnTileClicked?.Invoke(worldPos);
                Debug.Log(hit.collider.gameObject.name);
            }
        }
    }
}