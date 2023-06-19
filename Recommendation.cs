using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using SQLiteNetExtensions.Attributes;
using System.Collections.ObjectModel;
using SQLiteNetExtensions;
using System.Diagnostics.Metrics;

namespace Assignment7Rebuilt
{
    /// <summary>
    /// Class representing a recommendations object with properties like recommendation Id, title, location, country and description
    /// </summary>
    [Table("Recommendation")]
    public partial class Recommendation
    {

        public Recommendation()
        {
        }

        public Recommendation(Guid userId, string title, string location, Countries country, string description)
        {
            UserId = userId;
            RecommendationTitle = title;
            Location = location;
            Country = country;
            RecommendationDescription = description;

        }

        /// <summary>
        /// Property that gives read and write access to recommendation ID
        /// </summary>
        [PrimaryKey, AutoIncrement]
        [Column("RecommendationId")]
        public int RecommendationId { get; set; }

        /// <summary>
        /// Property that gives read and write access to user ID
        /// </summary>
        [ForeignKey(typeof(User))]
        public Guid UserId { get; set; }

        /// <summary>
        /// Property that gives read and write access to recommendation title
        /// </summary>
        [Column("RecommendationTitle")]
        public string RecommendationTitle { get; set; }

        /// <summary>
        /// Property that gives read and write access to location variable
        /// </summary>
        [Column("Location")]
        public string Location { get; set; }

        /// <summary>
        /// Property that gives read and write access to country variable
        /// </summary>
        [Column("Country")]
        public Countries Country { get; set; }


        /// <summary>
        /// Property that gives read and write access to recommendation description
        /// </summary>
        [Column("RecommendationDescription")]
        public string RecommendationDescription { get; set; }

        /// <summary>
        /// Property that gives read and write access to published status (true or false)
        /// </summary>
        [Column("IsPublished")]
        public bool IsPublished { get; set; }

        /// <summary>
        /// Property that give read and write access to favorite status (true or false)
        /// </summary>
        [Column("IsFavorite")]
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Property that gives read and write access to country and provides conversion to text after retrieval from database where it is stored as number(based on position)
        /// </summary>
        [Ignore] // message to SQLite to ignore the property
        public string CountryText
        {
            get { return Country.ToString(); }
            set { Country = Enum.Parse<Countries>(value); }
        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Comment> Comments { get; set; }
    }
}
