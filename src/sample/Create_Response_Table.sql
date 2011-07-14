CREATE TABLE [dbo].[Response](
	[Key] [uniqueidentifier] NOT NULL,
	[Url] [varchar](2000) NOT NULL,
	[CachedOn] [DateTime] NOT NULL,
	[Value] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Response] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)
) ON [PRIMARY]