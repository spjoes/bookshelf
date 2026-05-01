using System.Collections.Generic;
using NzbDrone.Core.Books;
using NzbDrone.Core.MediaFiles;

namespace Readarr.Api.V1.Author
{
    public class AuthorEditorResource
    {
        public List<int> AuthorIds { get; set; }
        public bool? Monitored { get; set; }
        public NewItemMonitorTypes? MonitorNewItems { get; set; }
        public int? QualityProfileId { get; set; }
        public int? EbookQualityProfileId { get; set; }
        public int? AudiobookQualityProfileId { get; set; }
        public WantedMediaTypes? WantedMediaTypes { get; set; }
        public int? MetadataProfileId { get; set; }
        public string RootFolderPath { get; set; }
        public List<int> Tags { get; set; }
        public ApplyTags ApplyTags { get; set; }
        public bool MoveFiles { get; set; }
        public bool DeleteFiles { get; set; }
    }
}
