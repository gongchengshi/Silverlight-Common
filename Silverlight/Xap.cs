// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Linq;

namespace SEL.Silverlight
{
    public class Xap
    {
        public static Stream GetStream(string uri)
        {
            return Application.GetResourceStream(new Uri(uri, UriKind.Relative)).Stream;
        }

        public Xap(Stream xapStream)
        {
            _stream = xapStream;
            _sri = new StreamResourceInfo(_stream, "application/binary");
        }

        private readonly Stream _stream;
        private readonly StreamResourceInfo _sri;

        public Assembly GetAssembly(string relativeUri)
        {
            var assemblySri = Application.GetResourceStream(_sri, new Uri(relativeUri, UriKind.Relative));

            using (assemblySri.Stream)
            {
                return new AssemblyPart().Load(assemblySri.Stream);
            }
        }

        public IEnumerable<Assembly> GetAssemblies()
        {
            var appManifestSri = Application.GetResourceStream(new StreamResourceInfo(_stream, null),
                                                               new Uri("AppManifest.xaml", UriKind.Relative));

            string appManifest;

            using (appManifestSri.Stream)
            {
                appManifest = new StreamReader(appManifestSri.Stream).ReadToEnd();
            }

            var assemblies = new List<Assembly>();

            foreach (var part in XDocument.Parse(appManifest).Root.Elements().Elements())
            {
                assemblies.Add(GetAssembly(part.Attribute("Source").Value));

                // For some reason this causes MEF to throw System.Reflection.ReflectionTypeLoadException, so don't use this.
                //yield return GetAssembly(part.Attribute("Source").Value);
            }

            return assemblies;

            // This also throws in the same manner, so don't use this.
            //return root.Elements().Elements().Select(part => GetAssembly(part.Attribute("Source").Value));
        }
    }
}