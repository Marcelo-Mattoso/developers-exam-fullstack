namespace Domain.Exceptions;

public class DomainValidationException : Exception
{
    public DomainValidationException(IEnumerable<string> errors)
        : base("Erro de validação.")
    {
        Errors = errors.ToArray();
    }

    public IReadOnlyCollection<string> Errors { get; }
}
