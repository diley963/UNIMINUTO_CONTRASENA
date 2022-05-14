--USE [master]
--GO

CREATE DATABASE [PasswordChange]
GO

Use [PasswordChange]
GO
--Begin Create Table
CREATE TABLE [dbo].[CodeByUser](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cedula] [varchar](20) NOT NULL,
	[email] [varchar](100) NULL,
	[codVerificacion] [nchar](10) NOT NULL,
	[fCaducidad] [datetime] NOT NULL,
	[codBloqueado] [bit] NULL,
 CONSTRAINT [PK_CodeByUser] PRIMARY KEY ([id] ASC)
 )
GO

CREATE TABLE [dbo].[UserLocked](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cedula] [varchar](20) NULL,
	[email] [varchar](100) NULL,
	[fDesbloqueo] [datetime] NULL,
 CONSTRAINT [PK_UserLocked] PRIMARY KEY ([id] ASC)
) 
GO

--end Create Table

--begin Create Sp

CREATE PROCEDURE [dbo].[GetCodeByUser]
	
	@cedula  varchar(20), 
	@email varchar(100)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT top(1) id,cedula,email,codVerificacion,fCaducidad,codBloqueado from CodeByUser where (cedula=@cedula and email=@email) order by id desc; 
    
END
GO

CREATE PROCEDURE [dbo].[GetUserBlocked]

    @cedula  varchar(20), 
	@email varchar(100)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT top(1) id,cedula,email,fDesbloqueo from UserLocked where cedula=@cedula and email=@email order by id desc; 
END
GO

CREATE PROCEDURE [dbo].[InsertBlockedUser]

    @cedula  varchar(20), 
	@email varchar(100),
	@fDesbloqueo varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

	 declare  @Existe smallint ;
	 set @Existe = (select count(*) from UserLocked where cedula=@cedula and email =@email);

	 if(@Existe>0)
		begin
				update UserLocked set fDesbloqueo=@fDesbloqueo where cedula=@cedula and email=@email;
		end

	  else
	  begin
		 			INSERT INTO [UserLocked]
				   ([cedula]
				   ,[email]
				   ,[fDesbloqueo])
			 VALUES
				   (@cedula,
					@email, 
				   CONVERT(datetime, @fDesbloqueo))
	  end
END
GO

CREATE PROCEDURE [dbo].[InsertCodeByUser]
	
	@cedula  varchar(20), 
	@email varchar(100),
	@codVerificacion nchar(10),
	@fCaducidad varchar(100)
AS
BEGIN

	SET NOCOUNT ON;

    declare  @Existe smallint ;
	 set @Existe = (select count(*) from CodeByUser where cedula=@cedula and email =@email);

	 if(@Existe>0)
		begin
				update CodeByUser set fCaducidad=@fCaducidad, codVerificacion=@codVerificacion,codBloqueado=0
				       where cedula=@cedula and email=@email;
		end

	  else
	  begin
		 			INSERT INTO [CodeByUser]
				   ([cedula]
				   ,[email]
				   ,[codVerificacion]
				   ,[fCaducidad]
				   ,[codBloqueado])
			 VALUES
				   (@cedula,
					@email, 
					@codVerificacion,
				   CONVERT(datetime, @fCaducidad),
				   0)
	  end
END
GO

CREATE PROCEDURE [dbo].[UpdateFlagCodBloqueado]
	@cedula  varchar(20), 
	@email varchar(100),
	@isBloqueado bit
AS
BEGIN
		SET NOCOUNT ON;

   update CodeByUser set codBloqueado=@isBloqueado
				       where cedula=@cedula and email=@email;
END
