using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public enum PLAYER_ACTION
    {
        INTERACTION,
        //SHOOT
    }
    public enum AXIS
    {
        MOVE
    }
    [SerializeField] private PlayerInput playerInput;
    private InputActionMap actionMap;

    private void Awake()
    {
        actionMap = playerInput.currentActionMap;
        
    }

    private void OnEnable()
    {
        actionMap.Enable();
    }

    private void OnDisable()
    {
        actionMap.Disable();
    }

   
    public Vector2 GetAxis(AXIS _axis)
    {
        
        InputAction inputAction = actionMap[_axis.ToString().ToLower()];
        switch(_axis)
        {
            case AXIS.MOVE:
                Vector2 local = Vector2.zero;
                local = new Vector2(inputAction.ReadValue<float>(), 0);
                Debug.Log(local);
                return local;
        }
       // Debug.Log(inputAction.ReadValue<Vector2>());
        return inputAction.ReadValue<Vector2>();
       
    }
    public bool ListenForClick(PLAYER_ACTION _action = PLAYER_ACTION.INTERACTION)
    {
        if (actionMap != null)
        {
            var actionName = _action.ToString().ToLower();
            var inputAction = actionMap[actionName];

            if (inputAction.triggered)
            {
                Debug.Log("Action Pressed: " + _action);
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
