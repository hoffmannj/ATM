using System;

namespace ATM
{
    public class ATMException : Exception
    {
        public ATMException() : base() { }
        public ATMException(string message) : base(message) { }
    }

    public class WrongNumberOfMappingsException : ATMException
    {
        public WrongNumberOfMappingsException(string message) : base(message) { }
    }

    public class RecursiveDependenciesException : ATMException
    {
        public RecursiveDependenciesException(string message) : base(message) { }
    }

    public class CantResolveDependenciesException : ATMException
    {
        public CantResolveDependenciesException(string message) : base(message) { }
    }
}
