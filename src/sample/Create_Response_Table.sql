CREATE TABLE [dbo].[Response](
	[Key] [uniqueidentifier] NOT NULL,
	[Value] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Response] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)
) ON [PRIMARY]