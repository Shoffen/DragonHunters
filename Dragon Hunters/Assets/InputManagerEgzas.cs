using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerEgzas : MonoBehaviour
{
    PlayerControls playercontrols;

    public Vector2 movementInput;

    private void OnEnable()
    {
        if(playercontrols == null)
        {
            playercontrols = new PlayerControls();

            playercontrols.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();

        }

        playercontrols.Enable();
    }
    private void OnDisable()
    {
        playercontrols.Disable();
    }

}
