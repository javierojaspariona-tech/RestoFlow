using System.Security.Cryptography;
namespace BiteFlow.Web.Data;
public class PasswordHasher
{
    private const int Iterations = 100_000;
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return $"PBKDF2-SHA256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }
    public bool Verify(string password, string stored)
    {
        var parts = stored.Split('$');
        if (parts.Length != 4 || parts[0] != "PBKDF2-SHA256" || !int.TryParse(parts[1], out var iterations)) return false;
        try {
            var salt=Convert.FromBase64String(parts[2]); var expected=Convert.FromBase64String(parts[3]);
            var actual=Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        } catch { return false; }
    }
}
