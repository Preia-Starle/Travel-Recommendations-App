using System;
using System.Linq;

namespace Assignment7Rebuilt;
/// <summary>
/// Class providing user interface and supporting methods for adding a recommendation
/// </summary>
public partial class RecommendationForm : ContentPage
{
    private StorageManager database;
    private User currentUser;

    public RecommendationForm()
    {
        InitializeComponent();
        // Populate the countryPicker with enum values
        var countryValues = Enum.GetValues(typeof(Countries)).Cast<Countries>().ToList();
        countryPicker.ItemsSource = countryValues;
    }

    public RecommendationForm(StorageManager database, User user) : this()
    {
        this.database = database;
        currentUser = user;

    }

    /// <summary>
    /// Collect user input and save recommendation with reference for the current user that is making the recommendation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSubmitRecommendationClicked(object sender, EventArgs e)
    {
        if (currentUser != null)
        {
            //collect user input
            string title = entryTitle.Text;
            string location = entryLocation.Text;
            string description = editorDescription.Text;

            //validate
            if (countryPicker.SelectedItem is Countries country)
            {
                // Pass user input to the recommendation constructor
                Recommendation recommendation = new Recommendation(currentUser.UserId, title, location, country, description);

                // Save the recommendation in the database
                await database.SaveRecommendationAsync(recommendation);

                await DisplayAlert("Success", "Recommendation submitted successfully.", "OK");
                await Navigation.PopAsync(); // Navigate back to the previous page
            }
            else
            {
                //handle the case where the selected country couldn't be parsed
                await DisplayAlert("Error", "Country not found", "OK");

            }

        }
    }
}