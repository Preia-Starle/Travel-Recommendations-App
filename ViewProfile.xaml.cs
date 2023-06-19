using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;

namespace Assignment7Rebuilt;
/// <summary>
/// Class providing user interface and supporting methods for displaying profile of logged in user
/// </summary>
public partial class ViewProfile : ContentPage, INotifyPropertyChanged
{
    //instance variables
    private StorageManager database;
    private User user;
    private string userInterests;

    /// <summary>
    /// Constructor taking database instance and user as parameters
    /// </summary>
    /// <param name="database"></param>
    /// <param name="user"></param>
    public ViewProfile(StorageManager database, User user)
    {
        InitializeComponent();
        this.database = database;
        this.user = user;

        BindingContext = user; // Set the current user as the binding context for data binding

        //split the user's interests at comma
        // Split the user's interests at comma and assign it to UserInterests
        UserInterests = string.Join(", ", user.Interests.Split(',').Select(i => i.Trim()));


    }

    /// <summary>
    /// Property that gives read and write access to interests
    /// </summary>
    // Property that gives read and write access to UserInterests
    public string UserInterests
    {
        get { return userInterests; }
        set
        {
            if (userInterests != value)
            {
                userInterests = value;
                OnPropertyChanged(nameof(UserInterests));
            }
        }
    }

    // <summary>
    /// Property to notify UI about property value changes
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Method for invoking the PropertyChanged Event
    /// </summary>
    /// <param name="propertyName"></param>
    protected override void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    /// <summary>
    /// Navigate to edit form to enable user to edit his information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnEditProfileClicked(object sender, EventArgs e)
    {
        var editProfilePage = new EditProfileForm(database, user);
        Navigation.PushAsync(editProfilePage);
    }
}