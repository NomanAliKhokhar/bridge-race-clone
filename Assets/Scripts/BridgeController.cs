using System;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [SerializeField] Step[] Steps;
    public Vector3 TopPosition => topPoint.position;

    [SerializeField] private Transform topPoint;


    int stepCount;
    public bool IsbridgeCompleted { get; private set; }
    public bool IsMarkBridgeUnUsed { get; private set; }


#if UNITY_EDITOR
    private void OnValidate()
    {
        Steps = GetComponentsInChildren<Step>();
    }
#endif

    private void Awake()
    {
        stepCount = Steps.Length;
    }

    public void SetAllStepsToNatural()
    {
        foreach (Step step in Steps)
        {
            step.SetNatural();
        }
    }
    public int GetColorCount(Color color)
    {
        int counter = 0;
        for (int i = 0; i < stepCount; i++)
        {
            if (Steps[i].Color.Equals(color))
                counter++;
        }
        return counter;
    }

    public void MarkBridgeCompleted()
    {
        IsbridgeCompleted = true;
        topPoint.gameObject.SetActive(false);
    }

    public void MarkBridgeUnUsed()
    {
        IsMarkBridgeUnUsed = true;
        topPoint.gameObject.SetActive(false);
    }

}
