using System.Collections.ObjectModel;

namespace Assignment7Rebuilt;
/// <summary>
/// Class providing user interface for displaying favorite recommendations for logged in user
/// </summary>
public partial class FavoritesPage : ContentPage
{
    //instance variables
    private StorageManager database;
    private User currentUser;
    public ObservableCollection<Favorite> Favorites { get; set; }


    /// <summary>
    /// Constructor taking database instance and user object as parameters
    /// </summary>
    /// <param name="database"></param>
    /// <param name="user"></param>
    public FavoritesPage(StorageManager database, User user)
    {
        InitializeComponent();
        this.database = database;
        currentUser = user;
        Favorites = new ObservableCollection<Favorite>();
        BindingContext = this;

        LoadFavorites();

    }

    /// <summary>
    /// Retrieve favorites for currentUser from database and add to collection of favorites
    /// </summary>
    private async void LoadFavorites()
    {
        Favorites.Clear();
        try
        {
            if (currentUser != null)
            {
                currentUser = await database.GetUserAsync(currentUser.SessionToken);
                if (currentUser != null && currentUser.UserId != Guid.Empty)
                {
                    var favorites = await database.GetFavoritesForUserAsync(currentUser.UserId);

                    foreach (var favorite in favorites)
                    {
                        var recommendation = await database.GetRecommendationAsync(favorite.RecommendationId);
                        if (recommendation != null)
                        {
                            favorite.Recommendation = recommendation;
                            Favorites.Add(favorite);
                        }
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
    /// Delete recommendation from favorites
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnDeleteFavClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var favorite = button?.BindingContext as Favorite;

        if (favorite != null)
        {
            bool result = await DisplayAlert("Confirmation", "Are you sure you want to delete this favorite?", "Yes", "No");

            if (result)
            {
                await database.DeleteFavoriteAsync(favorite);
                Favorites.Remove(favorite);
            }
        }
    }
}