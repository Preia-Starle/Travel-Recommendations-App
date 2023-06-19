using System;
using System.Linq;
using Microsoft.Maui.Controls;
using BCrypt.Net;
using Assignment7Rebuilt;

namespace Assignment7Rebuilt;
/// <summary>
/// Class providing user interface for loging in
/// </summary>
public partial class LoginForm : ContentPage
{
    private StorageManager database;

    public LoginForm(StorageManager database)
    {
        InitializeComponent();
        this.database = database;
    }

    /// <summary>
    /// Authenticate user
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnLoginFormClicked(object sender, EventArgs e)
    {
        try
        {
            string username = entryUsername.Text;
            string password = entryPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both username and password.", "OK");
                return;
            }

            // Retrieve the user from the database based on the username
            var users = await database.GetUsersAsync();
            var user = users.FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                await DisplayAlert("Error", "Invalid username.", "OK");
                return;
            }

            // Verify the password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.HashedPassword);

            if (!isPasswordValid)
            {
                await DisplayAlert("Error", "Invalid password.", "OK");
                return;
            }

            // Generate a session token
            string sessionToken = GenerateSessionToken();

            // Save the session token in the user object
            user.SessionToken = sessionToken;

            // Update the user in the database
            await database.SaveUserAsync(user);

            // Proceed with the authenticated user
            await DisplayAlert("Success", "Login successful!", "OK");

            //naviagate to main page and pass logged in user and database context as parameters
            MainPage mainPage = new MainPage(database, user);

            // Navigate to the main page
            await Navigation.PushAsync(mainPage);

        }
        catch (Exception ex)
        {
            await DisplayAlert("Check", ex.Message, "Ok");
        }
    }

    /// <summary>
    /// Generate session token
    /// </summary>
    /// <returns></returns>
    private string GenerateSessionToken()
    {
        // Generate a unique session token using a combination of user-specific data, timestamp, and a random value
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string randomValue = Guid.NewGuid().ToString();
        return $"{timestamp}-{randomValue}";

    }
}