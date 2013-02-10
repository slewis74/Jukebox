using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace Slew.WinRT.Data.Navigation
{
    public class NavigationStackStorage : INavigationStackStorage
    {
        public void StoreUris(string[] uris)
        {
            ApplicationData.Current.LocalSettings.DeleteContainer("Navigation");
            var navContainer = ApplicationData.Current.LocalSettings.CreateContainer("Navigation", ApplicationDataCreateDisposition.Always);

            var count = uris.Count();

            navContainer.Values["UriCount"] = count;
            
            for (var i = 0; i < count; i++)
            {
                navContainer.Values["Uri" + i] = uris[i];
            }
        }

        public string[] RetrieveUris()
        {
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("Navigation") == false)
                return null;

            var navContainer = ApplicationData.Current.LocalSettings.CreateContainer("Navigation", ApplicationDataCreateDisposition.Always);

            var count = Convert.ToInt32(navContainer.Values["UriCount"]);
            var results = new List<string>();

            // read the entries back in reverse index order, so the stack comes out the right
            // way around.
            for (var i = 0; i < count; i++)
            {
                results.Add(navContainer.Values["Uri" + (count - 1 - i)].ToString());
            }
            return results.ToArray();
        }
    }
}