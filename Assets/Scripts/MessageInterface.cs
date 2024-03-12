using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MessageInterface
{
    //This is a test
    Tuple<string, string> GetMessages();
    public void AddMessage(string message);

    public void RemoveMessage(string message);
}
