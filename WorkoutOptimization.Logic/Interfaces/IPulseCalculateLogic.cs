namespace WorkoutOptimization.Logic.Interfaces
{
    public interface IPulseCalculateLogic
    {
        float Calculate(float age, float restPulse);
        void Dispose();
    }
}