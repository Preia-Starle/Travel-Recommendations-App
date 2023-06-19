using System;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Assignment7Rebuilt;
/// <summary>
/// Class providing user interface and supporting methods for profile editing
/// </summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class EditRecommendationForm : ContentPage
{
    //instance variables
    private StorageManager database;
    private User currentUser;
    private Recommendation recommendation;

    private ObservableCollection<Recommendation> recommendations;

    /// <summary>
    /// Default constructor
    /// </summary>
    public EditRecommendationForm()
    {
        InitializeComponent();
        //populate the countryPicker with enum values
        var countryValues = Enum.GetValues(typeof(Countries)).Cast<Countries>().ToList();
        countryPicker.ItemsSource = countryValues;
    }

    /// <summary>
    /// Constructor taking database instance, user object and recommendations collection as parameters
    /// </summary>
    /// <param name="database"></param>
    /// <param name="user"></param>
    /// <param name="recommendation"></param>
    /// <param name="recommendations"></param>
    public EditRecommendationForm(StorageManager database, User user, Recommendation recommendation, ObservableCollection<Recommendation> recommendations) : this()
    {
        this.database = database;
        currentUser = user;
        this.recommendation = recommendation;
        this.recommendations = recommendations;  // initialize recommendations collection
                                                 //set the binding context to the recommendation
        BindingContext = recommendation ?? new Recommendation();

    }

    /// <summary>
    /// Update recommendation in the database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSaveRecommendationClicked(object sender, EventArgs e)
    {
        try
        {
            //get the recommendation from the binding context
            var editedRecommendation = BindingContext as Recommendation;

            //save the recommendation to the database
            await database.SaveRecommendationAsync(editedRecommendation);

            //if the recommendation is not in the ObservableCollection, add it
            if (!recommendations.Contains(editedRecommendation))
            {
                recommendations.Add(editedRecommendation);

            }

            //show a success message to the user
            await DisplayAlert("Success", "Recommendation edited successfully!", "OK");

            //navigate back to the MyRecommendationsPage
            await Navigation.PopAsync();


        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}