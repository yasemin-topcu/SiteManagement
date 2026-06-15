namespace SiteManagement.Application.Abstractions;

public interface IJoinCodeGenerator
{
    string Generate(int length = 6);
}