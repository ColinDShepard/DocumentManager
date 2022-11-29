using Firebase.Database;
using Firebase.Storage;
using Firebase.Auth;

namespace DocumentManager;

public partial class HomePage : ContentPage
{
    FirebaseAuthLink a;
	public HomePage(FirebaseAuthLink a)
	{
        this.a = a;


        InitializeComponent();


    }

       



    private async void UploadBtnOnClick(object sender, EventArgs e)
    {
        PickOptions options = new()
        {
            PickerTitle = "Please select a comic file",
            //FileTypes = customFileType,
        };



            var path = " ";
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                path = result.FullPath;
            }

           
        


        // Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
        //var stream =File.Open(@"C:\Users\colin\Downloads\files-icon.webp", FileMode.Open);
        var stream =File.Open(@path, FileMode.Open);
       

        //authentication
       /* var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCU5uJkE90JsYuLOjBqivcHa1JbXfIEHGc"));
          var a = await auth.SignInWithEmailAndPasswordAsync("test@gmail.com", "tester"); */
        


        // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
        var task = new FirebaseStorage(
            "doc-management-system-110ee.appspot.com",


             new FirebaseStorageOptions
             {
                 AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                 ThrowOnCancel = true,
             })
            .Child(a.User.Email)
            //.Child("random")
            .Child(result.FileName)
            .PutAsync(stream); 

        

        // Track progress of the upload
        task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

        // await the task to wait until upload completes and get the download url
        var downloadUrl = await task;

        TestText.Text = downloadUrl;


        SemanticScreenReader.Announce(UploadBtn.Text);

        //display();
    }

    private async void display() {
        var files = new FirebaseStorage(
       "doc-management-system-110ee.appspot.com",


       new FirebaseStorageOptions
       {
           AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
           ThrowOnCancel = true,
       })
       .Child(a.User.Email).GetDownloadUrlAsync();
       

        TestText.Text = files.Result.ToString();



    }



}