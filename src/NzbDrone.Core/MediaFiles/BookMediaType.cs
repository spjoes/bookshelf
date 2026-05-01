using System;
using System.Collections.Generic;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Books;
using NzbDrone.Core.Qualities;

namespace NzbDrone.Core.MediaFiles
{
    public enum BookMediaType
    {
        Unknown = 0,
        Ebook = 1,
        Audiobook = 2
    }

    [Flags]
    public enum WantedMediaTypes
    {
        None = 0,
        Ebook = 1,
        Audiobook = 2,
        All = Ebook | Audiobook
    }

    public static class BookMediaTypeExtensions
    {
        public static bool Includes(this WantedMediaTypes wantedMediaTypes, BookMediaType mediaType)
        {
            return mediaType switch
            {
                BookMediaType.Ebook => wantedMediaTypes.HasFlag(WantedMediaTypes.Ebook),
                BookMediaType.Audiobook => wantedMediaTypes.HasFlag(WantedMediaTypes.Audiobook),
                _ => false
            };
        }

        public static WantedMediaTypes ToWantedMediaType(this BookMediaType mediaType)
        {
            return mediaType switch
            {
                BookMediaType.Ebook => WantedMediaTypes.Ebook,
                BookMediaType.Audiobook => WantedMediaTypes.Audiobook,
                _ => WantedMediaTypes.None
            };
        }

        public static List<BookMediaType> ToMediaTypes(this WantedMediaTypes wantedMediaTypes)
        {
            var mediaTypes = new List<BookMediaType>();

            if (wantedMediaTypes.HasFlag(WantedMediaTypes.Ebook))
            {
                mediaTypes.Add(BookMediaType.Ebook);
            }

            if (wantedMediaTypes.HasFlag(WantedMediaTypes.Audiobook))
            {
                mediaTypes.Add(BookMediaType.Audiobook);
            }

            return mediaTypes;
        }

        public static bool SameMediaType(this BookFile bookFile, BookMediaType mediaType)
        {
            return bookFile.GetMediaType() == mediaType;
        }

        public static BookMediaType GetMediaType(this Edition edition)
        {
            if (edition == null)
            {
                return BookMediaType.Unknown;
            }

            if (edition.IsEbook)
            {
                return BookMediaType.Ebook;
            }

            if (edition.Format.IsNotNullOrWhiteSpace())
            {
                var format = edition.Format.ToLowerInvariant();

                if (format.Contains("audio") || format.Contains("audible") || format.Contains("mp3"))
                {
                    return BookMediaType.Audiobook;
                }
            }

            return BookMediaType.Unknown;
        }

        public static BookMediaType GetMediaType(this BookFile bookFile)
        {
            if (bookFile == null)
            {
                return BookMediaType.Unknown;
            }

            if (bookFile.MediaType != BookMediaType.Unknown)
            {
                return bookFile.MediaType;
            }

            var mediaType = MediaFileExtensions.GetMediaTypeForPath(bookFile.Path);
            if (mediaType != BookMediaType.Unknown)
            {
                return mediaType;
            }

            return MediaFileExtensions.GetMediaTypeForQuality(bookFile.Quality?.Quality);
        }

        public static BookMediaType GetMediaType(this QualityModel quality)
        {
            return MediaFileExtensions.GetMediaTypeForQuality(quality?.Quality);
        }
    }
}
