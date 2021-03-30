using VeritecCodingAssignment.Models;

namespace VeritecCodingAssignment.Services
{
    public interface ICommandReaderService
    {
        public decimal ReadSalaryPackageFromConsole();

        public PayFrequency ReadPayFrequencyFromConsole();
    }
}
