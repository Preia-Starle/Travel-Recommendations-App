using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Windows.Input;

namespace Assignment7Rebuilt;
/// <summary>
/// Class providing user interface and supporting methods to the application main page
/// </summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MainPage : ContentPage, INotifyPropertyChanged
{

    private StorageManager database;
    private User user;
    private ObservableCollection<Recommendation> publishedRecommendations;
    private ObservableCollection<Comment> comments;

    /// <summary>
    /// Constructor chaining
    /// </summary>
    public MainPage() : this(new StorageManager()) { }

    public MainPage(StorageManager database) : this(database, null) { }

    public MainPage(StorageManager database, User user)
    {
        InitializeComponent();
        this.database = database;
        this.user = user;
        BindingContext = this;
        // Initialize the PublishedRecommendations collection
        PublishedRecommendations = new ObservableCollection<Recommendation>();
        //Initialize the Comments collection
        Comments = new ObservableCollection<Comment>();
        InitializeAsync();

    }

    /// <summary>
    /// Property that gives read and write access to collection of published recommendation (with change notification for UI)
    /// </summary>
    public ObservableCollection<Recommendation> PublishedRecommendations
    {
        get { return publishedRecommendations; }
        set { publishedRecommendations = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Property that gives read and write access to collection of comments (with change notification)
    /// </summary>
    public ObservableCollection<Comment> Comments
    {
        get { return comments; }
        set { comments = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Async method that initializes UI loading the published recommendations and comments
    /// </summary>
    private async void InitializeAsync()
    {
        await LoadPublishedRecommendations();
        await LoadComments();
       
    }

    /// <summary>
    /// Property to notify UI about property value changes
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Method for invoking the PropertyChanged Event
    /// </summary>
    /// <param name="propertyName"></param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    /// <summary>
    /// Check if user logged in and navigate to create recommendation form if yes, prompt to log in if not
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnCreateRecommendationClick(object sender, EventArgs e)
	{
        try
        {
            // Retrieve the session token of the logged-in user
            string sessionToken = user?.SessionToken;

            // Retrieve the logged-in user based on the session token
            var loggedInUser = await database.GetUserAsync(sessionToken);

            if (loggedInUser == null)
            {
                await DisplayAlert("Error", "Please log in or create profile to add a recommendation.", "OK");
                return;
            }

            var recommendationFormPage = new RecommendationForm(database, loggedInUser);
            await Navigation.PushAsync(recommendationFormPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }

    }

    /// <summary>
    /// Check if user logged in, if yes, create an instance of My Recommendations page, pass database contxt and current user and navigate
    /// If user not logged in display error message
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnMyRecommendationsClick(object sender, EventArgs e)
	{
        try
        {
            // Retrieve the session token of the logged-in user
            string sessionToken = user?.SessionToken;

            // Retrieve the logged-in user based on the session token
            var loggedInUser = await database.GetUserAsync(sessionToken);

            if (loggedInUser == null)
            {
                await DisplayAlert("Error", "No user logged in.", "OK");
                return;
            }

            var myRecommendationsPage = new MyRecommendationsPage(database, loggedInUser);
            await Navigation.PushAsync(myRecommendationsPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }

    }

    /// <summary>
    /// Navigate to page with favorites recommendations of currently logged in user
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnFavoritesClick(object sender, EventArgs e)
	{
        try
        {
            string sessionToken = user?.SessionToken;

            var loggedInUser = await database.GetUserAsync(sessionToken);

            if (loggedInUser == null)
            {
                await DisplayAlert("Error", "No user logged in.", "OK");
                return;
            }

            var favoritesPage = new FavoritesPage(database, loggedInUser);
            await Navigation.PushAsync(favoritesPage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }

    }

    /// <summary>
    /// Redirect to ProfileForm to allow user to register
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnCreateProfileClick(object sender, EventArgs e)
	{
        var profileFormPage = new ProfileForm(database);
        await Navigation.PushAsync(profileFormPage);

    }

    /// <summary>
    /// Retrieve the logged in user data and show them on a separate page
    /// </summary>
    private async void OnViewProfileClick(object sender, EventArgs e)
	{
        try
        {
            // Retrieve the session token of the logged-in user
            string sessionToken = user?.SessionToken;

            // Retrieve the logged-in user based on the session token
            var loggedInUser = await database.GetUserAsync(sessionToken);

            if (loggedInUser == null)
            {
                await DisplayAlert("Error", "No user logged in.", "OK");
                return;
            }

            var profilePage = new ViewProfile(database, loggedInUser);
            await Navigation.PushAsync(profilePage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }

    }

    /// <summary>
    /// Redirect to LoginForm to allow user to log in
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnLoginClick(object sender, EventArgs e)
	{
        var loginFormPage = new LoginForm(database);
        await Navigation.PushAsync(loginFormPage);

    }

    /// <summary>
    /// Log out user and annulate token
    /// </summary>
    private async void OnLogoutClicked(object sender, EventArgs e)
	{

        try
        {
            // Retrieve the session token of the logged-in user
            string sessionToken = user?.SessionToken;

            if (string.IsNullOrEmpty(sessionToken))
            {
                await DisplayAlert("Error", "No user logged in.", "OK");
                return;
            }

            // Retrieve the logged-in user based on the session token
            var loggedInUser = await database.GetUserAsync(sessionToken);

            if (loggedInUser == null)
            {
                await DisplayAlert("Error", "No user logged in.", "OK");
                return;
            }

            // Clear the session token
            loggedInUser.SessionToken = null;


            // Update the user in the database
            await database.SaveUserAsync(loggedInUser);

            // Display message to user
            await DisplayAlert("Success", "You have been successfully logged out.", "OK");

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }

    }

    /// <summary>
    /// Retrieve published recommendations from database and add them to collection of published recommendations to be displayed on UI
    /// </summary>
    /// <returns></returns>
    private async Task LoadPublishedRecommendations()
    {
        try
        {
            // Retrieve published recommendations from the database
            var recommendations = await database.GetPublishedRecommendationsAsync();

            // Update the ObservableCollection
            PublishedRecommendations = new ObservableCollection<Recommendation>(recommendations);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }


    /// <summary>
    /// Call the LoadPublishedRecommendations method to update the published recommendations
    /// </summary>
    /// <returns></returns>
    public async Task UpdatePublishedRecommendations()
    {

        await LoadPublishedRecommendations();
    }

    /// <summary>
    /// Call the LoadComments method to update the comments section
    /// </summary>
    /// <returns></returns>
    public async Task UpdateComments()
    {
        await LoadComments();
    }

    /// <summary>
    /// Load content from database that is available for all users
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load the published recommendations when the application starts
        await UpdatePublishedRecommendations();
        // Update the IsFavorite property for each recommendation based on the users favorites
        await UpdateRecommendationsFavorites();
        await UpdateComments();
    }

    /// <summary>
    /// Check if user logged in and retrieve and update his favorites collection
    /// </summary>
    /// <returns></returns>
    private async Task UpdateRecommendationsFavorites()
    {
        try
        {
            string sessionToken = user?.SessionToken;

            var loggedInUser = await database.GetUserAsync(sessionToken);

            if (loggedInUser == null)
                return;

            foreach (var recommendation in PublishedRecommendations)
            {
                var favorite = await database.GetFavoriteAsync(loggedInUser.UserId, recommendation.RecommendationId);
                recommendation.IsFavorite = (favorite != null);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }


    /// <summary>
    /// Check if user logged in and on click save user recommendation to favouries collection and to database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSaveAsFavoriteClicked(object sender, EventArgs e)
    {
        try
        {
            var selectedRecommendation = (Recommendation)((Button)sender).CommandParameter;
            var sessionToken = user?.SessionToken;

            // Retrieve session token and assign to a variable
            var loggedInUser = await database.GetUserAsync(sessionToken);

            // Check if user logged in
            if (loggedInUser == null)
            {
                // Display message if not otherwise continue
                await DisplayAlert("Error", "Please log in to save to favorites.", "OK");
                return;
            }

            // Check if the selected recommendation is already in the user's favorites
            var favorite = await database.GetFavoriteAsync(loggedInUser.UserId, selectedRecommendation.RecommendationId);

            if (favorite != null)
            {
                // If the recommendation exists in favorites, delete it
                await database.DeleteFavoriteAsync(favorite);
                selectedRecommendation.IsFavorite = false;
                await DisplayAlert("Success", "Recommendation removed from favorites.", "OK");
            }
            else
            {
                // If the recommendation does not exist in favorites, add it
                favorite = new Favorite
                {
                    UserId = loggedInUser.UserId,
                    RecommendationId = selectedRecommendation.RecommendationId
                };
                await database.SaveFavoriteAsync(favorite);
                selectedRecommendation.IsFavorite = true;
                await DisplayAlert("Success", "Recommendation added to favorites.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }

    }


    /// <summary>
    /// Retrieve comments for published recommendations, add them to collection to display on UI
    /// </summary>
    private async Task LoadComments()
    {
        try
        {
            // Clear the existing comments collection
            Comments.Clear();

            // Check if there are any published recommendations
            if (PublishedRecommendations != null && PublishedRecommendations.Any())
            {
                // Iterate over each published recommendation
                foreach (var recommendation in PublishedRecommendations)
                {
                    // Retrieve comments for the current recommendation
                    var comments = await database.GetCommentsForRecommendationAsync(recommendation.RecommendationId);
                  
                    // Check if comments exist for the recommendation
                    if (comments != null && comments.Any())
                    {
                        // Add the comments to the overall comments collection
                        foreach (var comment in comments)
                        {
                            Comments.Add(comment);
                        }
                    }
                }
            }

            // Update the UI
            OnPropertyChanged(nameof(Comments));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    /// <summary>
    /// Check if user logged in and saved entered text in database if yes, if not prompt to log in
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSubmitCommentClicked(object sender, EventArgs e)
    {
        try
        {
            var selectedRecommendation = (Recommendation)((Button)sender).CommandParameter;
            var sessionToken = user?.SessionToken;

            // Retrieve session token and assign to a variable
            var loggedInUser = await database.GetUserAsync(sessionToken);

            // Check if user logged in
            if (loggedInUser == null)
            {
                // Display message if not otherwise continue
                await DisplayAlert("Error", "Please log in to save to comment.", "OK");
                return;
            }

            // Access the comment entry text
            var commentEntry = (Entry)((StackLayout)((Button)sender).Parent).Children.FirstOrDefault(c => c.AutomationId == "CommentEntry");
            string commentEntryText = commentEntry.Text;

            // Validate the comment input and save it
            if (!string.IsNullOrWhiteSpace(commentEntryText))
            {
                // Create a new comment object
                var comment = new Comment
                {
                    RecommendationId = selectedRecommendation.RecommendationId,
                    UserId = loggedInUser.UserId,
                    CommentText = commentEntryText,
                    CreatedAt = DateTime.Now
                };

                // Save the comment to the database
                await database.SaveCommentAsync(comment);

                await DisplayAlert("Success", "Comment saved successfully", "OK");

                // Reset the comment input field 
                commentEntry.Text = string.Empty;

                // Reload comments
                await LoadComments();

                // Update the UI
                OnPropertyChanged(nameof(Comments));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }

    }


}

