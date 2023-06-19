using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Maui.Controls;
using Assignment7Rebuilt;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel.Communication;

namespace Assignment7Rebuilt;
/// <summary>
/// Class providing user interface and supporting methods for editing profile
/// </summary>
public partial class EditProfileForm : ContentPage, INotifyPropertyChanged
{
    public event EventHandler<User> ProfileUpdated;
    // instance variables
    private ObservableCollection<string> userInterests;
    private string userInterest;
    private StorageManager database;
    private User currentUser;

    /// <summary>
    /// Default constructor
    /// </summary>
    public EditProfileForm()
    {
        InitializeComponent();
        BindingContext = this; //make the current instance of the form
        ProfileUpdated += (sender, args) => { }; //initialize the event to notify subscribers when property changes
    }

    /// <summary>
    /// Constructor taking database instance and user as parameters(chaining the default constructor)
    /// </summary>
    /// <param name="database"></param>
    /// <param name="user"></param>
    public EditProfileForm(StorageManager database, User user) : this()
    {

        this.database = database;
        currentUser = user;

        //load current user data
        LoadUserData();

    }

    /// <summary>
    /// Property that gives read and write access to the collection of current user interests
    /// </summary>
    public ObservableCollection<string> UserInterests
    {
        get { return userInterests; }
        set
        {
            userInterests = value;
            OnPropertyChanged(nameof(UserInterests));
        }
    }


    /// <summary>
    /// Property that gives read and write access to current user interest as a single item
    /// </summary>
    public string UserInterest
    {
        get
        {
            return userInterest;
        }
        set
        {
            userInterest = value;
            OnPropertyChanged(nameof(UserInterest));
        }
    }

    /// <summary>
    /// Declare a new event to notify subscribers when a property changes
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notify when a property changes
    /// </summary>
    /// <param name="propertyName"></param>
    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // If property changed, invoke the event
    }

    /// <summary>
    /// Load the user data into the form
    /// </summary>
    private void LoadUserData()
    {
        if (currentUser != null)
        {
            entryUsername.Text = currentUser.UserName;
            entryPassword.Text = currentUser.HashedPassword;
            entryEmail.Text = currentUser.Email;
            entryCity.Text = currentUser.City;
            entryAboutMe.Text = currentUser.AboutMe;

            // Load interests from the user object into the collection
            UserInterests = new ObservableCollection<string>(currentUser.Interests.Split(','));
        }
    }

    /// <summary>
    /// Collect the new input that has been submitted and save in the database replacing the old user info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnEditFormClicked(object sender, EventArgs e)
    {

        //collect new user data
        currentUser.UserName = entryUsername.Text;
        currentUser.HashedPassword = entryPassword.Text;
        currentUser.Email = entryEmail.Text;
        currentUser.City = entryCity.Text;
        currentUser.AboutMe = entryAboutMe.Text;
        currentUser.Interests = string.Join(",", UserInterests);

        //validate, make sure obligatory fields not null or empty
        if (string.IsNullOrEmpty(currentUser.UserName) || string.IsNullOrEmpty(currentUser.HashedPassword) || string.IsNullOrEmpty(currentUser.Email) || string.IsNullOrEmpty(currentUser.City))
        {
            await DisplayAlert("Error", "You must fill username, password, email, and city to create a profile.", "OK");
            return;
        }

        // Save the updated user in the database
        await database.SaveUserAsync(currentUser);

        await DisplayAlert("Success", "Profile updated successfully.", "OK");
        await Navigation.PushAsync(new MainPage(database, currentUser));//navigate to the main page
    }


    /// <summary>
    /// Add entered item to the collection and listView
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnAddCurrUserInterestClicked(object sender, EventArgs e)
    {
        string newInterest = interestEntry.Text.Trim();
        if (!string.IsNullOrEmpty(newInterest) && !UserInterests.Contains(newInterest))
        {
            UserInterests.Add(newInterest);
            interestEntry.Text = string.Empty;
        }
    }

    /// <summary>
    /// Remove the selected item from the collection and listView
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDeleteCurrUserInterestClicked(object sender, EventArgs e)
    {
        string selectedInterest = (sender as Button)?.CommandParameter as string;
        if (!string.IsNullOrEmpty(selectedInterest))
        {
            UserInterests.Remove(selectedInterest);
        }

    }
}