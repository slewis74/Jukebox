using System.Threading.Tasks;
using Jukebox.WinStore.Model;
using Slab.Data;

namespace Jukebox.WinStore.Storage
{
    public interface IMusicProvider
    {
        DistinctAsyncObservableCollection<Artist> Artists { get; }

        void LoadContent();

        Task<bool> ReScanMusicLibrary();
    }
}