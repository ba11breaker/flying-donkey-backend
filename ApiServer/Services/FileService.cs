using ApiServer.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using IO = System.IO;
using Amazon.S3;
using Amazon.S3.Model;

namespace ApiServer.Services
{
    public class FileService
    {
        private readonly IMongoCollection<File> _files;
        private readonly AmazonS3Client _s3Client;

        private readonly string _awsUrl;
        private readonly string BUCKET_NAME = "test-task-2021";
        private readonly string FilesCollectionName = "Files";
        private readonly Dictionary<string, string> _generalTypes;

        public FileService(
            IDatabaseSettings databaseSettings,
            IAWSS3Settings aWSS3Settings
        ) 
        {
            var client = new MongoClient(databaseSettings.ConnectionString);            
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _awsUrl = aWSS3Settings.Url;
            _files = database.GetCollection<File>(FilesCollectionName);
            _s3Client = new AmazonS3Client(aWSS3Settings.AccessKey, aWSS3Settings.AccessSecret, Amazon.RegionEndpoint.APSoutheast2);
            _generalTypes = new Mime().types;
        }
        public List<File> GetFiles() => _files.Find(GetFiles => true).ToList();
        public File GetFile(string id) => _files.Find<File>(file => file.Id == id).FirstOrDefault();

        public List<File> SearchFilesByName(string name) {   
            return _files.Find(file => file.OriginalName.Contains(name)).ToList();
        }

        public List<File> FilterFiles (string type, string name) {
            var filter = Builders<BsonDocument>.Filter;
            
            
            return _files.Find(file => file.GeneralType==type & file.OriginalName.Contains(name==null?"":name)).ToList();
        }

        public File Create(File file)
        {
            _files.InsertOne(file);
            return file;
        }
        
        public void Remove(File fileIn) => _files.DeleteOne(file => file.Id == fileIn.Id);

        public List<string> getGeneralTypes() 
        {
            List<string> arr = new List<string>();
            foreach(string key in _generalTypes.Keys)
            {
                arr.Add(key);
            }
            return arr;
        }

        // Upload files to AWS S3
        public async Task<UploadResult> UploadFile(IFormFile file, Int64 allowedSize, string allowedType)
        {
            try {
                string fileName = file.FileName;
                string type = file.ContentType;
                Int64 size = file.Length;
                if (getGeneralType(type) != allowedType) {
                    return new UploadResult{
                        success = false,
                        message = $"{fileName} is {type}, not consistent with {allowedType}. Please choose {getGeneralType(type)} type!"
                    };
                }
                if (!validateType(type)) {
                    return new UploadResult{
                        success = false,
                        message = $"{getGeneralType(type)} is an invalid type!"
                    };
                }
                if (size > allowedSize) {
                    return new UploadResult{
                        success = false,
                        message = $"The size of {fileName} is {this.sizeFormatter(size)} bytes, more than {this.sizeFormatter(allowedSize)} bytes!"
                    };
                }
                string filePath = $"{type}/{DateTime.Now.Ticks}_{file.FileName}";
                using (IO.Stream fileUpload = file.OpenReadStream())
                {
                    // upload file to aws s3 by bucket name and file path
                    var request = new PutObjectRequest();
                    request.BucketName = BUCKET_NAME;
                    request.Key = filePath;
                    request.InputStream = fileUpload;
                    request.ContentType = type;
                    

                    var response = await _s3Client.PutObjectAsync(request);

                    string url = $"{_awsUrl}/{filePath}";
                    File newFile = new File{
                        Name = filePath,
                        OriginalName = fileName,
                        Url = url,
                        Type = type,
                        GeneralType = getGeneralType(type),
                        Size = size,
                        TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ffff")
                    };
                    Create(newFile);
                    return new UploadResult{
                        success = true,
                        message = $"upload a new file {fileName} successfully!",
                        file = newFile
                    };
                }
            } catch(Exception e)
            {   
                return new UploadResult{
                    success = false,
                    message = e.ToString()
                };
            }
        }

        // check whether the general type is valid
        public bool checkType(string contentType)
        {
            return _generalTypes.ContainsKey(contentType);
        }

        // check Content Type
        private bool validateType(string contentType)
        {
            string[] types = contentType.Split('/');
            return _generalTypes.ContainsKey(types[0]);
        }

        // get general content type
        private string getGeneralType(string contentType)
        {
            string[] types = contentType.Split('/');
            return types[0];
        }

        // private file size formatter
        private string sizeFormatter(Int64 _size) {
            decimal size = (decimal)_size;
            if (size < 1024) {
                return $"{_size} bytes";
            }
            if (size < 1048576) {
                return $"{Math.Round(size/1024, 2)} KB";
            }
            return $"{Math.Round(size/1048576, 2)} MB";
        }
    }

    public class UploadResult
    {
        public bool success {get; set;}
        public string message {get; set;}
        public File file {get; set;}
    }
}