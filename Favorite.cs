using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Assignment7Rebuilt
{
    /// <summary>
    /// Class representing a favorite object with properties like favoriteId and properties related to other objects like userId and recommendation Id and recommendation
    /// </summary>
    [Table("Favorite")]
    public partial class Favorite
    {
        public Favorite()
        {
        }
        /// <summary>
        /// Property that gives read and write access to favorite ID
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int FavoriteId { get; set; }

        /// <summary>
        /// Property that gives read and write access to User ID property of type User
        /// </summary>
        [ForeignKey(typeof(User))]
        public Guid UserId { get; set; }

        /// <summary>
        /// Property that gives read and write access to recommendation Id of type Recommendation
        /// </summary>
		[ForeignKey(typeof(Recommendation))]
        public int RecommendationId { get; set; }

        /// <summary>
        /// Property that gives read and write access to Recommendation object
        /// </summary>
        [ManyToOne]
        public Recommendation Recommendation { get; set; }
    }
}
