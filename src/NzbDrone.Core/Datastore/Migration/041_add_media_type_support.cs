using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(041)]
    public class add_media_type_support : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("RootFolders").AddColumn("DefaultEbookQualityProfileId").AsInt32().WithDefaultValue(0);
            Alter.Table("RootFolders").AddColumn("DefaultAudiobookQualityProfileId").AsInt32().WithDefaultValue(0);
            Alter.Table("RootFolders").AddColumn("DefaultWantedMediaTypes").AsInt32().WithDefaultValue(1);

            Alter.Table("Authors").AddColumn("EbookQualityProfileId").AsInt32().WithDefaultValue(0);
            Alter.Table("Authors").AddColumn("AudiobookQualityProfileId").AsInt32().WithDefaultValue(0);
            Alter.Table("Authors").AddColumn("WantedMediaTypes").AsInt32().WithDefaultValue(1);

            Alter.Table("ImportLists").AddColumn("EbookProfileId").AsInt32().WithDefaultValue(0);
            Alter.Table("ImportLists").AddColumn("AudiobookProfileId").AsInt32().WithDefaultValue(0);
            Alter.Table("ImportLists").AddColumn("WantedMediaTypes").AsInt32().WithDefaultValue(1);

            Alter.Table("BookFiles").AddColumn("MediaType").AsInt32().WithDefaultValue(0);

            Execute.Sql(@"
                UPDATE ""RootFolders""
                SET ""DefaultEbookQualityProfileId"" = COALESCE(""DefaultQualityProfileId"", 0),
                    ""DefaultAudiobookQualityProfileId"" = COALESCE(""DefaultQualityProfileId"", 0),
                    ""DefaultWantedMediaTypes"" = CASE
                        WHEN EXISTS (
                            SELECT 1 FROM ""QualityProfiles""
                            WHERE ""QualityProfiles"".""Id"" = ""RootFolders"".""DefaultQualityProfileId""
                            AND lower(""QualityProfiles"".""Name"") LIKE '%spoken%'
                        ) THEN 2
                        ELSE 1
                    END");

            Execute.Sql(@"
                UPDATE ""Authors""
                SET ""EbookQualityProfileId"" = COALESCE(""QualityProfileId"", 0),
                    ""AudiobookQualityProfileId"" = COALESCE(""QualityProfileId"", 0),
                    ""WantedMediaTypes"" = CASE
                        WHEN EXISTS (
                            SELECT 1 FROM ""QualityProfiles""
                            WHERE ""QualityProfiles"".""Id"" = ""Authors"".""QualityProfileId""
                            AND lower(""QualityProfiles"".""Name"") LIKE '%spoken%'
                        ) THEN 2
                        ELSE 1
                    END");

            Execute.Sql(@"
                UPDATE ""ImportLists""
                SET ""EbookProfileId"" = COALESCE(""ProfileId"", 0),
                    ""AudiobookProfileId"" = COALESCE(""ProfileId"", 0),
                    ""WantedMediaTypes"" = CASE
                        WHEN EXISTS (
                            SELECT 1 FROM ""QualityProfiles""
                            WHERE ""QualityProfiles"".""Id"" = ""ImportLists"".""ProfileId""
                            AND lower(""QualityProfiles"".""Name"") LIKE '%spoken%'
                        ) THEN 2
                        ELSE 1
                    END");

            Execute.Sql(@"
                UPDATE ""BookFiles""
                SET ""MediaType"" = 1
                WHERE lower(""Path"") LIKE '%.epub'
                   OR lower(""Path"") LIKE '%.kepub'
                   OR lower(""Path"") LIKE '%.mobi'
                   OR lower(""Path"") LIKE '%.azw3'
                   OR lower(""Path"") LIKE '%.pdf'");

            Execute.Sql(@"
                UPDATE ""BookFiles""
                SET ""MediaType"" = 2
                WHERE lower(""Path"") LIKE '%.flac'
                   OR lower(""Path"") LIKE '%.ape'
                   OR lower(""Path"") LIKE '%.wavpack'
                   OR lower(""Path"") LIKE '%.wav'
                   OR lower(""Path"") LIKE '%.alac'
                   OR lower(""Path"") LIKE '%.mp2'
                   OR lower(""Path"") LIKE '%.mp3'
                   OR lower(""Path"") LIKE '%.wma'
                   OR lower(""Path"") LIKE '%.m4a'
                   OR lower(""Path"") LIKE '%.m4p'
                   OR lower(""Path"") LIKE '%.m4b'
                   OR lower(""Path"") LIKE '%.aac'
                   OR lower(""Path"") LIKE '%.mp4a'
                   OR lower(""Path"") LIKE '%.ogg'
                   OR lower(""Path"") LIKE '%.oga'
                   OR lower(""Path"") LIKE '%.vorbis'");
        }
    }
}
