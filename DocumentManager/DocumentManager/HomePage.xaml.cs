using Firebase.Database;
using Firebase.Storage;
using Firebase.Auth;
using Firebase.Database.Query;
using System.Collections;
using System.Net.WebSockets;

namespace DocumentManager;

public partial class HomePage : ContentPage
{
    FirebaseAuthLink a;
    String realtimeDbKey = "https://doc-management-system-110ee-default-rtdb.firebaseio.com";

    ArrayList arrayList = new ArrayList();
    public bool isDeleteClicked = false;
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
        try { 
            
        var stream = File.Open(@path, FileMode.Open);

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

        catch(Exception ex) { 
        
        
        }



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
        if (isDeleteClicked && arrayList.Count == 0)
        {
            Image image1 = new Image { Source = ImageSource.FromUri(new Uri("http://www.testurl.com")) };
            TestImage.Source = image1.Source;

        }
        isDeleteClicked = false;
    }

    public void OnPickerSelectedIndexchanged(object sender, EventArgs e) {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1) {
            TestText.Text = (string)picker.ItemsSource[selectedIndex];



        }



    }

    public void pickerDoStuff() {
        try {


            if (arrayList == null)
            {
                TestPicker.IsVisible= false;
            }
            else
            {
                TestPicker.IsVisible = true;    
                TestPicker.ItemsSource = arrayList;
                TestPicker.ItemsSource = TestPicker.GetItemsAsArray();




            }

        } catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
        }


    }



    private async void DeleteBtnOnClick(object sender, EventArgs e) {
        isDeleteClicked = true;
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
        Console.WriteLine("ArralListcount  "+arrayList.Count);
        
    }

    public async void createImage(object sender, EventArgs e) {
        var firebase = new FirebaseClient(realtimeDbKey);
        var items = await firebase
          .Child(a.User.LocalId)
          .OnceAsync<Files>();

        string s = TestText.Text;

        string url = "";
        Image image1 = new Image { Source = ImageSource.FromUri(new Uri("http://www.testurl.com")) };
        foreach (var item in items)
        {
            if (item.Object.fileName == s)
            {
                if (checkFileExtension(item.Object.fileName) == "jpg" || checkFileExtension(item.Object.fileName) == "jpeg" || checkFileExtension(item.Object.fileName) == "png") {
                    url = item.Object.downloadUrl;
                    image1 = new Image { Source = ImageSource.FromUri(new Uri(url)) };
                    TestImage.Source = image1.Source;



                } else {

                    image1 = image1 = new Image { Source = ImageSource.FromUri(new Uri("https://findicons.com/files/icons/2813/flat_jewels/512/file.png")) };
                    TestImage.Source = image1.Source;


                }
                



            }

        }



    }

    public string checkFileExtension(string filename) { 

        var list = filename.Split('.');
        string extension = list.Last();
        return extension;
    
    
    
    }





}