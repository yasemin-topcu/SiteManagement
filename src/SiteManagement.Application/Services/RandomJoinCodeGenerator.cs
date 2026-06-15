using SiteManagement.Application.Abstractions;

namespace SiteManagement.Application.Services;

public class RandomJoinCodeGenerator : IJoinCodeGenerator
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // karışıklık olmasın diye I/O/1/0 yok
    private readonly Random _random = new();

    public string Generate(int length = 6)
    {
        var chars = new char[length];
        for (int i = 0; i < length; i++)
            chars[i] = Alphabet[_random.Next(Alphabet.Length)];
        return new string(chars);
    }
}