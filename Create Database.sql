use master;
go

-- Drop the table and its backup history.

exec msdb.dbo.sp_delete_database_backuphistory @database_name = N'Timeline'
go

alter database [Timeline] set  single_user with rollback immediate
go

drop database if exists Timeline;
go

-- Create a new database named "Timeline". 

create database Timeline;
go

use Timeline;
go

-- Store commands, events, aggregates, and snapshots in a schema named "logs".

create schema logs;
go

create table logs.[Command]
(
    AggregateIdentifier uniqueidentifier not null
  , ExpectedVersion int null
  
  , IdentityTenant uniqueidentifier not null
  , IdentityUser uniqueidentifier not null

  , CommandClass varchar(200) not null
  , CommandType varchar(100) not null
  , CommandData nvarchar(max) not null
  
  , CommandIdentifier uniqueidentifier not null primary key
  
  , SendStatus varchar(20) null
  , SendError varchar(max) null

  , SendScheduled datetimeoffset(7) null
  , SendStarted datetimeoffset(7) null
  , SendCompleted datetimeoffset(7) null
  , SendCancelled datetimeoffset(7) null
);

create table logs.[Event]
(
    AggregateIdentifier uniqueidentifier not null
  , AggregateVersion int not null

  , IdentityTenant uniqueidentifier not null
  , IdentityUser uniqueidentifier not null

  , EventTime datetimeoffset(7) not null
  , EventClass varchar(200) not null
  , EventType varchar(100) not null
  , [EventData] nvarchar(max) not null

  , constraint PK_Event
        primary key clustered (
                                  AggregateIdentifier asc
                                , AggregateVersion asc
                              )
);

create table logs.[Aggregate]
(
    AggregateIdentifier uniqueidentifier not null
  , AggregateType varchar(100) not null
  , AggregateClass varchar(200) not null
  , AggregateExpires datetimeoffset(7) null
  , TenantIdentifier uniqueidentifier not null
  , constraint PK_Aggregate
        primary key clustered (AggregateIdentifier asc)
);

create table logs.[Snapshot]
(
    AggregateIdentifier uniqueidentifier not null
  , AggregateVersion int not null
  , AggregateState nvarchar(max) not null
  , constraint PK_Snapshot
        primary key clustered (AggregateIdentifier)
);

go

-- Store projections in a schema named "queries".

create schema queries;
go

create table queries.UserSummary 
(
  UserIdentifier uniqueidentifier not null primary key,
  LoginName varchar(100) not null,
  LoginPassword varchar(100) not null,
  UserRegistrationStatus varchar(10) not null
)
go

create table queries.PersonSummary 
(
  TenantIdentifier uniqueidentifier not null,
  
  UserIdentifier uniqueidentifier null,

  PersonIdentifier uniqueidentifier not null primary key,
  PersonName varchar(100) null,
  PersonRegistered datetimeoffset not null,

  OpenAccountCount int not null,
  TotalAccountBalance money null
)
go

create table queries.AccountSummary 
(
  TenantIdentifier uniqueidentifier not null,
  
  AccountIdentifier uniqueidentifier not null primary key,
  AccountCode varchar(100) null,
  AccountStatus varchar(10) null,
  AccountBalance money not null,

  OwnerIdentifier uniqueidentifier not null,
  OwnerName varchar(100) null
)
go

create table queries.TransferSummary 
(
  TenantIdentifier uniqueidentifier not null,
  
  TransferIdentifier uniqueidentifier not null primary key,
  TransferAmount money not null,
  TransferStatus varchar(10) null,
  TransferActivity varchar(20) null,

  FromAccountIdentifier uniqueidentifier not null,
  FromAccountOwner varchar(100) null,
  
  ToAccountIdentifier uniqueidentifier not null,
  ToAccountOwner varchar(100) null
)
go

-- Create a stored procedure to look after denormalizing projection data. For example, an event might contain an 
-- identifier for a person, and not contain the person's name. If we want the person's name in a projection query
-- then we have to chase that down after the data is inserted or updated in the query table.

create proc queries.Denormalize
as
begin

  -- Ensure the name of the account owner is current.

  update queries.AccountSummary set OwnerName = PersonName 
  from queries.PersonSummary where OwnerIdentifier = PersonIdentifier;

  update queries.TransferSummary set FromAccountOwner = PersonName 
  from queries.PersonSummary where FromAccountIdentifier = PersonIdentifier;

  update queries.TransferSummary set ToAccountOwner = PersonName 
  from queries.PersonSummary where ToAccountIdentifier = PersonIdentifier;

end;
go