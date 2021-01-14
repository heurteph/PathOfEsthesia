using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class InputsManager
{
    private Inputs inputs;
    public Inputs Inputs { get { return inputs; } }

    private static InputsManager instance = null;
    public static InputsManager Instance {
        get
        {
            if (instance == null)
            {
                instance = new InputsManager();
            }
            return instance;
        }
    }

    private InputsManager()
    {
        inputs = new Inputs();
        /*
        var array = inputs.Player.LookAround.bindings;
        InputBinding binding = array.First(i => i.GetNameOfComposite() == "OKLM");
        inputs.Player.LookAround.ChangeBinding(binding).WithName("OKLM(whichSideWins=1)");
        
        inputs.Player.AddCompositeBinding
            ("Axis(whichSideWins=1)");*/
    }

    private void SetGameControllerAsDevice()
    {

    }

    private void SetKeyboardMouseAsDevice()
    {

    }

    public static void Destroy()
    {
        instance = null;
    }
}
