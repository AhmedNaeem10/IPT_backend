using Firebase.Auth;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Netflix_backend.Data
{
    public class FileUploader
    {

        public async Task<string> FileUpload(dynamic File, string ApiKey, string email, string password, string Bucket)
        {

            Stream myBlob = new MemoryStream();
            myBlob = File.OpenReadStream();

            var auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(email, password);
            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                    })
                .Child("data")
                .Child("project")
                .Child(File.FileName)
                .PutAsync(myBlob, cancellation.Token);

            return await task;
        }
        public async Task<string> FileDelete(string File, string ApiKey, string email, string password, string Bucket)
        {

            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(email, password);
            string f = File.Split("%2F")[2].Split('?')[0];
            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
                .Child("data").Child("project").Child(f).DeleteAsync();

            return null;
        }
    }
}