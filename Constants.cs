using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;

namespace Assignment7Rebuilt
{
    public partial class Constants
    {
        public const string DatabaseFilename = "app.db";

        public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

        /// <summary>
        /// Define database path dynamically, consider execution context
        /// </summary>
        public static string DatabasePath
        {
            get
            {
                string baseDirectory = AppContext.BaseDirectory;

                // Check if running in debug mode and adjust the base directory accordingly
                if (baseDirectory.Contains("bin" + Path.DirectorySeparatorChar + "Debug"))
                {
                    baseDirectory = baseDirectory.Substring(
                        0,
                        baseDirectory.IndexOf("bin" + Path.DirectorySeparatorChar + "Debug", StringComparison.Ordinal)
                    );
                }

                string dataDirectory = Path.Combine(baseDirectory, "data");
                Directory.CreateDirectory(dataDirectory); // Ensure the "data" directory exists
                return Path.Combine(dataDirectory, DatabaseFilename);
            }
        }
    }
}
