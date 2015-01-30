namespace ATM
{
    public interface IATMOptions
    {
        IATMOptions AsSingleton();
        IATMOptions WithParameters(object paramObj);
    }
}
