using ApiServer.Models;
using ApiServer.Services;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Text;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using Xunit;
using Xunit.Abstractions;

namespace unitTest
{
    public class FileServiceShould
    {
        private readonly IDatabaseSettings databaseSettings = new DatabaseSettings{
            ConnectionString = "mongodb+srv://admin:FlyingDonkeyIT@cluster0.khvon.mongodb.net/myFirstDatabase?retryWrites=true&w=majority",
            // different data base for unit testing from the database used in development env
            DatabaseName = "test"
        };
        private readonly ITestOutputHelper _testOutputHelper;

        public FileServiceShould(
            ITestOutputHelper testOutputHelper
        ) {
            _testOutputHelper = testOutputHelper;
        }
        private readonly IAWSS3Settings aWSS3Settings = new AWSS3Settings{
            Url = "https://test-task-2021.s3.ap-southeast-2.amazonaws.com",
            AccessKey = "AKIA442DLO7IJKUZZ64J",
            AccessSecret = "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"  
        };

        [Fact]
        public void FilterFilesWithNoType()
        {
            var FileService = new FileService(
                databaseSettings,
                aWSS3Settings
            );
            // Should be empty when type is null
            Assert.Equal(0, (int)FileService.FilterFiles("", "test").Count);
        }

        [Fact]
        public void FilterFilesWithTypeTxtAndNoName()
        {
            var FileService = new FileService(
                databaseSettings,
                aWSS3Settings
            );
            // Should be get element id = 6112764b555b3f8e84f28d98 when name is empty and type is txt 
            var files = FileService.FilterFiles("text", null);
            Assert.Equal(1, (int)files.Count);
            Assert.Equal("6112764b555b3f8e84f28d98", files[0].Id);
        }

        [Fact]
        public void FilterFilesWithWrongType()
        {
            var FileService = new FileService(
                databaseSettings,
                aWSS3Settings
            );
            // Should be empty when type is a wrong file type test
            var files = FileService.FilterFiles("test", null);
            Assert.Equal(0, (int)files.Count);
        }

        [Fact]
        public async void UploadFileWithNoFile()
        {
            var FileService = new FileService(
                databaseSettings,
                aWSS3Settings
            );
            // Should return UploadResult that success is false
            var result = await FileService.UploadFile(null, 1204, "application", DateTime.Now.ToString());
            Assert.False(result.success);
        }

        [Fact]
        public async void UploadFileWithPDF()
        {
            var FileService = new FileService(
                databaseSettings,
                aWSS3Settings
            );
            // Should return UploadResult 
            var content = "This is a test pdf";
            var fileName = "test.pdf";
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "test", fileName){
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };
            var result = await FileService.UploadFile(file , 50000, "application", DateTime.Now.ToString());
            Assert.True(result.success);
            Assert.Equal("test.pdf", result.file.OriginalName);
            Assert.Equal(18, result.file.Size);
            Assert.Equal("application", result.file.GeneralType);
        }
    }
}