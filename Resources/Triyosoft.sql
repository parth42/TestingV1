USE [triyo-prod]
/****** Object:  Table [dbo].[tblAssignedWordPages]    Script Date: 2018-02-14 12:53:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAssignedWordPages](
	[AssignedWordPageID] [int] IDENTITY(1,1) NOT NULL,
	[AssignmentID] [int] NULL,
	[DocName] [varchar](100) NULL,
	[Sequence] [int] NULL,
	[IsPublished] [bit] NULL,
	[ReviewRequested] [bit] NULL,
	[PageRemarks] [varchar](500) NULL,
 CONSTRAINT [PK_tblAssignedDocs] PRIMARY KEY CLUSTERED 
(
	[AssignedWordPageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblAssignedExcelSheets]    Script Date: 2018-02-14 12:53:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAssignedExcelSheets](
	[AssignedExcelSheetID] [int] IDENTITY(1,1) NOT NULL,
	[AssignmentID] [int] NOT NULL,
	[SheetName] [varchar](250) NOT NULL,
	[Sequence] [int] NOT NULL,
	[SheetRemarks] [varchar](500) NULL,
	[IsSheetApproved] [bit] NOT NULL,
	[IsSheetModified] [bit] NOT NULL,
	[IsGrayedOut] [bit] NOT NULL,
 CONSTRAINT [PK_tblAssignedExcelSheets] PRIMARY KEY CLUSTERED 
(
	[AssignedExcelSheetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[tblAssignedPPTSlides]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAssignedPPTSlides](
	[AssignedPPTSildeID] [int] IDENTITY(1,1) NOT NULL,
	[AssignmentID] [int] NOT NULL,
	[SlideName] [nvarchar](50) NOT NULL,
	[Sequence] [int] NOT NULL,
	[PPTRemarks] [varchar](500) NULL,
	[IsPPTApproved] [bit] NULL,
	[IsTaskPPT] [bit] NULL,
	[AssignedDocsID] [int] NULL,
	[IsPPTModified] [bit] NULL,
	[IsGrayedOut] [bit] NULL,
	[ReviewRequested] [bit] NULL,
	[IsPublished] [bit] NULL,
 CONSTRAINT [PK_tblAssignedPPTSlides] PRIMARY KEY CLUSTERED 
(
	[AssignedPPTSildeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO


/****** Object:  Table [dbo].[tblAssignmentHistory]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAssignmentHistory](
	[AssignmentHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[AssignmentLogID] [int] NULL,
	[DocumentName] [varchar](max) NULL,
	[Comments] [varchar](max) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_tblAssignmentHistory] PRIMARY KEY CLUSTERED 
(
	[AssignmentHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[tblAssignmentLog]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAssignmentLog](
	[AssignmentLogID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentName] [nvarchar](max) NULL,
	[AssignmentID] [int] NULL,
	[Action] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_tblAssignmentLog] PRIMARY KEY CLUSTERED 
(
	[AssignmentLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


/****** Object:  Table [dbo].[tblAssignments]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblAssignments](
	[AssignmentID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentID] [int] NULL,
	[Content] [nvarchar](max) NULL,
	[ReplacementCode] [nvarchar](max) NULL,
	[Action] [int] NOT NULL,
	[DocumentType] [int] NULL,
	[DueDate] [datetime] NULL,
	[Comments] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[OrginalSourceFile] [nvarchar](max) NULL,
	[DocumentFile] [nvarchar](max) NULL,
	[ProjectID] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CompletedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[TaskName] [varchar](100) NOT NULL,
	[Status] [int] NULL,
	[MainAssignmentID] [int] NULL,
	[LockedByUserID] [int] NULL,
 CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED 
(
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[tblAssignmentMembers]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE TABLE [dbo].[tblAssignmentMembers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CanEdit] [bit] NOT NULL,
	[CanApprove] [bit] NOT NULL,
	[AssignmentID] [int] NULL,
	[UserID] [int] NULL,
	[CanPublish] [bit] NULL,
 CONSTRAINT [PK_tblAssignmentMembers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



/****** Object:  Table [dbo].[tblChatMessage]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblChatMessage](
	[ChatMessageID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserFromId] [int] NOT NULL,
	[UserToId] [int] NOT NULL,
	[Message] [varchar](max) NULL,
	[ClientGuid] [varchar](200) NOT NULL,
	[MessageSentDateTime] [datetime] NOT NULL,
	[IsRead] [bit] NOT NULL,
	[VisibleFrom] [bit] NULL,
	[VisibleTo] [bit] NULL,
 CONSTRAINT [PK_tblChatMessage] PRIMARY KEY CLUSTERED 
(
	[ChatMessageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCompany]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCompany](
	[CompanyID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyLogo] [varchar](250) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[Address] [varchar](50) NULL,
	[Zip] [varchar](25) NULL,
	[City] [varchar](50) NULL,
	[State] [varchar](50) NULL,
	[Country] [varchar](50) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsSuperAdmin] [bit] NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[WebsiteURL] [varchar](250) NULL,
	[IsAppointmentEnable] [bit] NOT NULL,
	[ExchangeServerURL] [varchar](255) NULL,
	[ExchangeServerUserName] [varchar](255) NULL,
	[ExchangeServerPassword] [varchar](255) NULL,
	[DateFormatID] [int] NULL,
	[IsMessengerServiceEnable] [bit] NOT NULL,
 CONSTRAINT [PK_tblCompany] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDateFormats]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDateFormats](
	[DateFormatID] [int] IDENTITY(1,1) NOT NULL,
	[DateFormat] [varchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_tblDateFormats] PRIMARY KEY CLUSTERED 
(
	[DateFormatID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblDocument]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblDocument](
	[DocumentID] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [varchar](max) NULL,
	[DocumentName] [varchar](max) NULL,
	[DocumentType] [int] NULL,
	[CompanyID] [int] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_tblDocument] PRIMARY KEY CLUSTERED 
(
	[DocumentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblExcelSheets]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblExcelSheets](
	[ExcelSheetID] [int] IDENTITY(1,1) NOT NULL,
	[SheetName] [varchar](250) NOT NULL,
	[MasterDocumentName] [varchar](500) NOT NULL,
	[Sequence] [int] NOT NULL,
	[IsOriginal] [bit] NULL,
 CONSTRAINT [PK_tblExcelSheets] PRIMARY KEY CLUSTERED 
(
	[ExcelSheetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblLogActivity]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblLogActivity](
	[LogActivityID] [int] IDENTITY(1,1) NOT NULL,
	[LogActivityTypeID] [int] NOT NULL,
	[ActivityDetails] [nvarchar](max) NULL,
	[IPAddress] [nvarchar](50) NULL,
	[ChangedID] [int] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[MenuItem] [nvarchar](50) NULL,
	[CompanyID] [int] NULL,
 CONSTRAINT [PK_tblLogActivity] PRIMARY KEY CLUSTERED 
(
	[LogActivityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblLogActivityTypes]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblLogActivityTypes](
	[LogActivityTypeID] [int] NOT NULL,
	[LogActivityName] [nvarchar](50) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblMenuItem]    Script Date: 10/23/2017 5:26:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblMenuItem](
	[MenuItemID] [int] IDENTITY(1,1) NOT NULL,
	[GUID] [uniqueidentifier] NULL,
	[MenuItem] [varchar](50) NULL,
	[MenuItemController] [varchar](50) NULL,
	[MenuItemView] [varchar](50) NULL,
	[SortOrder] [int] NULL,
	[ParentID] [int] NULL CONSTRAINT [DF__tblMenuIt__Paren__1B0907CE]  DEFAULT ((0)),
	[IsActive] [bit] NOT NULL CONSTRAINT [DF__tblMenuIt__IsAct__1BFD2C07]  DEFAULT ((1)),
 CONSTRAINT [PK__tblMenuI__8943F70203317E3D] PRIMARY KEY CLUSTERED 
(
	[MenuItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[tblMenuItems]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblMenuItems](
	[MenuItemID] [int] IDENTITY(1,1) NOT NULL,
	[MenuName] [nvarchar](50) NOT NULL,
	[Controller] [nvarchar](50) NOT NULL,
	[Action] [nvarchar](50) NOT NULL,
	[SortOrder] [int] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_tblMenuItems] PRIMARY KEY CLUSTERED 
(
	[MenuItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblPPTSlides]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblPPTSlides](
	[PPTSlidesID] [int] IDENTITY(1,1) NOT NULL,
	[SlideName] [nvarchar](50) NOT NULL,
	[MasterDocumentName] [nvarchar](max) NOT NULL,
	[Sequence] [int] NOT NULL,
	[IsOriginal] [bit] NULL,
 CONSTRAINT [PK_tblPPTSlides] PRIMARY KEY CLUSTERED 
(
	[PPTSlidesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblProjectDocuments]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProjectDocuments](
	[ProjectDocumentID] [int] IDENTITY(1,1) NOT NULL,
	[DocumentID] [int] NOT NULL,
	[ProjectID] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_tblProjectDocuments] PRIMARY KEY CLUSTERED 
(
	[ProjectDocumentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblProjectMembers]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProjectMembers](
	[ProjectMemberID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
 CONSTRAINT [PK_tblProjectMembers] PRIMARY KEY CLUSTERED 
(
	[ProjectMemberID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblProjects]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblProjects](
	[ProjectID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[FileName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
 CONSTRAINT [PK_tblProjects] PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblRole]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblRole](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyID] [int] NOT NULL,
	[Role] [varchar](50) NULL,
	[Description] [varchar](255) NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[IsAdminRole] [bit] NOT NULL,
 CONSTRAINT [PK__tblRole__8AFACE3A4222D4EF] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblRolePrivilages]    Script Date: 2018-02-14 12:53:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblRolePrivilages](
	[RolePrivilageID] [int] IDENTITY(1,1) NOT NULL,
	[MenuItemID] [int] NOT NULL,
	[ViewPermission] [bit] NOT NULL,
	[UserID] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Add] [bit] NULL,
	[Edit] [bit] NULL,
	[Delete] [bit] NULL,
	[Detail] [bit] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [int] NULL,
	[RoleID] [int] NULL,
 CONSTRAINT [PK_tblRolePrivilages] PRIMARY KEY CLUSTERED 
(
	[RolePrivilageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblSectionMaster]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSectionMaster](
	[SectionID] [int] IDENTITY(1,1) NOT NULL,
	[SectionName] [nvarchar](100) NOT NULL,
	[SectionURL] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[ContentType] [int] NOT NULL,
	[ContentFile] [nvarchar](100) NULL,
	[CompanyID] [int] NULL,
	[IsDeleted] [bit] NULL,
	[Status] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_tblSectionMaster] PRIMARY KEY CLUSTERED 
(
	[SectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblTemplateMaster]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblTemplateMaster](
	[TemplateID] [int] IDENTITY(1,1) NOT NULL,
	[TemplateName] [varchar](50) NOT NULL,
	[TemplateType] [int] NOT NULL,
	[Subject] [varchar](100) NULL,
	[TemplateContent] [varchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tblTemplateMaster] PRIMARY KEY CLUSTERED 
(
	[TemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblUserDepartment]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblUserDepartment](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[EmailID] [nvarchar](250) NULL,
	[UserName] [nvarchar](255) NOT NULL,
	[Password] [varchar](250) NULL,
	[Department] [varchar](250) NULL,
	[CompanyID] [int] NULL,
	[RoleID] [int] NULL,
	[ProfileImage] [varchar](250) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
	[IsAppointmentEnable] [bit] NOT NULL,
	[CanCreateSubtasks] [bit] NULL,
	[CanEdit] [bit] NULL,
	[CanApprove] [bit] NULL,
 CONSTRAINT [PK_tblUserDepartment] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[tblExcelRowMap]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE TABLE [dbo].[tblExcelRowMap](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RowId] [uniqueidentifier] NOT NULL,
	[MasterDocumentName] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[DateLastModified] [datetime] NOT NULL,
	[IsRemoved] [bit] NOT NULL,
	[AssignmentID] [int] NULL,
 CONSTRAINT [PK_tblExcelRowMap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  View [dbo].[vwRolePrivileges]    Script Date: 2018-02-14 12:53:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vwRolePrivileges]
AS
SELECT     dbo.tblMenuItem.MenuItemID, dbo.tblMenuItem.MenuItem, dbo.tblMenuItem.MenuItemController, dbo.tblMenuItem.MenuItemView, 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN CONVERT(bit, 0) ELSE dbo.tblRolePrivilages.[ViewPermission] END AS [ViewPermission], 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN CONVERT(bit, 0) ELSE dbo.tblRolePrivilages.[Add] END AS [Add], 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN CONVERT(bit, 0) ELSE dbo.tblRolePrivilages.Edit END AS Edit, 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN CONVERT(bit, 0) ELSE dbo.tblRolePrivilages.[Delete] END AS [Delete], 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN CONVERT(bit, 0) ELSE dbo.tblRolePrivilages.[Detail] END AS [Detail], 
                      dbo.tblMenuItem.SortOrder AS OrderID, dbo.tblMenuItem.ParentID, dbo.tblMenuItem.SortOrder,Convert(int, dbo.tblMenuItem.IsActive) as IsActive, 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN 0 ELSE dbo.tblRolePrivilages.RoleID END AS RoleID
FROM         dbo.tblMenuItem LEFT JOIN
                      dbo.tblRolePrivilages ON tblMenuItem.MenuItemID = dbo.tblRolePrivilages.MenuItemID
WHERE     dbo.tblMenuItem.ParentID = 0
UNION ALL
SELECT     dbo.tblMenuItem.MenuItemID, dbo.tblMenuItem.MenuItem, dbo.tblMenuItem.MenuItemController, dbo.tblMenuItem.MenuItemView, 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN CONVERT(bit, 0) ELSE dbo.tblRolePrivilages.[ViewPermission] END AS [ViewPermission], 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN CONVERT(bit, 0) ELSE dbo.tblRolePrivilages.[Add] END AS [Add], 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN CONVERT(bit, 0) ELSE dbo.tblRolePrivilages.Edit END AS Edit, 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN CONVERT(bit, 0) ELSE dbo.tblRolePrivilages.[Delete] END AS [Delete], 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN CONVERT(bit, 0) ELSE dbo.tblRolePrivilages.[Detail] END AS [Detail],
                          (SELECT     SortOrder
                            FROM          tblMenuItem
                            WHERE      MenuItemID = dbo.tblMenuItem.ParentID) AS OrderID, dbo.tblMenuItem.ParentID, dbo.tblMenuItem.SortOrder, Convert(int, dbo.tblMenuItem.IsActive) as IsActive, 
                      CASE WHEN dbo.tblRolePrivilages.MenuItemID IS NULL THEN 0 ELSE dbo.tblRolePrivilages.RoleID END AS RoleID
FROM         dbo.tblMenuItem LEFT JOIN
                      dbo.tblRolePrivilages ON tblMenuItem.MenuItemID = dbo.tblRolePrivilages.MenuItemID
WHERE     dbo.tblMenuItem.ParentID > 0
GO



/****** Object:  StoredProcedure [dbo].[RolePrev]    Script Date: 2018-02-14 12:54:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RolePrev] 
	-- Add the parameters for the stored procedure here
	@RoleID int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


   


CREATE TABLE #Temp1 (MenuItemID INT,MenuItem VARCHAR(50), MenuItemController VARCHAR(50), MenuItemView VARCHAR(50), [View] BIT, [Add] BIT, [Edit] BIT, [Delete] BIT,[Detail] BIT, OrderID int,MainParentID INT,ParentID INT, SortOrder int,IsActive bit ,RoleID int)
	DECLARE @MenuItemID INT
	DECLARE @ParentID INT
	DECLARE @MenuItem VARCHAR(50)
	DECLARE @MenuItemController VARCHAR(50)
	DECLARE @MenuItemView VARCHAR(50)	
	DECLARE @View BIT
	DECLARE @Add BIT
	DECLARE @Edit BIT
	DECLARE @Delete BIT
	DECLARE @Detail BIT
	DECLARE @SortOrder int
	DECLARE @IsActive BIT	
	DECLARE @OrderID BIT
	--DECLARE @RoleID BIT		
	
	
	DECLARE @FirstMenuItemID INT
	DECLARE @FirstParentID INT
	DECLARE @FirstMenuItem VARCHAR(50)
	DECLARE @FirstMenuItemController VARCHAR(50)
	DECLARE @FirstMenuItemView VARCHAR(50)	
	DECLARE @FirstView BIT
	DECLARE @FirstAdd BIT
	DECLARE @FirstEdit BIT
	DECLARE @FirstDelete BIT
	DECLARE @FirstDetail BIT
	DECLARE @FirstDispOrder int	
	DECLARE @FirstOrderID BIT
	DECLARE @FirstRoleID BIT
	DECLARE @FirstIsActive BIT
	
	DECLARE @SecondMenuItemID INT
	DECLARE @SecondParentID INT
	DECLARE @SecondMenuItem VARCHAR(50)
	DECLARE @SecondMenuItemController VARCHAR(50)
	DECLARE @SecondMenuItemView VARCHAR(50)	
	DECLARE @SecondView BIT
	DECLARE @SecondAdd BIT
	DECLARE @SecondEdit BIT
	DECLARE @SecondDelete BIT
	DECLARE @SecondDetail BIT
	DECLARE @SecondDispOrder int	
	DECLARE @SecondOrderID BIT
	DECLARE @SecondRoleID BIT
	DECLARE @SecondIsActive BIT


	DECLARE Cur_Parent CURSOR FOR SELECT MenuItemID, MenuItem,MenuItemController,MenuItemView,ParentID,SortOrder,IsActive FROM tblMenuItem WHERE ParentID = 0 AND IsActive = 1 ORDER BY SortOrder
	OPEN Cur_Parent
	
	FETCH NEXT FROM Cur_Parent INTO @MenuItemID,@MenuItem,@MenuItemController,@MenuItemView,@ParentID,@SortOrder,@IsActive	
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
	
		IF EXISTS(SELECT * FROM tblRolePrivilages WHERE MenuItemID = @MenuItemID AND RoleID = @RoleID)--AND RoleID = @RoleId
		BEGIN
			SELECT @View = ISNULL([ViewPermission],0), @Add = ISNULL([Add],0), @Edit = ISNULL([Edit],0), @Delete = ISNULL([Delete],0), @Detail = ISNULL(Detail,0)
			FROM tblRolePrivilages WHERE MenuItemID = @MenuItemID AND RoleId = @RoleID
			
		END
		ELSE
		BEGIN
			SET @View = 0
			SET @Add = 0 
			SET @Edit = 0
			SET @Delete = 0
			SET @Detail = 0
		END
		
		INSERT INTO #TEMP1 VALUES(@MenuItemID,@MenuItem, @MenuItemController,@MenuItemView, @View, @Add, @Edit, @Delete,@Detail,@MenuItemID,@ParentID,@ParentID,@SortOrder,1,@RoleID)
		
		DECLARE CurFirst_Child CURSOR FOR SELECT MenuItemID, MenuItem,MenuItemController,MenuItemView,ParentID,SortOrder,IsActive FROM tblMenuItem WHERE ParentID = @MenuItemID AND IsActive = 1 ORDER BY SortOrder
		OPEN CurFirst_Child
			FETCH NEXT FROM CurFirst_Child INTO @FirstMenuItemID,@FirstMenuItem,@FirstMenuItemController,@FirstMenuItemView,@FirstParentID,@FirstDispOrder,@FirstIsActive
			--FETCH NEXT FROM CurFirst_Child INTO @FirstMenuItemID,@FirstParentID, @FirstMenuItem, @FirstMenuItemView,@FirstDispOrder
			
			WHILE @@FETCH_STATUS = 0
			BEGIN
				IF EXISTS(SELECT * FROM tblRolePrivilages WHERE MenuItemID = @FirstMenuItemID AND RoleId = @RoleID)--AND RoleId = @RoleId
			BEGIN
				SELECT @FirstView = ISNULL([ViewPermission],0), @FirstAdd = ISNULL([Add],0), @FirstEdit = ISNULL([Edit],0), @FirstDelete = ISNULL([Delete],0), @FirstDetail = ISNULL(Detail,0)
				FROM tblRolePrivilages WHERE MenuItemID = @FirstMenuItemID  AND RoleId = @RoleID
			END
			ELSE
				BEGIN
				SET @FirstView = 0
				SET @FirstAdd = 0 
				SET @FirstEdit = 0
				SET @FirstDelete = 0
				SET @FirstDetail = 0
				END			
			
				INSERT INTO #TEMP1 VALUES(@FirstMenuItemID,@FirstMenuItem, @FirstMenuItemController,@FirstMenuItemView, @FirstView, @FirstAdd, @FirstEdit, @FirstDelete,@FirstDetail,@FirstMenuItemID,@FirstParentID,@FirstParentID,@FirstDispOrder,1,@RoleID)
				
					DECLARE CurSecond_Child CURSOR FOR SELECT MenuItemID, MenuItem,MenuItemController,MenuItemView,ParentID,SortOrder,IsActive FROM tblMenuItem  WHERE ParentID = @FirstMenuItemID AND IsActive = 1 ORDER BY MenuItem
					OPEN CurSecond_Child
					
						--FETCH NEXT FROM CurSecond_Child INTO @SecondMenuItemID,@SecondParentID, @SecondMenuItem, @SecondMenuItemView,@SecondDispOrder
						FETCH NEXT FROM CurSecond_Child INTO @SecondMenuItemID,@SecondMenuItem,@SecondMenuItemController,@SecondMenuItemView,@SecondParentID,@SecondDispOrder,@SecondIsActive
						
						WHILE @@FETCH_STATUS = 0
						BEGIN
							IF EXISTS(SELECT * FROM tblRolePrivilages WHERE MenuItemID = @SecondMenuItemID AND RoleId = @RoleID)--AND RoleId = @RoleId
						BEGIN
							SELECT @SecondView = ISNULL([ViewPermission],0), @SecondAdd = ISNULL([Add],0), @SecondEdit = ISNULL([Edit],0), @SecondDelete = ISNULL([Delete],0), @SecondDetail = ISNULL(Detail,0)
							FROM tblRolePrivilages WHERE MenuItemID = @SecondMenuItemID  AND RoleId = @RoleID 
						END
						ELSE
							BEGIN
							SET @SecondView = 0
							SET @SecondAdd = 0 
							SET @SecondEdit = 0
							SET @SecondDelete = 0
							SET @SecondDetail = 0
							END			
						
							--INSERT INTO #TEMP1 VALUES(@SecondMenuItemID,@SecondParentID, @SecondMenuItem, @SecondMenuItemView, @SecondView, @SecondAdd, @SecondEdit, @SecondDelete,@SecondDetail,@SecondDispOrder,@SecondMenuItemID,@RoleID)
							INSERT INTO #TEMP1 VALUES(@SecondMenuItemID,@SecondMenuItem, @SecondMenuItemController,@SecondMenuItemView, @SecondView, @SecondAdd, @SecondEdit, @SecondDelete,@SecondDetail,@SecondMenuItemID,@FirstParentID,@SecondParentID,@SecondDispOrder,1,@RoleID)
							
							--FETCH NEXT FROM CurSecond_Child INTO @SecondMenuItemID, @SecondParentID,@SecondMenuItem, @SecondMenuItemView,@SecondDispOrder 
							FETCH NEXT FROM CurSecond_Child INTO @SecondMenuItemID,@SecondMenuItem,@SecondMenuItemController,@SecondMenuItemView,@SecondParentID,@SecondDispOrder,@SecondIsActive
						END			
					
					CLOSE CurSecond_Child
					DEALLOCATE CurSecond_Child
				
				
				--FETCH NEXT FROM CurFirst_Child INTO @FirstMenuItemID, @FirstParentID,@FirstMenuItem, @FirstMenuItemView,@FirstDispOrder 
				FETCH NEXT FROM CurFirst_Child INTO @FirstMenuItemID,@FirstMenuItem,@FirstMenuItemController,@FirstMenuItemView,@FirstParentID,@FirstDispOrder,@FirstIsActive	
			END			
		
		CLOSE CurFirst_Child
		DEALLOCATE CurFirst_Child
		
		--FETCH NEXT FROM Cur_Parent INTO @MenuItemID,@ParentID, @MenuItem, @MenuItemView,@SortOrder	
		FETCH NEXT FROM Cur_Parent INTO @MenuItemID,@MenuItem,@MenuItemController,@MenuItemView,@ParentID,@SortOrder,@IsActive	
	END
	
	CLOSE Cur_Parent
	DEALLOCATE Cur_Parent

	SELECT * FROM #Temp1
	
	
	--DROP TABLE #Temp1

END
GO



SET IDENTITY_INSERT [dbo].[tblDateFormats] ON 

GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (1, N'MM/DD/YYYY', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (2, N'DD/MM/YYYY', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (3, N'YYYY/MM/DD', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (4, N'MM-DD-YYYY', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (5, N'DD-MM-YYYY', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (6, N'YYYY-MM-DD', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (9, N'YYYY-M-D', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (10, N'YYYY-MMM-DD', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (11, N'YYYY/M/D', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (12, N'YYYY/MMM/DD', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (14, N'YY/MM/DD', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (16, N'YY-MM-DD', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (17, N'D/M/YYYY', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (18, N'D-M-YYYY', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (19, N'M/D/YYYY', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (20, N'M-D-YYYY', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (21, N'MMM/DD/YYYY', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (23, N'DD/MMM/YYYY', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (24, N'MMM-DD-YYYY', 1)
GO
INSERT [dbo].[tblDateFormats] ([DateFormatID], [DateFormat], [IsActive]) VALUES (25, N'DD-MMM-YYYY', 1)
GO
SET IDENTITY_INSERT [dbo].[tblDateFormats] OFF
GO
SET IDENTITY_INSERT [dbo].[tblMenuItem] ON 

GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (1, N'c9eb23dd-6007-4608-82ad-2604035e4a1d', N'Create Task', N'Home', N'CreateAssignment', 1, 0, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (2, N'55298899-57f7-409a-b45f-dc32a41fc5fb', N'My Tasks', N'Home', N'Approval', 2, 0, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (3, N'7f1442a0-f81e-4c7c-b56b-4c3efdbdfea5', N'Assigned Tasks', N'Home', N'Assignment', 3, 0, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (4, N'58327bf9-4455-4a7f-8fd4-cc0df7f5ccd2', N'Section', N'Section', N'Index', 5, 0, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (5, N'948bc985-b7dd-49cc-a8b5-e3f576eb47e9', N'Projects', N'Projects', N'Index', 4, 0, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (6, N'5082d054-0929-4c50-bff9-75911a38af2c', N'Archived Projects', N'Projects', N'Archive', 1, 5, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (7, N'5d721fe4-909f-4a2d-a183-167baa656617', N'User', N'User', N'Index', 1, 0, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (8, N'71d5501d-6dbd-4371-82cc-bff8d70262ff', N'Role', N'Role', N'Index', 6, 7, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (9, N'cbff2c8b-e086-47c0-9ea0-95586ff3d5e2', N'Role Privileges', N'RolePrivileges', N'Index', 7, 7, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (10, N'8e9aedb0-d909-44e2-a3a7-6d0692a8f309', N'Company', N'Company', N'Index', 8, 0, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (11, N'd62664a8-314f-4b9e-8dd0-96874426de76', N'Completed Projects', N'Projects', N'Complete', 2, 5, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (12, N'44d1bb4d-1429-4312-96ea-ab99b1841e36', N'Log Activity', N'LogActivity', N'Index', 9, 0, 1)
GO
INSERT [dbo].[tblMenuItem] ([MenuItemID], [GUID], [MenuItem], [MenuItemController], [MenuItemView], [SortOrder], [ParentID], [IsActive]) VALUES (13, N'8e3ffb38-f689-41e0-a95d-872180ecde69', N'Updated Document', N'', N'', 9, 0, 1)
GO
SET IDENTITY_INSERT [dbo].[tblMenuItem] OFF
GO


SET IDENTITY_INSERT [dbo].[tblUserDepartment] ON 

INSERT [dbo].[tblUserDepartment] ([UserId], [FullName], [EmailID], [UserName], [Password], [Department], [CompanyID], [RoleID], [ProfileImage], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive], [IsAppointmentEnable],[CanCreateSubtasks],[CanEdit],[CanApprove]) VALUES (1, N'rajiv chatterjee', N'rajiv.chatterjee@triyosoft.com', N'rajiv.chatterjee', N'zQz8tO938Qbv+YK+/BDCrwKNpadgutomeb/Ey5/9ySA=', N'Central Team', 1, 1, N'', NULL, NULL, 1, CAST(N'2017-08-18 02:49:15.063' AS DateTime), 1, 1, 1, 1, 1)
SET IDENTITY_INSERT [dbo].[tblUserDepartment] OFF

SET IDENTITY_INSERT [dbo].[tblTemplateMaster] ON 

GO
INSERT [dbo].[tblTemplateMaster] ([TemplateID], [TemplateName], [TemplateType], [Subject], [TemplateContent], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'Forgot Password', 1, N'You have registered a request for forgot password.', N'<p>   Hello #FullName#,<br />   <br />   You have registered a request for forgot password.<br />   <br />   Kindly #click here# to retrieve your password.<br />   <br />   Thanks,<br />   Triyosoft.</p>', 1, 1, CAST(N'2017-08-24 07:02:34.550' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[tblTemplateMaster] ([TemplateID], [TemplateName], [TemplateType], [Subject], [TemplateContent], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'Task Notification', 1, N'#Subject#', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1.0"/>
<title>TRIYO</title>
<style>
div, p, a, li, td {
	-webkit-text-size-adjust:none;
}
h4, h3{ margin:0px; padding:0px; font-family:Arial, Helvetica, sans-serif; font-size:21px; color:#27676f; font-weight:500;}
p{ margin-top:5px;}
a {
	color:#313131;
	text-decoration:none;
}
#outlook a {
	padding:0;
} /* Force Outlook to provide a "view in browser" menu link. */
html {
	width: 100%;
}
body {
	width:100% !important;
	-webkit-text-size-adjust:100%;
	-ms-text-size-adjust:100%;
	margin:0;
	padding:0;
}
img {
	outline:none;
	text-decoration:none;
	border:none;
	-ms-interpolation-mode: bicubic;
}
a img {
	border:none;
}
.image_fix {
	display:block;
}
.content {
	width: 100%;
	max-width:100%;
}
.col425 {
	max-width:100% !important;
}
.col426 {
	max-width:100% !important;
}
.col427 {
	max-width:100% !important;
}
.myborder {
	border-right:1px solid #d9d4d4;
	border-left:1px solid #d9d4d4;
}
.col428 {
	max-width:100% !important;
}
.logo {
	text-align:left !important;
}
.tel {
	text-align:right !important;
}
.boxer {
	width:100% !important;
	text-align:left;
	max-width:100%!important;
}
.table {
	width: 100%;
	max-width: 100%;
	margin-bottom: 6px;
	border-width: 1px;
	border-collapse: collapse;
	border-spacing: 0;
	font-size: 13px;
}
	.table-bordered th,
  .table-bordered td {
    border: 1px solid #ddd !important;
	  padding: 2px 5px;
	  text-overflow: ellipsis;
  }
