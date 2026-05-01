using System.Linq;
using NLog;
using NzbDrone.Common.Cache;
using NzbDrone.Core.CustomFormats;
using NzbDrone.Core.IndexerSearch.Definitions;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.DecisionEngine.Specifications
{
    public class UpgradeDiskSpecification : IDecisionEngineSpecification
    {
        private readonly UpgradableSpecification _upgradableSpecification;
        private readonly ICustomFormatCalculationService _formatService;
        private readonly Logger _logger;

        public UpgradeDiskSpecification(UpgradableSpecification qualityUpgradableSpecification,
                                        ICacheManager cacheManager,
                                        ICustomFormatCalculationService formatService,
                                        Logger logger)
        {
            _upgradableSpecification = qualityUpgradableSpecification;
            _formatService = formatService;
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public virtual Decision IsSatisfiedBy(RemoteBook subject, SearchCriteriaBase searchCriteria)
        {
            var mediaType = subject.MediaType == BookMediaType.Unknown ? subject.ParsedBookInfo.Quality.GetMediaType() : subject.MediaType;
            var qualityProfile = subject.Author.GetQualityProfile(mediaType) ?? subject.Author.QualityProfile.Value;

            foreach (var file in subject.Books.SelectMany(c => c.BookFiles.Value).Where(f => mediaType == BookMediaType.Unknown || f.GetMediaType() == mediaType))
            {
                if (file == null)
                {
                    return Decision.Accept();
                }

                var customFormats = _formatService.ParseCustomFormat(file);

                if (!_upgradableSpecification.IsUpgradable(qualityProfile,
                                                           file.Quality,
                                                           customFormats,
                                                           subject.ParsedBookInfo.Quality,
                                                           subject.CustomFormats))
                {
                    return Decision.Reject("Existing files on disk is of equal or higher preference: {0}", file.Quality.Quality.Name);
                }
            }

            return Decision.Accept();
        }
    }
}
