using UnityEngine;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visual;
    [SerializeField] private new Rigidbody rigidbody;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    private Vector3 direction = Vector3.zero;

    private bool isMoving = false;
    private bool isStuck = false;

    private Vector3 mouseStartPos;
    private const float dragThreshold = 50f;

    private void Update()
    {
        if (!isMoving) HandleInput();

        if (isMoving && rigidbody.velocity == Vector3.zero)
            ResetMovement();
    }

    //Control Player
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 drag = Input.mousePosition - mouseStartPos;

            if (drag.magnitude >= dragThreshold)
            {
                Vector3 inputDir = GetDirectionFromDrag(drag);

                if (isStuck && inputDir != Vector3.back) return;
                if (inputDir == Vector3.back) isStuck = false;

                direction = inputDir;
                isMoving = true;
                mouseStartPos = Input.mousePosition;

                Move();
            }
        }
    }

    //Determine the swipe Direction: Left, right, top, bottom
    private Vector3 GetDirectionFromDrag(Vector3 drag)
    {
        drag.Normalize();

        if (Mathf.Abs(drag.x) > Mathf.Abs(drag.y))
            return new Vector3(Mathf.Sign(drag.x), 0, 0);
        else
            return new Vector3(0, 0, Mathf.Sign(drag.y));
    }

    private void Move()
    {
        rigidbody.velocity = speed * direction;
    }

    private void ResetMovement()
    {
        rigidbody.velocity = Vector3.zero;
        isMoving = false;
        mouseStartPos = Input.mousePosition;
    }

    private void StopPlayer()
    {
        rigidbody.velocity = Vector3.zero;
        enabled = false;
    }

    //Trigger
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "StackTile":
                StackManager.Instance.CollectTile(other.gameObject);
                break;

            case "BridgeTile":
                HandleBridgeTile(other.gameObject);
                break;

            case "BridgeRouter":
                ChangeDirection(other.name);
                break;

            case "MiniGame":
                HandleMiniGame();
                break;

            case "Multiplier":
                // Future: apply multiplier logic
                break;

            case "Finish":
                HandleFinish();
                break;
        }
    }

    //check player's brick go to the bridge
    private void HandleBridgeTile(GameObject tile)
    {

        bool passed = StackManager.Instance.RemoveTile(tile.transform.position);
        if (passed)
        {
            Destroy(tile);
            isStuck = false;
        }
        else
        {
            //End Game
            if (GameManager.Instance.EnteredMiniGame)
                HandleFinish();

            isStuck = true;
            ResetMovement();
        }
    }
    //On the bridge
    private void ChangeDirection(string routeName)
    {
        Vector3 newDirection = routeName switch
        {
            "Left" => Vector3.left,
            "Right" => Vector3.right,
            "Forward" => Vector3.forward,
            "Back" => Vector3.back,
            _ => Vector3.zero
        };

        if (newDirection != Vector3.zero)
        {
            rigidbody.velocity = Vector3.zero;
            direction = newDirection;
            Move();
        }
    } 

    private void HandleMiniGame()
    {
        //camera change direction
        GameManager.ActionMiniGame?.Invoke();
        visual.rotation = Quaternion.Euler(0, 90f, 0);
    }

    private void HandleFinish()
    {
        GameManager.ActionLevelPassed?.Invoke();
        visual.rotation = Quaternion.Euler(0, 180f, 0);
        StopPlayer();
    }
}
