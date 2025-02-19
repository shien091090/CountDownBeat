namespace GameCore
{
    public interface IFeverEnergyBarView
    {
        void SetFeverStage(int newFeverStage);
        void SetCurrentFeverEnergy(int currentFeverEnergy);
        void PlayFeverEnergyIncreaseEffect();
        void PlayFeverEnergyDecreaseEffect();
        void PlayShowAccuracyHintEffect(float accuracyValue);
    }
}