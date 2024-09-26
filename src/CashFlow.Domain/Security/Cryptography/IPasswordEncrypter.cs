namespace CashFlow.Domain.Security.Cryptography;
public interface IPasswordEncrypter
{
    string Encripty(string password);
    bool Verify(string password, string passwordHash);
}
