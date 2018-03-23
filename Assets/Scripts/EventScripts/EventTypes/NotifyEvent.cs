using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyEvent<T,S> : ScriptableObject {



    public delegate void GenEvent<K,J>(K info, J type);
    public event GenEvent<T,S> notifyEvent;

	public void Notify(T info, S type)
    {
        notifyEvent(info, type);
    }
}
