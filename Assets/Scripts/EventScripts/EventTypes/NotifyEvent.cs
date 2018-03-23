using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyEvent<T> {

    public delegate void GenericEvent<S>(S info);
    public event GenericEvent<T> NotifyEventOccurred = delegate { };

	public void Notify(T info)
    {
        Debug.Log("Notify");
        Debug.Log(info);
        NotifyEventOccurred(info);
    }
}

