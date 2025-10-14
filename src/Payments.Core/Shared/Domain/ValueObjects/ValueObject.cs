namespace Payments.Core.Shared.Domain.ValueObjects
{
    public interface IValueObject<out TPrimitive>
    {
        TPrimitive Value { get; }
    }
}
