using DocumentEngine.Interface;
using System;

namespace DocumentEngine
{
    public class CommonBase : ICommonBase
    {
        public virtual string CreateFileNameWithTimeStamp(string fileName, string delimiter = "_")
        {
            return fileName + delimiter + DateTime.Now;
        }
    }
}