.table th{background-color: #4ca0dd; color: #fff; text-align: left;}
	
 @media only screen and (min-width:700px) {
.content {
 width: 100%;
 max-width: 650px;
}
.ftable {
width:30.33% !important;
border-right:1px solid #d9d4d4;
border-bottom:none !important;
}
.ftable2 {
width:36.33% !important;
border-right:1px solid #d9d4d4;
border-bottom:none !important;
}
.ftable3 {
width:33.33% !important;
}
.htable {
width:47% !important;
}
.tagline {
font-size:40px !important;
}
.col425 {
width:100%!important;
text-align:left;
max-width:300px !important;
}
.col426 {
 width:100%!important;
text-align:left;
max-width:180px !important;
}
.col427 {
 width:100%!important;
text-align:left;
max-width:50% !important;
}
.col428 {
 width:100%!important;
text-align:left;
max-width:216px !important;
}
.boxer {
width:100% !important;
text-align:left;
max-width:291px !important;
}
}
@media only screen and (max-width:699px) {
 .logo {
text-align:center !important;
}
.tel {
text-align:center !important;
}
table[class=htable] {
width:100% !important;
text-align:center;
}
 table[class=hide] {
display:none;
}
 .banner-tagline {
font-size:18px !important;
}
 tr[class=hide] {
display:none;
}
.col427 {
border:none;
}
.boxer {
width:100% !important;
text-align:left;
max-width:100% !important;
}
.myborder {
 border-right:none;
 border-left:none;
}
.social-icons {
text-align:center !important;
width:100% !important;
}
.text-left {
text-align:left !important;
}
.email {
text-align:center !important;
}
}
</style>
</head>
<body style="margin:0px; padding:0px; font-family:Arial, Helvetica, sans-serif; font-size:14px; color:#313131; background:#f5f5f5;">
<table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#f5f5f5">
  <tr>
    <td><!--[if (gte mso 9)|(IE)]>
    <table width="650" align="center" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <![endif]-->
      <table class="content" align="center" cellpadding="0" cellspacing="0" border="0" bgcolor="#ffffff">
        <tr>
          <td align="left" valign="top"  style="border:1px solid #d9d4d4; " bgcolor="#ffffff"><table class="content" align="center" cellpadding="0" cellspacing="0" border="0">
            
              <!--- Header -->
              <tr>
                <td><table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#efefef" style="border-bottom:solid 1px #1b75bb;">
                    <tr>
                      <td width="15">&nbsp;</td>
                      <td><!--[if (gte mso 9)|(IE)]>
    <table width="47%" align="left" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
            <![endif]-->
                        <table class="col425" align="left" border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                          <tr>
                            <td align="center" class="logo" height="71"><a href="#" style="text-decoration:none;"><img src="http://rrpdemo.triyosoft.com/CSS/images/logo.png" alt="Triyosoft" width="145" height="36" border="0" style="font-family:Calibri; font-size:25px; color:#1075c3; font-weight:bold;" /></a></td>
                          </tr>
                        </table>
                        <!--[if (gte mso 9)|(IE)]>
            </td>
        </tr>
    </table>
    <![endif]-->
                        <!--[if (gte mso 9)|(IE)]>
    <table width="40%" align="right" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
            <![endif]--><!--[if (gte mso 9)|(IE)]>
            </td>
        </tr>
    </table>
    <![endif]--></td>
                     
                    </tr>
                  </table></td>
              </tr>
              <!--- Header End -->
                       
              <!--- Line End -->
              <!--- Salutation -->
              <tr>
                <td><table width="100%" border="0" cellspacing="0" cellpadding="0" style="border-bottom:solid 1px #ececec;">
                    <tr>
                      <td colspan="3">&nbsp;</td>
                    </tr>
                    <tr>
                      <td width="15">&nbsp;</td>
                      <td><table width="100%" align="left" border="0" cellspacing="0" cellpadding="0">
                          <tr>
                            <td height="30" align="left" valign="middle" style="font-family:Arial, Helvetica, sans-serif; font-size:14px; color:#1a1a1a;">Hi #FullName#</td>
                          </tr>
                        </table></td>
                      <td width="15">&nbsp;</td>
                    </tr>
                    <tr>
                      <td>&nbsp;</td>
                      <td>&nbsp;</td>
                      <td>&nbsp;</td>
                    </tr>
                  </table></td>
              </tr>
              <!--- Salutation End -->
              <!--- Spcing Row -->
              <tr>
                <td></td>
              </tr>
              <!--- Spcing Row End -->
              <tr>
                <td><table width="100%" border="0" cellspacing="0" cellpadding="0">
                    
                    <tr>
                      <td width="15">&nbsp;</td>
                      <td valign="top"  style="font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#2d2d2d;  line-height:22px;  mso-line-height-rule:exactly; text-align:left;"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                          <td align="left" valign="top"><h3 style="margin-top:20px; color:#578726; font-size:22px;"><a title=''Click here to triyosoft'' href=''http://demo.triyosoft.com/Home/Approval'' alt=''Click here to triyosoft''>#TaskName#</a></h3></td>
                        </tr>
                        <tr>
                          <td align="left" valign="top"><p>#Action#. #Link#</p></td>
                        </tr>
						<tr>
                          <td align="left" valign="top">#Button#</td>
                        </tr>
                        <tr>
                          <td align="left" valign="top" style="border-bottom:solid 1px #dedede; height:0px;"></td>
                        </tr>
                      </table></td>
                      <td width="15">&nbsp;</td>
                    </tr>
                    <tr>
                      <td>&nbsp;</td>
                      <td valign="top"  style="font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#2d2d2d;  line-height:22px;  mso-line-height-rule:exactly; text-align:left;">
                   <table width="100%" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                          <td align="left" valign="top"><h3 style="margin-top:15px;">Description</h3></td>
                        </tr>
                        <tr>
                          <td align="left" valign="top"><p>#TaskDescription#</p></td>
                        </tr>
                        <tr>
                          <td align="left" valign="top" style="border-bottom:solid 1px #dedede; height:0px;"></td>
                        </tr>
                      </table>
                      
                      </td>
                      <td>&nbsp;</td>
                    </tr>
                    <tr>
                      <td>&nbsp;</td>
                      <td valign="top"  style="font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#2d2d2d;  line-height:22px;  mso-line-height-rule:exactly; text-align:left;"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                          <td align="left" valign="top"><h3 style="margin-top:15px;">Initial comment</h3></td>
                        </tr>
                        <tr>
                          <td align="left" valign="top"><p>#InitialComment#</p></td>
                        </tr>
                        <tr>
                          <td align="left" valign="top" style="border-bottom:solid 1px #dedede; height:0px;"></td>
                        </tr>
                      </table></td>
                      <td>&nbsp;</td>
                    </tr>
                    <tr>
                      <td>&nbsp;</td>
                      <td valign="top"  style="font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#2d2d2d;  line-height:22px;  mso-line-height-rule:exactly; text-align:left;"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                          <td align="left" valign="top"><h3 style="margin-top:15px; margin-bottom:12px;">Previous comments</h3></td>
                        </tr>
                        <tr>
                          <td align="left" valign="top">
                          <table class="table table-bordered" id="CommentHistoryTable">
						  #CommentHistory#
</table>
						</td>
                        </tr>
                        
                      </table></td>
                      <td>&nbsp;</td>
                    </tr>
                    </table>
                  </td>
              </tr>
              <tr>
              	<td>&nbsp;</td>
              </tr>
             
              
              
             
            
             
              
              <tr>
                <td><table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#e9e9e9" style="border-top: solid 1px #dfdfdf;">
                    <tr>
                      <td height="1"></td>
                    </tr>
                    <tr>
                      <td><table width="100%" border="0" cellspacing="0" cellpadding="0">
                          <tr>
                            <td width="15" rowspan="4" align="left">&nbsp;</td>
                            <td align="left">&nbsp;</td>
                          </tr>                          
                          <tr>
                            <td align="left" style="font-family:Arial, Helvetica, sans-serif; font-size:10px; font-weight:bold; color:#1b75bc;">©2017 Triyosoft</td>
                          </tr>
                          <tr>
                            <td align="left">&nbsp;</td>
                          </tr>
                        </table></td>
                    </tr>
                    <tr>
                      <td height="1"></td>
                    </tr>
                  </table></td>
              </tr>
              <!--- Footer End -->
            </table></td>
        </tr>
      </table>
      <!--[if (gte mso 9)|(IE)]>
        </td>
    </tr>
</table>
<![endif]--></td>
  </tr>
</table>
</body>
</html>
', 1, 1, CAST(N'2017-09-08 08:49:50.517' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[tblTemplateMaster] ([TemplateID], [TemplateName], [TemplateType], [Subject], [TemplateContent], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'Appointment Notification', 1, N'#Subject#', N'<p>   Hello, <br />   <br />   you have one task assigned <br />   <br /> <b>Task Name</b> : #TaskName# <br />   <br />   Thanks,<br />   Triyosoft.</p>', 1, 1, CAST(N'2017-10-17 11:42:35.270' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[tblTemplateMaster] ([TemplateID], [TemplateName], [TemplateType], [Subject], [TemplateContent], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, N'Welcome Page', 1, N'Welcome to Triyo Soft', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1.0"/>
<title>TRIYO</title>
<style>
div, p, a, li, td {
	-webkit-text-size-adjust:none;
}
h4, h3{ margin:0px; padding:0px; font-family:Arial, Helvetica, sans-serif; font-size:21px; color:#27676f; font-weight:500;}
p{ margin-top:5px;}
a {
	color:#313131;
	text-decoration:none;
}
#outlook a {
	padding:0;
} /* Force Outlook to provide a "view in browser" menu link. */
html {
	width: 100%;
}
body {
	width:100% !important;
	-webkit-text-size-adjust:100%;
	-ms-text-size-adjust:100%;
	margin:0;
	padding:0;
}
img {
	outline:none;
	text-decoration:none;
	border:none;
	-ms-interpolation-mode: bicubic;
}
a img {
	border:none;
}
.image_fix {
	display:block;
}
.content {
	width: 100%;
	max-width:100%;
}
.col425 {
	max-width:100% !important;
}
.col426 {
	max-width:100% !important;
}
.col427 {
	max-width:100% !important;
}
.myborder {
	border-right:1px solid #d9d4d4;
	border-left:1px solid #d9d4d4;
}
.col428 {
	max-width:100% !important;
}
.logo {
	text-align:left !important;
}
.tel {
	text-align:right !important;
}
.boxer {
	width:100% !important;
	text-align:left;
	max-width:100%!important;
}
.table {
	width: 100%;
	max-width: 100%;
	margin-bottom: 6px;
	border-width: 1px;
	border-collapse: collapse;
	border-spacing: 0;
	font-size: 13px;
}
	.table-bordered th,
  .table-bordered td {
    border: 1px solid #ddd !important;
	  padding: 2px 5px;
	  text-overflow: ellipsis;
  }
.table th{background-color: #4ca0dd; color: #fff; text-align: left;}
	
 @media only screen and (min-width:700px) {
.content {
 width: 100%;
 max-width: 650px;
}
.ftable {
width:30.33% !important;
border-right:1px solid #d9d4d4;
border-bottom:none !important;
}
.ftable2 {
width:36.33% !important;
border-right:1px solid #d9d4d4;
border-bottom:none !important;
}
.ftable3 {
width:33.33% !important;
}
.htable {
width:47% !important;
}
.tagline {
font-size:40px !important;
}
.col425 {
width:100%!important;
text-align:left;
max-width:300px !important;
}
.col426 {
 width:100%!important;
text-align:left;
max-width:180px !important;
}
.col427 {
 width:100%!important;
text-align:left;
max-width:50% !important;
}
.col428 {
 width:100%!important;
text-align:left;
max-width:216px !important;
}
.boxer {
width:100% !important;
text-align:left;
max-width:291px !important;
}
}
@media only screen and (max-width:699px) {
 .logo {
text-align:center !important;
}
.tel {
text-align:center !important;
}
table[class=htable] {
width:100% !important;
text-align:center;
}
 table[class=hide] {
display:none;
}
 .banner-tagline {
font-size:18px !important;
}
 tr[class=hide] {
display:none;
}
.col427 {
border:none;
}
.boxer {
width:100% !important;
text-align:left;
max-width:100% !important;
}
.myborder {
 border-right:none;
 border-left:none;
}
.social-icons {
text-align:center !important;
width:100% !important;
}
.text-left {
text-align:left !important;
}
.email {
text-align:center !important;
}
}
</style>
</head>
<body style="margin:0px; padding:0px; font-family:Arial, Helvetica, sans-serif; font-size:14px; color:#313131; background:#f5f5f5;">
<table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#f5f5f5">
  <tr>
    <td><!--[if (gte mso 9)|(IE)]>
    <table width="650" align="center" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <![endif]-->
      <table class="content" align="center" cellpadding="0" cellspacing="0" border="0" bgcolor="#ffffff">
        <tr>
          <td align="left" valign="top"  style="border:1px solid #d9d4d4; " bgcolor="#ffffff"><table class="content" align="center" cellpadding="0" cellspacing="0" border="0">
            
              <!--- Header -->
              <tr>
                <td><table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#efefef" style="border-bottom:solid 1px #1b75bb;">
                    <tr>
                      <td width="15">&nbsp;</td>
                      <td><!--[if (gte mso 9)|(IE)]>
    <table width="47%" align="left" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
            <![endif]-->
                        <table class="col425" align="left" border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                          <tr>
                            <td align="center" class="logo" height="71"><a href="#" style="text-decoration:none;"><img src="http://rrpdemo.triyosoft.com/CSS/images/logo.png" alt="Triyosoft" width="145" height="36" border="0" style="font-family:Calibri; font-size:25px; color:#1075c3; font-weight:bold;" /></a></td>
                          </tr>
                        </table>
                        <!--[if (gte mso 9)|(IE)]>
            </td>
        </tr>
    </table>
    <![endif]-->
                        <!--[if (gte mso 9)|(IE)]>
    <table width="40%" align="right" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
            <![endif]--><!--[if (gte mso 9)|(IE)]>
            </td>
        </tr>
    </table>
    <![endif]--></td>
                     
                    </tr>
                  </table></td>
              </tr>
              <!--- Header End -->
                       
              <!--- Line End -->
              <!--- Salutation -->
              <tr>
                <td><table width="100%" border="0" cellspacing="0" cellpadding="0" style="border-bottom:solid 1px #ececec;">
                    <tr>
                      <td colspan="3">&nbsp;</td>
                    </tr>
                    <tr>
                      <td width="15">&nbsp;</td>
                      <td><table width="100%" align="left" border="0" cellspacing="0" cellpadding="0">
                          <tr>
                            <td height="30" align="left" valign="middle" style="font-family:Arial, Helvetica, sans-serif; font-size:14px; color:#1a1a1a;">Hi #FullName#</td>
                          </tr>
                        </table></td>
                      <td width="15">&nbsp;</td>
                    </tr>
                    <tr>
                      <td>&nbsp;</td>
                      <td>&nbsp;</td>
                      <td>&nbsp;</td>
                    </tr>
                  </table></td>
              </tr>
              <!--- Salutation End -->
              <!--- Spcing Row -->
              <tr>
                <td></td>
              </tr>
              <!--- Spcing Row End -->
              <tr>
                <td><table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <td>&nbsp;</td>
                      <td valign="top"  style="font-family:Arial, Helvetica, sans-serif; font-size:15px; color:#2d2d2d;  line-height:22px;  mso-line-height-rule:exactly; text-align:left;"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                          <td align="left" valign="top"><h3 style="margin-top:15px; margin-bottom:12px;"><br />Welcome to Triyo!<br /></h3>
						  <p>Your account is now active.
						  </p>
						  <div class="tile-body table-content">
                                Use Link to Login  : <b><a href="#Login#" >Login Here</a></b>
                            </div>
							<div class="tile-body table-content">
                               Your UserName is : <b>#UserName#</b>
                            </div>
                            <div class="tile-body table-content">
                               Your password is : <b>#Password#</b>
                            </div>
						  </td>
                        </tr>
                        <tr>
                          <td align="left" valign="top">
						  <div class="tile-header">
                                <br /><br />
                                <strong>Regards,<br />Triyo Soft</strong>
                            </div>
						</td>
                        </tr>
                      </table></td>
                      <td></td>
                    </tr>
                    </table>
                  </td>
              </tr><tr>
                <td><table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#e9e9e9" style="border-top: solid 1px #dfdfdf;">
                    <tr>
                      <td height="1"></td>
                    </tr>
                    <tr>
                      <td><table width="100%" border="0" cellspacing="0" cellpadding="0">
                          <tr>
                            <td width="15" rowspan="4" align="left">&nbsp;</td>
                            <td align="left">&nbsp;</td>
                          </tr>                          
                          <tr>
                            <td align="left" style="font-family:Arial, Helvetica, sans-serif; font-size:10px; font-weight:bold; color:#1b75bc;">©2017 Triyosoft</td>
                          </tr>
                          <tr>
                            <td align="left">&nbsp;</td>
                          </tr>
                        </table></td>
                    </tr>
                    <tr>
                      <td height="1">
					  
					  </td>
                    </tr>
                  </table></td>
              </tr>
              <!--- Footer End -->
            </table></td>
        </tr>
      </table>
      <!--[if (gte mso 9)|(IE)]>
        </td>
    </tr>
</table>
<![endif]--></td>
  </tr>
</table>
</body>
</html>
', 1, 1, CAST(N'2017-10-17 11:42:35.270' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[tblTemplateMaster] OFF
GO

SET IDENTITY_INSERT [dbo].[tblMenuItems] OFF

SET IDENTITY_INSERT [dbo].[tblCompany] ON 

INSERT [dbo].[tblCompany] ([CompanyID], [CompanyLogo], [Name], [Address], [Zip], [City], [State], [Country], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsSuperAdmin], [IsActive], [IsDeleted], [WebsiteURL], [IsAppointmentEnable], [ExchangeServerURL], [ExchangeServerUserName], [ExchangeServerPassword], [DateFormatID], [IsMessengerServiceEnable]) VALUES (1, N'', N'Triyosoft', NULL, NULL, NULL, NULL, NULL, 1, CAST(N'2017-08-18 02:18:29.097' AS DateTime), 1, CAST(N'2017-08-23 08:31:54.737' AS DateTime), 1, 1, 0, NULL, 1, NULL, NULL, NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[tblCompany] OFF


SET IDENTITY_INSERT [dbo].[tblRole] ON 

INSERT [dbo].[tblRole] ([RoleID], [CompanyID], [Role], [Description], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive], [IsAdminRole]) VALUES (1, 1, N'Super Admin', N'', 1, CAST(N'2017-08-18 02:19:00.680' AS DateTime), NULL, NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[tblRole] OFF
SET IDENTITY_INSERT [dbo].[tblRolePrivilages] ON 

INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (1, 1, 1, 1, 1, CAST(N'2017-08-18 02:20:43.753' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (2, 2, 1, 1, 1, CAST(N'2017-08-18 02:20:43.763' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (3, 3, 1, 1, 1, CAST(N'2017-08-18 02:20:43.770' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (4, 4, 1, 1, 1, CAST(N'2017-08-18 02:20:43.777' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (5, 5, 1, 1, 1, CAST(N'2017-08-18 02:20:43.783' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (6, 6, 1, 1, 1, CAST(N'2017-08-18 02:20:43.787' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (12, 7, 1, 1, 1, CAST(N'2017-08-18 02:22:58.780' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (13, 8, 1, 1, 1, CAST(N'2017-08-18 02:22:58.787' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (14, 9, 1, 1, 1, CAST(N'2017-08-18 02:22:58.790' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (15, 10, 1, 1, 1, CAST(N'2017-08-18 02:22:58.800' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (16, 11, 1, 1, 1, CAST(N'2017-08-18 02:22:58.800' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
INSERT [dbo].[tblRolePrivilages] ([RolePrivilageID], [MenuItemID], [ViewPermission], [UserID], [CreatedBy], [CreatedDate], [Add], [Edit], [Delete], [Detail], [ModifiedBy], [ModifiedDate], [IsActive], [RoleID]) VALUES (17, 12, 1, 1, 1, CAST(N'2017-08-18 02:22:58.800' AS DateTime), 1, 1, 1, 1, NULL, NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[tblRolePrivilages] OFF


/****** Object:  Index [IX_FK_tblAssignedDocs_tblAssignments]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblAssignedDocs_tblAssignments] ON [dbo].[tblAssignedWordPages]
(
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblAssignedExcelSheets_tblAssignments]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblAssignedExcelSheets_tblAssignments] ON [dbo].[tblAssignedExcelSheets]
(
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblAssignmentHistory_tblAssignmentLog]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblAssignmentHistory_tblAssignmentLog] ON [dbo].[tblAssignmentHistory]
(
	[AssignmentLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblAssignmentHistory_tblUserDepartment]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblAssignmentHistory_tblUserDepartment] ON [dbo].[tblAssignmentHistory]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblAssignmentLog_tblAssignments]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblAssignmentLog_tblAssignments] ON [dbo].[tblAssignmentLog]
(
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblAssignmentLog_tblUserDepartment]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblAssignmentLog_tblUserDepartment] ON [dbo].[tblAssignmentLog]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_AssignmentID]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_AssignmentID] ON [dbo].[tblAssignmentMembers]
(
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblAssignmentMembers_tblAssignments]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblAssignmentMembers_tblAssignments] ON [dbo].[tblAssignmentMembers]
(
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblAssignmentMembers_tblUserDepartment]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblAssignmentMembers_tblUserDepartment] ON [dbo].[tblAssignmentMembers]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_UserID]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_UserID] ON [dbo].[tblAssignmentMembers]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblAssignments_tblProjects]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblAssignments_tblProjects] ON [dbo].[tblAssignments]
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblAssignments_tblUserDepartment]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblAssignments_tblUserDepartment] ON [dbo].[tblAssignments]
(
	[LockedByUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK__tblCompan__DateF__27F8EE98]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK__tblCompan__DateF__27F8EE98] ON [dbo].[tblCompany]
(
	[DateFormatID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblDocument_tblUserDepartment]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblDocument_tblUserDepartment] ON [dbo].[tblDocument]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblProjectDocuments_tblDocument]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblProjectDocuments_tblDocument] ON [dbo].[tblExcelRowMap]
(
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblProjectDocuments_tblDocument]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblProjectDocuments_tblDocument] ON [dbo].[tblProjectDocuments]
(
	[DocumentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblProjectDocuments_tblProjects]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblProjectDocuments_tblProjects] ON [dbo].[tblProjectDocuments]
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblProjectMembers_tblProjects]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblProjectMembers_tblProjects] ON [dbo].[tblProjectMembers]
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblProjectMembers_tblUserDepartment]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblProjectMembers_tblUserDepartment] ON [dbo].[tblProjectMembers]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblProjectMembers_tblUserDepartment1]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblProjectMembers_tblUserDepartment1] ON [dbo].[tblProjectMembers]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblProjects_tblUserDepartment]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblProjects_tblUserDepartment] ON [dbo].[tblProjects]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblProjects_tblUserDepartment1]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblProjects_tblUserDepartment1] ON [dbo].[tblProjects]
(
	[ModifiedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblRole_tblCompany]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblRole_tblCompany] ON [dbo].[tblRole]
(
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblRolePrivilages_tblUserDepartment]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblRolePrivilages_tblUserDepartment] ON [dbo].[tblRolePrivilages]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblSectionMaster_tblCompany]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblSectionMaster_tblCompany] ON [dbo].[tblSectionMaster]
(
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblSectionMaster_tblUserDepartment]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblSectionMaster_tblUserDepartment] ON [dbo].[tblSectionMaster]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblTemplateMaster_tblUserDepartment]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblTemplateMaster_tblUserDepartment] ON [dbo].[tblTemplateMaster]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblTemplateMaster_tblUserDepartment1]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblTemplateMaster_tblUserDepartment1] ON [dbo].[tblTemplateMaster]
(
	[ModifiedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblUserDepartment_tblCompany]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblUserDepartment_tblCompany] ON [dbo].[tblUserDepartment]
(
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_tblUserDepartment_tblRole]    Script Date: 2018-02-14 12:54:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_tblUserDepartment_tblRole] ON [dbo].[tblUserDepartment]
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblAssignedPPTSlides] ADD  CONSTRAINT [DF_tblAssignedPPTSlides_IsTaskPPT]  DEFAULT ((0)) FOR [IsTaskPPT]
GO
ALTER TABLE [dbo].[tblCompany] ADD  CONSTRAINT [DF_tblCompany_IsSuperAdmin]  DEFAULT ((0)) FOR [IsSuperAdmin]
GO
ALTER TABLE [dbo].[tblCompany] ADD  CONSTRAINT [DF_tblCompany_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[tblCompany] ADD  CONSTRAINT [DF_tblCompany_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[tblLogActivity] ADD  DEFAULT ((0)) FOR [CompanyID]
GO
ALTER TABLE [dbo].[tblRolePrivilages] ADD  CONSTRAINT [DF_tblRolePrivilages_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tblSectionMaster] ADD  CONSTRAINT [DF_tblSectionMaster_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[tblSectionMaster] ADD  CONSTRAINT [DF_tblSectionMaster_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[tblAssignedWordPages]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignedDocs_tblAssignments] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[tblAssignments] ([AssignmentID])
GO
ALTER TABLE [dbo].[tblAssignedWordPages] CHECK CONSTRAINT [FK_tblAssignedDocs_tblAssignments]
GO
ALTER TABLE [dbo].[tblAssignedExcelSheets]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignedExcelSheets_tblAssignments] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[tblAssignments] ([AssignmentID])
GO
ALTER TABLE [dbo].[tblAssignedExcelSheets] CHECK CONSTRAINT [FK_tblAssignedExcelSheets_tblAssignments]
GO
ALTER TABLE [dbo].[tblAssignmentHistory]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignmentHistory_tblAssignmentLog] FOREIGN KEY([AssignmentLogID])
REFERENCES [dbo].[tblAssignmentLog] ([AssignmentLogID])
GO
ALTER TABLE [dbo].[tblAssignmentHistory] CHECK CONSTRAINT [FK_tblAssignmentHistory_tblAssignmentLog]
GO
ALTER TABLE [dbo].[tblAssignmentHistory]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignmentHistory_tblUserDepartment] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblAssignmentHistory] CHECK CONSTRAINT [FK_tblAssignmentHistory_tblUserDepartment]
GO
ALTER TABLE [dbo].[tblAssignmentLog]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignmentLog_tblAssignments] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[tblAssignments] ([AssignmentID])
GO
ALTER TABLE [dbo].[tblAssignmentLog] CHECK CONSTRAINT [FK_tblAssignmentLog_tblAssignments]
GO
ALTER TABLE [dbo].[tblAssignmentLog]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignmentLog_tblUserDepartment] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblAssignmentLog] CHECK CONSTRAINT [FK_tblAssignmentLog_tblUserDepartment]
GO
ALTER TABLE [dbo].[tblAssignmentMembers]  WITH CHECK ADD  CONSTRAINT [FK_AssignmentID] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[tblAssignments] ([AssignmentID])
GO
ALTER TABLE [dbo].[tblAssignmentMembers] CHECK CONSTRAINT [FK_AssignmentID]
GO
ALTER TABLE [dbo].[tblAssignmentMembers]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignmentMembers_tblAssignments] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[tblAssignments] ([AssignmentID])
GO
ALTER TABLE [dbo].[tblAssignmentMembers] CHECK CONSTRAINT [FK_tblAssignmentMembers_tblAssignments]
GO
ALTER TABLE [dbo].[tblAssignmentMembers]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignmentMembers_tblUserDepartment] FOREIGN KEY([UserID])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblAssignmentMembers] CHECK CONSTRAINT [FK_tblAssignmentMembers_tblUserDepartment]
GO
ALTER TABLE [dbo].[tblAssignmentMembers]  WITH CHECK ADD  CONSTRAINT [FK_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblAssignmentMembers] CHECK CONSTRAINT [FK_UserID]
GO
ALTER TABLE [dbo].[tblAssignments]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignments_tblAssignments] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[tblAssignments] ([AssignmentID])
GO
ALTER TABLE [dbo].[tblAssignments] CHECK CONSTRAINT [FK_tblAssignments_tblAssignments]
GO
ALTER TABLE [dbo].[tblAssignments]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignments_tblProjects] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[tblProjects] ([ProjectID])
GO
ALTER TABLE [dbo].[tblAssignments] CHECK CONSTRAINT [FK_tblAssignments_tblProjects]
GO
ALTER TABLE [dbo].[tblAssignments]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignments_tblUserDepartment] FOREIGN KEY([LockedByUserID])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblAssignments] CHECK CONSTRAINT [FK_tblAssignments_tblUserDepartment]
GO
ALTER TABLE [dbo].[tblAssignments]  WITH CHECK ADD  CONSTRAINT [FK_tblAssignments_tblUserDepartment1] FOREIGN KEY([DocumentID])
REFERENCES [dbo].[tblDocument] ([DocumentID])
GO
ALTER TABLE [dbo].[tblAssignments] CHECK CONSTRAINT [FK_tblAssignments_tblUserDepartment1]
GO
ALTER TABLE [dbo].[tblCompany]  WITH CHECK ADD  CONSTRAINT [FK__tblCompan__DateF__27F8EE98] FOREIGN KEY([DateFormatID])
REFERENCES [dbo].[tblDateFormats] ([DateFormatID])
GO
ALTER TABLE [dbo].[tblCompany] CHECK CONSTRAINT [FK__tblCompan__DateF__27F8EE98]
GO
ALTER TABLE [dbo].[tblDocument]  WITH CHECK ADD  CONSTRAINT [FK_tblDocument_tblUserDepartment] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblDocument] CHECK CONSTRAINT [FK_tblDocument_tblUserDepartment]
GO
ALTER TABLE [dbo].[tblExcelRowMap]  WITH CHECK ADD  CONSTRAINT [FK_tblExcelRowMap_tblAssignments] FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[tblAssignments] ([AssignmentID])
GO
ALTER TABLE [dbo].[tblExcelRowMap] CHECK CONSTRAINT [FK_tblExcelRowMap_tblAssignments]
GO
ALTER TABLE [dbo].[tblProjectDocuments]  WITH CHECK ADD  CONSTRAINT [FK_tblProjectDocuments_tblDocument] FOREIGN KEY([DocumentID])
REFERENCES [dbo].[tblDocument] ([DocumentID])
GO
ALTER TABLE [dbo].[tblProjectDocuments] CHECK CONSTRAINT [FK_tblProjectDocuments_tblDocument]
GO
ALTER TABLE [dbo].[tblProjectDocuments]  WITH CHECK ADD  CONSTRAINT [FK_tblProjectDocuments_tblProjects] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[tblProjects] ([ProjectID])
GO
ALTER TABLE [dbo].[tblProjectDocuments] CHECK CONSTRAINT [FK_tblProjectDocuments_tblProjects]
GO
ALTER TABLE [dbo].[tblProjectMembers]  WITH CHECK ADD  CONSTRAINT [FK_tblProjectMembers_tblProjects] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[tblProjects] ([ProjectID])
GO
ALTER TABLE [dbo].[tblProjectMembers] CHECK CONSTRAINT [FK_tblProjectMembers_tblProjects]
GO
ALTER TABLE [dbo].[tblProjectMembers]  WITH CHECK ADD  CONSTRAINT [FK_tblProjectMembers_tblUserDepartment] FOREIGN KEY([UserID])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblProjectMembers] CHECK CONSTRAINT [FK_tblProjectMembers_tblUserDepartment]
GO
ALTER TABLE [dbo].[tblProjectMembers]  WITH CHECK ADD  CONSTRAINT [FK_tblProjectMembers_tblUserDepartment1] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblProjectMembers] CHECK CONSTRAINT [FK_tblProjectMembers_tblUserDepartment1]
GO
ALTER TABLE [dbo].[tblProjects]  WITH CHECK ADD  CONSTRAINT [FK_tblProjects_tblUserDepartment] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblProjects] CHECK CONSTRAINT [FK_tblProjects_tblUserDepartment]
GO
ALTER TABLE [dbo].[tblProjects]  WITH CHECK ADD  CONSTRAINT [FK_tblProjects_tblUserDepartment1] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblProjects] CHECK CONSTRAINT [FK_tblProjects_tblUserDepartment1]
GO
ALTER TABLE [dbo].[tblRole]  WITH CHECK ADD  CONSTRAINT [FK_tblRole_tblCompany] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[tblCompany] ([CompanyID])
GO
ALTER TABLE [dbo].[tblRole] CHECK CONSTRAINT [FK_tblRole_tblCompany]
GO
ALTER TABLE [dbo].[tblRolePrivilages]  WITH CHECK ADD  CONSTRAINT [FK_tblRolePrivilages_tblUserDepartment] FOREIGN KEY([UserID])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblRolePrivilages] CHECK CONSTRAINT [FK_tblRolePrivilages_tblUserDepartment]
GO
ALTER TABLE [dbo].[tblSectionMaster]  WITH CHECK ADD  CONSTRAINT [FK_tblSectionMaster_tblCompany] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[tblCompany] ([CompanyID])
GO
ALTER TABLE [dbo].[tblSectionMaster] CHECK CONSTRAINT [FK_tblSectionMaster_tblCompany]
GO
ALTER TABLE [dbo].[tblSectionMaster]  WITH CHECK ADD  CONSTRAINT [FK_tblSectionMaster_tblUserDepartment] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblSectionMaster] CHECK CONSTRAINT [FK_tblSectionMaster_tblUserDepartment]
GO
ALTER TABLE [dbo].[tblTemplateMaster]  WITH CHECK ADD  CONSTRAINT [FK_tblTemplateMaster_tblUserDepartment] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblTemplateMaster] CHECK CONSTRAINT [FK_tblTemplateMaster_tblUserDepartment]
GO
ALTER TABLE [dbo].[tblTemplateMaster]  WITH CHECK ADD  CONSTRAINT [FK_tblTemplateMaster_tblUserDepartment1] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[tblUserDepartment] ([UserId])
GO
ALTER TABLE [dbo].[tblTemplateMaster] CHECK CONSTRAINT [FK_tblTemplateMaster_tblUserDepartment1]
GO
ALTER TABLE [dbo].[tblUserDepartment]  WITH CHECK ADD  CONSTRAINT [FK_tblUserDepartment_tblCompany] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[tblCompany] ([CompanyID])
GO
ALTER TABLE [dbo].[tblUserDepartment] CHECK CONSTRAINT [FK_tblUserDepartment_tblCompany]
GO
ALTER TABLE [dbo].[tblUserDepartment]  WITH CHECK ADD  CONSTRAINT [FK_tblUserDepartment_tblRole] FOREIGN KEY([RoleID])
REFERENCES [dbo].[tblRole] ([RoleID])
GO
ALTER TABLE [dbo].[tblUserDepartment] CHECK CONSTRAINT [FK_tblUserDepartment_tblRole]
GO

