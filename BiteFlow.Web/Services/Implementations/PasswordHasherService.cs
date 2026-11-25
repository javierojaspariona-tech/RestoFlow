using System.Security.Cryptography;
using BiteFlow.Web.Services.Interfaces;

namespace BiteFlow.Web.Services.Implementations;

/// <summary>
/// PBKDF2-HMAC-SHA256 con sal aleatoria. Formato guardado:
///   PBKDF2-SHA256$100000$&lt;salt base64&gt;$&lt;hash base64&gt;
/// (Los usuarios de prueba de Database/02_seed.sql usan este mismo formato.)
/// </summary>
public class PasswordHasherService : IPasswordHasherService
{
    private const string Prefijo = "PBKDF2-SHA256";
    private const int TamanoSal = 16;
    private const int TamanoHash = 32;
    private const int Iteraciones = 100_000;

    public string Hash(string password)
    {
        var sal = RandomNumberGenerator.GetBytes(TamanoSal);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, sal, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);
        return $"{Prefijo}${Iteraciones}${Convert.ToBase64String(sal)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string hashAlmacenado)
    {
        var partes = hashAlmacenado.Split('$');
        if (partes.Length != 4 || partes[0] != Prefijo || !int.TryParse(partes[1], out var iteraciones) || iteraciones <= 0)
            return false;

        try
        {
            var sal = Convert.FromBase64String(partes[2]);
            var esperado = Convert.FromBase64String(partes[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, sal, iteraciones, HashAlgorithmName.SHA256, esperado.Length);
            return CryptographicOperations.FixedTimeEquals(actual, esperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
