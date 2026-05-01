using System;
using NzbDrone.Core.Books;
using NzbDrone.Core.ImportLists;
using NzbDrone.Core.MediaFiles;

namespace Readarr.Api.V1.ImportLists
{
    public class ImportListResource : ProviderResource<ImportListResource>
    {
        public bool EnableAutomaticAdd { get; set; }
        public ImportListMonitorType ShouldMonitor { get; set; }
        public bool ShouldMonitorExisting { get; set; }
        public bool ShouldSearch { get; set; }
        public string RootFolderPath { get; set; }
        public NewItemMonitorTypes MonitorNewItems { get; set; }
        public int QualityProfileId { get; set; }
        public int EbookQualityProfileId { get; set; }
        public int AudiobookQualityProfileId { get; set; }
        public WantedMediaTypes WantedMediaTypes { get; set; }
        public int MetadataProfileId { get; set; }
        public ImportListType ListType { get; set; }
        public int ListOrder { get; set; }
        public TimeSpan MinRefreshInterval { get; set; }
    }

    public class ImportListResourceMapper : ProviderResourceMapper<ImportListResource, ImportListDefinition>
    {
        public override ImportListResource ToResource(ImportListDefinition definition)
        {
            if (definition == null)
            {
                return null;
            }

            var resource = base.ToResource(definition);

            resource.EnableAutomaticAdd = definition.EnableAutomaticAdd;
            resource.ShouldMonitor = definition.ShouldMonitor;
            resource.ShouldMonitorExisting = definition.ShouldMonitorExisting;
            resource.ShouldSearch = definition.ShouldSearch;
            resource.RootFolderPath = definition.RootFolderPath;
            resource.MonitorNewItems = definition.MonitorNewItems;
            resource.QualityProfileId = definition.ProfileId;
            resource.EbookQualityProfileId = definition.EbookProfileId == 0 ? definition.ProfileId : definition.EbookProfileId;
            resource.AudiobookQualityProfileId = definition.AudiobookProfileId == 0 ? definition.ProfileId : definition.AudiobookProfileId;
            resource.WantedMediaTypes = definition.WantedMediaTypes == WantedMediaTypes.None ? WantedMediaTypes.Ebook : definition.WantedMediaTypes;
            resource.MetadataProfileId = definition.MetadataProfileId;
            resource.ListType = definition.ListType;
            resource.ListOrder = (int)definition.ListType;
            resource.MinRefreshInterval = definition.MinRefreshInterval;

            return resource;
        }

        public override ImportListDefinition ToModel(ImportListResource resource)
        {
            if (resource == null)
            {
                return null;
            }

            var definition = base.ToModel(resource);

            definition.EnableAutomaticAdd = resource.EnableAutomaticAdd;
            definition.ShouldMonitor = resource.ShouldMonitor;
            definition.ShouldMonitorExisting = resource.ShouldMonitorExisting;
            definition.ShouldSearch = resource.ShouldSearch;
            definition.RootFolderPath = resource.RootFolderPath;
            definition.MonitorNewItems = resource.MonitorNewItems;
            definition.ProfileId = resource.QualityProfileId;
            definition.EbookProfileId = resource.EbookQualityProfileId == 0 ? resource.QualityProfileId : resource.EbookQualityProfileId;
            definition.AudiobookProfileId = resource.AudiobookQualityProfileId == 0 ? resource.QualityProfileId : resource.AudiobookQualityProfileId;
            definition.WantedMediaTypes = resource.WantedMediaTypes == WantedMediaTypes.None ? WantedMediaTypes.Ebook : resource.WantedMediaTypes;
            definition.MetadataProfileId = resource.MetadataProfileId;
            definition.ListType = resource.ListType;
            definition.MinRefreshInterval = resource.MinRefreshInterval;

            return definition;
        }
    }
}
