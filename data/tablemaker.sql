CREATE TABLE PackageData (
  ResponseBody TEXT,
  QualifiedId Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.@id')) VIRTUAL,
  Authors Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.authors')) VIRTUAL,
  CatalogCommitId Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.catalog:commitId')) VIRTUAL,
  CatalogCommitTimeStampt datetime GENERATED ALWAYS AS (json_extract(ResponseBody, '$.catalog:commitTimeStamp')) VIRTUAL,
  Copyright Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.copyright')) VIRTUAL,
  Created datetime GENERATED ALWAYS AS (json_extract(ResponseBody, '$.created')) VIRTUAL,
  Description Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.description')) VIRTUAL,
  IconUrl Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.iconUrl')) VIRTUAL,
  Id Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.id')) VIRTUAL,
  IsPrerelease Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.isPrerelease')) VIRTUAL,
  LastEdited datetime GENERATED ALWAYS AS (json_extract(ResponseBody, '$.lastEdited')) VIRTUAL,
  LicenseUrl Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.licenseUrl')) VIRTUAL,
  Listed Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.listed')) VIRTUAL,
  PackageHash Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.packageHash')) VIRTUAL,
  PackageHashAlgorithm Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.packageHashAlgorithm')) VIRTUAL,
  PackageSize Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.packageSize')) VIRTUAL,
  ProjectUrl Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.projectUrl')) VIRTUAL,
  Published datetime GENERATED ALWAYS AS (json_extract(ResponseBody, '$.published')) VIRTUAL,
  Title Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.title')) VIRTUAL,
  VerbatimVersion Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.verbatimVersion')) VIRTUAL,
  Version Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.version')) VIRTUAL,
  DependencyGroups Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.dependencyGroups')) VIRTUAL,
  PackageEntries Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.packageEntries')) VIRTUAL,
  Tags Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.tags')) VIRTUAL,
  Context Text GENERATED ALWAYS AS (json_extract(ResponseBody, '$.@context')) VIRTUAL
)