using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System.Collections.ObjectModel;
using System.Xml;
using System;

namespace Assignment7Rebuilt;
/// <summary>
/// Class providing user interface and supporting methods for displaying recommendations created by logged in user
/// </summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MyRecommendationsPage : ContentPage
{
    //instance variables
    private StorageManager database;
    private User currentUser;

    public ObservableCollection<Recommendation> Recommendations { get; set; }


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="database"></param>
    /// <param name="user"></param>
    public MyRecommendationsPage(StorageManager database, User user)
    {
        InitializeComponent();
        this.database = database;
        currentUser = user;
        Recommendations = new ObservableCollection<Recommendation>();
        BindingContext = this;

        LoadRecommendations();
    }

    // Method to mark published recommendations
    public bool IsPublished(Recommendation recommendation) => recommendation.IsPublished;

    // Method to mark unpublished recommendations
    public bool IsNotPublished(Recommendation recommendation) => !recommendation.IsPublished;

    /// <summary>
    /// Retrieve the recommendations for currently logged in user and display on UI
    /// </summary>
    private async void LoadRecommendations()
    {
        Recommendations.Clear();
        try
        {

            if (currentUser != null)
            {
                // Retrieve the user based on the session token
                currentUser = await database.GetUserAsync(currentUser.SessionToken);

                // Check if the user exists and has a valid UserId
                if (currentUser != null && currentUser.UserId != Guid.Empty)
                {
                    // Retrieve the recommendations for the current user from the database
                    var recommendations = await database.GetRecommendationsForUserAsync(currentUser.UserId);

                    // Add each recommendation to the ObservableCollection
                    foreach (var recommendation in recommendations)
                    {
                        Recommendations.Add(recommendation);
                    }

                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    /// <summary>
    /// Delete selected recommendation from database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var recommendation = button?.BindingContext as Recommendation;

        //validate
        if (recommendation != null)
        {

            bool result = await DisplayAlert("Confirmation", "Are you sure you want to delete this recommendation?", "Yes", "No");

            if (result)
            {
                // Delete the recommendation from the database
                await database.DeleteRecommendationAsync(recommendation);

                // Remove the recommendation from the ObservableCollection
                Recommendations.Remove(recommendation);
            }
        }

    }

    /// <summary>
    /// Navigate to Create Recommendation page to allow for data editing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var recommendation = button?.BindingContext as Recommendation;

        // Validate
        if (recommendation != null)
        {
            // Navigate to the CreateRecommendationPage and pass the recommendation as a parameter
            await Navigation.PushAsync(new EditRecommendationForm(database, currentUser, recommendation, Recommendations));
        }

    }

    /// <summary>
    /// Update the status of recommendation to published true in the database and update recommendations on the main page for carousel view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnPublishClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var recommendation = button?.BindingContext as Recommendation;

        // Validate
        if (recommendation != null)
        {
            // Set the published property to true
            recommendation.IsPublished = true;

            // Update the recommendation in the database
            await database.SaveRecommendationAsync(recommendation);

            // Create an instance of MainPage
            var mainPage = new MainPage(database, currentUser);

            // Call the UpdatePublishedRecommendations method on the instance
            await mainPage.UpdatePublishedRecommendations();

            await DisplayAlert("Success", "Recommendation published successfully!", "OK");

            await Navigation.PushAsync(new MainPage(database, currentUser));
        }
        else
        {
            await DisplayAlert("Error", "Error occured while publishing.", "OK");

        }
    }

    /// <summary>
    /// Update the status of recommendation to unpublished and unpublish recommendation on Unpublish button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnUnpublishClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var recommendation = button?.BindingContext as Recommendation;

        if (recommendation != null)
        {
            // Set the IsPublished property to false
            recommendation.IsPublished = false;

            // Update the recommendation in the database
            await database.SaveRecommendationAsync(recommendation);

            // Display a success message
            await DisplayAlert("Success", "Recommendation unpublished successfully!", "OK");

            // Refresh the displayed recommendations
            LoadRecommendations();

            await Navigation.PushAsync(new MainPage(database, currentUser));
        }
    }
}