using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using SASgenerator.Services;

namespace SASgenerator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SAStokenController : ControllerBase
    {
        private readonly StorageService _storageService;

        public SAStokenController(StorageService storageService)
        {
            _storageService = storageService;
        }

        //[HttpGet("GetAccountSAS")]
        //public async Task<IActionResult> GetAccountSasToken()
        //{
        //    // Replace these values with your actual Storage Account name and key
        //    string accountName = "juantestsstorage";
        //    string accountKey = "";

        //    // Create StorageSharedKeyCredential
        //    StorageSharedKeyCredential sharedKey = new StorageSharedKeyCredential(accountName, accountKey);

        //    try
        //    {
        //        // Call the method to generate SAS token
        //        string sasToken = await StorageUtility.CreateAccountSAS(sharedKey);
        //        return Ok(sasToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred: {ex.Message}");
        //    }
        //}

        [HttpGet("GetContainerSAS")]
        public async Task<IActionResult> GetUserDelegationContainerSASAsync(string containerName)
        {
            // Create a Uri object with a service SAS appended
            try
            {
                string blobSASURI = await _storageService.CreateUserDelegationContainerSASAsync(containerName);

                return Ok(blobSASURI);
            } catch (Exception ex)
            {
                return StatusCode(401, $"An error occurred: {ex.Message}");
            }
          
        }

        [HttpGet("GetAccountSAS")]
        public async Task<IActionResult> GetUserDelegationAccountSASAsync()
        {
            // Create a Uri object with a service SAS appended
            try
            {
                //string blobSASURI = await _storageService.CreateUserDelegationAccountSASAsync();

                string blobSASURI = await _storageService.CreateAccountSAS();

                return Ok(blobSASURI);
            }
            catch (Exception ex)
            {
                return StatusCode(401, $"An error occurred: {ex.Message}");
            }

        }
    }
}
