using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentEngine.Interface
{
    public interface ICommonBase
    {
        string CreateFileNameWithTimeStamp(string fileName, string delimiter = "_");
    }
}
