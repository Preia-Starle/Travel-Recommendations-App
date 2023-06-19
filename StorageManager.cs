using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using SQLite;

namespace Assignment7Rebuilt
{
    /// <summary>
    /// Database access class that centralizes query logic and manages database initialisation
    /// </summary>
    public partial class StorageManager
    {
        //instance variable
        SQLiteAsyncConnection database;

        /// <summary>
        /// Default constructor
        /// </summary>
        public StorageManager()
        {
        }

        /// <summary>
        /// Initialise database and establish connection
        /// </summary>
        /// <returns></returns>
        private async Task Init()
        {

            if (database is not null)
                return;

            database = new SQLiteAsyncConnection(Constants.DatabasePath);
            await database.CreateTableAsync<User>();
            //execute table creation logic
            await database.CreateTablesAsync(CreateFlags.None, typeof(User), typeof(Recommendation), typeof(Favorite), typeof(Comment));
        }

        /// <summary>
        /// Retrieve all users from User table
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetUsersAsync()
        {
            await Init();
            return await database.Table<User>().ToListAsync();
        }

        /// <summary>
        /// Retrieve user from table based on userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(Guid userId)
        {
            await Init();
            return await database.Table<User>().Where(u => u.UserId == userId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Overload to retrieve user based on session token
        /// </summary>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(string sessionToken)
        {
            await Init();
            return await database.Table<User>().Where(u => u.SessionToken == sessionToken).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Save user to database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<int> SaveUserAsync(User user)
        {
            try
            {
                await Init();
                if (user.UserId != Guid.Empty)
                    return await database.UpdateAsync(user);
                else
                {
                    user.UserId = Guid.NewGuid();
                    return await database.InsertAsync(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user data: {ex.Message}");
                throw; // Rethrow the exception to be caught in the calling code
            }
        }

        /// <summary>
        /// Save recommendation to database if it does not exist, if yes update
        /// </summary>
        /// <param name="recommendation"></param>
        /// <returns></returns>
        public async Task<int> SaveRecommendationAsync(Recommendation recommendation)
        {
            try
            {
                await Init();
                if (recommendation.RecommendationId != 0)
                    return await database.UpdateAsync(recommendation);
                else
                    return await database.InsertAsync(recommendation);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving recommendation data: {ex.Message}");
                throw; // Rethrow the exception to be caught in the calling code
            }
        }

        /// <summary>
        /// Retrieve recommendations for a specific user from the database
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>A list of recommendations for the user</returns>
        public async Task<List<Recommendation>> GetRecommendationsForUserAsync(Guid userId)
        {
            await Init();
            return await database.Table<Recommendation>().Where(r => r.UserId == userId).ToListAsync();
        }

        /// <summary>
        /// Retrieve recommendation based on recommendation Id
        /// </summary>
        /// <param name="recommendationId"></param>
        /// <returns></returns>
        public async Task<Recommendation> GetRecommendationAsync(int recommendationId)
        {
            await Init();
            return await database.Table<Recommendation>().Where(r => r.RecommendationId == recommendationId).FirstOrDefaultAsync();
        }


        /// <summary>
        /// Delete a recommendation from the database
        /// </summary>
        /// <param name="recommendation">The recommendation to delete</param>
        /// <returns>The number of rows affected</returns>
        public async Task<int> DeleteRecommendationAsync(Recommendation recommendation)
        {
            await Init();
            return await database.DeleteAsync(recommendation);
        }

        /// <summary>
        /// Retrieve all published recommendations from the database
        /// </summary>
        /// <returns>A list of published recommendations</returns>
        public async Task<List<Recommendation>> GetPublishedRecommendationsAsync()
        {
            await Init();
            return await database.Table<Recommendation>().Where(r => r.IsPublished).ToListAsync();
        }

        /// <summary>
        /// Save favorite to database if it does not exist, update if it does
        /// </summary>
        /// <param name="favorite"></param>
        /// <returns></returns>
        public async Task<int> SaveFavoriteAsync(Favorite favorite)
        {
            try
            {
                await Init();

                //retrieve the specific recommendation from the Recommendation table based on the Recommendation Id stored in the Favorites object
                Recommendation recommendation = await database.Table<Recommendation>()
                    .Where(r => r.RecommendationId == favorite.RecommendationId)
                    .FirstOrDefaultAsync();

                if (recommendation != null)
                {
                    recommendation.IsFavorite = true;
                    await database.UpdateAsync(recommendation);
                }

                if (favorite.FavoriteId != 0)
                    return await database.UpdateAsync(favorite);
                else
                    return await database.InsertAsync(favorite);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving favorite data: {ex.Message}");
                throw; // Rethrow the exception to be caught in the calling code
            }
        }

        /// <summary>
        /// Retrieve favorites for a specific user from the database
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>A list of favorites for the user</returns>
        public async Task<List<Favorite>> GetFavoritesForUserAsync(Guid userId)
        {
            await Init();
            return await database.Table<Favorite>().Where(f => f.UserId == userId).ToListAsync();
        }

        /// <summary>
        /// Delete a favorite from the database
        /// </summary>
        /// <param name="favorite">The favorite to delete</param>
        /// <returns>The number of rows affected</returns>
        public async Task<int> DeleteFavoriteAsync(Favorite favorite)
        {
            await Init();
            return await database.DeleteAsync(favorite);
        }

        /// <summary>
        /// Retrieve favorite based on user ID and recommendation ID
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="recommendationId">The ID of the recommendation</param>
        /// <returns>The favorite object if found, or null if not found</returns>
        public async Task<Favorite> GetFavoriteAsync(Guid userId, int recommendationId)
        {
            await Init();
            return await database.Table<Favorite>().Where(f => f.UserId == userId && f.RecommendationId == recommendationId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Save a comment to the database or any other storage mechanism
        /// </summary>
        /// <param name="comment">The comment to save</param>
        /// <returns>The number of rows affected</returns>
        public async Task<int> SaveCommentAsync(Comment comment)
        {
            try
            {
                await Init();

                if (comment.CommentId != 0)
                    return await database.UpdateAsync(comment);
                else
                    return await database.InsertAsync(comment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving comment data: {ex.Message}");
                throw; // Rethrow the exception to be caught in the calling code
            }
        }

        /// <summary>
        /// Retrieve comments for a specific recommendation from the database
        /// </summary>
        /// <param name="recommendationId">The ID of the recommendation</param>
        /// <returns>A list of comments for the recommendation</returns>
        public async Task<List<Comment>> GetCommentsForRecommendationAsync(int recommendationId)
        {
            await Init();
            return await database.Table<Comment>().Where(c => c.RecommendationId == recommendationId).ToListAsync();
        }

        /// <summary>
        /// Delete a comment from the database
        /// </summary>
        /// <param name="comment">The comment to delete</param>
        /// <returns>The number of rows affected</returns>
        public async Task<int> DeleteCommentAsync(Comment comment)
        {
            await Init();
            return await database.DeleteAsync(comment);
        }
    }
}
