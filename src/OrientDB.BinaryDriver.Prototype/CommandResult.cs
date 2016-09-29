using System.Collections.Generic;

namespace OrientDB.BinaryDriver.Prototype
{
    internal class CommandResult<T>
    {
        public IEnumerable<T> Results { get; }
    }
}