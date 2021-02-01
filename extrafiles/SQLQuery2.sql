--USE AdventureWorks2016  
--GO  
--DECLARE @g geography = 'POINT(-121.626 47.8315)';  
--SELECT TOP(7) SpatialLocation.ToString(), City FROM Person.Address  
--WHERE SpatialLocation.STDistance(@g) IS NOT NULL  
--ORDER BY SpatialLocation.STDistance(@g); 


--DECLARE @g geometry;  
--SET @g = geometry::STGeomFromText('POINT(-121.626 47.8315)', 0);  
--SELECT @g.STX; 

--DECLARE @g geography;  
--SET @g = geography::STGeomFromText('LINESTRING(-122.360 47.656, -122.343 47.656)', 4326);  
--SELECT @g.STBuffer(1).ToString(); 


--DECLARE @address1 GEOGRAPHY
--DECLARE @address2 GEOGRAPHY
--DECLARE @distance float
--SET @address1 = GEOGRAPHY::STGeomFromText ('point(53.046908 -2.991673)',4326)
--SET @address2 = GEOGRAPHY::STGeomFromText ('point(51.500152 -0.126236)',4326)
--SET @distance = @address1.STDistance(@address2)
--SELECT @distance --this is the distance in meters 

DECLARE @g geometry;  
SET @g = geometry::STGeomFromText('POLYGON((-122.358 47.653, -122.348 47.649, -122.348 47.658, -122.358 47.658, -122.358 47.653))', 4326);  
SELECT @g.STGeometryType();  