WITH RAW_DEPS AS (
	SELECT PackageData.Id, Version, Description, ProjectUrl, value DependencyBatch, max(Published) LastPublished
	FROM PackageData, json_each(DependencyGroups)
	WHERE json_extract(DependencyBatch, '$.@type') LIKE 'PackageDependencyGroup'
	GROUP BY PackageData.Id),
FLAT_DEPS AS (
	SELECT RAW_DEPS.Id, Version, Description, ProjectUrl, LastPublished, json_extract(value, '$.id') DependentPackageId, json_extract(value, '$.range') DependentPackageVersionRange 
	FROM RAW_DEPS, json_each(json_extract(DependencyBatch, '$.dependencies')))
SELECT *
FROM FLAT_DEPS
WHERE DependentPackageId LIKE 'Microsoft.IdentityModel.Clients.ActiveDirectory'