using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLitePCL;
using SQLiteNetExtensions.Attributes;

namespace Assignment7Rebuilt
{
    /// <summary>
    /// Class representing a comment object with properties like comment Id, comment text, create at and properties related to other type of objects like User Id, recommendation Id and Recommendation object
    [Table("Comment")]
    public partial class Comment
    {
        public Comment()
        {
        }

        /// <summary>
        /// Property that gives read and write access to comment Id
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int CommentId { get; set; }

        /// <summary>
        /// Property that gives read and write access to user ID
        /// </summary>
        [ForeignKey(typeof(User))]
        public Guid UserId { get; set; }

        /// <summary>
        /// Property that gives read and write access to recommendation ID
        /// </summary>
        [ForeignKey(typeof(Recommendation))]
        public int RecommendationId { get; set; }

        /// <summary>
        /// Property that gives read and write access to recommendation object
        /// </summary>
        [ManyToOne]
        public Recommendation Recommendation { get; set; }

        /// <summary>
        /// Property that gives reada nd write access to comment text
        /// </summary>
        [Column("CommentText")]
        public string CommentText { get; set; }

        /// <summary>
        /// Property that gives read and write access to create at
        /// </summary>
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}
