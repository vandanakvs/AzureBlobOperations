using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentEngine.Interface
{
    public interface IDocumentManagerFactory
    {
        IDocumentActions Instance(string documentManager);
    }
}
