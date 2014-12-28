using System.Threading.Tasks;
using Jukebox.WinStore.Model;
using Slab.Data;

namespace Jukebox.WinStore.Storage
{
    public interface IMusicProvider
    {
        DistinctAsyncObservableCollection<Artist> Artists { get; }

        Task LoadContent();

        Task<bool> ReScanMusicLibrary();
    }
}