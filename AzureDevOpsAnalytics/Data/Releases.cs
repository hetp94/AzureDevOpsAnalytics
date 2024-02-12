using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOpsAnalytics.Data
{
    public class Releases
    {
        [Key]
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public int BuildPipelineId { get; set; }
        public string BuildPipelineName { get; set; }
        public int ReleasePipelineId { get; set; }
        public string ReleasePipelineName { get; set; }
        public int BuildId { get; set; }
        public string BuildName { get; set; }
        public int ReleaseId { get; set; }
        public string ReleaseName { get; set; }
        public string StageName { get; set; }
        public string CreatedFor { get; set; }
        public string? ApprovedBy { get; set; }
        public string DeploymentResult { get; set; }
        public DateTime ReleaseQueuedOn { get; set; }
        public DateTime ReleaseCreatedOn { get; set; }
        public DateTime ReleaseCompletedOn { get; set; }
    }
}
