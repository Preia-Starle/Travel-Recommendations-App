using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls.Platform;
using BCrypt.Net;
using Assignment7Rebuilt;
using SQLite;
using SQLiteNetExtensions;
using SQLitePCL;

namespace Assignment7Rebuilt;

public partial class ProfileForm : ContentPage, INotifyPropertyChanged
{
    public event EventHandler<User> ProfileCreated;
    //instance variables
    private ObservableCollection<string> selectedInterests; //declare collection of interests 
    private string selectedInterest;

    private StorageManager database;



    public ProfileForm()
    {
        InitializeComponent();
        BindingContext = this; //make the current instance of the form the source for data binding in the xaml
        SelectedInterests = new ObservableCollection<string>(); //bind creation of instance of ObservableCollection on selected interests
        ProfileCreated += (sender, args) => { }; // Initialize the event with an empty handler
    }

    public ProfileForm(StorageManager database) : this()
    {

        this.database = database;

    }


    /// <summary>
    /// Property that gives read and write access to collection of selected interests
    /// Using ObservableCollection class to track changes when selecting and deselecting items to allow real-time sync with UI
    /// </summary>
    public ObservableCollection<string> SelectedInterests
    {
        get
        {
            return selectedInterests;
        }
        set
        {
            selectedInterests = value;
            OnPropertyChanged(nameof(SelectedInterests));
        }
    }

    /// <summary>
    /// Property that gives read and write access to a currently selected interest as a single item
    /// </summary>
    public string SelectedInterest
    {
        get
        {
            return selectedInterest;
        }
        set
        {
            selectedInterest = value;
            OnPropertyChanged(nameof(SelectedInterest));
        }
    }

    /// <summary>
    /// Declare a new event to notify the subscribers when a property changes
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notify when a property changes
    /// </summary>
    /// <param name="propertyName"></param>
    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); //if property changed invoke
    }

    /// <summary>
    /// Add entered item to the collection and listView
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnAddInterestClicked(object sender, EventArgs e)
    {
        //save the entry text in a variable
        string customInterest = interestEntry.Text;
        //validate
        if (!string.IsNullOrEmpty(customInterest) && !SelectedInterests.Contains(customInterest))
        {
            SelectedInterests.Add(customInterest); //add to collection
        }

        interestEntry.Text = string.Empty; // Clear the interest entry

        OnPropertyChanged(nameof(SelectedInterest));
    }

    /// <summary>
    /// Remove the selected item from the collection and listView
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDeleteInterestClicked(object sender, EventArgs e)
    {
        //save the click event to a variable
        string selectedInterest = (sender as Button)?.CommandParameter as string;
        //validate
        if (!string.IsNullOrEmpty(selectedInterest))
        {
            SelectedInterests.Remove(selectedInterest); //remove selected interest
        }

        OnPropertyChanged(nameof(SelectedInterest));
    }

    /// <summary>
    /// Add new user profile after Submit button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        string userName = entryUsername.Text;
        string passWord = entryPassword.Text;
        string email = entryEmail.Text;
        string city = entryCity.Text;
        string interests = string.Join(",", SelectedInterests);
        string aboutMe = entryAboutMe.Text;

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(city))
        {
            await DisplayAlert("Error", "You must fill username, password, email, and city to create a profile.", "OK");
            return;
        }

        if (database != null)
        {
            try
            {
                //hash password using bcrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passWord);
                //pass the hashed password along with the rest of the user input to new user object
                User newUser = new User(userName, hashedPassword, email, city, interests, aboutMe);

                //save user in the database
                await database.SaveUserAsync(newUser);

                ProfileCreated?.Invoke(this, newUser);

                await DisplayAlert("Success", "User added successfully to database:" + Constants.DatabasePath, "OK");

                await Navigation.PopAsync();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Database null", "OK");
        }

    }
}