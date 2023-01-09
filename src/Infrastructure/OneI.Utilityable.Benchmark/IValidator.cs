namespace OneI.Utilityable;

public interface IValidator
{
#if DEBUG
    void Validate();
#endif
}
