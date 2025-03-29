using GameCore;

public interface ICheckmarkDetectorView
{
    void SetSecondTriggerAreaActive(CheckmarkSecondTriggerAreaType secondTriggerAreaType, bool isActive);
    void HideAllFirstTriggerArea();
    void HideAllSecondTriggerArea();
    void ShowAllFirstTriggerArea();
    void InitTriggerArea();
}