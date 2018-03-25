using MichaelBigos.HashSync.Models;

namespace MichaelBigos.HashSync.Services
{
    public interface IInputValidationService
    {
        bool Validate(CommandLineArgumentInput input);
    }
}