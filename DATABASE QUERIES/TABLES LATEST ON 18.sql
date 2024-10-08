USE [WFS_2590]
GO
/****** Object:  Table [dbo].[Drawing]    Script Date: 3/18/2024 12:36:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Drawing](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PackageID] [int] NOT NULL,
	[RegistDate] [datetime] NOT NULL,
	[Detail] [varchar](512) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Event]    Script Date: 3/18/2024 12:36:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Event](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](10) NOT NULL,
	[ClientID] [int] NOT NULL,
	[Regist] [datetime] NOT NULL,
	[TargetID] [int] NULL,
	[Role] [varchar](10) NULL,
	[Username] [varchar](20) NULL,
	[PackageNo] [varchar](20) NULL,
	[WPSNo] [varchar](20) NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoice]    Script Date: 3/18/2024 12:36:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoice](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ClientID] [int] NOT NULL,
	[Username] [varchar](20) NOT NULL,
	[Email] [varchar](30) NOT NULL,
	[PackageCount] [int] NOT NULL,
	[Total] [float] NOT NULL,
	[Status] [varchar](10) NOT NULL,
	[Regist] [datetime] NOT NULL,
	[GST] [float] NOT NULL,
	[InvoiceNo] [varchar](15) NULL,
	[City] [varchar](20) NULL,
	[Country] [varchar](20) NULL,
	[PostalCode] [varchar](10) NULL,
	[BusinessNo] [varchar](20) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Package]    Script Date: 3/18/2024 12:36:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Package](
	[PackageID] [int] IDENTITY(1,1) NOT NULL,
	[ClientID] [int] NOT NULL,
	[PackageNumber] [varchar](255) NOT NULL,
	[TypeOfRequest] [varchar](50) NOT NULL,
	[DateSubmitted] [date] NOT NULL,
	[EndDate] [date] NULL,
	[Deadline] [date] NOT NULL,
	[Status] [varchar](255) NULL,
	[Information] [nvarchar](max) NULL,
	[FollowUpPackageNumber] [varchar](255) NULL,
	[Priority] [int] NULL,
	[FileNames] [varchar](260) NULL,
	[Urls] [varchar](512) NULL,
	[Rate] [float] NULL,
	[Spend] [int] NULL,
	[ApproveState] [varchar](10) NULL,
	[DateRemoved] [date] NULL,
	[Reason] [varchar](100) NULL,
	[Summery] [varchar](512) NULL,
	[InvoiceID] [int] NULL,
 CONSTRAINT [PK__Package__322035ECC80DD3B9] PRIMARY KEY CLUSTERED 
(
	[PackageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WPS]    Script Date: 3/18/2024 12:36:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WPS](
	[WPSFormID] [int] NOT NULL,
	[PackageID] [int] NULL,
	[WPSNo] [varchar](255) NULL,
	[PQRNumber] [varchar](50) NULL,
	[EmployeeID] [int] NULL,
	[CompanyName] [varchar](50) NULL,
	[Date] [datetime] NULL,
	[Detail] [varchar](2048) NULL,
 CONSTRAINT [PK__WPS__63A3988A9086AB33] PRIMARY KEY CLUSTERED 
(
	[WPSFormID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WPS1]    Script Date: 3/18/2024 12:36:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WPS1](
	[WPSFormID] [int] IDENTITY(1,1) NOT NULL,
	[PackageID] [int] NOT NULL,
	[WPSNo] [varchar](255) NOT NULL,
	[PQRNumber] [varchar](50) NOT NULL,
	[EmployeeID] [int] NOT NULL,
	[CompanyName] [varchar](50) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Detail] [varchar](2048) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Package]  WITH CHECK ADD  CONSTRAINT [FK_Package_Client] FOREIGN KEY([ClientID])
REFERENCES [dbo].[Client] ([ClientID])
GO
ALTER TABLE [dbo].[Package] CHECK CONSTRAINT [FK_Package_Client]
GO
ALTER TABLE [dbo].[WPS]  WITH CHECK ADD  CONSTRAINT [FK__WPS__PackageID__44FF419A] FOREIGN KEY([PackageID])
REFERENCES [dbo].[Package] ([PackageID])
GO
ALTER TABLE [dbo].[WPS] CHECK CONSTRAINT [FK__WPS__PackageID__44FF419A]
GO
ALTER TABLE [dbo].[WPS]  WITH CHECK ADD  CONSTRAINT [FK_WPS_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([EmployeeID])
GO
ALTER TABLE [dbo].[WPS] CHECK CONSTRAINT [FK_WPS_Employee]
GO
ALTER TABLE [dbo].[Package]  WITH CHECK ADD  CONSTRAINT [CK__Package__TypeOfR__403A8C7D] CHECK  (([TypeOfRequest]='WPS' OR [TypeOfRequest]='Drawing'))
GO
ALTER TABLE [dbo].[Package] CHECK CONSTRAINT [CK__Package__TypeOfR__403A8C7D]
GO
