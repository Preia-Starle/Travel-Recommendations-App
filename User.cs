using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLitePCL;

namespace Assignment7Rebuilt
{
    /// <summary>
    /// Class representing a user object with properties like userId, username, password, email, city, interests and about me
    /// </summary>
    [Table("User")]
    public partial class User
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public User()
        {
            Guid userId = Guid.NewGuid(); //generate new unique ID for each newly created user
            //UserRecommendations = new List<Recommendation>();
            //FavoriteRecommendations = new List<Recommendation>();
        }

        /// <summary>
        /// Constructor taking userName, email, city, interests and aboutMe as parameters chaining parameterless constructor where UserId is generated
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="city"></param>
        /// <param name="interests"></param>
        /// <param name="aboutMe"></param>
        public User(string userName, string hashedPassword, string email, string city, string interests, string aboutMe) : this()
        {
            UserName = userName;
            HashedPassword = hashedPassword;
            Email = email;
            City = city;
            Interests = interests;
            AboutMe = aboutMe;
        }


        /// <summary>
        /// Property that gives read and write access to variable userId
        /// </summary>
        [PrimaryKey]
        [Column("UserId")]
        public Guid UserId { get; set; }



        /// <summary>
        /// Property that gives read and write access to userName variable
        /// </summary>
        [Column("UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// Property that gives read and write access to password variable
        /// </summary
        [Column("Password")]
        public string HashedPassword { get; set; }


        /// <summary>
        /// Property that gives read and write access to email variable
        /// </summary>
        [Column("Email")]
        public string Email { get; set; }

        /// <summary>
        /// Property that gives read and write access to city variable
        /// </summary>
        [Column("City")]
        public string City { get; set; }


        /// <summary>
        /// Property that gives read and write access to interests variable
        /// </summary>
        [Column("Interests")]
        public string Interests { get; set; }

        /// <summary>
        /// Property that gives read and write access to aboutMe variable
        /// </summary>
        [Column("AboutMe")]
        public string AboutMe { get; set; }

        /// <summary>
        /// Property that gives read and write access to variable session token
        /// </summary>
        [Column("SessionToken")]
        public string SessionToken { get; set; }

        /// <summary>
        /// Property representing the collection of recommendations associated with the user
        /// </summary>
       /* [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Recommendation> UserRecommendations { get; set; }

        /// <summary>
        /// Property representing the collection of favourite recommendations related to user and recommendation
        /// </summary>
        [ManyToMany(typeof(Favorite))]
        public List<Recommendation> FavoriteRecommendations { get; set; }*/
        
    }
}
