using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DocumentEngine;
using DocumentEngine.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DocumentEngine.Models;

namespace Services.Test
{
    [TestClass]
    public class DocumentEngineTest
    {
        private IDocumentActions _documentActions;
        private IConfiguration _configuration;
        private ILogger _logger;

        [TestInitialize]
        public void Initialize()
        {
            _configuration = Substitute.For<IConfiguration>();
            _documentActions = new BlobActions("_Enter_AzureStorageName_", "_Enter_AzureStorageKey_", "_containerName_", 3, _logger);
        }

        [TestMethod]
        public void UploadDocumentTest()
        {
            var result = _documentActions.UploadTextAsync("Test5.txt", "ajhshdksf", "123Test").Result;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UploadDocumentsTest()
        {
            var documents = new List<DocumentDetail>();

            documents.Add(new DocumentDetail
            {
                Name = "Test1.txt",
                Data = "Test1.txt"
            });

            documents.Add(new DocumentDetail
            {
                Name = "Test2.txt",
                Data = "Test2.txt"
            });

            //parallel
            Parallel.ForEach(documents, (a) =>
            {
                var result =  _documentActions.UploadTextAsync(a.Name, a.Data, "123Test").Result;
                Assert.AreEqual(result.Success, true);
            });
        }

        [TestMethod]
        public void UploadDocumentByteTest()
        {
            var fileBytes = File.ReadAllBytes(@"..\..\..\Documents\Apache Tika.docx");
            var result = _documentActions.UploadBytesAsync("Apache Tika.docx", fileBytes, "Test-Bytes").Result;
            Assert.IsNotNull(result.BlobUri);
        }

        [TestMethod]
        public void BulkUploadDocumentsTest()
        {
            var documents = new List<DocumentDetail>();

            documents.Add(new DocumentDetail
            {
                Name = "Test1.txt",
                Data = "Test1.txt"
            });

            documents.Add(new DocumentDetail
            {
                Name = "Test2.txt",
                Data = "Test2.txt"
            });

            var result = _documentActions.BulkUploadTextAsync(documents).Result;

            result.ForEach(a =>
            {
                Assert.IsNotNull(a.BlobUri);
            });
        }        

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void DownloadNonExistingFileTest()
        {
            var result = _documentActions.DownloadTextAsync("xyz.txt", "123Test").Result;
        }

        [TestMethod]
        public void DownloadTest()
        {
            var result = _documentActions.DownloadTextAsync("Test1.txt", "123Test").Result;
            Assert.AreEqual(result.Success, true);
        }

        [TestMethod]
        public void DownloadApacheByteTest()
        {
            var result = _documentActions.DownloadByteAsync("Apache Tika.docx", "Test-Bytes").Result;
            Assert.AreEqual(result.Success, true);
        }        

        [TestMethod]
        public async Task DeleteDocumentTest()
        {
            var result = await _documentActions.DeleteAsync("Test2.txt", "123Test");
            Assert.AreEqual(true, result.Success);

            var result1 = await _documentActions.DeleteAsync("Test2.txt", "123Test");
            Assert.AreEqual(false, result1.Success);
        }
    }
}
