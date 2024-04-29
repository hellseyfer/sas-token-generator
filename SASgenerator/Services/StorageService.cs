using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;

namespace SASgenerator.Services
{
    public class StorageService
    {
        private BlobServiceClient _blobServiceClient;
        private IConfiguration _configuration;

        public StorageService(BlobServiceClient blobServiceClient, IConfiguration configuration)
        {
            _blobServiceClient = blobServiceClient;
            _configuration = configuration;
        }
        public async Task<string> CreateAccountSAS()
        {
            var accountName = _configuration.GetValue<string>("ACCOUNT_STORAGE_NAME");
            var accountKey = _configuration.GetValue<string>("ACCOUNT_STORAGE_KEY");
            StorageSharedKeyCredential storageSharedKeyCredential =
                new(accountName, accountKey);


            // Your provided code goes here
            AccountSasBuilder sasBuilder = new AccountSasBuilder()
            {
                Services = AccountSasServices.Blobs,
                ResourceTypes = AccountSasResourceTypes.Object,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
                Protocol = SasProtocol.Https,
            };

            sasBuilder.SetPermissions(AccountSasPermissions.Read);

            // Use the key to get the SAS token
            string sasToken = sasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();

            return sasToken;
        }

        public async Task<string> CreateUserDelegationContainerSASAsync(
            string containerName,
            string? storedPolicyName = null)
        {
            BlobContainerClient containerClient = _blobServiceClient
               .GetBlobContainerClient(containerName);

            // Get a user delegation key for the Blob service that's valid for 1 day
            UserDelegationKey userDelegationKey =
                await _blobServiceClient.GetUserDelegationKeyAsync(
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow.AddHours(1));


            // Create a SAS token that's valid for one day
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerClient.Name,
                Resource = "b"  // For container-level access, set the resource type to "b" (blob) and specify the container name.
            };

            if (storedPolicyName == null)
            {
                sasBuilder.StartsOn = DateTimeOffset.UtcNow.AddMinutes(-10);
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
                sasBuilder.CacheControl = "max-age=600";
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }

            // Add the SAS token to the blob URI
            BlobUriBuilder uriBuilder = new BlobUriBuilder(containerClient.Uri)
            {
                // Specify the user delegation key
                Sas = sasBuilder.ToSasQueryParameters(
                    userDelegationKey,
                    containerClient
                    .GetParentBlobServiceClient().AccountName
                    )
            };

            return uriBuilder.Sas.ToString();
            
        }

        //public async Task<string> CreateUserDelegationAccountSASAsync(
        //    string? storedPolicyName = null)
        //{

        //    // Get a user delegation key for the Blob service that's valid for 1 day
        //    UserDelegationKey userDelegationKey =
        //        await _blobServiceClient.GetUserDelegationKeyAsync(
        //            DateTimeOffset.UtcNow,
        //            DateTimeOffset.UtcNow.AddHours(1));


        //    // Create a SAS token that's valid for one day
        //    BlobSasBuilder sasBuilder = new BlobSasBuilder()
        //    {
        //        Resource = "c", // For account-level access, set the resource type to "c" (container) and specify the desired permissions (e.g., read, write, list).
        //    };

        //    if (storedPolicyName == null)
        //    {
        //        sasBuilder.StartsOn = DateTimeOffset.UtcNow.AddMinutes(-10);
        //        sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
        //        // Set the permissions for the account, e.g., read, write, list
        //        sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
        //        sasBuilder.CacheControl = "max-age=600";
        //    }
        //    else
        //    {
        //        sasBuilder.Identifier = storedPolicyName;
        //    }

        //    // Add the SAS token to the blob URI
        //    BlobUriBuilder uriBuilder = new BlobUriBuilder(_blobServiceClient.Uri)
        //    {
        //        // Specify the user delegation key
        //        Sas = sasBuilder.ToSasQueryParameters(
        //            userDelegationKey,
        //            _blobServiceClient.AccountName
        //            )
        //    };

        //    return uriBuilder.Sas.ToString();

        //}

    }
}
