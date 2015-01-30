namespace ATM
{
    internal interface IATMFactory
    {
        object Get();
    }

    internal interface IATMFactory<T> where T : class
    {
        T Get();
    }
}
