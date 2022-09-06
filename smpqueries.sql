/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [SessionClassArchiveId]
      ,[SessionClassId]
      ,[SessionTermId]
      ,[StudentContactId]
      ,[HasPrintedResult]
      ,[Deleted]
      ,[CreatedOn]
      ,[UpdatedOn]
      ,[CreatedBy]
      ,[UpdatedBy]
  FROM [db_a86846_spmbackend].[dbo].[SessionClassArchive]

  select SessionClass.SessionClassId, ClassLookUp.ClassLookupId
  from SessionClass
  join ClassLookUp on ClassLookupId =  SessionClass.SessionClassId
  where SessionClassId = '04905A5D-4C61-4208-C0D9-08DA8453AE85'

  select * from SessionClassArchive where SessionTermId = '1afab6de-4d41-41e8-8780-08da89a56409' --'dd6f651e-97c0-4c5d-84eb-7e58d3f96248'
    select * from SessionClassArchive where SessionClassId = 'e9bad1b4-5bd3-4a9e-ac74-08da89b1958b' --'dd6f651e-97c0-4c5d-84eb-7e58d3f96248'

  select * from StudentNoteComment where StudentNoteId = '8697c8ad-39d5-494a-3566-08da89cd03ad'


  select SessionClass.SessionClassId, ClassLookUp.Name, Session.SessionId, Session.StartDate, Session.EndDate, SessionTerm.SessionTermId
  from SessionClass
  join Session on Session.SessionId =  SessionClass.SessionId
  join ClassLookUp on ClassId = ClassLookUpId
  join SessionTerm on SessionTerm.SessionId = Session.SessionId
  where Session.StartDate = '3336'

  select * from SessionTerm where SessionId = '10E58E7D-6ECA-46C8-45DC-08DA8EFC2A35'