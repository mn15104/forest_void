using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyEvent<T> {

    public delegate void GenericEvent<S>(S info);
    public event GenericEvent<T> NotifyEventOccurred = delegate { };

	public void Notify(T info)
    {
        NotifyEventOccurred(info);
    }
}

