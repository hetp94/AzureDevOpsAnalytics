using AzureDevOpsAnalytics.Data;
using AzureDevOpsAnalytics.Extensions;
using AzureDevOpsAnalytics.Models;
using AzureDevOpsAnalytics.Models.Release;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace AzureDevOpsAnalytics
{
    internal class Program
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        private List<Builds> existingBuilds = new List<Builds>();
        private HashSet<(string, int, int)> existingBuildHashSet = new HashSet<(string, int, int)>();

        private List<Releases> existingReleases = new List<Releases>();
        private HashSet<(string, int, int)> existingReleaseHashSet = new HashSet<(string, int, int)>();

        private static readonly NLog.ILogger _Nlogger = LogManager.GetCurrentClassLogger();

        public Program(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public static async Task Main(string[] args)
        {
            try
            {
                LogManager.Configuration = new XmlLoggingConfiguration("nlog.config");

                _Nlogger.Info("Application started.");

                Stopwatch stopwatch = Stopwatch.StartNew();

                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddUserSecrets<Program>()
                    .Build();

                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

                using (var context = new ApplicationDbContext(optionsBuilder.Options))
                {
                    var program = new Program(context, config);

                    await program.AddBuildData();

                    await program.AddReleaseData();
                }

                stopwatch.Stop();

                await Console.Out.WriteLineAsync("Time Taken: " + stopwatch.ElapsedMilliseconds.ToString());
                _Nlogger.Info("Time Taken: " + stopwatch.ElapsedMilliseconds.ToString());
                _Nlogger.Info("Application finished.");
            }
            catch (Exception ex)
            {
                _Nlogger.Error(ex, "An unexpected error occurred.");
            }
            finally
            {
                LogManager.Flush();
            }
        }

        #region Build
        public async Task AddBuildData()
        {
            if (ProjectRelatedInfo.ProjectNames == null)
            {
                _Nlogger.Warn("No projects found.");
                return;
            }

            string organizationName, apiVersion, pat;
            LoadAPIValues(out organizationName, out apiVersion, out pat);

            var tasks = ProjectRelatedInfo.ProjectNames.Select(async projectName =>
            {
                try
                {
                    string apiUrl = $"https://dev.azure.com/{organizationName}/{projectName}/_apis/build/builds?api-version={apiVersion}";

                    string jsonResponse = await GetApiResponse(apiUrl, pat);

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        await ConvertBuildData(jsonResponse);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    _Nlogger.Error(ex, $"Error processing project: {projectName}");
                }
            });

            await Task.WhenAll(tasks);

            //foreach (var project in ProjectRelatedInfo.ProjectNames)
            //{
            //    string apiUrl = $"https://dev.azure.com/{organizationName}/{project}/_apis/build/builds?api-version={apiVersion}";

            //    string jsonResponse = await GetApiResponse(apiUrl, pat);

            //    if (!string.IsNullOrEmpty(jsonResponse))
            //    {
            //        await ConvertBuildData(jsonResponse);
            //    }
            //}

        }

        private async Task ConvertBuildData(string jsonResponse)
        {
            BuildAPI myDeserializedClass = new BuildAPI();
            try
            {
                myDeserializedClass = JsonConvert.DeserializeObject<BuildAPI>(jsonResponse);
            }
            catch (Exception ex)
            {
                _Nlogger.Error(ex, "Error while desearlizing json!");
                throw new Exception("Error while desearlizing json!");
            }

            if (myDeserializedClass != null)
            {
                await LoadBuildDataIntoMemory();

                List<Builds> builds = new List<Builds>();

                foreach (var item in myDeserializedClass.value)
                {
                    //Console.WriteLine(
                    //   //"Project Name: " + item.project.name
                    //   " " + "Repository: " + item.repository.name
                    //  + " " + "Agent Pool: " + item.queue.pool.name
                    //  + " " + "Build Pipeline: " + item.definition.name
                    //   + " " + "Build ID: " + item.id
                    //  + " " + "Build Name: " + item.buildNumber
                    // //+ " " + "Release Pipeline Name: " + item.releaseDefinition.name
                    // //+ " " + "Release Name:  " + item.release.name
                    // //+ " " + "Stage Name:  " + item.releaseEnvironment.name
                    // + " " + "Build Result: " + item.result
                    // + " " + "Created For: " + item.requestedFor.displayName
                    // //+ " " + "Createt At: " + item.queuedOn.ToCentralTimeZone()
                    // //+ " " + "Completed At: " + item.completedOn.ToCentralTimeZone()
                    // );

                    Builds build = new Builds();

                    build.ProjectName = item.project.name;
                    build.Repository = item.repository.name;
                    build.BranchInfo = item.sourceBranch;
                    build.AgentPoolName = item.queue.pool.name;
                    build.BuildPipelineId = item.definition.id;
                    build.BuildPipelineName = item.definition.name;
                    build.BuildId = item.id;
                    build.BuildName = item.buildNumber;
                    build.BuildStatus = item.status;
                    build.BuildResult = item.result;
                    build.CreatedFor = item.requestedFor.displayName;
                    build.BuildQueuedOn = item.queueTime.ToCentralTimeZone();
                    build.BuildCreatedOn = item.startTime.ToCentralTimeZone();
                    build.BuildCompletedOn = item.finishTime.ToCentralTimeZone();

                    builds.Add(build);
                }

                await AddBuildsIfNotExists(builds);
            }
        }

        private async Task LoadBuildDataIntoMemory()
        {
            existingBuilds = await _context.Builds.ToListAsync();

            existingBuildHashSet = new HashSet<(string, int, int)>(existingBuilds.Select(r => (r.ProjectName, r.BuildPipelineId, r.BuildId)));
        }

        private async Task AddBuildsIfNotExists(List<Builds> builds)
        {
            foreach (var build in builds)
            {
                //we don't want to add data if pipeline is running
                if (build.BuildStatus == "completed")
                {
                    if (!existingBuildHashSet.Contains((build.ProjectName, build.BuildPipelineId, build.BuildId)))
                    {
                        _context.Builds.Add(build);
                        Console.WriteLine($"Build added successfully: {build.BuildName}");
                        _Nlogger.Info($"Build added successfully: {build.BuildName} for Project Name: {build.ProjectName}");
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Release

        private async Task AddReleaseData()
        {
            if (ProjectRelatedInfo.ProjectNames == null)
            {
                _Nlogger.Warn("No projects found.");
                return;
            }

            string organizationName, apiVersion, pat;
            LoadAPIValues(out organizationName, out apiVersion, out pat);

            var tasks = ProjectRelatedInfo.ProjectNames.Select(async projectName =>
            {
                try
                {
                    string apiUrl = $"https://vsrm.dev.azure.com/{organizationName}/{projectName}/_apis/release/deployments?api-version={apiVersion}";

                    string jsonResponse = await GetApiResponse(apiUrl, pat);

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        await ConvertReleaseData(jsonResponse);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    _Nlogger.Error(ex, $"Error processing project: {projectName}");
                }
            });

            await Task.WhenAll(tasks);

            //foreach (var project in ProjectRelatedInfo.ProjectNames)
            //{
            //    string apiUrl = $"https://vsrm.dev.azure.com/{organizationName}/{project}/_apis/release/deployments?api-version={apiVersion}";

            //    string jsonResponse = await GetApiResponse(apiUrl, pat);

            //    if (!string.IsNullOrEmpty(jsonResponse))
            //    {
            //        await ConvertReleaseData(jsonResponse);
            //    }
            //}

        }

        private async Task ConvertReleaseData(string jsonResponse)
        {
            ReleaseRoot myDeserializedClass = new ReleaseRoot();
            try
            {
                myDeserializedClass = JsonConvert.DeserializeObject<ReleaseRoot>(jsonResponse);
            }
            catch (Exception ex)
            {
                _Nlogger.Error(ex, "Error while desearlizing json!");
                throw new Exception("Error while desearlizing json!");
            }


            if (myDeserializedClass != null)
            {
                await LoadReleaseDataIntoMemory();

                List<Releases> releases = new List<Releases>();

                foreach (var item in myDeserializedClass.value)
                {
                    Releases release = new Releases();
                    foreach (var root in item.release.artifacts)
                    {
                        //Console.WriteLine("Project Name: " + root.definitionReference.project.name
                        //  + " " + "Build Pipeline Name: " + root.definitionReference.definition.name
                        //  + " " + "Build Name:  " + root.definitionReference.version.name
                        //  + " " + "Release Pipeline Name: " + item.releaseDefinition.name
                        //  + " " + "Release Name:  " + item.release.name
                        //  //+ " " + "Stage Name:  " + item.releaseEnvironment.name
                        //  //+ " " + "Deployment Status: " + item.deploymentStatus
                        //  //+ " " + "Createt At: " + item.queuedOn.ToCentralTimeZone()
                        //  //+ " " + "Completed At: " + item.completedOn.ToCentralTimeZone()
                        // );

                        release.ProjectName = root.definitionReference.project.name;
                        release.BuildPipelineId = root.definitionReference.definition.id;
                        release.BuildPipelineName = root.definitionReference.definition.name;
                        release.BuildId = root.definitionReference.version.id;
                        release.BuildName = root.definitionReference.version.name;
                        release.ReleasePipelineId = item.releaseDefinition.id;
                        release.ReleasePipelineName = item.releaseDefinition.name;
                        release.ReleaseId = item.id;
                        release.ReleaseName = item.release.name;
                        release.StageName = item.releaseEnvironment.name;
                        release.DeploymentResult = item.deploymentStatus;
                        release.CreatedFor = item.requestedFor.displayName;
                        release.ReleaseQueuedOn = item.queuedOn.ToCentralTimeZone();
                        release.ReleaseCreatedOn = item.startedOn.ToCentralTimeZone();
                        release.ReleaseCompletedOn = item.completedOn.ToCentralTimeZone();

                        if (item.preDeployApprovals.Count > 0)
                        {
                            foreach (var deploy in item.preDeployApprovals)
                            {
                                if (deploy != null && deploy.approvedBy != null)
                                {
                                    //Console.WriteLine("Approved By: " + deploy.approvedBy.displayName);
                                    release.ApprovedBy = deploy.approvedBy.displayName;
                                }
                            }
                        }
                    }

                    releases.Add(release);
                }

                await AddReleasesIfNotExists(releases);
            }
        }

        private async Task LoadReleaseDataIntoMemory()
        {
            existingReleases = await _context.Releases.ToListAsync();

            existingReleaseHashSet = new HashSet<(string, int, int)>(existingReleases.Select(r => (r.ProjectName, r.ReleasePipelineId, r.ReleaseId)));
        }

        public async Task AddReleasesIfNotExists(List<Releases> releases)
        {
            foreach (var release in releases)
            {
                if (release.DeploymentResult != "inProgress")
                {
                    if (!existingReleaseHashSet.Contains((release.ProjectName, release.ReleasePipelineId, release.ReleaseId)))
                    {
                        _context.Releases.Add(release);
                        Console.WriteLine($"Release added successfully: {release.ReleaseName}");
                        _Nlogger.Info($"Release added successfully: {release.BuildName} for Project Name: {release.ProjectName}");
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        private async Task<string> GetApiResponse(string url, string pat)
        {
            using (HttpClient client = new HttpClient())
            {
                // Set the Authorization header using the PAT
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    _Nlogger.Error($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }
            }
        }

        private void LoadAPIValues(out string organizationName, out string apiVersion, out string pat)
        {
            organizationName = _configuration.GetValue<string>("OrganizationName");
            apiVersion = _configuration.GetValue<string>("APIVersion");
            pat = _configuration.GetValue<string>("Pat");

            if (string.IsNullOrEmpty(organizationName))
            {
                throw new ArgumentException("OrganizationName can not be null!");
            }

            if (string.IsNullOrEmpty(apiVersion))
            {
                throw new ArgumentException("apiVersion can not be null!");
            }

            if (string.IsNullOrEmpty(pat))
            {
                throw new ArgumentException("PAT can not be null!");
            }
        }
    }
}
