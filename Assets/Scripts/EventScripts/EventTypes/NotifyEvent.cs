using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyEvent<T> : ScriptableObject {



    public delegate void GenEvent<K>(K info);
    public event GenEvent<T> notifyEvent;

	public void Notify(T info)
    {
        notifyEvent(info);
    }
}
