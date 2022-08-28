using System;
using System.Threading.Tasks;

namespace Slidecrew_Interfaces
{
    /// <summary>
    /// Enum indicating if a FileProvider gets its data from the local network or externalDB/network
    /// </summary>
    public enum FileProviderType
    {
        LocalDB,
        LocalNetwork,
        ExternalDB
    }
    public interface IFileProvider
    {
        FileProviderType ProviderType { get; }
        void Setup();

        /// <summary>
        /// Get file from source (local/external)
        /// </summary>
        Task<bool> GetFile();
    }
}
