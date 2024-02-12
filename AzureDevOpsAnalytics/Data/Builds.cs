using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOpsAnalytics.Data
{
    public class Builds
    {
        [Key]
        public int Id { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? Repository { get; set; } = null!;
        public string AgentPoolName { get; set; } = null!;
        public string BranchInfo { get; set; } = null!;
        public int BuildPipelineId { get; set; }
        public string BuildPipelineName { get; set; } = null!;
        public int BuildId { get; set; }
        public string BuildName { get; set; } = null!;
        public string CreatedFor { get; set; } = null!;
        [NotMapped]
        public string BuildStatus { get; set; } = null!;
        public string BuildResult { get; set; } = null!;
        public DateTime BuildQueuedOn { get; set; }
        public DateTime BuildCreatedOn { get; set; }
        public DateTime BuildCompletedOn { get; set; }

    }
}
