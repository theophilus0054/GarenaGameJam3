using UnityEngine;

public interface ILevelEvent
{
    bool IsActive { get; }

    void Activate();
    void Complete();
}
