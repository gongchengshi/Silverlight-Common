// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using SEL.Silverlight;

namespace SEL.MEF
{
    public interface ICatalogService
    {
        void AddRemoteXap(string uri);
        void AddLocalXap(string uri);
        void RemoveXap(string uri);
    }

    [Export(typeof (ICatalogService))]
    public class CatalogService : ICatalogService
    {
        private static readonly AggregateCatalog _aggregateCatalog = new AggregateCatalog();

        static CatalogService()
        {
            CompositionHost.Initialize(_aggregateCatalog);
        }

        // Used to find the catalog given its source URI
        private readonly Dictionary<string, ComposablePartCatalog> _catalogs =
            new Dictionary<string, ComposablePartCatalog>();

        public void AddThisXap()
        {
            ComposablePartCatalog catalog;

            var key = Application.Current.ToString();

            if (!_catalogs.TryGetValue(key, out catalog))
            {
                catalog = new DeploymentCatalog();

                _catalogs[key] = catalog;
            }

            _aggregateCatalog.Catalogs.Add(catalog);
        }

        public void AddLocalXap(string file)
        {
            using (var stream = Xap.GetStream(file))
            {
                var assemblies = new Xap(stream).GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    ComposablePartCatalog catalog;

                    var key = assembly.ToString();

                    if (!_catalogs.TryGetValue(key, out catalog))
                    {
                        catalog = new AssemblyCatalog(assembly);
                        _catalogs[key] = catalog;
                    }

                    _aggregateCatalog.Catalogs.Add(catalog);
                }
            }
        }

        public void AddRemoteXap(string uri)
        {
            ComposablePartCatalog catalog;
            if (!_catalogs.TryGetValue(uri, out catalog))
            {
                catalog = new DeploymentCatalog(uri);

                var deploymentCatalog = catalog as DeploymentCatalog;

                deploymentCatalog.DownloadCompleted += CatalogDownloadCompleted;

                deploymentCatalog.DownloadAsync();
                _catalogs[uri] = catalog;
            }
            _aggregateCatalog.Catalogs.Add(catalog);
        }

        private void CatalogDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null) return;

            if (e.Error.InnerException is ReflectionTypeLoadException)
            {
                var loaderMessages = new StringBuilder();
                loaderMessages.AppendLine(
                    "While trying to load composable parts the following loader exceptions were found: ");
                foreach (var loaderException in (e.Error.InnerException as ReflectionTypeLoadException).LoaderExceptions)
                {
                    loaderMessages.AppendLine(loaderException.Message);
                }

                Debug.WriteLine(loaderMessages);
            }
            else if (e.Error.InnerException is NotSupportedException)
            {
                // Couldn't find the xaps.
            }
            else
            {
#if DEBUG
                Debugger.Break();
#endif
            }
        }

        public void RemoveXap(string uri)
        {
            ComposablePartCatalog catalog;
            if (_catalogs.TryGetValue(uri, out catalog))
            {
                _aggregateCatalog.Catalogs.Remove(catalog);
                _catalogs.Remove(uri);
            }
        }
    }
}