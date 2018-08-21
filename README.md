# AzureBlobOperations

This repository is used to implement Azure Blob operations.

Consists of two projects 
1. DocumentEngine: has all the logic to perform blob operations 
2. DocumentEngineTest: has unit tests to test blob operations.

Blob operations which are covered in this repository are:
1.	Upload a single file – with bytes or text content
2.	Bulk upload files – with bytes or text content
Using parallel programming to upload number of files parallelly
3.	Download file – bytes or text
4.	Delete a file

Using [Polly](https://github.com/App-vNext/Polly) for retry mechanism.

To instantiate the BlobActions class you need azure storage account, storage key, container name, max retry value to perform blob operations and logger object
```
_documentActions = new BlobActions("_Enter_AzureStorageName_", "_Enter_AzureStorageKey_", "_containerName_", "_maxRetryValue_", _logger);
```
Enter these values and you will be able to perform the blob operations.

It is wriiten in asp.net core framework
