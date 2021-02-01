select count(id) from SuperMarkets;

ALTER INDEX ALL ON SuperMarkets
DISABLE;

ALTER INDEX ALL ON SuperMarkets
REBUILD;

select * from SuperMarkets s where s.Id = 6000000;

----   select points inside polygon ------

DECLARE @tehranZone geometry;   
SET @tehranZone = geometry::STPolyFromText('POLYGON ((51.35833740234375 35.73703779932528,
 51.359710693359375 35.67626300279665,
 51.40571594238281 35.646137228802424,
 51.44554138183594 35.68351380631503,
 51.44279479980469 35.73982452242507,
 51.402969360351555 35.762114795721,
 51.35833740234375 35.73703779932528))', 0);  

select top 5 * from SuperMarkets s 
WITH(INDEX("SpatialIndex-20200405-013341"))
where @tehranZone.STIntersects(s.GeoData) = 1 

---SELECT @tehranZone.ToString(); 
--SELECT @g.STIntersects(@h)


--WITH(INDEX("SpatialIndex-20200405-005707"))
--where s.GeoData.STIntersects(@tehranZone) = 1 --STContains
--where @tehranZone.STContains(s.GeoData) = 1 

--------       -----------------------

-- WITH(INDEX("PK_dbo.SuperMarkets"))

------

--35.648721, 51.26348
DECLARE @loc geometry =  geometry::STGeomFromText('POINT(51.26348 35.648721)', 0);
select * from SuperMarkets where GeoData.STEquals(@loc) = 1 ;

DECLARE @place geometry = 'POINT(51.26348 35.648721)'; 
SELECT TOP(7) id,Content,GeoData, GeoData.STDistance(@place) FROM SuperMarkets
WITH(INDEX("SpatialIndex-20200405-013341"))
WHERE GeoData.STDistance(@place) IS NOT NULL  
ORDER BY GeoData.STDistance(@place);  

DECLARE @g geometry =  geometry::STGeomFromText('POINT(51.235813 35.554685)', 0);
select * from SuperMarkets s
where (SELECT @g.STEquals(s.GeoData)) = 1 ;



select top 5 * INTO tempTable from SuperMarkets order by SuperMarkets.Id desc 

select top(5) GeoData.STAsText() from SuperMarkets;

--UPDATE SuperMarkets SET loc = geometry::STGeomFromText(GeoData.STAsText(), 0);

--minx
select top 1 GeoData.STX from SuperMarkets  order by GeoData.STX

--maxx
select top 1 GeoData.STX from SuperMarkets WHERE GeoData IS NOT NULL order by GeoData.STX desc

--miny
select top 1  GeoData.STY from SuperMarkets WHERE GeoData IS NOT NULL order by GeoData.STY 

--maxy
select top 1  GeoData.STY from SuperMarkets WHERE GeoData IS NOT NULL order by GeoData.STY desc



