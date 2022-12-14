using Firebase.Auth;
using Firebase.Database;
using System.Collections.ObjectModel;

namespace DocumentManager;


public partial class MainPage : ContentPage
{


    FirebaseClient firebaseClient = new FirebaseClient("https://doc-management-system-110ee-default-rtdb.firebaseio.com/");

    public ObservableCollection<Login> LoginInfo { get; set; } = new ObservableCollection<Login>();

    public MainPage()
	{
		InitializeComponent();




    }

    
	private async void OnLoginClicked(object sender, EventArgs e)
	{
        var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCU5uJkE90JsYuLOjBqivcHa1JbXfIEHGc"));


        try {
            var a = await auth.SignInWithEmailAndPasswordAsync(EmailEntry.Text, PasswordEntry.Text);
            App.Current.MainPage = new NavigationPage(new HomePage(a));


        } catch {
            ErrorLabel.Text = "Login or Password is not correct.";
        }
  
        SemanticScreenReader.Announce(LoginBtn.Text);
    }

    private async void OnRegisterClicked(object sender, EventArgs e) {

        try {

            var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyCU5uJkE90JsYuLOjBqivcHa1JbXfIEHGc"));
            var a = await auth.CreateUserWithEmailAndPasswordAsync(EmailEntry.Text, PasswordEntry.Text);
            App.Current.MainPage = new NavigationPage(new HomePage(a));



        } catch (Exception ex) {

            ErrorLabel.Text = "Login or Password is not correct.";
        
        
        
        }



        SemanticScreenReader.Announce(RegisterButton.Text);

    }

    void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        string oldText = e.OldTextValue;
        string newText = e.NewTextValue;
        string myText = PasswordEntry.Text;
        ErrorLabel.Text = " ";
    }

    void OnPasswordCompleted(object sender, EventArgs e)
    {
        string text = ((Entry)sender).Text;
    }

    void OnEmailTextChanged(object sender, TextChangedEventArgs e)
    {
        string oldText = e.OldTextValue;
        string newText = e.NewTextValue;
        string myText = EmailEntry.Text;
        ErrorLabel.Text = " ";
    }

    void OnEmailCompleted(object sender, EventArgs e)
    {
        string text = ((Entry)sender).Text;
    }



}

