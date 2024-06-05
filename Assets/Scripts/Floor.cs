using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] public int floorIndex = 0;

   [SerializeField] private BridgeController[] bridges;
    [SerializeField] private BrickSpawner brickSpawners;

#if UNITY_EDITOR
    private void OnValidate()
    {
        bridges = GetComponentsInChildren<BridgeController>();
        brickSpawners = GetComponentInChildren<BrickSpawner>();
    }
#endif

    public void Init()
    {
        foreach (BridgeController bridge in bridges)
        {
            bridge.SetAllStepsToNatural();
        }
        brickSpawners.Init();
    }

    public BridgeController FindBestBridge(Color characterColor)
    {
        BridgeController bestBridge = bridges[Random.Range(0, bridges.Length)];
        int maxCount = 0;
        foreach (BridgeController bridge in bridges)
        {
            int count = bridge.GetColorCount(characterColor);
            if (count > maxCount)
            {
                bestBridge = bridge;
                maxCount = count;
            }
        }
        return bestBridge;

    }

    public void DisableAllBridgePoints(BridgeGate bridgeGate)
    {
        foreach(BridgeController bridge in bridges)
        {
            if(bridgeGate.GetComponentInParent<BridgeController>() == bridge)
            {
                bridge.MarkBridgeCompleted();
            }else
            {
                bridge.MarkBridgeUnUsed();
            }
        }
    }


}
