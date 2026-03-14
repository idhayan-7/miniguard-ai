namespace MiniGuard.API.Services;

public interface ILogSimulator
{
    void WriteSingleEntry(string level, string service, string message);
    void ApplyRotation(string logPath, int maxLines);
    void InjectAnomalyBurst();
    string GenerateInfoEntry();
    string GenerateErrorEntry();
}
