using Firebase.Database;
using Firebase.Storage;
using Firebase.Auth;
using Firebase.Database.Query;
using System.Collections;


namespace DocumentManager;

public partial class HomePage : ContentPage
{
    FirebaseAuthLink a;
    String realtimeDbKey = "https://doc-management-system-110ee-default-rtdb.firebaseio.com";

    ArrayList arrayList = new ArrayList();

    public HomePage(FirebaseAuthLink a)
    {
        this.a = a;

        Retrieve();


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
        var stream = File.Open(@path, FileMode.Open);


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
            .Child(a.User.LocalId)
            //.Child("random")
            .Child(result.FileName)
            .PutAsync(stream);



        // Track progress of the upload
        task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

        // await the task to wait until upload completes and get the download url
        var downloadUrl = await task;

        //TestText.Text = downloadUrl;

        var firebase = new FirebaseClient(realtimeDbKey);
        Files testfile = new Files();


        testfile.downloadUrl = downloadUrl;
        testfile.fileName = result.FileName;

        // add new item to list of data and let the client generate new key for you (done offline)
        var item = await firebase
          .Child(a.User.LocalId)
          .PostAsync(testfile);

        // note that there is another overload for the PostAsync method which delegates the new key generation to the firebase server

        //Console.WriteLine($"Key for the new dinosaur: {item.Key}");

        // add new item directly to the specified location (this will overwrite whatever data already exists at that location)
        /*await firebase
          .Child("dinosaurs")
          .Child("t-rex")
          .PutAsync(new Dinosaur()); */


        SemanticScreenReader.Announce(UploadBtn.Text);

        Retrieve();
    }


    private async void DownloadBtnOnClick(object sender, EventArgs e)
    {


        try
        {

            string s = TestText.Text;

            var firebase = new FirebaseClient(realtimeDbKey);
            var items = await firebase
              .Child(a.User.LocalId)
              .OnceAsync<Files>();
            string url = "";
            foreach (var item in items)
            {
                if (item.Object.fileName == s)
                {
                    url = item.Object.downloadUrl;


                }

            }




            Uri uri = new Uri(url);
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            // An unexpected error occured. No browser may be installed on the device.
        }
        SemanticScreenReader.Announce(DownloadButton.Text);

    }

    private async void Retrieve() {

        var firebase = new FirebaseClient(realtimeDbKey);
        var items = await firebase
          .Child(a.User.LocalId)
          .OnceAsync<Files>();


        FileName.Text = "";
        arrayList.Clear();
        foreach (var item in items)
        {
            FileName.Text += "\n" + item.Object.fileName;
            arrayList.Add(item.Object.fileName);

        }

        pickerDoStuff();


    }

    public void OnPickerSelectedIndexchanged(object sender, EventArgs e) {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        Console.WriteLine("Am I here?");
        if (selectedIndex != -1) {
            TestText.Text = (string)picker.ItemsSource[selectedIndex];



        }



    }

    public void pickerDoStuff() {
        try {


            if (arrayList == null)
            {

            }
            else
            {

                TestPicker.ItemsSource = arrayList;
                TestPicker.ItemsSource = TestPicker.GetItemsAsArray();




            }

        } catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
        }


    }



    private async void DeleteBtnOnClick(object sender, EventArgs e) {

        var task = new FirebaseStorage(
                   "doc-management-system-110ee.appspot.com",


                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true,
                    })
                   .Child(a.User.LocalId).Child(TestText.Text).DeleteAsync();


        string s = TestText.Text;

        var firebase = new FirebaseClient(realtimeDbKey);
        var items = await firebase
          .Child(a.User.LocalId)
          .OnceAsync<Files>();
        string key = "";
        foreach (var item in items)
        {
            if (item.Object.fileName == s)
            {
               key = item.Key;


            }

        }

        await firebase.Child(a.User.LocalId).Child(key).DeleteAsync();
        Retrieve();


    }

    public async void createImage(object sender, EventArgs e) {
        var firebase = new FirebaseClient(realtimeDbKey);
        var items = await firebase
          .Child(a.User.LocalId)
          .OnceAsync<Files>();

        string s = TestText.Text;

        string url = "";
        foreach (var item in items)
        {
            if (item.Object.fileName == s)
            {
                url = item.Object.downloadUrl;
                Image image1 = new Image { Source = ImageSource.FromUri(new Uri(url)) };
                TestImage.Source = image1.Source;


            }

        }



    }





}