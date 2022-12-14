using Firebase.Auth;
using Firebase.Storage;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;

namespace DocumentManager
{
    internal class User
    {
        string uid;
        string email;
        FirebaseAuthLink a;
        string storageLink = "doc-management-system-110ee.appspot.com";
        String realtimeDbKey = "https://doc-management-system-110ee-default-rtdb.firebaseio.com";

        ArrayList fileList = new ArrayList();

        public User(FirebaseAuthLink a) {
            this.a = a;
            this.uid = a.User.LocalId;
            this.email = a.User.Email;  
        
        
        }
        public FirebaseStorageTask uploadFile(string path, FirebaseAuthLink a, FileResult result) {

            var stream = File.Open(@path, FileMode.Open);

            var task = new FirebaseStorage(
            storageLink,


             new FirebaseStorageOptions
             {
                 AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                 ThrowOnCancel = true,
             })
            .Child(a.User.LocalId)
            .Child(result.FileName)
            .PutAsync(stream);

            return task;

        }

        public void deleteFile(string text) {

            var task = new FirebaseStorage(
           "doc-management-system-110ee.appspot.com",


            new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                ThrowOnCancel = true,
            })
           .Child(a.User.LocalId).Child(text).DeleteAsync();



        }


            

    }
}
