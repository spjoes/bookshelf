using System;
using System.Collections.Generic;
using Equ;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Profiles.Metadata;
using NzbDrone.Core.Profiles.Qualities;

namespace NzbDrone.Core.Books
{
    public class Author : Entity<Author>
    {
        public Author()
        {
            Tags = new HashSet<int>();
            Metadata = new AuthorMetadata();
        }

        // These correspond to columns in the Authors table
        public int AuthorMetadataId { get; set; }
        public string CleanName { get; set; }
        public bool Monitored { get; set; }
        public NewItemMonitorTypes MonitorNewItems { get; set; }
        public DateTime? LastInfoSync { get; set; }
        public string Path { get; set; }
        public string RootFolderPath { get; set; }
        public DateTime Added { get; set; }
        public int QualityProfileId { get; set; }
        public int EbookQualityProfileId { get; set; }
        public int AudiobookQualityProfileId { get; set; }
        public WantedMediaTypes WantedMediaTypes { get; set; }
        public int MetadataProfileId { get; set; }
        public HashSet<int> Tags { get; set; }
        [MemberwiseEqualityIgnore]
        public AddAuthorOptions AddOptions { get; set; }

        // Dynamically loaded from DB
        [MemberwiseEqualityIgnore]
        public LazyLoaded<AuthorMetadata> Metadata { get; set; }
        [MemberwiseEqualityIgnore]
        public LazyLoaded<QualityProfile> QualityProfile { get; set; }
        [MemberwiseEqualityIgnore]
        public LazyLoaded<QualityProfile> EbookQualityProfile { get; set; }
        [MemberwiseEqualityIgnore]
        public LazyLoaded<QualityProfile> AudiobookQualityProfile { get; set; }
        [MemberwiseEqualityIgnore]
        public LazyLoaded<MetadataProfile> MetadataProfile { get; set; }
        [MemberwiseEqualityIgnore]
        public LazyLoaded<List<Book>> Books { get; set; }
        [MemberwiseEqualityIgnore]
        public LazyLoaded<List<Series>> Series { get; set; }

        //compatibility properties
        [MemberwiseEqualityIgnore]
        public string Name
        {
            get { return Metadata.Value.Name; } set { Metadata.Value.Name = value; }
        }

        [MemberwiseEqualityIgnore]
        public string ForeignAuthorId
        {
            get { return Metadata.Value.ForeignAuthorId; } set { Metadata.Value.ForeignAuthorId = value; }
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", Metadata.Value.ForeignAuthorId.NullSafe(), Metadata.Value.Name.NullSafe());
        }

        public override void UseMetadataFrom(Author other)
        {
            CleanName = other.CleanName;
        }

        public override void UseDbFieldsFrom(Author other)
        {
            Id = other.Id;
            AuthorMetadataId = other.AuthorMetadataId;
            Monitored = other.Monitored;
            MonitorNewItems = other.MonitorNewItems;
            LastInfoSync = other.LastInfoSync;
            Path = other.Path;
            RootFolderPath = other.RootFolderPath;
            Added = other.Added;
            QualityProfileId = other.QualityProfileId;
            QualityProfile = other.QualityProfile;
            EbookQualityProfileId = other.EbookQualityProfileId;
            EbookQualityProfile = other.EbookQualityProfile;
            AudiobookQualityProfileId = other.AudiobookQualityProfileId;
            AudiobookQualityProfile = other.AudiobookQualityProfile;
            WantedMediaTypes = other.WantedMediaTypes;
            MetadataProfileId = other.MetadataProfileId;
            MetadataProfile = other.MetadataProfile;
            Tags = other.Tags;
            AddOptions = other.AddOptions;
        }

        public override void ApplyChanges(Author other)
        {
            Path = other.Path;
            QualityProfileId = other.QualityProfileId;
            QualityProfile = other.QualityProfile;
            EbookQualityProfileId = other.EbookQualityProfileId;
            EbookQualityProfile = other.EbookQualityProfile;
            AudiobookQualityProfileId = other.AudiobookQualityProfileId;
            AudiobookQualityProfile = other.AudiobookQualityProfile;
            WantedMediaTypes = other.WantedMediaTypes;
            MetadataProfileId = other.MetadataProfileId;
            MetadataProfile = other.MetadataProfile;

            Books = other.Books;
            Tags = other.Tags;
            AddOptions = other.AddOptions;
            RootFolderPath = other.RootFolderPath;
            Monitored = other.Monitored;
            MonitorNewItems = other.MonitorNewItems;
        }

        public int GetQualityProfileId(BookMediaType mediaType)
        {
            return mediaType switch
            {
                BookMediaType.Audiobook => AudiobookQualityProfileId > 0 ? AudiobookQualityProfileId : QualityProfileId,
                BookMediaType.Ebook => EbookQualityProfileId > 0 ? EbookQualityProfileId : QualityProfileId,
                _ => QualityProfileId
            };
        }

        public QualityProfile GetQualityProfile(BookMediaType mediaType)
        {
            return mediaType switch
            {
                BookMediaType.Audiobook when AudiobookQualityProfile != null && AudiobookQualityProfile.Value != null => AudiobookQualityProfile.Value,
                BookMediaType.Ebook when EbookQualityProfile != null && EbookQualityProfile.Value != null => EbookQualityProfile.Value,
                _ => QualityProfile?.Value
            };
        }

        public bool WantsMediaType(BookMediaType mediaType)
        {
            var wantedMediaTypes = WantedMediaTypes == WantedMediaTypes.None ? mediaType.ToWantedMediaType() : WantedMediaTypes;

            return wantedMediaTypes.Includes(mediaType);
        }
    }
}
