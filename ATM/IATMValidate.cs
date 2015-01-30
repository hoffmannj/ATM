using System.Collections.Generic;

namespace ATM
{
    internal interface IATMValidate
    {
        bool Validated { get; }

        bool Validate(List<IATMap> chain);
    }
}
