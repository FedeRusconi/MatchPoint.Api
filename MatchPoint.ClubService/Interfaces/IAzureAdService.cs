using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace MatchPoint.ClubService.Interfaces
{
    public interface IAzureAdService
    {
        /// <summary>
        /// Get an instance of <see cref="GraphServiceClient"/>.
        /// </summary>
        /// <param name="scopes"> Any necessary scopes. If null, the .defaul scopes for ms graph will be requested. </param>
        /// <returns> An instance of <see cref="GraphServiceClient"/>. </returns>
        GraphServiceClient GetGraphClient(string[]? scopes = null);

        /// <summary>
        /// Get an AzureAd <see cref="User"/> by its Id.
        /// </summary>
        /// <param name="userId"> The Id of the user to find. </param>
        /// <param name="client"> 
        /// The <see cref="GraphServiceClient"/> instance to use. 
        /// If null, a default client is used.
        /// </param>
        /// <returns> The <see cref="User"/> instance with information from AzureAd. </returns>
        Task<User?> GetUserByIdAsync(Guid userId, GraphServiceClient? client = null);

        /// <summary>
        /// Get an AzureAd <see cref="User"/>'s profile picture.
        /// </summary>
        /// <param name="userId"> The Id of the user to find. </param>
        /// <param name="client"> 
        /// The <see cref="GraphServiceClient"/> instance to use. 
        /// If null, a default client is used.
        /// </param>
        /// <returns> The <see cref="Stream"/> of the image file. </returns>
        /// <exception cref = "HttpRequestException"></exception>
        Task<Stream?> GetUserPhotoAsync(Guid userId, GraphServiceClient? client = null);

        /// <summary>
        /// Get an AzureAd <see cref="User"/>'s assigned manager info.
        /// </summary>
        /// <param name="userId"> The Id of the user to find. </param>
        /// <param name="client"> 
        /// The <see cref="GraphServiceClient"/> instance to use. 
        /// If null, a default client is used.
        /// </param>
        /// <returns> The <see cref="User"/> instance with the manager info from Azure Ad. </returns>
        Task<User?> GetUserManagerAsync(Guid userId, GraphServiceClient? client = null);

        /// <summary>
        /// Create a new <see cref="User"/> in AzureAd.
        /// </summary>
        /// <param name="user"> The <see cref="User"/> instance to create in AzureAd. </param>
        /// <param name="client"> 
        /// The <see cref="GraphServiceClient"/> instance to use. 
        /// If null, a default client is used.
        /// </param>
        /// <returns> The <see cref="User"/> instance created in AzureAd. </returns>
        Task<User?> CreateUserAsync(User user, GraphServiceClient? client = null);
    }
}
