using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using NzbDrone.Core.Books;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.MediaFiles;

namespace NzbDrone.Core.AuthorStats
{
    public interface IAuthorStatisticsRepository
    {
        List<BookStatistics> AuthorStatistics();
        List<BookStatistics> AuthorStatistics(int authorId);
    }

    public class AuthorStatisticsRepository : IAuthorStatisticsRepository
    {
        private const string _selectTemplate = "SELECT /**select**/ FROM \"Editions\" /**join**/ /**innerjoin**/ /**leftjoin**/ /**where**/ /**groupby**/ /**having**/ /**orderby**/";

        private readonly IMainDatabase _database;

        public AuthorStatisticsRepository(IMainDatabase database)
        {
            _database = database;
        }

        public List<BookStatistics> AuthorStatistics()
        {
            return Query(Builder());
        }

        public List<BookStatistics> AuthorStatistics(int authorId)
        {
            return Query(Builder().Where<Author>(x => x.Id == authorId));
        }

        private List<BookStatistics> Query(SqlBuilder builder)
        {
            var sql = builder.AddTemplate(_selectTemplate).LogQuery();

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<BookStatistics>(sql.RawSql, sql.Parameters).ToList();
            }
        }

        private SqlBuilder Builder()
        {
            var trueIndicator = _database.DatabaseType == DatabaseType.PostgreSQL ? "true" : "1";

            var hasEbook = @"EXISTS (
                         SELECT 1 FROM ""BookFiles"" ""EbookFiles""
                         INNER JOIN ""Editions"" ""EbookEditions"" ON ""EbookFiles"".""EditionId"" = ""EbookEditions"".""Id""
                         WHERE ""EbookEditions"".""BookId"" = ""Books"".""Id"" AND ""EbookFiles"".""MediaType"" = 1
                     )";
            var hasAudiobook = @"EXISTS (
                         SELECT 1 FROM ""BookFiles"" ""AudiobookFiles""
                         INNER JOIN ""Editions"" ""AudiobookEditions"" ON ""AudiobookFiles"".""EditionId"" = ""AudiobookEditions"".""Id""
                         WHERE ""AudiobookEditions"".""BookId"" = ""Books"".""Id"" AND ""AudiobookFiles"".""MediaType"" = 2
                     )";

            return new SqlBuilder(_database.DatabaseType)
            .Select($@"""Authors"".""Id"" AS ""AuthorId"",
                     ""Books"".""Id"" AS ""BookId"",
                     SUM(COALESCE(""BookFiles"".""Size"", 0)) AS ""SizeOnDisk"",
                     1 AS ""TotalBookCount"",
                     CASE WHEN ""Authors"".""WantedMediaTypes"" = 0 AND MIN(""BookFiles"".""Id"") IS NOT NULL THEN 1
                     WHEN ""Authors"".""WantedMediaTypes"" != 0 AND
                        ((""Authors"".""WantedMediaTypes"" & 1) = 0 OR {hasEbook})
                        AND ((""Authors"".""WantedMediaTypes"" & 2) = 0 OR {hasAudiobook})
                     THEN 1 ELSE 0 END AS ""AvailableBookCount"",
                     CASE WHEN (""Books"".""Monitored"" = {trueIndicator} AND (""Books"".""ReleaseDate"" < @currentDate) OR ""Books"".""ReleaseDate"" IS NULL) OR MIN(""BookFiles"".""Id"") IS NOT NULL THEN 1 ELSE 0 END AS ""BookCount"",
                     CASE WHEN MIN(""BookFiles"".""Id"") IS NULL THEN 0 ELSE COUNT(""BookFiles"".""Id"") END AS ""BookFileCount""")
            .Join<Edition, Book>((e, b) => e.BookId == b.Id)
            .Join<Book, Author>((book, author) => book.AuthorMetadataId == author.AuthorMetadataId)
            .LeftJoin<Edition, BookFile>((t, f) => t.Id == f.EditionId)
            .Where<Edition>(x => x.Monitored == true)
            .GroupBy<Author>(x => x.Id)
            .GroupBy<Book>(x => x.Id)
            .AddParameters(new Dictionary<string, object> { { "currentDate", DateTime.UtcNow } });
        }
    }
}
