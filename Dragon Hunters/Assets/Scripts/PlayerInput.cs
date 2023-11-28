using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public enum PLAYER_ACTION
    {
        INTERACTION,
        GO_LEFT,
        GO_RIGHT,
        SHOOT
    }

    public InputActionMap playerInputMap;


    private void Awake()
    {
      
        //playerInputMap["GO_LEFT"].performed += context => HandleAction(PLAYER_ACTION.GO_LEFT);
        //playerInputMap["GoRight"].performed += context => HandleAction(PLAYER_ACTION.GO_RIGHT);
    }

    private void OnEnable()
    {
        playerInputMap.Enable();
    }

    private void OnDisable()
    {
        playerInputMap.Disable();
    }

    public bool ListenForHeldDown(PLAYER_ACTION action = PLAYER_ACTION.INTERACTION)
    {
        if (playerInputMap != null)
        {
            string actionName = action.ToString();
            InputAction inputAction = playerInputMap[actionName];

            // Check if the button is actively being held down
            if (inputAction.ReadValue<float>() > 0.0f)
            {
                Debug.Log("Action Held: " + action);
                return true;
            }
        }

        return false;
    }
    public bool ListenForClick(PLAYER_ACTION action = PLAYER_ACTION.INTERACTION)
    {
        if (playerInputMap != null)
        {
            var actionName = action.ToString();
            var inputAction = playerInputMap[actionName];

            if (inputAction.triggered)
            {
                Debug.Log("Action Pressed: " + action);
                return true;
            }
        }

        return false;
    }




    /*private void HandleAction(PLAYER_ACTION action)
    {
        Debug.Log("Performed action: " + action);
        
    }*/
}
