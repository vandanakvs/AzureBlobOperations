using Autofac;
using DocumentEngine.Interface;
using System.Globalization;

namespace DocumentEngine
{
    public class DocumentManagerFactory : IDocumentManagerFactory
    {
        private readonly IComponentContext _componentContext;

        public DocumentManagerFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public IDocumentActions Instance(string documentManager)
        {
            IDocumentActions instance = _componentContext.ResolveNamed<IDocumentActions>(documentManager.ToLower(CultureInfo.InvariantCulture));
            return instance;
        }
    }
}
