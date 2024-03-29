﻿using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace DocumentManager;


public partial class MainPage : ContentPage
{


    FirebaseClient firebaseClient = new FirebaseClient("https://doc-management-system-110ee-default-rtdb.firebaseio.com/");

    public ObservableCollection<Login> LoginInfo { get; set; } = new ObservableCollection<Login>();
    
    



    public MainPage()
	{
		InitializeComponent();




    }

    
	private void OnCounterClicked(object sender, EventArgs e)
	{
    
    //THERE IS A PROBLEM WITH YOUR INSTANCE OF OF FIREBASE CLIENT CHILD ONCE ASYNC 
    // It is messing up the registration. Async meaning concurrent threads. There must be a way to stack these threads. 
    //Maybe scrap entire thing and start over??
        String testerson = "It didn't work";

        //You need to use await, but this is hard to figure out to implement 
        var collection = firebaseClient.Child("Login").OnceAsync<Login>().Result;
        //testerson = collection.ToString();

        //using login instead of key to simplify search 
        foreach (var key in collection) {
            if (EmailEntry.Text == key.Object.email && PasswordEntry.Text == key.Object.password) {
                testerson = $"{key.Object.email} is email and {key.Object.password} is password";
                //firebaseClient.Child("Logon").Dispose(); //possible way??
                break;
              

            } else {
                testerson =  $"Wrong login info password";
               

            }

        } 



        //CounterBtn.Text = EmailEntry.Text;


        //CounterBtn.Text = test;
        CounterBtn.Text = testerson;    
        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private void OnRegisterClicked(object sender, EventArgs e) {

        firebaseClient.Child("Login").PostAsync(new Login
        {
            email = EmailEntry.Text,
            password = PasswordEntry.Text,

        });





        



        //RegisterButton.Text = testerson;
        SemanticScreenReader.Announce(RegisterButton.Text);

    }

    void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        string oldText = e.OldTextValue;
        string newText = e.NewTextValue;
        string myText = PasswordEntry.Text;
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
    }

    void OnEmailCompleted(object sender, EventArgs e)
    {
        string text = ((Entry)sender).Text;
    }

}

