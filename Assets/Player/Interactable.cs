using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    public static Interactable currentSelected {  get; private set; }

    public UnityEvent OnSelect = new UnityEvent();
    public UnityEvent OnUnselect = new UnityEvent();
    public UnityEvent OnAction = new UnityEvent();

    [SerializeField]
    List<GameObject> highLightObjects = new List<GameObject>();
    List<int> highLightObjectsLayer = new List<int>();

    [SerializeField]
    int OutlineLayer=6;

    int currentLayer;

    public bool isSelected { get; private set; }

    private void Start()
    {
        currentLayer = gameObject.layer;
        for (int i= 0;i<highLightObjects.Count;i++)
        {
            highLightObjectsLayer.Add(highLightObjects[i].layer);
        }
    }

    public void Select()
    {
        if (isSelected)
            return;
        isSelected = true;
        if (currentSelected != null)
        {
            currentSelected.Unselect();
        }
        OnSelect?.Invoke();
        currentSelected = this;
        foreach (GameObject go in highLightObjects)
        {
            go.layer = OutlineLayer;

        }
    }

    public void Unselect()
    {
        isSelected = false;
        if (currentSelected == this)
        {
            currentSelected = null;
        }

        OnUnselect?.Invoke();


        for (int i = 0; i < highLightObjects.Count; i++)
        {
            highLightObjects[i].layer=highLightObjectsLayer[i];
        }

    }

    public void DoAction()
    {
        if (!isSelected)
        {
            Debug.LogError("Trying to do action without selection.");
            return;
        }
        try
        {
            OnAction?.Invoke();

        }catch(Exception e)
        {
            Debug.LogError("Error in '" + name + "'" + e.ToString());
        }


    }


    




}
