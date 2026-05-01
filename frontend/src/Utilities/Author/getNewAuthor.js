
function getNewAuthor(author, payload) {
  const {
    rootFolderPath,
    monitor,
    monitorNewItems,
    qualityProfileId,
    ebookQualityProfileId,
    audiobookQualityProfileId,
    wantedMediaTypes,
    metadataProfileId,
    tags,
    searchForMissingBooks = false
  } = payload;

  const addOptions = {
    monitor,
    searchForMissingBooks
  };

  author.addOptions = addOptions;
  author.monitored = true;
  author.monitorNewItems = monitorNewItems;
  author.qualityProfileId = qualityProfileId;
  author.ebookQualityProfileId = ebookQualityProfileId || qualityProfileId;
  author.audiobookQualityProfileId = audiobookQualityProfileId || qualityProfileId;
  author.wantedMediaTypes = wantedMediaTypes || 1;
  author.metadataProfileId = metadataProfileId;
  author.rootFolderPath = rootFolderPath;
  author.tags = tags;

  return author;
}

export default getNewAuthor;
