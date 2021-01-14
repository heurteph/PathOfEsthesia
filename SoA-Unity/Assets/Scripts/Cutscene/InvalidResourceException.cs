using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvalidResourceException : System.IO.IOException
{
    public InvalidResourceException(string name, string path) : base(string.Format("Resources '{0}' could not be loaded from the resources folder at '{1}'", name, path))
    {

    }
}