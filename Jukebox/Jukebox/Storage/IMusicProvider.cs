using System.Threading.Tasks;
using Jukebox.Model;
using Slab.Data;

namespace Jukebox.Storage
{
    public interface IMusicProvider
    {
        DistinctAsyncObservableCollection<Artist> Artists { get; }

        void LoadContent();

        Task<bool> ReScanMusicLibrary();
    }
}