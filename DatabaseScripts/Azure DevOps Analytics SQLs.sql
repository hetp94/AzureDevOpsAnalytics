USE [AzureAnalytics]
GO

/****** Object:  Table [dbo].[Builds]    ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Builds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectName] [varchar](100) NOT NULL,
	[Repository] [varchar](100) NULL,
	[AgentPoolName] [varchar](100) NOT NULL,
	[BranchInfo] [varchar](200) NOT NULL,
	[BuildPipelineId] [int] NOT NULL,
	[BuildPipelineName] [varchar](100) NOT NULL,
	[BuildId] [int] NOT NULL,
	[BuildName] [varchar](100) NOT NULL,
	[CreatedFor] [varchar](100) NOT NULL,
	[BuildResult] [varchar](50) NOT NULL,
	[BuildQueuedOn] [datetime] NOT NULL,
	[BuildCreatedOn] [datetime] NOT NULL,
	[BuildCompletedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Builds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[Releases]    ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Releases](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectName] [varchar](100) NOT NULL,
	[BuildPipelineId] [int] NOT NULL,
	[BuildPipelineName] [varchar](100) NOT NULL,
	[BuildId] [int] NOT NULL,
	[BuildName] [varchar](100) NOT NULL,
	[ReleasePipelineId] [int] NOT NULL,
	[ReleasePipelineName] [varchar](100) NOT NULL,
	[ReleaseId] [int] NOT NULL,
	[ReleaseName] [varchar](100) NOT NULL,
	[StageName] [varchar](100) NOT NULL,
	[DeploymentResult] [varchar](100) NOT NULL,
	[CreatedFor] [varchar](100) NOT NULL,
	[ReleaseQueuedOn] [datetime] NOT NULL,
	[ReleaseCreatedOn] [datetime] NOT NULL,
	[ReleaseCompletedOn] [datetime] NOT NULL,
	[ApprovedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Releases] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  View [dbo].[AzureDashboard]    ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AzureDashboard]
AS
SELECT        b.Id, b.ProjectName, b.Repository, b.AgentPoolName, b.BuildPipelineName, b.BuildId, b.BuildName, r.ReleasePipelineName, r.ReleaseId, r.ReleaseName, 
                         r.StageName AS ServerType, CONVERT(DATE, b.BuildCreatedOn) 
                         AS BuildDate, b.CreatedFor, r.ApprovedBy, b.BuildResult, r.DeploymentResult, DATEDIFF(SECOND, b.BuildCreatedOn, b.BuildCompletedOn) AS BuildTime, DATEDIFF(SECOND, r.ReleaseCreatedOn, r.ReleaseCompletedOn) 
                         AS ReleaseTime, DATEDIFF(SECOND, b.BuildCreatedOn, b.BuildCompletedOn) + DATEDIFF(SECOND, r.ReleaseCreatedOn, r.ReleaseCompletedOn) AS TimeTakenInSeconds
FROM            dbo.Builds AS b LEFT OUTER JOIN
                         dbo.Releases AS r ON r.ProjectName = b.ProjectName AND r.BuildId = b.BuildId
GO