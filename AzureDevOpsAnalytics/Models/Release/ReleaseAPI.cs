using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOpsAnalytics.Models.Release
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse)  ;
    public class ApprovedBy
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
    }

    public class Approver
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
    }

    public class Artifact
    {
        public string sourceId { get; set; }
        public string type { get; set; }
        public string alias { get; set; }
        public DefinitionReference definitionReference { get; set; }
        public bool isPrimary { get; set; }
    }

    public class ArtifactSourceDefinitionUrl
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class ArtifactSourceVersionUrl
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Branch
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Condition
    {
        public bool? result { get; set; }
        public string name { get; set; }
        public string conditionType { get; set; }
        public string value { get; set; }
    }

    public class DefaultVersionBranch
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class DefaultVersionSpecific
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class DefaultVersionTags
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class DefaultVersionType
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Definition
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class DefinitionReference
    {
        public ArtifactSourceDefinitionUrl artifactSourceDefinitionUrl { get; set; }
        public DefaultVersionBranch defaultVersionBranch { get; set; }
        public DefaultVersionSpecific defaultVersionSpecific { get; set; }
        public DefaultVersionTags defaultVersionTags { get; set; }
        public DefaultVersionType defaultVersionType { get; set; }
        public Definition definition { get; set; }
        public Project project { get; set; }
        public Version version { get; set; }
        public ArtifactSourceVersionUrl artifactSourceVersionUrl { get; set; }
        public Branch branch { get; set; }
    }

    public class LastModifiedBy
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
        public Web web { get; set; }
    }

    public class PostDeployApproval
    {
        public int id { get; set; }
        public int revision { get; set; }
        public string approvalType { get; set; } = null!;
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        public string status { get; set; }
        public string comments { get; set; }
        public bool isAutomated { get; set; }
        public bool isNotificationOn { get; set; }
        public int trialNumber { get; set; }
        public int attempt { get; set; }
        public int rank { get; set; }
        public Release release { get; set; }
        public ReleaseDefinition releaseDefinition { get; set; }
        public ReleaseEnvironment releaseEnvironment { get; set; }
        public string url { get; set; }
    }

    public class PreDeployApproval
    {
        public int id { get; set; }
        public int revision { get; set; }
        public Approver approver { get; set; }
        public ApprovedBy approvedBy { get; set; }
        public string approvalType { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        public string status { get; set; }
        public string comments { get; set; }
        public bool isAutomated { get; set; }
        public bool isNotificationOn { get; set; }
        public int trialNumber { get; set; }
        public int attempt { get; set; }
        public int rank { get; set; }
        public Release release { get; set; }
        public ReleaseDefinition releaseDefinition { get; set; }
        public ReleaseEnvironment releaseEnvironment { get; set; }
        public string url { get; set; }
    }

    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Release
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public List<Artifact> artifacts { get; set; }
        public string webAccessUri { get; set; }
        public Links _links { get; set; }
        public object description { get; set; }
        public object releaseDefinition { get; set; }
        public DateTime createdOn { get; set; }
        public object createdBy { get; set; }
        public object modifiedBy { get; set; }
        public string reason { get; set; }
    }

    public class ReleaseDefinition
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
    }

    public class ReleaseEnvironment
    {
        public int id { get; set; }
        public string name { get; set; }
        public Links _links { get; set; }
    }

    public class RequestedBy
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
    }

    public class RequestedFor
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
    }

    public class ReleaseRoot
    {
        public int count { get; set; }
        public List<Value> value { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Value
    {
        public int id { get; set; }
        public Release release { get; set; }
        public ReleaseDefinition releaseDefinition { get; set; }
        public ReleaseEnvironment releaseEnvironment { get; set; }
        public int definitionEnvironmentId { get; set; }
        public int attempt { get; set; }
        public string reason { get; set; }
        public string deploymentStatus { get; set; }
        public string operationStatus { get; set; }
        public RequestedBy requestedBy { get; set; }
        public RequestedFor requestedFor { get; set; }
        public DateTime queuedOn { get; set; }
        public DateTime startedOn { get; set; }
        public DateTime completedOn { get; set; }
        public DateTime lastModifiedOn { get; set; }
        public LastModifiedBy lastModifiedBy { get; set; }
        public List<Condition> conditions { get; set; }
        public DateTime scheduledDeploymentTime { get; set; }
        public List<PreDeployApproval> preDeployApprovals { get; set; }
        public List<PostDeployApproval> postDeployApprovals { get; set; }
        public Links _links { get; set; }
    }

    public class Version
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Web
    {
        public string href { get; set; }
    }






}
