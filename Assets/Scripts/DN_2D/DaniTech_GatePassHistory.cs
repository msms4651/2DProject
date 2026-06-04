using System.Collections.Generic;
using UnityEngine;

public class DaniTech_GatePassHistory : MonoBehaviour
{
  
    private readonly HashSet<int> _passedGGateIds = new HashSet<int>();

    public bool HasPassedGate(int gateId)
    {
        return _passedGGateIds.Contains(gateId);
    }

    public void MarkPassedGate(int gateId)
    {
        if (_passedGGateIds.Contains(gateId))
        {
            return;
        }

        _passedGGateIds.Add(gateId);
    }

}
